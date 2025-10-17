using System;
using System.Threading.Tasks;
using api.Models.Elasticsearch;
using api.Models.Log;
using api.Models.Ttv;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Nest;

namespace api.Services
{
    /*
     * ElasticsearchService handles person index modifications.
     */
    public class ElasticsearchService : IElasticsearchService
    {
        public ElasticClient ESclient;
        public IConfiguration Configuration { get; }
        private readonly ILogger<ElasticsearchService> _logger;
        private readonly string elasticsearchProfileIndexName = "person";
        private readonly IBackgroundProfiledata _backgroundProfiledata;
        private readonly IBackgroundTaskQueue _taskQueue;

        /*
         * Check if Elasticsearch synchronization is enabled and related configuration is valid.
         */
        public bool IsElasticsearchSyncEnabled()
        {
            return Configuration["ELASTICSEARCH:ENABLED"] != null
                && Configuration["ELASTICSEARCH:ENABLED"] == "true"
                && Configuration["ELASTICSEARCH:URL"] != null
                && Uri.IsWellFormedUriString(
                    Configuration["ELASTICSEARCH:URL"],
                    UriKind.Absolute
                );
        }

        /*
         * Constructor.
         * Do not setup Elasticsearch client unless configuration values are ok.
         *
         * Compatibility mode is enabled for Elasticsearch 8:
         * https://www.elastic.co/guide/en/elasticsearch/client/net-api/7.17/connecting-to-elasticsearch-v8.html#enabling-compatibility-mode
         */
        public ElasticsearchService(
            IConfiguration configuration,
            ILogger<ElasticsearchService> logger,
            IBackgroundProfiledata backgroundProfiledata,
            IBackgroundTaskQueue taskQueue)
        {
            Configuration = configuration;
            _logger = logger;
            _backgroundProfiledata = backgroundProfiledata;
            _taskQueue = taskQueue;

            if (IsElasticsearchSyncEnabled())
            {
                // Elasticsearch client
                ConnectionSettings settings = new ConnectionSettings(new Uri(Configuration["ELASTICSEARCH:URL"]))
                    .MaximumRetries(3)
                    .DefaultIndex(elasticsearchProfileIndexName)
                    .BasicAuthentication(Configuration["ELASTICSEARCH:USERNAME"], Configuration["ELASTICSEARCH:PASSWORD"])
                    .EnableApiVersioningHeader(); // Required for Elasticsearch 8
                ESclient = new ElasticClient(settings);

                // Ensure required index exists.
                // Use attribute mapping when creating index:
                //     https://www.elastic.co/guide/en/elasticsearch/client/net-api/7.17/attribute-mapping.html
                if (!ESclient.Indices.Exists(elasticsearchProfileIndexName).Exists)
                {
                    _logger.LogInformation("ElasticsearchService: required index not found, creating index: " + elasticsearchProfileIndexName);

                    var createIndexResponse = ESclient.Indices.Create(elasticsearchProfileIndexName, c => c
                        .Map<ElasticsearchPerson>(m => m.AutoMap())
                    );

                    if (!createIndexResponse.IsValid)
                    {
                        _logger.LogError("ElasticsearchService: failed creating index: " + elasticsearchProfileIndexName + " : " + createIndexResponse.ToString());
                    }
                }
                else
                {
                    _logger.LogInformation("ElasticsearchService: required index found: " + elasticsearchProfileIndexName);
                }
            }
        }

        /*
         *  Background task for updating profile.
         */
        public async Task<bool> BackgroundUpdate(string orcidId, int userprofileId, LogUserIdentification logUserIdentification, string logAction=LogContent.Action.ELASTICSEARCH_UPDATE)
        {
            if (IsElasticsearchSyncEnabled())
            {
                // Elasticsearch sync is enabled, update index.
                await _taskQueue.QueueBackgroundWorkItemAsync(async token =>
                {
                    _logger.LogInformation(
                        LogContent.MESSAGE_TEMPLATE,
                        logUserIdentification,
                        new LogApiInfo(
                            action: logAction,
                            state: LogContent.ActionState.START));

                    // Get Elasticsearch person entry from profile data.
                    Models.Elasticsearch.ElasticsearchPerson person =
                        await _backgroundProfiledata.GetProfiledataForElasticsearch(
                            orcidId: orcidId,
                            userprofileId: userprofileId,
                            logUserIdentification: logUserIdentification);

                    // Update Elasticsearch person index.
                    await UpdateEntryInElasticsearchPersonIndex(person, logUserIdentification, logAction);

                    _logger.LogInformation(
                        LogContent.MESSAGE_TEMPLATE,
                        logUserIdentification,
                        new LogApiInfo(
                            action: logAction,
                            state: LogContent.ActionState.COMPLETE));
                });
                return true;
            }
            else
            {
                // Elasticsearch sync is disabled, cancel.
                _logger.LogInformation(
                    LogContent.MESSAGE_TEMPLATE,
                    logUserIdentification,
                    new LogApiInfo(
                        action: logAction,
                        state: LogContent.ActionState.CANCELLED,
                        message: "Elasticsearch sync disabled in configuration"));
            }
            return false;
        }


        /*
         * Update entry in Elasticsearch person index
         * TODO: When 3rd party sharing feature is implemented, check that TTV share is enabled in user profile.
         */
        private async Task<bool> UpdateEntryInElasticsearchPersonIndex(ElasticsearchPerson person, LogUserIdentification logUserIdentification, string logAction)
        {
            IndexResponse asyncIndexResponse = await ESclient.IndexDocumentAsync(person);

            if (asyncIndexResponse.IsValid)
            {
                return true;
            }
            else
            {
                string errormessage = "";
                if (asyncIndexResponse.OriginalException != null && asyncIndexResponse.OriginalException.Message != null)
                {
                    errormessage = asyncIndexResponse.OriginalException.Message;
                }
                else if (asyncIndexResponse.ServerError != null && asyncIndexResponse.ServerError != null && asyncIndexResponse.ServerError.Error != null && asyncIndexResponse.ServerError.Error.Reason != null)
                {
                    errormessage = asyncIndexResponse.ServerError.Error.Reason;
                }
                _logger.LogError(
                    LogContent.MESSAGE_TEMPLATE,
                    logUserIdentification,
                    new LogApiInfo(
                        action: logAction,
                        state: LogContent.ActionState.FAILED,
                        error: true,
                        message: errormessage));

                return false;
            }
        }

        /*
         *  Background task for deleting profile.
         */
        public async Task<bool> BackgroundDelete(string orcidId, LogUserIdentification logUserIdentification, string logAction = LogContent.Action.ELASTICSEARCH_DELETE)
        {
            if (IsElasticsearchSyncEnabled())
            {
                // Elasticsearch sync is enabled, delete from index.
                bool successfulDelete = false;
                await _taskQueue.QueueBackgroundWorkItemAsync(async token =>
                {
                    _logger.LogInformation(
                        LogContent.MESSAGE_TEMPLATE,
                        logUserIdentification,
                        new LogApiInfo(
                            action: logAction,
                            state: LogContent.ActionState.START));

                    // Delete entry from Elasticsearch person index.
                    successfulDelete = await DeleteEntryFromElasticsearchPersonIndex(orcidId, logUserIdentification, logAction);

                    if (successfulDelete)
                    {
                        _logger.LogInformation(
                            LogContent.MESSAGE_TEMPLATE,
                            logUserIdentification,
                            new LogApiInfo(
                                action: logAction,
                                state: LogContent.ActionState.COMPLETE));
                    }
                });
                return successfulDelete;
            }
            else
            {
                // Elasticsearch sync is disabled, cancel.
                _logger.LogInformation(
                    LogContent.MESSAGE_TEMPLATE,
                    logUserIdentification,
                    new LogApiInfo(
                        action: logAction,
                        state: LogContent.ActionState.CANCELLED,
                        message: "Elasticsearch sync disabled in configuration"));
            }
            return false;
        }

        /*
         * Delete entry from Elasticsearch person index.
         */
        public async Task<bool> DeleteEntryFromElasticsearchPersonIndex(string orcidId, LogUserIdentification logUserIdentification, string logAction)
        {
            // Check if entry exists
            var getResponse = await ESclient.GetAsync<ElasticsearchPerson>(orcidId);
            if (!getResponse.Found)
            {
                // Entry not found, consider delete successful.
                return true;
            }

            DeleteResponse asyncDeleteResponse = await ESclient.DeleteAsync<ElasticsearchPerson>(orcidId);

            if (asyncDeleteResponse.IsValid)
            {
                return true;
            }
            else
            {
                string errormessage = "";
                if (asyncDeleteResponse.OriginalException != null && asyncDeleteResponse.OriginalException.Message != null)
                {
                    errormessage = asyncDeleteResponse.OriginalException.Message;
                }
                else if (asyncDeleteResponse.ServerError != null && asyncDeleteResponse.ServerError != null && asyncDeleteResponse.ServerError.Error != null && asyncDeleteResponse.ServerError.Error.Reason != null)
                {
                    errormessage = asyncDeleteResponse.ServerError.Error.Reason;
                }
                else
                {
                    errormessage = asyncDeleteResponse.DebugInformation;
                }

                _logger.LogError(
                    LogContent.MESSAGE_TEMPLATE,
                    logUserIdentification,
                    new LogApiInfo(
                        action: logAction,
                        state: LogContent.ActionState.FAILED,
                        error: true,
                        message: errormessage));

                return false;
            }
        }
    }
}