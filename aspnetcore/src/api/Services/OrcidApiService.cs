using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace api.Services
{
    /*
     * OrcidApiService handles ORCID api http requests.
     */
    public class OrcidApiService : IOrcidApiService
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public OrcidApiService(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        /*
         * Get path for retrieving ORCID record. 
         */
        public String GetOrcidRecordPath(String orcidId)
        {
            return orcidId + "/record";
        }

        /*
         * Get ORCID record.
         * ORCID API call MUST include user's ORCID access token in the authorization header.
         */
        public async Task<String> GetRecord(String orcidId, String orcidAccessToken)
        {
            // TODO: check orcidId and orcidAccessToken

            // Get ORCID specific http client.
            HttpClient orcidApiHttpClient = _httpClientFactory.CreateClient("ORCID");
            // Get path for retrieving ORCID record. API base address is already set in http client.
            string orcidRecordPath = GetOrcidRecordPath(orcidId);
            // GET request
            HttpRequestMessage request = new(method: HttpMethod.Get, requestUri: orcidRecordPath);
            // Insert ORCID access token into authorization header for each request.
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", orcidAccessToken);
            HttpResponseMessage response = await orcidApiHttpClient.SendAsync(request);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadAsStringAsync();
        }
    }
}