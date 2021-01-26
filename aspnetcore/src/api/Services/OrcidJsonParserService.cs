using System;
using System.Collections.Generic;
using System.Text.Json;
using api.Models;

namespace api.Services
{ 
    public class OrcidJsonParserService
    {
        private OrcidDate getOrcidDate(JsonElement orcidJsonDateElement)
        {
            var orcidDate = new OrcidDate();

            if (orcidJsonDateElement.ValueKind != JsonValueKind.Null)
            {
                // Year
                orcidJsonDateElement.TryGetProperty("year", out var yearElement);
                if (yearElement.ValueKind != JsonValueKind.Null)
                {
                    orcidDate.Year = int.Parse(yearElement.GetProperty("value").GetString());
                }

                // Month
                orcidJsonDateElement.TryGetProperty("month", out var monthElement);
                if (monthElement.ValueKind != JsonValueKind.Null)
                {
                    orcidDate.Month = int.Parse(monthElement.GetProperty("value").GetString());
                }

                // Day
                orcidJsonDateElement.TryGetProperty("day", out var dayElement);
                if (dayElement.ValueKind != JsonValueKind.Null)
                {
                    orcidDate.Day = int.Parse(dayElement.GetProperty("value").GetString());
                }
            }

            return orcidDate;
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
        public List<(string organizationName, string departmentName, string roleTitle, OrcidDate startDate, OrcidDate endDate)> GetEducations(String json)
        {
            var educations = new List <(string organizationName, string departmentName, string roleTitle, OrcidDate startDate, OrcidDate endDate)> { };
            using (JsonDocument document = JsonDocument.Parse(json))
            {
                foreach (JsonElement element in document.RootElement.GetProperty("activities-summary").GetProperty("educations").GetProperty("education-summary").EnumerateArray())
                {
                    educations.Add(
                        (
                            organizationName: element.GetProperty("organization").GetProperty("name").GetString(),
                            departmentName: element.GetProperty("department-name").GetString(),
                            roleTitle: element.GetProperty("role-title").GetString(),
                            startDate: getOrcidDate(element.GetProperty("start-date")),
                            endDate: getOrcidDate(element.GetProperty("end-date"))
                        )
                    );
                }
            }
            return educations;
        }

        // Get employments
        public List<(string organizationName, string departmentName, string roleTitle, OrcidDate startDate, OrcidDate endDate)> GetEmployments(String json)
        {
            var employments = new List<(string organizationName, string departmentName, string roleTitle, OrcidDate startDate, OrcidDate endDate)> { };
            using (JsonDocument document = JsonDocument.Parse(json))
            {
                foreach (JsonElement element in document.RootElement.GetProperty("activities-summary").GetProperty("employments").GetProperty("employment-summary").EnumerateArray())
                {
                    employments.Add(
                        (
                            organizationName: element.GetProperty("organization").GetProperty("name").GetString(),
                            departmentName: element.GetProperty("department-name").GetString(),
                            roleTitle: element.GetProperty("role-title").GetString(),
                            startDate: getOrcidDate(element.GetProperty("start-date")),
                            endDate: getOrcidDate(element.GetProperty("end-date"))
                        )
                    );
                }
            }
            return employments;
        }
    }
}