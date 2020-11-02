using System;
using System.Text.Json;

namespace api.Services
{ 
    public class OrcidJsonParserService
    {
        public String GetBiography(String json)
        {
            using (JsonDocument document = JsonDocument.Parse(json))
            {
                return document.RootElement.GetProperty("person").GetProperty("biography").GetProperty("content").GetString();
            }
        }
    }
}