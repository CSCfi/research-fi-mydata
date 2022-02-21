using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Text.Json;

namespace api.Services
{
    /*
     * OrcidApiService handles ORCID api http requests.
     */
    public class OrcidApiService
    {
        public HttpClient Client { get; }

        public OrcidApiService(HttpClient client)
        {
            // TODO: Get ORCID API address from configuration
            // client.BaseAddress = new Uri("https://pub.orcid.org/v3.0/");
            client.BaseAddress = new Uri("https://api.sandbox.orcid.org/v3.0/");
            client.DefaultRequestHeaders.Add("Accept", "application/json");
            Client = client;
        }

        public String GetUrlRecord(String orcidId)
        {
            return Client.BaseAddress + orcidId + "/record";
        }

        public String GetUrlPersonalDetails(String orcidId)
        {
            return Client.BaseAddress + orcidId + "/personal-details";
        }

        // Get ORCID record.
        // ORCID API call must include user's access token in authorization header.
        // MUST NOT set the authorization via Client.DefaultRequestHeaders.
        public async Task<String> GetRecord(String orcidId, String orcidAccessToken)
        {
            // TODO: check orcidId and orcidAccessToken
            var uri = GetUrlRecord(orcidId);
            var requestMessage = new HttpRequestMessage
            {
                Method = HttpMethod.Get,
                RequestUri = new Uri(uri)
            };
            // Set the authorization header for each request.
            requestMessage.Headers.Authorization = new AuthenticationHeaderValue("Bearer", orcidAccessToken);
            HttpResponseMessage response = await Client.SendAsync(requestMessage);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadAsStringAsync();
        }

        /*
        // Get ORCID personal details
        public async Task<String> GetPersonalDetails(String orcidId)
        {
            string result = string.Empty;
            var url = GetUrlPersonalDetails(orcidId);
            HttpResponseMessage response = await Client.GetAsync(url);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadAsStringAsync();
        }
        */
    }
}