using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Threading.Tasks;
using api.Models.Orcid;
using Microsoft.Extensions.Configuration;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;

namespace api.Services
{
    /*
     * TokenService implements functionality related to access tokens.
     */
    public class TokenService : ITokenService
    {
        private readonly IHttpClientFactory _httpClientFactory;
        public IConfiguration Configuration { get; }

        public TokenService(IConfiguration configuration, IHttpClientFactory httpClientFactory)
        {
            Configuration = configuration;
            _httpClientFactory = httpClientFactory;
        }

        /*
         * Get JWT from string.
         */
        public JwtSecurityToken GetJwtFromString(String tokenStr)
        {
            JwtSecurityTokenHandler handler = new();
            return handler.ReadJwtToken(tokenStr);
        }

        /*
         * Check if JSON Web Token contains claim "orcid".
         */
        public static bool JwtContainsOrcid(JwtSecurityToken jwt)
        {
            return jwt.Claims.FirstOrDefault(x => x.Type == "orcid") != null;
        }

        /*
         * Get Keycloak user id from JSON Web Token.
         */
        public string GetKeycloakUserIdFromJwt(JwtSecurityToken jwt)
        {
            return jwt.Subject;
        }

        /*
         * Get user's ORCID tokens json from Keycloak.
         * Keycloak can store access tokens from external IDPs, such as ORCID.
         * Those can be requested from Keycloak using IDP specific endpoint.
         * https://www.keycloak.org/docs/latest/server_development/#retrieving-external-idp-tokens
         */
        public async Task<String> GetOrcidTokensJsonFromKeycloak(String usersKeycloakAccessToken)
        {
            // Get http client.
            HttpClient keycloakHttpClient = _httpClientFactory.CreateClient("keycloakUserOrcidTokens");
            // GET request
            HttpRequestMessage request = new(method: HttpMethod.Get, requestUri: "");
            // Insert ORCID access token into authorization header for each request.
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", usersKeycloakAccessToken);
            HttpResponseMessage response = await keycloakHttpClient.SendAsync(request);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadAsStringAsync();
        }

        /*
         * Parse ORCID tokens json into OrcidTokens object
         */
        public OrcidTokens ParseOrcidTokensJson(String orcidTokensJson)
        {
            using JsonDocument document = JsonDocument.Parse(orcidTokensJson);
            // Access token
            document.RootElement.TryGetProperty("access_token", out JsonElement accessTokenElement);
            // Refresh token
            document.RootElement.TryGetProperty("refresh_token", out JsonElement refreshTokenElement);
            // Scope
            document.RootElement.TryGetProperty("scope", out JsonElement scopeElement);
            // Access token expires seconds
            document.RootElement.TryGetProperty("accessTokenExpiration", out JsonElement expiresSecondsElement);

            return new OrcidTokens(
                accessToken: accessTokenElement.ToString(),
                refreshToken: refreshTokenElement.ToString(),
                scope: scopeElement.ToString(),
                expiresSeconds: expiresSecondsElement.GetInt64(),
                expiresDatetime: null
            )
            { };
        }
    }
}