using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace api.Services
{
    /*
     * ElasticsearchService handles person index update.
     */
    public class ElasticsearchService
    {
        public HttpClient Client { get; }
        public IConfiguration Configuration { get; }
        private readonly ILogger<ElasticsearchService> _logger;

        // Check if Elasticsearch synchronization is enabled and related configuration is valid.
        public Boolean IsElasticsearchSyncEnabled()
        {
            return Configuration["ELASTICSEARCH"] != null
                && Configuration["ELASTICSEARCH:ENABLED"] != null
                && Configuration["ELASTICSEARCH:ENABLED"] == "true"
                && Configuration["ELASTICSEARCH:URL"] != null
                && Uri.IsWellFormedUriString(
                    Configuration["ELASTICSEARCH:URL"],
                    UriKind.Absolute
                );
        }

        // Constructor.
        // Do not setup HttpClient unless configuration values are ok.
        public ElasticsearchService(IConfiguration configuration, HttpClient client, ILogger<ElasticsearchService> logger)
        {
            Configuration = configuration;

            if (this.IsElasticsearchSyncEnabled())
            {
                client.BaseAddress = new Uri(Configuration["ELASTICSEARCH:URL"]);
                Client = client;
                _logger = logger;
            }
        }

        // Get Elasticsearch person index url
        public String GetUrlElasticsearchPersonIndex(String orcidId)
        {
            return Client.BaseAddress + "/portalapi/person" + orcidId;
        }

        // Update entry in Elasticsearch person index
        // TODO: When 3rd party sharing feature is implemented, check that TTV share is enabled in user profile.
        public async Task UpdateEntryInElasticsearchPersonIndex(String orcidId, String personData)
        {
            string result = string.Empty;
            var url = GetUrlElasticsearchPersonIndex(orcidId);
            HttpResponseMessage response = await Client.PostAsync(
                url,
                new StringContent(personData, Encoding.UTF8, "application/json")
            );
            response.EnsureSuccessStatusCode();
        }

        // Delete entry from Elasticsearch person index
        public async Task DeleteEntryFromElasticsearchPersonIndex(String orcidId)
        {
            string result = string.Empty;
            var url = GetUrlElasticsearchPersonIndex(orcidId);
            HttpResponseMessage response = await Client.DeleteAsync(url);
            response.EnsureSuccessStatusCode();
        }
    }
}