using System;
using System.Net.Http;
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
    public class ElasticsearchService
    {
        public ElasticClient ESclient;
        public IConfiguration _configuration { get; }
        private readonly ILogger<ElasticsearchService> _logger;

        // Check if Elasticsearch synchronization is enabled and related configuration is valid.
        public Boolean IsElasticsearchSyncEnabled()
        {
            return _configuration["ELASTICSEARCH:ENABLED"] != null
                && _configuration["ELASTICSEARCH:ENABLED"] == "true"
                && _configuration["ELASTICSEARCH:URL"] != null
                && Uri.IsWellFormedUriString(
                    _configuration["ELASTICSEARCH:URL"],
                    UriKind.Absolute
                );
        }

        // Constructor.
        // Do not setup HttpClient unless configuration values are ok.
        public ElasticsearchService(IConfiguration configuration, HttpClient client, ILogger<ElasticsearchService> logger)
        {
            _configuration = configuration;
            _logger = logger;

            if (this.IsElasticsearchSyncEnabled())
            {
                var settings = new ConnectionSettings(new Uri(_configuration["ELASTICSEARCH:URL"]))
                    .DefaultIndex("person")
                    .BasicAuthentication(_configuration["ELASTICSEARCH:USERNAME"], _configuration["ELASTICSEARCH:PASSWORD"]);
                ESclient = new ElasticClient(settings);
            }
        }
            

        // Update entry in Elasticsearch person index
        // TODO: When 3rd party sharing feature is implemented, check that TTV share is enabled in user profile.
        public async Task UpdateEntryInElasticsearchPersonIndex(string orcidId, Person person)
        {
            if (!this.IsElasticsearchSyncEnabled())
            {
                return;
            }

            _logger.LogInformation("ElasticsearchService: updating entry: " + orcidId);

            var asyncIndexResponse = await ESclient.IndexDocumentAsync(person);

            if (asyncIndexResponse.IsValid)
            {
                _logger.LogInformation("ElasticsearchService: updated entry: " + orcidId);
            }
            else {
                var errormessage = "";
                if (asyncIndexResponse.OriginalException != null && asyncIndexResponse.OriginalException.Message != null)
                {
                    errormessage = asyncIndexResponse.OriginalException.Message;
                }
                else if (asyncIndexResponse.ServerError != null && asyncIndexResponse.ServerError != null && asyncIndexResponse.ServerError.Error != null && asyncIndexResponse.ServerError.Error.Reason != null)
                {
                    errormessage = asyncIndexResponse.ServerError.Error.Reason;
                }
                _logger.LogError("ElasticsearchService: ERROR updating: " + orcidId + ": " + errormessage);
            }
        }

        // Delete entry from Elasticsearch person index
        public async Task DeleteEntryFromElasticsearchPersonIndex(String orcidId)
        {
            if (!this.IsElasticsearchSyncEnabled())
            {
                return;
            }

            _logger.LogInformation("ElasticsearchService: deleting entry: " + orcidId);

            var asyncDeleteResponse = await ESclient.DeleteAsync<ElasticPerson>(orcidId);

            if (asyncDeleteResponse.IsValid)
            {
                _logger.LogInformation("ElasticsearchService: deleted entry: " + orcidId);
            }
            else { 
                var errormessage = "";
                if (asyncDeleteResponse.OriginalException != null && asyncDeleteResponse.OriginalException.Message != null)
                {
                    errormessage = asyncDeleteResponse.OriginalException.Message;
                }
                else if (asyncDeleteResponse.ServerError != null && asyncDeleteResponse.ServerError != null && asyncDeleteResponse.ServerError.Error != null && asyncDeleteResponse.ServerError.Error.Reason != null)
                {
                    errormessage = asyncDeleteResponse.ServerError.Error.Reason;
                }
                _logger.LogError("ElasticsearchService: ERROR deleting: " + orcidId + ": " + errormessage);
            }
        }
    }
}