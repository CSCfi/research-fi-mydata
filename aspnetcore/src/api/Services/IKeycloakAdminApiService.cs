using System.IdentityModel.Tokens.Jwt;
using System.Threading.Tasks;
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
        Task<string> GetRawUserDataFromKeycloakAdminApi(string keycloakUserId);
        Task<bool> LogoutUser(string tokenStr);
        Task<bool> RemoveUser(string tokenStr);
        Task<bool> SetOrcidAttributedInKeycloak(JwtSecurityToken jwtFromUser);
        Task<bool> SetOrcidIdAsKeycloakUserAttribute(string keycloakUserId, string orcidId);
    }
}