using System;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Net.Http.Headers;
using Microsoft.Extensions.Configuration;
using System.IdentityModel.Tokens.Jwt;
using System.Text;

namespace api.Services
{
    /*
     * KeycloakAdminApiService implements Keycloak Admin API calls.
     * https://www.keycloak.org/docs-api/15.0/rest-api/index.html
     */
    public class KeycloakAdminApiService
    {
        private readonly TokenService _tokenService;
        public IConfiguration _configuration { get; }
        private readonly IHttpClientFactory _httpClientFactory;

        public KeycloakAdminApiService(TokenService tokenService, IConfiguration configuration, IHttpClientFactory httpClientFactory)
        {
            _tokenService = tokenService;
            _configuration = configuration;
            _httpClientFactory = httpClientFactory;
        }

        /*
         * Get access token from HttpRequest header Authorization.
         */
        public String GetAccessTokenFromHttpRequest(HttpRequest httpRequest)
        {
            return httpRequest.Headers[HeaderNames.Authorization].ToString().Replace("Bearer ", "");
        }

        /*
         * Get Keycloak user ID from access token.
         */
        public String GetKeycloakUserIdFromAccessToken(HttpRequest httpRequest)
        {
            return httpRequest.Headers[HeaderNames.Authorization].ToString().Replace("Bearer ", "");
        }

        /*
         * Get user data from Keycloak Admin API.
         * Use name HttpClient "keycloakClient".
         */
        public async Task<string> GetRawUserDataFromKeycloakAdminApi(string keycloakUserId)
        {
            var keycloakAdminApiHttpClient = _httpClientFactory.CreateClient("keycloakClient");
            var request = new HttpRequestMessage(method: HttpMethod.Get, requestUri: keycloakUserId);
            HttpResponseMessage response = await keycloakAdminApiHttpClient.SendAsync(request);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadAsStringAsync();
        }

        /*
         * Get orcid id from raw Keycloak user data.
         */
        public string? GetOrcidIdFromRawKeycloakUserData(string keycloakUserDataRaw)
        {
            using (JsonDocument document = JsonDocument.Parse(keycloakUserDataRaw))
            {
                JsonElement federatedIdentitiesElement;
                document.RootElement.TryGetProperty("federatedIdentities", out federatedIdentitiesElement);
                foreach (JsonElement identityElement in federatedIdentitiesElement.EnumerateArray())
                {
                    JsonElement identityProviderElement;
                    JsonElement userIdElement;
                    identityElement.TryGetProperty("identityProvider", out identityProviderElement);
                    identityElement.TryGetProperty("userId", out userIdElement);
                    if (identityProviderElement.GetString() == "orcid")
                    {
                        return userIdElement.GetString();
                    }
                }
            }
            return null;
        }

        /*
         * Set ORCID ID as Keycloak user attribute using Keycloak Admin API.
         * Use name HttpClient "keycloakClient".
         */
        public async Task SetOrcidIdAsKeycloakUserAttribute(string keycloakUserId, string orcidId)
        {
            var keycloakAdminApiHttpClient = _httpClientFactory.CreateClient("keycloakClient");
            var request = new HttpRequestMessage(method: HttpMethod.Put, requestUri: keycloakUserId);
            var stringPayload = "{\"attributes\": {\"orcid\": [\"" + orcidId + "\"]}}";
            request.Content = new StringContent(stringPayload, Encoding.UTF8, "application/json");
            HttpResponseMessage response = await keycloakAdminApiHttpClient.SendAsync(request);
            response.EnsureSuccessStatusCode();
        }

        /*
         * Set attribute "orcid" for Keycloak user.
         */
        public async Task<bool> SetOrcidAttributedInKeycloak(JwtSecurityToken jwtFromUser)
        {
            /*
             * check is orcid already in token
             * yes: return
             * no:
             *    get sub (=kc user id)
             *    get userdata from kc
             *    update user attribute in kc
             *    return
             */
            if (!TokenService.JwtContainsOrcid(jwtFromUser))
            {
                // Get Keycloak user id.
                var keycloakUserId = _tokenService.GetKeycloakUserIdFromJwt(jwtFromUser);

                // Get Keycloak user data.
                var keycloakUserDataRaw = await this.GetRawUserDataFromKeycloakAdminApi(keycloakUserId);

                // Get orcid id from user data.
                var orcidId = this.GetOrcidIdFromRawKeycloakUserData(keycloakUserDataRaw);

                // Stop if ORCID ID was not found.
                if (orcidId == null)
                    return false;

                // Set orcid id as Keycloak user attribute.
                await this.SetOrcidIdAsKeycloakUserAttribute(keycloakUserId, orcidId);
            }
            return true;
        }
    }
}