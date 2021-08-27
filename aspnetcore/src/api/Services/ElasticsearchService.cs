using System;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace api.Services
{
    /*
     * ElasticsearchService handles person index update.
     */
    public class ElasticsearchService
    {
        public HttpClient Client { get; }
        private readonly ILogger<ElasticsearchService> _logger;

        public ElasticsearchService(HttpClient client, ILogger<ElasticsearchService> logger)
        {
            //client.BaseAddress = new Uri("https://<api address>");
            Client = client;
            _logger = logger;
        }
    }
}