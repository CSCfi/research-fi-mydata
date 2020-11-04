using System;
using System.Collections.Generic;
using System.Text.Json;

namespace api.Services
{ 
    public class OrcidJsonParserService
    {
        // Get given names
        public String GetGivenNames(String json)
        {
            using (JsonDocument document = JsonDocument.Parse(json))
            {
                return document.RootElement.GetProperty("person").GetProperty("name").GetProperty("given-names").GetProperty("value").GetString();
            }
        }

        // Get family names
        public String GetFamilyName(String json)
        {
            using (JsonDocument document = JsonDocument.Parse(json))
            {
                return document.RootElement.GetProperty("person").GetProperty("name").GetProperty("family-name").GetProperty("value").GetString();
            }
        }

        // Get biography
        public String GetBiography(String json)
        {
            using (JsonDocument document = JsonDocument.Parse(json))
            {
                return document.RootElement.GetProperty("person").GetProperty("biography").GetProperty("content").GetString();
            }
        }

        // Get web links
        public List<(string LinkName, string LinkUrl)> GetWebLinks(String json)
        {
            var links = new List<(string LinkName, string LinkUrl)> { };
            using (JsonDocument document = JsonDocument.Parse(json))
            {
                foreach (JsonElement element in document.RootElement.GetProperty("person").GetProperty("researcher-urls").GetProperty("researcher-url").EnumerateArray())
                {
                    links.Add(
                        (
                            element.GetProperty("url-name").GetString(),
                            element.GetProperty("url").GetProperty("value").GetString()
                        )
                    );
                }
            }
            return links;
        }
    }
}