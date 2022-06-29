using System.Threading.Tasks;
using api.Models.Elasticsearch;

namespace api.Services
{
    public interface IBackgroundProfiledata
    {
        Task<Person> GetProfiledataForElasticsearch(string orcidId, int userprofileId);
    }
}