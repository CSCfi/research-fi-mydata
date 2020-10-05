using System;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace api.Services
{ 
    public class OrcidService
    {
        public HttpClient Client { get; }

        public OrcidService(HttpClient client)
        {
            // https://pub.orcid.org/v3.0/0000-0002-5173-6291/record
            client.BaseAddress = new Uri("https://pub.orcid.org/v3.0/");
            // GitHub API versioning
            client.DefaultRequestHeaders.Add("Accept", "application/json");
            Client = client;
        }


        public async Task<String> GetRecord(String orcid)
        {
            string result = string.Empty;
            var uri = Client.BaseAddress + orcid + "/record";

            HttpResponseMessage response = await Client.GetAsync(uri);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadAsStringAsync();
        }
    }
}