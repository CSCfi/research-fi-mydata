using System.Threading.Tasks;
using api.Models.Ai;

namespace api.Services
{
    public interface IBiographyService
    {
        Task<string?> GetProfileDataForPromt(string orcidId);
        Task<Biography> GetBiography(int userprofileId);
        Task<bool> CreateOrUpdateBiography(int userprofileId, Biography biography);
        Task<bool> DeleteBiography(int userprofileId);
    }
}