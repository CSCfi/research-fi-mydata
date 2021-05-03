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
                    putCodeElement.TryGetInt32(out Int32 putCodeParsed);
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

        // Check if Json document is full ORCID record
        private Boolean isFullRecord(JsonDocument orcidJsonDocument)
        {
            var myValue = new System.Text.Json.JsonElement();
            return orcidJsonDocument.RootElement.TryGetProperty("person", out myValue);
        }

        // Get given names
        public OrcidGivenNames GetGivenNames(String json)
        {
            using (JsonDocument document = JsonDocument.Parse(json))
            {
                if (this.isFullRecord(document))
                {
                    return new OrcidGivenNames(
                        document.RootElement.GetProperty("person").GetProperty("name").GetProperty("given-names").GetProperty("value").GetString()
                    );
                }
                else
                {
                    return new OrcidGivenNames(
                        document.RootElement.GetProperty("name").GetProperty("given-names").GetProperty("value").GetString()
                    );
                }
            }
        }

        // Get family name
        public OrcidFamilyName GetFamilyName(String json)
        {
            using (JsonDocument document = JsonDocument.Parse(json))
            {
                if (this.isFullRecord(document))
                {
                    return new OrcidFamilyName(
                        document.RootElement.GetProperty("person").GetProperty("name").GetProperty("family-name").GetProperty("value").GetString()
                    );
                }
                else
                {
                    return new OrcidFamilyName(
                        document.RootElement.GetProperty("name").GetProperty("family-name").GetProperty("value").GetString()
                    );
                }
            }
        }

        // Get credit name
        public OrcidCreditName GetCreditName(String json)
        {
            using (JsonDocument document = JsonDocument.Parse(json))
            {
                if (this.isFullRecord(document))
                {
                    return new OrcidCreditName(
                        document.RootElement.GetProperty("person").GetProperty("name").GetProperty("credit-name").GetProperty("value").GetString()
                    );
                }
                else
                {
                    return new OrcidCreditName(
                        document.RootElement.GetProperty("name").GetProperty("credit-name").GetProperty("value").GetString()
                    );
                }
            }
        }

        // Get other names
        public List<OrcidOtherName> GetOtherNames(String json)
        {
            var otherNamesElement = new JsonElement();
            var otherNames = new List<OrcidOtherName> { };
            using (JsonDocument document = JsonDocument.Parse(json))
            {
                if (this.isFullRecord(document))
                {
                    otherNamesElement = document.RootElement.GetProperty("person").GetProperty("other-names");
                }
                else
                {
                    otherNamesElement = document.RootElement.GetProperty("other-names");
                }

                foreach (JsonElement element in otherNamesElement.GetProperty("other-name").EnumerateArray())
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
                var biographyElement = new JsonElement();

                if (this.isFullRecord(document))
                {
                    biographyElement = document.RootElement.GetProperty("person").GetProperty("biography");
                }
                else
                {
                    biographyElement = document.RootElement.GetProperty("biography");
                }
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
        public List<OrcidKeyword> GetKeywords(String json)
        {
            var keywords = new List<OrcidKeyword> { };
            using (JsonDocument document = JsonDocument.Parse(json))
            {
                foreach (JsonElement element in document.RootElement.GetProperty("person").GetProperty("keywords").GetProperty("keyword").EnumerateArray())
                {
                    keywords.Add(
                        new OrcidKeyword(
                            element.GetProperty("content").GetString(),
                            this.getOrcidPutCode(element)
                        )
                    );
                }
            }
            return keywords;
        }

        // Get external identifiers
        public List<OrcidExternalIdentifier> GetExternalIdentifiers(String json)
        {
            var externalIdentifiers = new List<OrcidExternalIdentifier> { };
            using (JsonDocument document = JsonDocument.Parse(json))
            {
                foreach (JsonElement element in document.RootElement.GetProperty("person").GetProperty("external-identifiers").GetProperty("external-identifier").EnumerateArray())
                {
                    externalIdentifiers.Add(
                        new OrcidExternalIdentifier(
                            element.GetProperty("external-id-type").GetString(),
                            element.GetProperty("external-id-value").GetString(),
                            element.GetProperty("external-id-url").GetProperty("value").GetString(),
                            this.getOrcidPutCode(element)
                        )
                    );
                }
            }
            return externalIdentifiers;
        }

        // Get educations
        public List<OrcidEducation> GetEducations(String json)
        {
            var educations = new List <OrcidEducation> { };
            using (JsonDocument document = JsonDocument.Parse(json))
            {
                foreach (JsonElement element in document.RootElement.GetProperty("activities-summary").GetProperty("educations").GetProperty("education-summary").EnumerateArray())
                {
                    educations.Add(
                        new OrcidEducation(
                            element.GetProperty("organization").GetProperty("name").GetString(),
                            element.GetProperty("department-name").GetString(),
                            element.GetProperty("role-title").GetString(),
                            getOrcidDate(element.GetProperty("start-date")),
                            getOrcidDate(element.GetProperty("end-date")),
                            this.getOrcidPutCode(element)
                        )
                    );
                }
            }
            return educations;
        }

        // Get employments
        public List<OrcidEmployment> GetEmployments(String json)
        {
            var employments = new List<OrcidEmployment> { };
            using (JsonDocument document = JsonDocument.Parse(json))
            {
                foreach (JsonElement element in document.RootElement.GetProperty("activities-summary").GetProperty("employments").GetProperty("employment-summary").EnumerateArray())
                {
                    employments.Add(
                        new OrcidEmployment(
                            element.GetProperty("organization").GetProperty("name").GetString(),
                            element.GetProperty("department-name").GetString(),
                            element.GetProperty("role-title").GetString(),
                            getOrcidDate(element.GetProperty("start-date")),
                            getOrcidDate(element.GetProperty("end-date")),
                            this.getOrcidPutCode(element)
                        )
                    );
                }
            }
            return employments;
        }
    }
}