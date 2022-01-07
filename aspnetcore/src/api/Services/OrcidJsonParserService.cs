﻿using System;
using System.Collections.Generic;
using System.Text.Json;
using api.Models.Orcid;

namespace api.Services
{
    /*
     * OrcidJsonParserService parses elements from user's ORCID json record.
     * 
     * ORCID record schema
     * https://info.orcid.org/documentation/integration-guide/orcid-record/
     */
    public class OrcidJsonParserService
    {
        /*
         * Put code
         */
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

        /*
         * Date
         */
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

        /*
         * Get DOI from ORCID publication
         */
        private string getPublicationDoi(JsonElement workElement)
        {
            string doi = "";
            JsonElement externalIdsElement;
            if (workElement.TryGetProperty("external-ids", out externalIdsElement))
            {
                JsonElement externalIdElement;
                if (externalIdsElement.ValueKind != JsonValueKind.Null && externalIdsElement.TryGetProperty("external-id", out externalIdElement))
                {
                    foreach (JsonElement idElement in externalIdElement.EnumerateArray())
                    {
                        if (idElement.GetProperty("external-id-type").GetString() == "doi")
                        {
                            doi = idElement.GetProperty("external-id-value").GetString();
                        }
                    }
                }
            }
            return doi;
        }

        /*
         * Get publication year from ORCID publication
         */
        private int? getPublicationYear(JsonElement workElement)
        {
            int? publicationYear = null;
            JsonElement publicationDateElement;
            if (workElement.TryGetProperty("publication-date", out publicationDateElement))
            {
                JsonElement yearElement;
                if (publicationDateElement.ValueKind != JsonValueKind.Null && publicationDateElement.TryGetProperty("year", out yearElement))
                {
                    JsonElement valueElement;
                    if (yearElement.ValueKind != JsonValueKind.Null && yearElement.TryGetProperty("value", out valueElement))
                    {
                        publicationYear = Int32.Parse(valueElement.GetString());
                    }
                }
            }
            return publicationYear;
        }

        /*
         * Check if Json document is full ORCID record
         */
        private Boolean isFullRecord(JsonDocument orcidJsonDocument)
        {
            var myValue = new System.Text.Json.JsonElement();
            return orcidJsonDocument.RootElement.TryGetProperty("person", out myValue);
        }

        /*
         * Given names
         */
        public OrcidGivenNames GetGivenNames(String json)
        {
            using (JsonDocument document = JsonDocument.Parse(json))
            {
                JsonElement nameElement;
                JsonElement givenNamesElement;

                // Get name element
                if (this.isFullRecord(document))
                {
                    nameElement = document.RootElement.GetProperty("person").GetProperty("name");
                }
                else
                {
                    nameElement = document.RootElement.GetProperty("name");
                }

                // Check if name element is null
                if (nameElement.ValueKind == JsonValueKind.Null)
                {
                    return new OrcidGivenNames("");
                }
                else
                {
                    givenNamesElement = nameElement.GetProperty("given-names");
                }

                // Get value
                JsonElement valueElement;
                if (givenNamesElement.ValueKind != JsonValueKind.Null && givenNamesElement.TryGetProperty("value", out valueElement))
                {
                    return new OrcidGivenNames(
                        valueElement.GetString()
                    );
                } else
                {
                    return new OrcidGivenNames("");
                }
            }
        }

        /*
         * Family name
         */
        public OrcidFamilyName GetFamilyName(String json)
        {
            using (JsonDocument document = JsonDocument.Parse(json))
            {
                JsonElement nameElement;
                JsonElement familyNameElement;

                // Get name element
                if (this.isFullRecord(document))
                {
                    nameElement = document.RootElement.GetProperty("person").GetProperty("name");
                }
                else
                {
                    nameElement = document.RootElement.GetProperty("name");
                }

                // Check if name element is null
                if (nameElement.ValueKind == JsonValueKind.Null)
                {
                    return new OrcidFamilyName("");
                }
                else
                {
                    familyNameElement = nameElement.GetProperty("family-name");
                }

                // Get value
                JsonElement valueElement;
                if (familyNameElement.ValueKind != JsonValueKind.Null && familyNameElement.TryGetProperty("value", out valueElement))
                {
                    return new OrcidFamilyName(
                        valueElement.GetString()
                    );
                }
                else
                {
                    return new OrcidFamilyName("");
                }
            }
        }

        /*
         * Credit name
         */
        public OrcidCreditName GetCreditName(String json)
        {
            using (JsonDocument document = JsonDocument.Parse(json))
            {
                JsonElement nameElement;
                JsonElement creditNameElement;

                // Get name element
                if (this.isFullRecord(document))
                {
                    nameElement = document.RootElement.GetProperty("person").GetProperty("name");
                }
                else
                {
                    nameElement = document.RootElement.GetProperty("name");
                }

                // Check if name element is null
                if (nameElement.ValueKind == JsonValueKind.Null)
                {
                    return new OrcidCreditName("");
                }
                else
                {
                    creditNameElement = nameElement.GetProperty("credit-name");
                }

                // Get value
                JsonElement valueElement;
                if (creditNameElement.ValueKind != JsonValueKind.Null && creditNameElement.TryGetProperty("value", out valueElement))
                {
                    return new OrcidCreditName(
                        valueElement.GetString()
                    );
                }
                else
                {
                    return new OrcidCreditName("");
                }
            }
        }

        /*
         * Other names
         */
        public List<OrcidOtherName> GetOtherNames(String json)
        {
            JsonElement otherNamesElement;
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

        /*
         * Biography
         */
        public OrcidBiography GetBiography(String json)
        {
            using (JsonDocument document = JsonDocument.Parse(json))
            {
                JsonElement biographyElement;
                JsonElement contentElement;

                if (this.isFullRecord(document))
                {
                    biographyElement = document.RootElement.GetProperty("person").GetProperty("biography");
                }
                else
                {
                    biographyElement = document.RootElement.GetProperty("biography");
                }

                if (biographyElement.ValueKind == JsonValueKind.Null)
                {
                    return null;
                }
                else
                {
                    biographyElement.TryGetProperty("content", out contentElement);

                    if (contentElement.ValueKind.Equals(null))
                    {
                        return null;
                    }
                    else
                    {
                        return new OrcidBiography(
                            contentElement.GetString()
                        );
                    }
                }
            }
        }

        /*
         * Researcher urls
         */
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

        /*
         * Emails
         */
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

        /*
         * Keywords
         */
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

        /*
         * External identifiers
         */
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

        /*
         * Educations
         */
        public List<OrcidEducation> GetEducations(String json)
        {
            var educations = new List <OrcidEducation> { };
            using (JsonDocument document = JsonDocument.Parse(json))
            {
                JsonElement educationsElement = document.RootElement.GetProperty("activities-summary").GetProperty("educations");
                JsonElement affiliationGroupsElement;

                if (educationsElement.TryGetProperty("affiliation-group", out affiliationGroupsElement))
                { 
                    foreach(JsonElement affiliationGroupElement in affiliationGroupsElement.EnumerateArray()) { 
                        JsonElement summariesElement;

                        if (affiliationGroupElement.TryGetProperty("summaries", out summariesElement))
                        {
                            foreach (JsonElement summaryElement in summariesElement.EnumerateArray())
                            {
                                JsonElement educationSummaryElement;
                                if (summaryElement.TryGetProperty("education-summary", out educationSummaryElement)) {
                                    educations.Add(
                                        new OrcidEducation(
                                            educationSummaryElement.GetProperty("organization").GetProperty("name").GetString(),
                                            educationSummaryElement.GetProperty("department-name").GetString(),
                                            educationSummaryElement.GetProperty("role-title").GetString(),
                                            getOrcidDate(educationSummaryElement.GetProperty("start-date")),
                                            getOrcidDate(educationSummaryElement.GetProperty("end-date")),
                                            this.getOrcidPutCode(educationSummaryElement)
                                        )
                                    );
                                }
                            }
                        }
                    }
                }
            }
            return educations;
        }

        /*
         * Employments
         */
        public List<OrcidEmployment> GetEmployments(String json)
        {
            var employments = new List<OrcidEmployment> { };
            using (JsonDocument document = JsonDocument.Parse(json))
            {
                JsonElement employmentsElement = document.RootElement.GetProperty("activities-summary").GetProperty("employments");
                JsonElement affiliationGroupsElement;

                if (employmentsElement.TryGetProperty("affiliation-group", out affiliationGroupsElement))
                {
                    foreach (JsonElement affiliationGroupElement in affiliationGroupsElement.EnumerateArray())
                    {
                        JsonElement summariesElement;

                        if (affiliationGroupElement.TryGetProperty("summaries", out summariesElement))
                        {
                            foreach (JsonElement summaryElement in summariesElement.EnumerateArray())
                            {
                                JsonElement employmentSummaryElement;
                                if (summaryElement.TryGetProperty("employment-summary", out employmentSummaryElement))
                                {
                                    employments.Add(
                                      new OrcidEmployment(
                                          employmentSummaryElement.GetProperty("organization").GetProperty("name").GetString(),
                                          employmentSummaryElement.GetProperty("department-name").GetString(),
                                          employmentSummaryElement.GetProperty("role-title").GetString(),
                                          getOrcidDate(employmentSummaryElement.GetProperty("start-date")),
                                          getOrcidDate(employmentSummaryElement.GetProperty("end-date")),
                                          this.getOrcidPutCode(employmentSummaryElement)
                                      )
                                  );
                                }
                            }
                        }
                    }
                }
            }
            return employments;
        }

        /*
         * Publications
         */
        public List<OrcidPublication> GetPublications(String json)
        {
            var publications = new List<OrcidPublication> { };
            using (JsonDocument document = JsonDocument.Parse(json))
            {
                JsonElement publicationsElement = document.RootElement.GetProperty("activities-summary").GetProperty("works");
                JsonElement groupsElement;

                if (publicationsElement.TryGetProperty("group", out groupsElement))
                {
                    foreach (JsonElement groupElement in groupsElement.EnumerateArray())
                    {
                        JsonElement workSummariesElement;
                        if (groupElement.TryGetProperty("work-summary", out workSummariesElement))
                        {
                            foreach (JsonElement workElement in workSummariesElement.EnumerateArray())
                            {
                                publications.Add(
                                    new OrcidPublication()
                                    {
                                        PublicationName = workElement.GetProperty("title").GetProperty("title").GetProperty("value").GetString(),
                                        DoiHandle = this.getPublicationDoi(workElement),
                                        PublicationYear = this.getPublicationYear(workElement),
                                        Type = workElement.GetProperty("type").GetString(),
                                        PutCode = this.getOrcidPutCode(workElement)
                                    }
                                );
                            }
                        }
                    }
                }
            }
            return publications;
        }
    }
}