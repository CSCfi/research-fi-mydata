using System;
using System.Collections.Generic;
using System.Text.Json;

namespace api.Services
{ 
    public class OrcidJsonParserService
    {
        private (UInt16?, UInt16?, UInt16?) getDateTimeComponents(JsonElement orcidJsonDateElement)
        {
            UInt16? year = null;
            UInt16? month = null;
            UInt16? day = null;

            if (orcidJsonDateElement.ValueKind != JsonValueKind.Null)
            {
                // Year
                orcidJsonDateElement.TryGetProperty("year", out var yearElement);
                if (yearElement.ValueKind != JsonValueKind.Null)
                {
                    year = UInt16.Parse(yearElement.GetProperty("value").GetString());
                }

                // Month
                orcidJsonDateElement.TryGetProperty("month", out var monthElement);
                if (monthElement.ValueKind != JsonValueKind.Null)
                {
                    month = UInt16.Parse(monthElement.GetProperty("value").GetString());
                }

                // Day
                orcidJsonDateElement.TryGetProperty("day", out var dayElement);
                if (dayElement.ValueKind != JsonValueKind.Null)
                {
                    day = UInt16.Parse(dayElement.GetProperty("value").GetString());
                }
            }
            return (year, month, day);
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
        public List<(string organizationName, string departmentName, string roleTitle, UInt16? startYear, UInt16? startMonth, UInt16? startDay, UInt16? endYear, UInt16? endMonth, UInt16? endDay)> GetEducations(String json)
        {
            var educations = new List <(string organizationName, string departmentName, string roleTitle, UInt16? startYear, UInt16? startMonth, UInt16? startDay, UInt16? endYear, UInt16? endMonth, UInt16? endDay)> { };
            using (JsonDocument document = JsonDocument.Parse(json))
            {
                foreach (JsonElement element in document.RootElement.GetProperty("activities-summary").GetProperty("educations").GetProperty("education-summary").EnumerateArray())
                {
                    var startDateResult = getDateTimeComponents(element.GetProperty("start-date"));
                    var endDateResult = getDateTimeComponents(element.GetProperty("end-date"));

                    educations.Add(
                        (
                            organizationName: element.GetProperty("organization").GetProperty("name").GetString(),
                            departmentName: element.GetProperty("department-name").GetString(),
                            roleTitle: element.GetProperty("role-title").GetString(),
                            startYear: startDateResult.Item1,
                            startMonth: startDateResult.Item2,
                            startDay: startDateResult.Item3,
                            endYear: endDateResult.Item1,
                            endMonth: endDateResult.Item2,
                            endDay: endDateResult.Item3
                        )
                    );
                }
            }
            return educations;
        }

        // Get employments
        public List<(string organizationName, string departmentName, string roleTitle, UInt16? startYear, UInt16? startMonth, UInt16? startDay, UInt16? endYear, UInt16? endMonth, UInt16? endDay)> GetEmployments(String json)
        {
            var employments = new List<(string organizationName, string departmentName, string roleTitle, UInt16? startYear, UInt16? startMonth, UInt16? startDay, UInt16? endYear, UInt16? endMonth, UInt16? endDay)> { };
            using (JsonDocument document = JsonDocument.Parse(json))
            {
                foreach (JsonElement element in document.RootElement.GetProperty("activities-summary").GetProperty("employments").GetProperty("employment-summary").EnumerateArray())
                {
                    var startDateResult = getDateTimeComponents(element.GetProperty("start-date"));
                    var endDateResult = getDateTimeComponents(element.GetProperty("end-date"));

                    employments.Add(
                        (
                            organizationName: element.GetProperty("organization").GetProperty("name").GetString(),
                            departmentName: element.GetProperty("department-name").GetString(),
                            roleTitle: element.GetProperty("role-title").GetString(),
                            startYear: startDateResult.Item1,
                            startMonth: startDateResult.Item2,
                            startDay: startDateResult.Item3,
                            endYear: endDateResult.Item1,
                            endMonth: endDateResult.Item2,
                            endDay: endDateResult.Item3
                        )
                    );
                }
            }
            return employments;
        }
    }
}