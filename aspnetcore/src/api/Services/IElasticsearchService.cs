using System.Threading.Tasks;
using api.Models.Elasticsearch;
using api.Models.Log;
using Microsoft.Extensions.Configuration;

namespace api.Services
{
    public interface IElasticsearchService
    {
        IConfiguration Configuration { get; }

        Task<bool> DeleteEntryFromElasticsearchPersonIndex(string orcidId, LogUserIdentification logUserIdentification);
        bool IsElasticsearchSyncEnabled();
        Task<bool> UpdateEntryInElasticsearchPersonIndex(string orcidId, ElasticsearchPerson person, LogUserIdentification logUserIdentification);
    }
}