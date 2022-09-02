using System.Threading.Tasks;
using api.Models.Elasticsearch;

namespace api.Services
{
    public interface IBackgroundProfiledata
    {
        Task<ElasticsearchPerson> GetProfiledataForElasticsearch(string orcidId, int userprofileId);
    }
}