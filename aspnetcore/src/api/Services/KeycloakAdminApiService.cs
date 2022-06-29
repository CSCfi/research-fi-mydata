using System;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Net.Http.Headers;
using Microsoft.Extensions.Configuration;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using Microsoft.Extensions.Logging;

namespace api.Services
{
    /*
     * KeycloakAdminApiService implements Keycloak Admin API calls.
     * https://www.keycloak.org/docs-api/15.0/rest-api/index.html
     */
    public class KeycloakAdminApiService : IKeycloakAdminApiService
    {
        private readonly ITokenService _tokenService;
        public IConfiguration Configuration { get; }
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ILogger<KeycloakAdminApiService> _logger;

        public KeycloakAdminApiService(ITokenService tokenService, IConfiguration configuration,
            IHttpClientFactory httpClientFactory, ILogger<KeycloakAdminApiService> logger)
        {
            _tokenService = tokenService;
            Configuration = configuration;
            _httpClientFactory = httpClientFactory;
            _logger = logger;
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
            _logger.LogInformation("KeycloakAdminApiService: get userdata for user id: " + keycloakUserId);
            HttpClient keycloakAdminApiHttpClient = _httpClientFactory.CreateClient("keycloakClient");
            HttpRequestMessage request = new(method: HttpMethod.Get, requestUri: keycloakUserId);
            HttpResponseMessage response = await keycloakAdminApiHttpClient.SendAsync(request);
            try
            {
                response.EnsureSuccessStatusCode();
                return await response.Content.ReadAsStringAsync();
            }
            catch (HttpRequestException)
            {
                _logger.LogError("KeycloakAdminApiService: could not get userdata for user id: " + keycloakUserId);
                return "";
            }
        }

        /*
         * Get orcid id from raw Keycloak user data.
         */
        public string? GetOrcidIdFromRawKeycloakUserData(string keycloakUserDataRaw)
        {
            using (JsonDocument document = JsonDocument.Parse(keycloakUserDataRaw))
            {
                document.RootElement.TryGetProperty("federatedIdentities", out JsonElement federatedIdentitiesElement);
                foreach (JsonElement identityElement in federatedIdentitiesElement.EnumerateArray())
                {
                    identityElement.TryGetProperty("identityProvider", out JsonElement identityProviderElement);
                    identityElement.TryGetProperty("userId", out JsonElement userIdElement);
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
        public async Task<bool> SetOrcidIdAsKeycloakUserAttribute(string keycloakUserId, string orcidId)
        {
            _logger.LogInformation("KeycloakAdminApiService: Set ORCID ID " + orcidId + " as attribute to Keycloak user: " + keycloakUserId);
            HttpClient keycloakAdminApiHttpClient = _httpClientFactory.CreateClient("keycloakClient");
            HttpRequestMessage request = new(method: HttpMethod.Put, requestUri: keycloakUserId);
            string stringPayload = "{\"attributes\": {\"orcid\": [\"" + orcidId + "\"]}}";
            request.Content = new StringContent(stringPayload, Encoding.UTF8, "application/json");
            HttpResponseMessage response = await keycloakAdminApiHttpClient.SendAsync(request);
            try
            {
                response.EnsureSuccessStatusCode();
                return true;
            }
            catch (HttpRequestException)
            {
                _logger.LogError("KeycloakAdminApiService: could not set ORCID ID " + orcidId + " as attribute to Keycloak user: " + keycloakUserId);
                return false;
            }
        }

        /*
         * Set attribute "orcid" for Keycloak user.
         */
        public async Task<bool> SetOrcidAttributedInKeycloak(JwtSecurityToken jwtFromUser)
        {
            /*
             * check is orcid already in token
             * yes:
             *    return
             * no:
             *    get sub (=kc user id)
             *    get userdata from kc
             *    update user attribute in kc
             *    return
             */
            if (!TokenService.JwtContainsOrcid(jwtFromUser))
            {
                // Get Keycloak user id.
                string keycloakUserId = _tokenService.GetKeycloakUserIdFromJwt(jwtFromUser);

                // Get Keycloak user data.
                string keycloakUserDataRaw = await this.GetRawUserDataFromKeycloakAdminApi(keycloakUserId);

                // Stop if Keycloak user data was not received.
                if (keycloakUserDataRaw == "")
                {
                    return false;
                }

                // Get orcid id from user data.
                string orcidId = this.GetOrcidIdFromRawKeycloakUserData(keycloakUserDataRaw);

                // Stop if ORCID ID was not found.
                if (orcidId == null)
                {
                    return false;
                }

                // Set orcid id as Keycloak user attribute.
                return await this.SetOrcidIdAsKeycloakUserAttribute(keycloakUserId, orcidId);
            }
            return true;
        }

        /*
         *  Logout Keycloak user.
         *  Remove all user sessions associated with the user by sending Keycloak admin api message:
         *      POST /{realm}/users/{id}/logout
         */
        public async Task<bool> LogoutUser(String tokenStr)
        {
            // Get jwt from string
            JwtSecurityToken jwtFromUser = _tokenService.GetJwtFromString(tokenStr);

            // Get Keycloak user id.
            string keycloakUserId = _tokenService.GetKeycloakUserIdFromJwt(jwtFromUser);
            _logger.LogInformation("KeycloakAdminApiService: Logout Keycloak user: " + keycloakUserId);

            HttpClient keycloakAdminApiHttpClient = _httpClientFactory.CreateClient("keycloakClient");
            HttpRequestMessage request = new(method: HttpMethod.Post, requestUri: keycloakUserId + "/logout");
            HttpResponseMessage response = await keycloakAdminApiHttpClient.SendAsync(request);
            try
            {
                response.EnsureSuccessStatusCode();
                _logger.LogInformation("KeycloakAdminApiService: Successfully logged out Keycloak user: " + keycloakUserId);
                return true;
            }
            catch (HttpRequestException)
            {
                _logger.LogError("KeycloakAdminApiService: could not logout Keycloak user: " + keycloakUserId);
                return false;
            }
        }

        /*
         *  Remove Keycloak user.
         *      DELETE /{realm}/users/{id}
         */
        public async Task<bool> RemoveUser(String tokenStr)
        {
            // Get jwt from string
            JwtSecurityToken jwtFromUser = _tokenService.GetJwtFromString(tokenStr);

            // Get Keycloak user id.
            string keycloakUserId = _tokenService.GetKeycloakUserIdFromJwt(jwtFromUser);
            _logger.LogInformation("KeycloakAdminApiService: Delete Keycloak user: " + keycloakUserId);

            HttpClient keycloakAdminApiHttpClient = _httpClientFactory.CreateClient("keycloakClient");
            HttpRequestMessage request = new(method: HttpMethod.Delete, requestUri: keycloakUserId);
            HttpResponseMessage response = await keycloakAdminApiHttpClient.SendAsync(request);
            try
            {
                response.EnsureSuccessStatusCode();
                _logger.LogInformation("KeycloakAdminApiService: Successfully deleted Keycloak user: " + keycloakUserId);
                return true;
            }
            catch (HttpRequestException)
            {
                _logger.LogError("KeycloakAdminApiService: could not delete Keycloak user: " + keycloakUserId);
                return false;
            }
        }
    }
}