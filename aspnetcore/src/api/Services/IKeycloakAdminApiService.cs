using System.IdentityModel.Tokens.Jwt;
using System.Threading.Tasks;
using api.Models.Log;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;

namespace api.Services
{
    public interface IKeycloakAdminApiService
    {
        IConfiguration Configuration { get; }

        string GetAccessTokenFromHttpRequest(HttpRequest httpRequest);
        string GetKeycloakUserIdFromAccessToken(HttpRequest httpRequest);
        string GetOrcidIdFromRawKeycloakUserData(string keycloakUserDataRaw);
        Task<string> GetRawUserDataFromKeycloakAdminApi(string keycloakUserId, LogUserIdentification logUserIdentification);
        Task<bool> LogoutUser(string tokenStr, LogUserIdentification logUserIdentification);
        Task<bool> RemoveUser(string tokenStr, LogUserIdentification logUserIdentification);
        Task<bool> SetOrcidAttributedInKeycloak(JwtSecurityToken jwtFromUser, LogUserIdentification logUserIdentification);
        Task<bool> SetOrcidIdAsKeycloakUserAttribute(string keycloakUserId, string orcidId, LogUserIdentification logUserIdentification);
    }
}