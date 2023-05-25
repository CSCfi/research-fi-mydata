using System.Threading.Tasks;
using api.Models.Elasticsearch;
using api.Models.Log;
using Microsoft.Extensions.Configuration;

namespace api.Services
{
    public interface IElasticsearchService
    {
        IConfiguration Configuration { get; }

        Task<bool> BackgroundDelete(string orcidId, LogUserIdentification logUserIdentification, string logAction = LogContent.Action.ELASTICSEARCH_DELETE);
        Task<bool> BackgroundUpdate(string orcidId, int userprofileId, LogUserIdentification logUserIdentification, string logAction = LogContent.Action.ELASTICSEARCH_UPDATE);
        bool IsElasticsearchSyncEnabled();
    }
}