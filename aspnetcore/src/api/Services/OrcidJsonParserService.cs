using System;
using System.Collections.Generic;
using System.Text.Json;

namespace api.Services
{ 
    public class OrcidJsonParserService
    {
        private DateTime getDateTime(JsonElement orcidJsonDateElement)
        {
            return new DateTime(
                int.Parse(orcidJsonDateElement.GetProperty("year").GetProperty("value").GetString()),
                int.Parse(orcidJsonDateElement.GetProperty("month").GetProperty("value").GetString()),
                int.Parse(orcidJsonDateElement.GetProperty("day").GetProperty("value").GetString()),
                0,
                0,
                0
            );
        }

        // Get given names
        public String GetGivenNames(String json)
        {
            using (JsonDocument document = JsonDocument.Parse(json))
            {
                return document.RootElement.GetProperty("person").GetProperty("name").GetProperty("given-names").GetProperty("value").GetString();
            }
        }

        // Get family name
        public String GetFamilyName(String json)
        {
            using (JsonDocument document = JsonDocument.Parse(json))
            {
                return document.RootElement.GetProperty("person").GetProperty("name").GetProperty("family-name").GetProperty("value").GetString();
            }
        }

        // Get credit name
        public String GetCreditName(String json)
        {
            using (JsonDocument document = JsonDocument.Parse(json))
            {
                return document.RootElement.GetProperty("person").GetProperty("name").GetProperty("credit-name").GetProperty("value").GetString();
            }
        }

        // Get other names
        public List<string> GetOtherNames(String json)
        {
            var otherNames = new List<string> { };
            using (JsonDocument document = JsonDocument.Parse(json))
            {
                foreach (JsonElement element in document.RootElement.GetProperty("person").GetProperty("other-names").GetProperty("other-name").EnumerateArray())
                {
                    otherNames.Add(element.GetProperty("content").GetString());
                }
            }
            return otherNames;
        }

        // Get biography
        public String GetBiography(String json)
        {
            using (JsonDocument document = JsonDocument.Parse(json))
            {
                return document.RootElement.GetProperty("person").GetProperty("biography").GetProperty("content").GetString();
            }
        }

        // Get researcher urls
        public List<(string LinkName, string LinkUrl)> GetResearcherUrls(String json)
        {
            var urls = new List<(string UrlName, string Url)> { };
            using (JsonDocument document = JsonDocument.Parse(json))
            {
                foreach (JsonElement element in document.RootElement.GetProperty("person").GetProperty("researcher-urls").GetProperty("researcher-url").EnumerateArray())
                {
                    urls.Add(
                        (
                            UrlName: element.GetProperty("url-name").GetString(),
                            Url: element.GetProperty("url").GetProperty("value").GetString()
                        )
                    );
                }
            }
            return urls;
        }

        // Get emails
        public List<string> GetEmails(String json)
        {
            var emails = new List<string> { };
            using (JsonDocument document = JsonDocument.Parse(json))
            {
                foreach (JsonElement element in document.RootElement.GetProperty("person").GetProperty("emails").GetProperty("email").EnumerateArray())
                {
                    emails.Add(element.GetProperty("email").GetString());
                }
            }
            return emails;
        }

        // Get keywords
        public List<string> GetKeywords(String json)
        {
            var keywords = new List<string> { };
            using (JsonDocument document = JsonDocument.Parse(json))
            {
                foreach (JsonElement element in document.RootElement.GetProperty("person").GetProperty("keywords").GetProperty("keyword").EnumerateArray())
                {
                    keywords.Add(element.GetProperty("content").GetString());
                }
            }
            return keywords;
        }

        // Get external identifiers
        public List<(string externalIdType, string externalIdValue, string externalIdUrl)> GetExternalIdentifiers(String json)
        {
            var externalIdentifiers = new List<(string externalIdType, string externalIdValue, string externalIdUrl)> { };
            using (JsonDocument document = JsonDocument.Parse(json))
            {
                foreach (JsonElement element in document.RootElement.GetProperty("person").GetProperty("external-identifiers").GetProperty("external-identifier").EnumerateArray())
                {
                    externalIdentifiers.Add(
                        (
                            externalIdType: element.GetProperty("external-id-type").GetString(),
                            externalIdValue: element.GetProperty("external-id-value").GetString(),
                            externalIdUrl: element.GetProperty("external-id-url").GetProperty("value").GetString()
                        )
                    );
                }
            }
            return externalIdentifiers;
        }

        // Get educations
        public List<(string organizationName, string departmentName, string roleTitle, DateTime startDate, DateTime endDate)> GetEducations(String json)
        {
            var educations = new List <(string organizationName, string departmentName, string roleTitle, DateTime startDate, DateTime endDate)> { };
            using (JsonDocument document = JsonDocument.Parse(json))
            {
                foreach (JsonElement element in document.RootElement.GetProperty("activities-summary").GetProperty("educations").GetProperty("education-summary").EnumerateArray())
                {
                    educations.Add(
                        (
                            organizationName: element.GetProperty("organization").GetProperty("name").GetString(),
                            departmentName: element.GetProperty("department-name").GetString(),
                            roleTitle: element.GetProperty("role-title").GetString(),
                            startDate: getDateTime(element.GetProperty("start-date")),
                            endDate: getDateTime(element.GetProperty("end-date"))

                        )
                    );
                }
            }
            return educations;
        }
    }
}