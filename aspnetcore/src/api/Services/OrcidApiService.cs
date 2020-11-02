using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace api.Services
{ 
    public class OrcidApiService
    {
        public HttpClient Client { get; }

        public OrcidApiService(HttpClient client)
        {
            client.BaseAddress = new Uri("https://pub.orcid.org/v3.0/");
            client.DefaultRequestHeaders.Add("Accept", "application/json");
            Client = client;
        }

        public String GetUrl(String orcidId)
        {
            return Client.BaseAddress + orcidId + "/record";
        }

        public async Task<String> GetJson(String orcidId)
        {
            string result = string.Empty;
            var url = GetUrl(orcidId);
            HttpResponseMessage response = await Client.GetAsync(url);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadAsStringAsync();
        }
    }
}