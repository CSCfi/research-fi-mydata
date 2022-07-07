using System.Threading.Tasks;
using api.Models.Elasticsearch;
using Microsoft.Extensions.Configuration;

namespace api.Services
{
    public interface IElasticsearchService
    {
        IConfiguration Configuration { get; }

        Task<bool> DeleteEntryFromElasticsearchPersonIndex(string orcidId);
        bool IsElasticsearchSyncEnabled();
        Task<bool> UpdateEntryInElasticsearchPersonIndex(string orcidId, Person person);
    }
}