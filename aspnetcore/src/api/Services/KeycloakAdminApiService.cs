﻿using System;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Net.Http.Headers;
using Microsoft.Extensions.Configuration;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using Microsoft.Extensions.Logging;
using api.Models.Log;
using api.Models.Keycloak;

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
         * https://www.keycloak.org/docs-api/latest/rest-api/
         */
        public async Task<string> GetRawUserDataFromKeycloakAdminApi(string keycloakUserId, LogUserIdentification logUserIdentification)
        {
            _logger.LogInformation(
                LogContent.MESSAGE_TEMPLATE,
                logUserIdentification,
                new LogApiInfo(
                    action: LogContent.Action.KEYCLOAK_GET_RAW_USER_DATA,
                    state: LogContent.ActionState.START));

            HttpClient keycloakAdminApiHttpClient = _httpClientFactory.CreateClient("keycloakClient");
            HttpRequestMessage request = new(method: HttpMethod.Get, requestUri: keycloakUserId);
            HttpResponseMessage response = await keycloakAdminApiHttpClient.SendAsync(request);
            try
            {
                response.EnsureSuccessStatusCode();

                _logger.LogInformation(
                    LogContent.MESSAGE_TEMPLATE,
                    logUserIdentification,
                    new LogApiInfo(
                        action: LogContent.Action.KEYCLOAK_GET_RAW_USER_DATA,
                        state: LogContent.ActionState.COMPLETE));

                return await response.Content.ReadAsStringAsync();
            }
            catch (HttpRequestException)
            {
                _logger.LogError(
                    LogContent.MESSAGE_TEMPLATE,
                    logUserIdentification,
                    new LogApiInfo(
                        action: LogContent.Action.KEYCLOAK_GET_RAW_USER_DATA,
                        state: LogContent.ActionState.FAILED,
                        error: true));
                return "";
            }
        }

        /*
         * Update Keycloak user data using Keycloak Admin API.
         * Use name HttpClient "keycloakClient".
         * https://www.keycloak.org/docs-api/latest/rest-api/
         */
        public async Task<bool> UpdateKeycloakUser(string keycloakUserId, string keycloakUserModelSerialized, LogUserIdentification logUserIdentification)
        {
            _logger.LogInformation(
                LogContent.MESSAGE_TEMPLATE,
                logUserIdentification,
                new LogApiInfo(
                    action: LogContent.Action.KEYCLOAK_SET_ORCID_ATTRIBUTE,
                    state: LogContent.ActionState.START));

            HttpClient keycloakAdminApiHttpClient = _httpClientFactory.CreateClient("keycloakClient");
            HttpRequestMessage request = new(method: HttpMethod.Put, requestUri: keycloakUserId);
            request.Content = new StringContent(keycloakUserModelSerialized, Encoding.UTF8, "application/json");
            HttpResponseMessage response = await keycloakAdminApiHttpClient.SendAsync(request);
            try
            {
                response.EnsureSuccessStatusCode();

                _logger.LogInformation(
                    LogContent.MESSAGE_TEMPLATE,
                    logUserIdentification,
                    new LogApiInfo(
                        action: LogContent.Action.KEYCLOAK_SET_ORCID_ATTRIBUTE,
                        state: LogContent.ActionState.COMPLETE));

                return true;
            }
            catch (HttpRequestException)
            {
                _logger.LogError(
                    LogContent.MESSAGE_TEMPLATE,
                    logUserIdentification,
                    new LogApiInfo(
                        action: LogContent.Action.KEYCLOAK_SET_ORCID_ATTRIBUTE,
                        state: LogContent.ActionState.FAILED,
                        error: true));

                return false;
            }
        }

        /*
         * Set attribute "orcid" for Keycloak user.
         */
        public async Task<bool> SetOrcidAttributedInKeycloak(JwtSecurityToken jwtFromUser, LogUserIdentification logUserIdentification)
        {
            /*
             * Check is "orcid" already in token
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
                string keycloakUserDataRaw = await this.GetRawUserDataFromKeycloakAdminApi(keycloakUserId, logUserIdentification);

                // Stop if Keycloak user data was not received.
                if (keycloakUserDataRaw == "")
                {
                    return false;
                }

                // Parse Keycloak user data.
                // https://www.keycloak.org/docs-api/latest/rest-api/#UserRepresentation
                KeycloakUserDTO? keycloakUserDTO = new();
                try {
                    keycloakUserDTO = JsonSerializer.Deserialize<KeycloakUserDTO>(keycloakUserDataRaw);
                }
                catch (JsonException)
                {
                    _logger.LogError(
                        LogContent.MESSAGE_TEMPLATE,
                        logUserIdentification,
                        new LogApiInfo(
                            action: LogContent.Action.KEYCLOAK_SET_ORCID_ATTRIBUTE,
                            error: true,
                            state: LogContent.ActionState.FAILED,
                            message: "Failed to parse Keycloak user model JSON: " + keycloakUserDataRaw));
                    return false;
                }

                // Search ORCID ID from federated identities list.
                string orcidId = "";
                foreach (FederatedIdentity federatedIdentity in keycloakUserDTO.FederatedIdentities)
                {
                    if (federatedIdentity.IdentityProvider == "orcid")
                    {
                        orcidId = federatedIdentity.UserId;
                        break;
                    }
                }

                // Stop if ORCID ID was not found.
                if (string.IsNullOrWhiteSpace(orcidId))
                {
                    return false;
                }

                // Set ORCID ID as Keycloak user attribute.
                keycloakUserDTO.Attributes = new UserAttributes()
                {
                    Orcid = new() { orcidId }
                };

                // Serialize Keycloak user data.
                string keycloakUserModelSerialized = JsonSerializer.Serialize(keycloakUserDTO);

                // Update user in Keycloak.
                return await this.UpdateKeycloakUser(keycloakUserId, keycloakUserModelSerialized, logUserIdentification);
            }
            return true;
        }

        /*
         *  Logout Keycloak user.
         *  Remove all user sessions associated with the user by sending Keycloak admin api message:
         *      POST /{realm}/users/{id}/logout
         */
        public async Task<bool> LogoutUser(String tokenStr, LogUserIdentification logUserIdentification)
        {
            _logger.LogInformation(
                LogContent.MESSAGE_TEMPLATE,
                logUserIdentification,
                new LogApiInfo(
                    action: LogContent.Action.KEYCLOAK_USER_LOGOUT,
                    state: LogContent.ActionState.START));

            // Get jwt from string
            JwtSecurityToken jwtFromUser = _tokenService.GetJwtFromString(tokenStr);

            // Get Keycloak user id.
            string keycloakUserId = _tokenService.GetKeycloakUserIdFromJwt(jwtFromUser);

            HttpClient keycloakAdminApiHttpClient = _httpClientFactory.CreateClient("keycloakClient");
            HttpRequestMessage request = new(method: HttpMethod.Post, requestUri: keycloakUserId + "/logout");
            HttpResponseMessage response = await keycloakAdminApiHttpClient.SendAsync(request);
            try
            {
                response.EnsureSuccessStatusCode();

                _logger.LogInformation(
                    LogContent.MESSAGE_TEMPLATE,
                    logUserIdentification,
                    new LogApiInfo(
                        action: LogContent.Action.KEYCLOAK_USER_LOGOUT,
                        state: LogContent.ActionState.COMPLETE));

                return true;
            }
            catch (HttpRequestException)
            {
                _logger.LogError(
                    LogContent.MESSAGE_TEMPLATE,
                    logUserIdentification,
                    new LogApiInfo(
                        action: LogContent.Action.KEYCLOAK_USER_LOGOUT,
                        error: true,
                        state: LogContent.ActionState.FAILED));

                return false;
            }
        }

        /*
         *  Remove Keycloak user.
         *      DELETE /{realm}/users/{id}
         */
        public async Task<bool> RemoveUser(String tokenStr, LogUserIdentification logUserIdentification)
        {
            _logger.LogInformation(
                LogContent.MESSAGE_TEMPLATE,
                logUserIdentification,
                new LogApiInfo(
                    action: LogContent.Action.KEYCLOAK_USER_DELETE,
                    state: LogContent.ActionState.START));

            // Get jwt from string
            JwtSecurityToken jwtFromUser = _tokenService.GetJwtFromString(tokenStr);

            // Get Keycloak user id.
            string keycloakUserId = _tokenService.GetKeycloakUserIdFromJwt(jwtFromUser);

            HttpClient keycloakAdminApiHttpClient = _httpClientFactory.CreateClient("keycloakClient");
            HttpRequestMessage request = new(method: HttpMethod.Delete, requestUri: keycloakUserId);
            HttpResponseMessage response = await keycloakAdminApiHttpClient.SendAsync(request);
            try
            {
                response.EnsureSuccessStatusCode();

                _logger.LogInformation(
                    LogContent.MESSAGE_TEMPLATE,
                    logUserIdentification,
                    new LogApiInfo(
                        action: LogContent.Action.KEYCLOAK_USER_DELETE,
                        state: LogContent.ActionState.COMPLETE));

                return true;
            }
            catch (HttpRequestException)
            {
                _logger.LogError(
                    LogContent.MESSAGE_TEMPLATE,
                    logUserIdentification,
                    new LogApiInfo(
                        action: LogContent.Action.KEYCLOAK_USER_DELETE,
                        error: true,
                        state: LogContent.ActionState.FAILED));

                return false;
            }
        }
    }
}