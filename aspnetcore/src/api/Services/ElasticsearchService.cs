using System;
using System.Net.Http;
using System.Threading.Tasks;
using api.Models;
using api.Models.Ttv;
using api.Models.Elasticsearch;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Nest;

namespace api.Services
{
    /*
     * ElasticsearchService handles person index update.
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

            var asyncIndexResponse = await ESclient.IndexDocumentAsync(person);

            if (!asyncIndexResponse.IsValid)
            {
                _logger.LogInformation("Elasticsearch: ERROR: " + orcidId + ": " + asyncIndexResponse.OriginalException.Message);
            }
            else
            {
                _logger.LogInformation("Elasticsearch: Updated: " + orcidId);
            }
        }

        // Delete entry from Elasticsearch person index
        public async Task DeleteEntryFromElasticsearchPersonIndex(String orcidId)
        {
            if (!this.IsElasticsearchSyncEnabled())
            {
                return;
            }

            var asyncDeleteResponse = await ESclient.DeleteAsync<ElasticPerson>(orcidId);

            if (!asyncDeleteResponse.IsValid)
            {
                _logger.LogInformation("Elasticsearch: ERROR: " + orcidId + ": " + asyncDeleteResponse.OriginalException.Message);
            }
            else
            {
                _logger.LogInformation("Elasticsearch: Deleted: " + orcidId);
            }
        }
    }
}