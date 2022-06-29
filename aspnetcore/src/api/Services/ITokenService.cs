using System.IdentityModel.Tokens.Jwt;
using System.Threading.Tasks;
using api.Models.Orcid;
using Microsoft.Extensions.Configuration;

namespace api.Services
{
    public interface ITokenService
    {
        IConfiguration Configuration { get; }

        JwtSecurityToken GetJwtFromString(string tokenStr);
        string GetKeycloakUserIdFromJwt(JwtSecurityToken jwt);
        Task<string> GetOrcidTokensJsonFromKeycloak(string usersKeycloakAccessToken);
        OrcidTokens ParseOrcidTokensJson(string orcidTokensJson);
    }
}