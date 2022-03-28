using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Net.Http.Headers;
using api.Models.Orcid;
using Microsoft.Extensions.Configuration;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;

namespace api.Services
{
    /*
     * TokenService implements functionality related to access tokens.
     */
    public class TokenService
    {
        private readonly IHttpClientFactory _httpClientFactory;
        public IConfiguration _configuration { get; }

        public TokenService(IConfiguration configuration, IHttpClientFactory httpClientFactory)
        {
            _configuration = configuration;
            _httpClientFactory = httpClientFactory;
        }

        /*
         * Get JWT from string.
         */
        public JwtSecurityToken GetJwtFromString(String tokenStr)
        {
            var handler = new JwtSecurityTokenHandler();
            return handler.ReadJwtToken(tokenStr);
        }

        /*
         * Check if JSON Web Token contains claim "orcid".
         */
        public static bool JwtContainsOrcid(JwtSecurityToken jwt)
        {
            if (jwt.Claims.FirstOrDefault(x => x.Type == "orcid") != null)
                return true;
            return false;
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
            var keycloakHttpClient = _httpClientFactory.CreateClient("keycloakUserOrcidTokens");
            // GET request
            var request = new HttpRequestMessage(method: HttpMethod.Get, requestUri: "");
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
            using (JsonDocument document = JsonDocument.Parse(orcidTokensJson))
            {
                JsonElement accessTokenElement;
                JsonElement refreshTokenElement;
                JsonElement scopeElement;
                JsonElement expiresSecondsElement;

                // Access token
                document.RootElement.TryGetProperty("access_token", out accessTokenElement);
                // Refresh token
                document.RootElement.TryGetProperty("refresh_token", out refreshTokenElement);
                // Scope
                document.RootElement.TryGetProperty("scope", out scopeElement);
                // Access token expires seconds
                document.RootElement.TryGetProperty("accessTokenExpiration", out expiresSecondsElement);

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
}