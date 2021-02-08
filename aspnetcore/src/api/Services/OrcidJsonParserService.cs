using System;
using System.Collections.Generic;
using System.Text.Json;
using api.Models.Orcid;

namespace api.Services
{ 
    public class OrcidJsonParserService
    {
        private OrcidPutCode getOrcidPutCode(JsonElement orcidJsonElement)
        {
            var putCode = new OrcidPutCode(null);

            // put code
            if (orcidJsonElement.TryGetProperty("put-code", out JsonElement putCodeElement))
            {
                if (putCodeElement.ValueKind == JsonValueKind.Number)
                {
                    putCodeElement.TryGetUInt32(out UInt32 putCodeParsed);
                    putCode.Value = putCodeParsed;
                }
            }

            return putCode;
        }

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
        public OrcidGivenNames GetGivenNames(String json)
        {
            using (JsonDocument document = JsonDocument.Parse(json))
            {
                return new OrcidGivenNames(
                    document.RootElement.GetProperty("person").GetProperty("name").GetProperty("given-names").GetProperty("value").GetString()
                );
            }
        }

        // Get family name
        public OrcidFamilyName GetFamilyName(String json)
        {
            using (JsonDocument document = JsonDocument.Parse(json))
            {
                return new OrcidFamilyName(
                    document.RootElement.GetProperty("person").GetProperty("name").GetProperty("family-name").GetProperty("value").GetString()
                );
            }
        }

        // Get credit name
        public OrcidCreditName GetCreditName(String json)
        {
            using (JsonDocument document = JsonDocument.Parse(json))
            {
                return new OrcidCreditName(
                    document.RootElement.GetProperty("person").GetProperty("name").GetProperty("credit-name").GetProperty("value").GetString()
                );
            }
        }

        // Get other names
        public List<OrcidOtherName> GetOtherNames(String json)
        {
            var otherNames = new List<OrcidOtherName> { };
            using (JsonDocument document = JsonDocument.Parse(json))
            {
                foreach (JsonElement element in document.RootElement.GetProperty("person").GetProperty("other-names").GetProperty("other-name").EnumerateArray())
                {
                    var value = element.GetProperty("content").GetString();
                    var putCode = this.getOrcidPutCode(element);
                    otherNames.Add(
                        new OrcidOtherName(value, putCode)
                    );
                }
            }
            return otherNames;
        }

        // Get biography
        public OrcidBiography GetBiography(String json)
        {
            using (JsonDocument document = JsonDocument.Parse(json))
            {
                var biographyElement = document.RootElement.GetProperty("person").GetProperty("biography");
                var value = biographyElement.GetProperty("content").GetString();
                var putCode = this.getOrcidPutCode(biographyElement);

                return new OrcidBiography(
                    value,
                    putCode
                );
            }
        }

        // Get researcher urls
        public List<OrcidResearcherUrl> GetResearcherUrls(String json)
        {
            var urls = new List<OrcidResearcherUrl> { };
            using (JsonDocument document = JsonDocument.Parse(json))
            {
                foreach (JsonElement element in document.RootElement.GetProperty("person").GetProperty("researcher-urls").GetProperty("researcher-url").EnumerateArray())
                {
                    urls.Add(
                        new OrcidResearcherUrl(
                            element.GetProperty("url-name").GetString(),
                            element.GetProperty("url").GetProperty("value").GetString(),
                            this.getOrcidPutCode(element)
                        )
                    );
                }
            }
            return urls;
        }

        // Get emails
        public List<OrcidEmail> GetEmails(String json)
        {
            var emails = new List<OrcidEmail> { };
            using (JsonDocument document = JsonDocument.Parse(json))
            {
                foreach (JsonElement element in document.RootElement.GetProperty("person").GetProperty("emails").GetProperty("email").EnumerateArray())
                {
                    emails.Add(
                        new OrcidEmail(
                            element.GetProperty("email").GetString(),
                            this.getOrcidPutCode(element)
                        )
                    );
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