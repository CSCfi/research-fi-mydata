using System;
using System.Net.Http;
using System.Threading.Tasks;

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
            client.BaseAddress = new Uri("https://pub.orcid.org/v3.0/");
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

        // Get ORCID record
        public async Task<String> GetRecord(String orcidId)
        {
            string result = string.Empty;
            var url = GetUrlRecord(orcidId);
            HttpResponseMessage response = await Client.GetAsync(url);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadAsStringAsync();
        }

        // Get ORCID personal details
        public async Task<String> GetPersonalDetails(String orcidId)
        {
            string result = string.Empty;
            var url = GetUrlPersonalDetails(orcidId);
            HttpResponseMessage response = await Client.GetAsync(url);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadAsStringAsync();
        }
    }
}