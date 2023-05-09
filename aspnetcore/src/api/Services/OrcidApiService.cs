using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Web;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Nest;
using Serilog.Core;

namespace api.Services
{
    /*
     * OrcidApiService handles ORCID api http requests.
     */
    public class OrcidApiService : IOrcidApiService
    {
        private readonly IHttpClientFactory _httpClientFactory;
        public IConfiguration Configuration { get; }
        private readonly ILogger<OrcidApiService> _logger;

        public OrcidApiService() { } // for unit test
        public OrcidApiService(IConfiguration configuration) { Configuration = configuration; } // for unit test

        public OrcidApiService(IHttpClientFactory httpClientFactory, IConfiguration configuration, ILogger<OrcidApiService> logger)
        {
            _httpClientFactory = httpClientFactory;
            _logger = logger;
            Configuration = configuration;
        }

        /*
         * Get path for retrieving ORCID record. 
         */
        public String GetOrcidRecordPath(String orcidId)
        {
            return orcidId + "/record";
        }

        /*
         * Get ORCID record from ORCID public API.
         */
        public async Task<String> GetRecordFromPublicApi(String orcidId)
        {
            // Get ORCID public API specific http client.
            HttpClient orcidPublicApiHttpClient = _httpClientFactory.CreateClient("ORCID_PUBLIC_API");
            // Get path for retrieving ORCID record. API base address is already set in http client.
            string orcidRecordPath = GetOrcidRecordPath(orcidId);
            // GET request
            HttpRequestMessage request = new(method: HttpMethod.Get, requestUri: orcidRecordPath);
            // Send request
            HttpResponseMessage response = await orcidPublicApiHttpClient.SendAsync(request);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadAsStringAsync();
        }

        /*
         * Get ORCID record from ORCID member API.
         * ORCID API call MUST include user's ORCID access token in the authorization header.
         */
        public async Task<String> GetRecordFromMemberApi(String orcidId, String orcidAccessToken)
        {
            // TODO: check orcidId and orcidAccessToken

            // Get ORCID member API specific http client.
            HttpClient orcidMemberApiHttpClient = _httpClientFactory.CreateClient("ORCID_MEMBER_API");
            // Get path for retrieving ORCID record. API base address is already set in http client.
            string orcidRecordPath = GetOrcidRecordPath(orcidId);
            // GET request
            HttpRequestMessage request = new(method: HttpMethod.Get, requestUri: orcidRecordPath);
            // Insert ORCID access token into authorization header for each request.
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", orcidAccessToken);
            // Send request
            HttpResponseMessage response = await orcidMemberApiHttpClient.SendAsync(request);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadAsStringAsync();
        }

        /*
         * Check if ORCID webhook is enabled and related configuration is valid.
         * Required configuration:
         * - ORCID:WEBHOOK:ENABLED
         * - ORCID:WEBHOOK:API
         * - ORCID:WEBHOOK:ACCESSTOKEN
         */
        public bool IsOrcidWebhookEnabled()
        {
            return !string.IsNullOrWhiteSpace(Configuration["ORCID:WEBHOOK:ENABLED"])
                && Configuration["ORCID:WEBHOOK:ENABLED"] == "true"
                && !string.IsNullOrWhiteSpace(Configuration["ORCID:WEBHOOK:API"])
                && Uri.IsWellFormedUriString(
                    Configuration["ORCID:WEBHOOK:API"],
                    UriKind.Absolute
                )
                && !string.IsNullOrWhiteSpace(Configuration["ORCID:WEBHOOK:ACCESSTOKEN"]);
        }

        /*
         * Get ORCID webhook callback URL
         */
        public string? GetOrcidWebhookCallbackUri(string orcidId)
        {
            UriBuilder callbackUri = new(Configuration["SERVICEURL"]);
            callbackUri.Scheme = "https";
            callbackUri.Path = $"api/webhook/orcid/{orcidId}";
            return callbackUri.Uri.ToString();
        }

        /*
         * URL encode URI
         */
        public string GetUrlEncodedString(string uriToEncode)
        {
            return HttpUtility.UrlEncode(uriToEncode);
        }

        /*
         * Get ORCID webhook registration URI
         */
        public string GetOrcidWebhookRegistrationUri(string orcidId)
        {
            // Get webhook callback uri
            string callBackUri = GetOrcidWebhookCallbackUri(orcidId);
            // URL encode callback uri
            string urlEncodedCallbackUri = GetUrlEncodedString(callBackUri);

            UriBuilder webhookRegistrationUri = new(Configuration["ORCID:WEBHOOK:API"]);
            webhookRegistrationUri.Scheme = "https";
            webhookRegistrationUri.Path = $"{orcidId}/webhook/{urlEncodedCallbackUri}";
            return webhookRegistrationUri.Uri.ToString();
        }

        /*
         * Get ORCID webhook access token
         */
        public string GetOrcidWebhookAccessToken()
        {
            return Configuration["ORCID:WEBHOOK:ACCESSTOKEN"];
        }

        /*
         * Register ORCID webhook.
         * https://info.orcid.org/documentation/api-tutorials/api-tutorial-registering-a-notification-webhook/
         */
        public async Task<bool> RegisterOrcidWebhook(string orcidId)
        {
            // Get ORCID webhook API specific http client.
            HttpClient orcidWebhookApiHttpClient = _httpClientFactory.CreateClient("ORCID_WEBHOOK_API");
            // PUT request
            HttpRequestMessage request = new(method: HttpMethod.Put, requestUri: GetOrcidWebhookRegistrationUri(orcidId));
            // Insert ORCID webhook access token into authorization header for each request.
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", GetOrcidWebhookAccessToken());
            // Send request
            try
            {
                HttpResponseMessage response = await orcidWebhookApiHttpClient.SendAsync(request);
                response.EnsureSuccessStatusCode();
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError($"OrcidApiService: {ex.ToString()}");
                return false;
            }
            return true;
        }

        /*
         * Unregister ORCID webhook.
         * https://info.orcid.org/documentation/api-tutorials/api-tutorial-registering-a-notification-webhook/
         */
        public async Task<bool> UnregisterOrcidWebhook(string orcidId)
        {
            // Get ORCID webhook API specific http client.
            HttpClient orcidWebhookApiHttpClient = _httpClientFactory.CreateClient("ORCID_WEBHOOK_API");
            // DELETE request
            HttpRequestMessage request = new(method: HttpMethod.Delete, requestUri: GetOrcidWebhookRegistrationUri(orcidId));
            // Insert ORCID webhook access token into authorization header for each request.
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", GetOrcidWebhookAccessToken());
            // Send request
            try
            {
                HttpResponseMessage response = await orcidWebhookApiHttpClient.SendAsync(request);
                response.EnsureSuccessStatusCode();
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError($"OrcidApiService: {ex.ToString()}");
                return false;
            }
            return true;
        }

        /*
         * 
         */
        public async Task<string> GetDataFromMemberApi(String path, String orcidAccessToken)
        {
            // Get ORCID member API specific http client.
            HttpClient orcidMemberApiHttpClient = _httpClientFactory.CreateClient("ORCID_MEMBER_API");
            // GET request. Make sure "path" does not start with '/'
            HttpRequestMessage request = new(method: HttpMethod.Get, requestUri: path.TrimStart('/'));
            // Insert ORCID access token into authorization header for each request.
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", orcidAccessToken);
            // Send request
            try
            {
                HttpResponseMessage response = await orcidMemberApiHttpClient.SendAsync(request);
                response.EnsureSuccessStatusCode();
                return await response.Content.ReadAsStringAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError($"OrcidApiService: {ex.ToString()}");
                return "";
            }
        }
    }
}