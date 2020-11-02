using System;
using System.Collections.Generic;
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