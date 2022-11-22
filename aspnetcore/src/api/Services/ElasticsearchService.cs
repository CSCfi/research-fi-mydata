using System;
using System.Threading.Tasks;
using api.Models.Elasticsearch;
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

        // Check if Elasticsearch synchronization is enabled and related configuration is valid.
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

        // Constructor.
        // Do not setup Elasticsearch client unless configuration values are ok.
        public ElasticsearchService(IConfiguration configuration, ILogger<ElasticsearchService> logger)
        {
            Configuration = configuration;
            _logger = logger;

            if (IsElasticsearchSyncEnabled())
            {
                // Elasticsearch client
                ConnectionSettings settings = new ConnectionSettings(new Uri(Configuration["ELASTICSEARCH:URL"]))
                    .MaximumRetries(3)
                    .DefaultIndex(elasticsearchProfileIndexName)
                    .BasicAuthentication(Configuration["ELASTICSEARCH:USERNAME"], Configuration["ELASTICSEARCH:PASSWORD"]);
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


        // Update entry in Elasticsearch person index
        // TODO: When 3rd party sharing feature is implemented, check that TTV share is enabled in user profile.
        public async Task<bool> UpdateEntryInElasticsearchPersonIndex(string orcidId, ElasticsearchPerson person)
        {
            if (!IsElasticsearchSyncEnabled())
            {
                return false;
            }

            //_logger.LogInformation("ElasticsearchService: updating entry: " + orcidId);

            IndexResponse asyncIndexResponse = await ESclient.IndexDocumentAsync(person);

            if (asyncIndexResponse.IsValid)
            {
                return true;
            }
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
                _logger.LogError("ElasticsearchService: ERROR updating: " + orcidId + ": " + errormessage);
                return false;
            }
        }

        // Delete entry from Elasticsearch person index
        public async Task<bool> DeleteEntryFromElasticsearchPersonIndex(string orcidId)
        {
            if (!IsElasticsearchSyncEnabled())
            {
                return false;
            }

            //_logger.LogInformation("ElasticsearchService: deleting entry: " + orcidId);

            DeleteResponse asyncDeleteResponse = await ESclient.DeleteAsync<ElasticsearchPerson>(orcidId);

            if (asyncDeleteResponse.IsValid)
            {
                return true;
            }
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
                _logger.LogError("ElasticsearchService: ERROR deleting: " + orcidId + ": " + errormessage);
                return false;
            }
        }
    }
}