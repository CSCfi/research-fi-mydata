using System.Threading.Tasks;
using api.Models.Elasticsearch;
using api.Models.Log;

namespace api.Services
{
    public interface IBackgroundProfiledata
    {
        Task<ElasticsearchPerson> GetProfiledataForElasticsearch(string orcidId, int userprofileId, LogUserIdentification logUserIdentification);
    }
}