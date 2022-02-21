using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Net.Http.Headers;
using api.Models.Orcid;
using api.Models.Ttv;
using Microsoft.Extensions.Configuration;

namespace api.Services
{
    /*
     * TokenService implements functionality related to access tokens.
     */
    public class TokenService
    {
        private readonly TtvContext _ttvContext;
        public HttpClient Client { get; }
        public IConfiguration _configuration { get; }

        public TokenService(TtvContext ttvContext, HttpClient client)
        {
            _ttvContext = ttvContext;
            client.DefaultRequestHeaders.Add("Accept", "application/json");
            Client = client;
        }

        /*
         * Get access token from HttpRequest header Authorization.
         */
        public String GetAccessTokenFromHttpRequest(HttpRequest httpRequest)
        {
            return httpRequest.Headers[HeaderNames.Authorization].ToString().Replace("Bearer ", "");
        }

        /*
         * Get Keycloak external IDP token enpoint for ORCID.
         * General form is: /realms/{realm}/broker/{provider_alias}/token
         * https://www.keycloak.org/docs/latest/server_development/#retrieving-external-idp-tokens
         */
        public string GetKeycloakExternalIdpEnpointForOrcid()
        {
            return _configuration["OAUTH:AUTHORITY"] + "/broker/orcid/token";
        }

        /*
         * Get user's ORCID tokens json from Keycloak.
         * Keycloak can store access tokens from external IDPs, such as ORCID.
         * Those can be requested from Keycloak using IDP specific endpoint.
         * https://www.keycloak.org/docs/latest/server_development/#retrieving-external-idp-tokens
         */
        public async Task<String> GetOrcidTokensJsonFromKeycloak(String keyCloakAccessToken)
        {
            var keycloakExternalIdpTokenEndpoint = this.GetKeycloakExternalIdpEnpointForOrcid();
            var requestMessage = new HttpRequestMessage
            {
                Method = HttpMethod.Get,
                RequestUri = new Uri(keycloakExternalIdpTokenEndpoint)
            };
            requestMessage.Headers.Authorization = new AuthenticationHeaderValue("Bearer", keyCloakAccessToken);
            HttpResponseMessage response = await Client.SendAsync(requestMessage);
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

        /*
         * Update ORCID tokens in DimUserProfile
         */
        public async Task UpdateOrcidTokensInDimUserProfile(int dimUserProfileId, OrcidTokens orcidTokens)
        {
            var dimUserProfile = await _ttvContext.DimUserProfiles.FindAsync(dimUserProfileId);
            dimUserProfile.OrcidAccessToken = orcidTokens.AccessToken;
            dimUserProfile.OrcidRefreshToken = orcidTokens.RefreshToken;
            dimUserProfile.OrcidTokenScope = orcidTokens.Scope;
            dimUserProfile.OrcidTokenExpires = orcidTokens.ExpiresDatetime;
            await _ttvContext.SaveChangesAsync();
        }
    }
}