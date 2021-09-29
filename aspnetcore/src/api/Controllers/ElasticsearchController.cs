using api.Services;
using api.Models;
using api.Models.Ttv;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Nest;
using System;

namespace api.Controllers
{
    [Route("api/elasticsearch")]
    [ApiController]
    public class ElasticsearchController : TtvControllerBase
    {
        private readonly TtvContext _ttvContext;
        private readonly ElasticsearchService _elasticsearchService;
        private readonly ILogger<UserProfileController> _logger;

        public ElasticsearchController(TtvContext ttvContext, ElasticsearchService elasticsearchService, ILogger<UserProfileController> logger)
        {
            _ttvContext = ttvContext;
            _elasticsearchService = elasticsearchService;
            _logger = logger;
        }

        //// Check if profile exists.
        //[HttpGet]
        //public async Task<IActionResult> Get()
        //{
        //    var settings = new ConnectionSettings(new Uri("http://localhost:9200"))
        //        .DefaultIndex("person");
        //    var client = new ElasticClient(settings);

        //    var person = new El
        //    {
        //        FirstName = "John"
        //    };

        //    var indexResponse = client.IndexDocument(person);

        //    var asyncIndexResponse = await client.IndexDocumentAsync(person);

        //    return Ok(asyncIndexResponse);
        //}

       
    }
}