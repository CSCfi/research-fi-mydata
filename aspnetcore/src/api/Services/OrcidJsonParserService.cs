using System;
using System.Collections.Generic;
using System.Text.Json;
using api.Models.Common;
using api.Models.Orcid;

namespace api.Services
{
    /*
     * OrcidJsonParserService parses elements from user's ORCID json record.
     * 
     * ORCID record schema
     * https://info.orcid.org/documentation/integration-guide/orcid-record/
     */
    public class OrcidJsonParserService : IOrcidJsonParserService
    {
        /*
         * List of ORCID work types, which in research.fi are treated as publications
         * https://info.orcid.org/ufaqs/what-work-types-does-orcid-support/
         * https://wiki.eduuni.fi/pages/viewpage.action?pageId=285247018
         */
        List<string> orcidWorkType_publications = new List<string>
        {
            "book",
            "book-chapter",
            "book-review",
            "dictionary-entry",
            "dissertation",
            "dissertation-thesis",
            "encyclopaedia-entry",
            "edited-book",
            "journal-article",
            "journal-issue",
            "magazine-article",
            "manual",
            "online-resource",
            "newsletter-article",
            "newspaper-article",
            "preprint",
            "report",
            "review",
            "research-tool",
            "test",
            "website",
            "working-paper"
        };

        /*
         * List of ORCID work types, which in research.fi are treated as research activities
         * https://info.orcid.org/ufaqs/what-work-types-does-orcid-support/
         * https://wiki.eduuni.fi/pages/viewpage.action?pageId=285247018
         */
        List<string> orcidWorkType_researchActivities = new List<string>
        {
            "conference-abstract",
            "conference-paper",
            "conference-poster",
            "lecture-speech",
            "other",
            "supervised-student-publication",
            "translation"
        };

        /*
         * ORCID dataset work types
         * https://info.orcid.org/ufaqs/what-work-types-does-orcid-support/
         */
        string orcidWorkType_dataset = "data-set";


        /*
         * Get ORCID putcode
         */
        private OrcidPutCode GetOrcidPutCode(JsonElement orcidJsonElement)
        {
            OrcidPutCode putCode = new(null);

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
         * Check that ORCID work type is publication
         */
        public bool IsPublication(string orcidWorkType)
        {
            return orcidWorkType_publications.Contains(orcidWorkType);
        }

        /*
         * Check that ORCID work type is publication
         */
        public bool IsResearchActivity(string orcidWorkType)
        {
            return orcidWorkType_researchActivities.Contains(orcidWorkType);
        }

        /*
         * Check that ORCID work type is dataset
         */
        public bool IsDataset(string orcidWorkType)
        {
            return orcidWorkType == orcidWorkType_dataset;
        }

        /*
         * Map ORCID work type to DimReferencedata.CodeValue
         */
        public string MapOrcidWorkTypeToDimReferencedataCodeValue(string workType)
        {
            switch (workType)
            {
                case "conference-abstract":
                    return Constants.OrcidResearchActivity_To_ReferenceDataCodeValue.WORK_CONFERENCE;
                case "conference-paper":
                    return Constants.OrcidResearchActivity_To_ReferenceDataCodeValue.WORK_CONFERENCE;
                case "conference-poster":
                    return Constants.OrcidResearchActivity_To_ReferenceDataCodeValue.WORK_CONFERENCE;
                case "lecture-speech":
                    return Constants.OrcidResearchActivity_To_ReferenceDataCodeValue.WORK_LECTURE_SPEECH;
                case "other":
                    return Constants.OrcidResearchActivity_To_ReferenceDataCodeValue.WORK_OTHER;
                case "supervised-student-publication":
                    return Constants.OrcidResearchActivity_To_ReferenceDataCodeValue.WORK_SUPERVISED_STUDENT_PUBLICATION;
                case "translation":
                    return Constants.OrcidResearchActivity_To_ReferenceDataCodeValue.WORK_TRANSLATION;
            }
            return "";
        }

        /*
         * Date
         */
        private OrcidDate GetOrcidDate(JsonElement orcidJsonDateElement)
        {
            OrcidDate orcidDate = new();

            if (orcidJsonDateElement.ValueKind != JsonValueKind.Null)
            {
                // Year
                orcidJsonDateElement.TryGetProperty("year", out JsonElement yearElement);
                if (yearElement.ValueKind != JsonValueKind.Null)
                {
                    orcidDate.Year = int.Parse(yearElement.GetProperty("value").GetString());
                }

                // Month
                orcidJsonDateElement.TryGetProperty("month", out JsonElement monthElement);
                if (monthElement.ValueKind != JsonValueKind.Null)
                {
                    orcidDate.Month = int.Parse(monthElement.GetProperty("value").GetString());
                }

                // Day
                orcidJsonDateElement.TryGetProperty("day", out JsonElement dayElement);
                if (dayElement.ValueKind != JsonValueKind.Null)
                {
                    orcidDate.Day = int.Parse(dayElement.GetProperty("value").GetString());
                }
            }

            return orcidDate;
        }

        /*
         * Get DOI from ORCID work
         */
        private string GetDoi(JsonElement externalIdsElement)
        {
            string doi = "";
            if (externalIdsElement.ValueKind != JsonValueKind.Null && externalIdsElement.TryGetProperty("external-id", out JsonElement externalIdElement))
            {
                foreach (JsonElement idElement in externalIdElement.EnumerateArray())
                {
                    if (idElement.GetProperty("external-id-type").GetString() == "doi")
                    {
                        doi = idElement.GetProperty("external-id-value").GetString();
                        break;
                    }
                }
            }
            return doi;
        }

        /*
         * Get publication year from ORCID publication
         */
        private int? GetPublicationYear(JsonElement workElement)
        {
            int? publicationYear = null;
            if (workElement.TryGetProperty("publication-date", out JsonElement publicationDateElement))
            {
                if (publicationDateElement.ValueKind != JsonValueKind.Null && publicationDateElement.TryGetProperty("year", out JsonElement yearElement))
                {
                    if (yearElement.ValueKind != JsonValueKind.Null && yearElement.TryGetProperty("value", out JsonElement valueElement))
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
        private Boolean IsFullRecord(JsonDocument orcidJsonDocument)
        {
            //var myValue = new System.Text.Json.JsonElement();
            //return orcidJsonDocument.RootElement.TryGetProperty("person", out myValue);

            return orcidJsonDocument.RootElement.TryGetProperty("person", out _);
        }

        /*
         * Given names
         */
        public OrcidGivenNames GetGivenNames(String json)
        {
            using JsonDocument document = JsonDocument.Parse(json);

            // Get name element
            JsonElement nameElement = IsFullRecord(document)
                ? document.RootElement.GetProperty("person").GetProperty("name")
                : document.RootElement.GetProperty("name");

            JsonElement givenNamesElement;
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
            if (givenNamesElement.ValueKind != JsonValueKind.Null && givenNamesElement.TryGetProperty("value", out JsonElement valueElement))
            {
                return new OrcidGivenNames(
                    valueElement.GetString()
                );
            }
            else
            {
                return new OrcidGivenNames("");
            }
        }

        /*
         * Family name
         */
        public OrcidFamilyName GetFamilyName(String json)
        {
            using JsonDocument document = JsonDocument.Parse(json);

            // Get name element
            JsonElement nameElement = IsFullRecord(document)
                ? document.RootElement.GetProperty("person").GetProperty("name")
                : document.RootElement.GetProperty("name");

            JsonElement familyNameElement;
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
            if (familyNameElement.ValueKind != JsonValueKind.Null && familyNameElement.TryGetProperty("value", out JsonElement valueElement))
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

        /*
         * Credit name
         */
        public OrcidCreditName GetCreditName(String json)
        {
            using JsonDocument document = JsonDocument.Parse(json);

            // Get name element
            JsonElement nameElement = IsFullRecord(document)
                ? document.RootElement.GetProperty("person").GetProperty("name")
                : document.RootElement.GetProperty("name");

            JsonElement creditNameElement;
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
            if (creditNameElement.ValueKind != JsonValueKind.Null && creditNameElement.TryGetProperty("value", out JsonElement valueElement))
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

        /*
         * Other names
         */
        public List<OrcidOtherName> GetOtherNames(String json)
        {
            List<OrcidOtherName> otherNames = new() { };
            using (JsonDocument document = JsonDocument.Parse(json))
            {
                JsonElement otherNamesElement = IsFullRecord(document)
                    ? document.RootElement.GetProperty("person").GetProperty("other-names")
                    : document.RootElement.GetProperty("other-names");

                foreach (JsonElement element in otherNamesElement.GetProperty("other-name").EnumerateArray())
                {
                    string value = element.GetProperty("content").GetString();
                    OrcidPutCode putCode = this.GetOrcidPutCode(element);
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
            using JsonDocument document = JsonDocument.Parse(json);

            JsonElement biographyElement = IsFullRecord(document)
                ? document.RootElement.GetProperty("person").GetProperty("biography")
                : document.RootElement.GetProperty("biography");

            if (biographyElement.ValueKind == JsonValueKind.Null)
            {
                return null;
            }
            else
            {
                biographyElement.TryGetProperty("content", out JsonElement contentElement);

                if (contentElement.ValueKind.Equals(null))
                {
                    return null;
                }
                else
                {
                    return new OrcidBiography(
                        value: contentElement.GetString()
                    );
                }
            }
        }

        /*
         * Researcher urls
         */
        public List<OrcidResearcherUrl> GetResearcherUrls(String json)
        {
            List<OrcidResearcherUrl> urls = new() { };
            using (JsonDocument document = JsonDocument.Parse(json))
            {
                foreach (JsonElement element in document.RootElement.GetProperty("person").GetProperty("researcher-urls").GetProperty("researcher-url").EnumerateArray())
                {
                    urls.Add(
                        new OrcidResearcherUrl(
                            urlName: element.GetProperty("url-name").GetString(),
                            url: element.GetProperty("url").GetProperty("value").GetString(),
                            putCode: this.GetOrcidPutCode(element)
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
            List<OrcidEmail> emails = new() { };
            using (JsonDocument document = JsonDocument.Parse(json))
            {
                foreach (JsonElement element in document.RootElement.GetProperty("person").GetProperty("emails").GetProperty("email").EnumerateArray())
                {
                    emails.Add(
                        new OrcidEmail(
                            value: element.GetProperty("email").GetString(),
                            putCode: this.GetOrcidPutCode(element)
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
            List<OrcidKeyword> keywords = new() { };
            using (JsonDocument document = JsonDocument.Parse(json))
            {
                foreach (JsonElement element in document.RootElement.GetProperty("person").GetProperty("keywords").GetProperty("keyword").EnumerateArray())
                {
                    keywords.Add(
                        new OrcidKeyword(
                            value: element.GetProperty("content").GetString(),
                            putCode: this.GetOrcidPutCode(element)
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
            List<OrcidExternalIdentifier> externalIdentifiers = new() { };
            using (JsonDocument document = JsonDocument.Parse(json))
            {
                foreach (JsonElement element in document.RootElement.GetProperty("person").GetProperty("external-identifiers").GetProperty("external-identifier").EnumerateArray())
                {
                    externalIdentifiers.Add(
                        new OrcidExternalIdentifier(
                            externalIdType: element.GetProperty("external-id-type").GetString(),
                            externalIdValue: element.GetProperty("external-id-value").GetString(),
                            externalIdUrl: element.GetProperty("external-id-url").GetProperty("value").GetString(),
                            putCode: this.GetOrcidPutCode(element)
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
            List<OrcidEducation> educations = new() { };
            using (JsonDocument document = JsonDocument.Parse(json))
            {
                JsonElement educationsElement = document.RootElement.GetProperty("activities-summary").GetProperty("educations");

                if (educationsElement.TryGetProperty("affiliation-group", out JsonElement affiliationGroupsElement))
                {
                    foreach (JsonElement affiliationGroupElement in affiliationGroupsElement.EnumerateArray())
                    {

                        if (affiliationGroupElement.TryGetProperty("summaries", out JsonElement summariesElement))
                        {
                            foreach (JsonElement summaryElement in summariesElement.EnumerateArray())
                            {
                                if (summaryElement.TryGetProperty("education-summary", out JsonElement educationSummaryElement))
                                {
                                    string disambiguatedOrganizationIdentifier = "";
                                    string disambiguationSource = "";
                                    if (educationSummaryElement.GetProperty("organization").TryGetProperty("disambiguated-organization", out JsonElement disambiguatedOrganizationElement))
                                    {
                                        if (disambiguatedOrganizationElement.ValueKind != JsonValueKind.Null)
                                        {
                                            disambiguatedOrganizationIdentifier = disambiguatedOrganizationElement.GetProperty("disambiguated-organization-identifier").GetString();
                                            disambiguationSource = disambiguatedOrganizationElement.GetProperty("disambiguation-source").GetString();
                                        }
                                    }

                                    educations.Add(
                                        new OrcidEducation(
                                            organizationName: educationSummaryElement.GetProperty("organization").GetProperty("name").GetString(),
                                            departmentName: educationSummaryElement.GetProperty("department-name").GetString(),
                                            roleTitle: educationSummaryElement.GetProperty("role-title").GetString(),
                                            startDate: GetOrcidDate(educationSummaryElement.GetProperty("start-date")),
                                            endDate: GetOrcidDate(educationSummaryElement.GetProperty("end-date")),
                                            putCode: this.GetOrcidPutCode(educationSummaryElement)
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
            List<OrcidEmployment> employments = new() { };
            using (JsonDocument document = JsonDocument.Parse(json))
            {
                JsonElement employmentsElement = document.RootElement.GetProperty("activities-summary").GetProperty("employments");

                if (employmentsElement.TryGetProperty("affiliation-group", out JsonElement affiliationGroupsElement))
                {
                    foreach (JsonElement affiliationGroupElement in affiliationGroupsElement.EnumerateArray())
                    {

                        if (affiliationGroupElement.TryGetProperty("summaries", out JsonElement summariesElement))
                        {
                            foreach (JsonElement summaryElement in summariesElement.EnumerateArray())
                            {
                                if (summaryElement.TryGetProperty("employment-summary", out JsonElement employmentSummaryElement))
                                {
                                    string disambiguatedOrganizationIdentifier = "";
                                    string disambiguationSource = "";
                                    if (employmentSummaryElement.GetProperty("organization").TryGetProperty("disambiguated-organization", out JsonElement disambiguatedOrganizationElement))
                                    {
                                        if (disambiguatedOrganizationElement.ValueKind != JsonValueKind.Null)
                                        {
                                            disambiguatedOrganizationIdentifier = disambiguatedOrganizationElement.GetProperty("disambiguated-organization-identifier").GetString();
                                            disambiguationSource = disambiguatedOrganizationElement.GetProperty("disambiguation-source").GetString();
                                        }
                                    }

                                    employments.Add(
                                      new OrcidEmployment(
                                          organizationName: employmentSummaryElement.GetProperty("organization").GetProperty("name").GetString(),
                                          disambiguatedOrganizationIdentifier: disambiguatedOrganizationIdentifier,
                                          disambiguationSource: disambiguationSource,
                                          departmentName: employmentSummaryElement.GetProperty("department-name").GetString(),
                                          roleTitle: employmentSummaryElement.GetProperty("role-title").GetString(),
                                          startDate: GetOrcidDate(employmentSummaryElement.GetProperty("start-date")),
                                          endDate: GetOrcidDate(employmentSummaryElement.GetProperty("end-date")),
                                          putCode: this.GetOrcidPutCode(employmentSummaryElement)
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
         * Works
         */
        public OrcidWorks GetWorks(String json, bool processOnlyResearchActivities=false)
        {
            OrcidWorks orcidWorks = new();

            using (JsonDocument document = JsonDocument.Parse(json))
            {
                JsonElement worksElement = document.RootElement.GetProperty("activities-summary").GetProperty("works");
                if (worksElement.TryGetProperty("group", out JsonElement groupsElement))
                {
                    foreach (JsonElement groupElement in groupsElement.EnumerateArray())
                    {
                        /*
                         *  Elements in "group" can contain "external-ids" and "work-summary".
                         *  "work-summary" can contain multiple entries of the same work.
                         *  Get DOI from "external-ids" and the other properties from the first element in "work-summary".
                         */
                        string DOI = "";

                        // Get DOI from "external-ids" array.
                        if (groupElement.TryGetProperty("external-ids", out JsonElement externalIdsElement))
                        {
                            DOI = GetDoi(externalIdsElement);
                        }

                        // Get work properties from "work-summary" array.
                        if (groupElement.TryGetProperty("work-summary", out JsonElement workSummariesElement))
                        {
                            foreach (JsonElement workElement in workSummariesElement.EnumerateArray())
                            {
                                string orcidWorkType = workElement.GetProperty("type").GetString();

                                /*
                                 * Publication
                                 */
                                if (IsPublication(orcidWorkType) && !processOnlyResearchActivities)
                                {
                                    orcidWorks.Publications.Add(
                                        new OrcidPublication()
                                        {
                                            PublicationName = workElement.GetProperty("title").GetProperty("title").GetProperty("value").GetString(),
                                            Doi = DOI,
                                            PublicationYear = this.GetPublicationYear(workElement),
                                            Type = orcidWorkType,
                                            PutCode = this.GetOrcidPutCode(workElement)
                                        }
                                    );

                                    // Import only one element from "work-summary" array.
                                    break;
                                }

                                /*
                                 * Dataset
                                 */
                                if (IsDataset(orcidWorkType) && !processOnlyResearchActivities)
                                {
                                    string url = (workElement.GetProperty("url").ValueKind == JsonValueKind.Null) ?
                                        "" : workElement.GetProperty("url").GetProperty("value").GetString();

                                    orcidWorks.Datasets.Add(
                                        new OrcidDataset()
                                        {
                                            DatasetName = workElement.GetProperty("title").GetProperty("title").GetProperty("value").GetString(),
                                            DatasetDate = GetOrcidDate(workElement.GetProperty("publication-date")),
                                            Type = orcidWorkType,
                                            PutCode = this.GetOrcidPutCode(workElement),
                                            Url = url
                                        }
                                    );

                                    // Import only one element from "work-summary" array.
                                    break;
                                }

                                /*
                                 * Research acvitity
                                 */
                                if (IsResearchActivity(orcidWorkType))
                                {
                                    string url = (workElement.GetProperty("url").ValueKind == JsonValueKind.Null) ?
                                            "" : workElement.GetProperty("url").GetProperty("value").GetString();

                                    string workType = workElement.GetProperty("type").GetString();

                                    // Import only one element from "work-summary" array.
                                    orcidWorks.ResearchActivities.Add(
                                        new OrcidResearchActivity(
                                          dimReferencedataCodeValue: MapOrcidWorkTypeToDimReferencedataCodeValue(workType),
                                          workType: workType,
                                          organizationName: "",
                                          disambiguatedOrganizationIdentifier: "",
                                          disambiguationSource: "",
                                          departmentName: "",
                                          name: workElement.GetProperty("title").GetProperty("title").GetProperty("value").GetString(),
                                          startDate: GetOrcidDate(workElement.GetProperty("publication-date")),
                                          endDate: new OrcidDate(),
                                          putCode: this.GetOrcidPutCode(workElement),
                                          url: url
                                        )
                                    );
                                    break;
                                }
                            }
                        }
                    }
                }
            }

            return orcidWorks;
        }

        /*
         * Distinctions, invited positions, peer reviews, memberships, qualifications and services.
         * These map to profile only research activity in TTV database (dim_profile_only_research_activity).
         */
        public List<OrcidResearchActivity> GetProfileOnlyResearchActivityItems(String json)
        {
            List<OrcidResearchActivity> profileOnlyResearchActivityItems = new() { };
           
            using (JsonDocument document = JsonDocument.Parse(json))
            {       
                // Distinctions
                JsonElement distinctionsElement = document.RootElement.GetProperty("activities-summary").GetProperty("distinctions");
                if (distinctionsElement.TryGetProperty("affiliation-group", out JsonElement affiliationGroupsElement_distinctions))
                {
                    foreach (JsonElement affiliationGroupElement in affiliationGroupsElement_distinctions.EnumerateArray())
                    {
                        if (affiliationGroupElement.TryGetProperty("summaries", out JsonElement summariesElement))
                        {
                            foreach (JsonElement summaryElement in summariesElement.EnumerateArray())
                            {
                                if (summaryElement.TryGetProperty("distinction-summary", out JsonElement distinctionSummaryElement))
                                {
                                    string disambiguatedOrganizationIdentifier = "";
                                    string disambiguationSource = "";
                                    if (distinctionSummaryElement.GetProperty("organization").TryGetProperty("disambiguated-organization", out JsonElement disambiguatedOrganizationElement))
                                    {
                                        if (disambiguatedOrganizationElement.ValueKind != JsonValueKind.Null)
                                        {
                                            disambiguatedOrganizationIdentifier = disambiguatedOrganizationElement.GetProperty("disambiguated-organization-identifier").GetString();
                                            disambiguationSource = disambiguatedOrganizationElement.GetProperty("disambiguation-source").GetString();
                                        }
                                    }

                                    string url = (distinctionSummaryElement.GetProperty("url").ValueKind == JsonValueKind.Null) ?
                                        "" : distinctionSummaryElement.GetProperty("url").GetProperty("value").GetString();

                                    string name = (distinctionSummaryElement.GetProperty("role-title").ValueKind == JsonValueKind.Null) ?
                                        "" : distinctionSummaryElement.GetProperty("role-title").GetString();
                                    name = name.Length > 255 ? name.Substring(0, 255) : name; // Database size 255

                                    profileOnlyResearchActivityItems.Add(
                                      new OrcidResearchActivity(
                                          dimReferencedataCodeValue: Constants.OrcidResearchActivity_To_ReferenceDataCodeValue.DISTINCTION,
                                          workType: "",
                                          organizationName: distinctionSummaryElement.GetProperty("organization").GetProperty("name").GetString(),
                                          disambiguatedOrganizationIdentifier: disambiguatedOrganizationIdentifier,
                                          disambiguationSource: disambiguationSource,
                                          departmentName: distinctionSummaryElement.GetProperty("department-name").GetString(),
                                          name: name,
                                          startDate: GetOrcidDate(distinctionSummaryElement.GetProperty("start-date")),
                                          endDate: GetOrcidDate(distinctionSummaryElement.GetProperty("end-date")),
                                          putCode: this.GetOrcidPutCode(distinctionSummaryElement),
                                          url: url
                                      )
                                   );
                                }
                            }
                        }
                    }
                }

                // Invited positions
                JsonElement invitedPositionsElement = document.RootElement.GetProperty("activities-summary").GetProperty("invited-positions");
                if (invitedPositionsElement.TryGetProperty("affiliation-group", out JsonElement affiliationGroupsElement_invitedPositions))
                {
                    foreach (JsonElement affiliationGroupElement in affiliationGroupsElement_invitedPositions.EnumerateArray())
                    {
                        if (affiliationGroupElement.TryGetProperty("summaries", out JsonElement summariesElement))
                        {
                            foreach (JsonElement summaryElement in summariesElement.EnumerateArray())
                            {
                                if (summaryElement.TryGetProperty("invited-position-summary", out JsonElement invitedPositionsSummaryElement))
                                {
                                    string disambiguatedOrganizationIdentifier = "";
                                    string disambiguationSource = "";
                                    if (invitedPositionsSummaryElement.GetProperty("organization").TryGetProperty("disambiguated-organization", out JsonElement disambiguatedOrganizationElement))
                                    {
                                        if (disambiguatedOrganizationElement.ValueKind != JsonValueKind.Null)
                                        {
                                            disambiguatedOrganizationIdentifier = disambiguatedOrganizationElement.GetProperty("disambiguated-organization-identifier").GetString();
                                            disambiguationSource = disambiguatedOrganizationElement.GetProperty("disambiguation-source").GetString();
                                        }
                                    }

                                    string url = (invitedPositionsSummaryElement.GetProperty("url").ValueKind == JsonValueKind.Null) ?
                                        "" : invitedPositionsSummaryElement.GetProperty("url").GetProperty("value").GetString();

                                    string name = (invitedPositionsSummaryElement.GetProperty("role-title").ValueKind == JsonValueKind.Null) ?
                                        "" : invitedPositionsSummaryElement.GetProperty("role-title").GetString();
                                    name = name.Length > 255 ? name.Substring(0, 255) : name; // Database size 255

                                    profileOnlyResearchActivityItems.Add(
                                      new OrcidResearchActivity(
                                          dimReferencedataCodeValue: Constants.OrcidResearchActivity_To_ReferenceDataCodeValue.INVITED_POSITION,
                                          workType: "",
                                          organizationName: invitedPositionsSummaryElement.GetProperty("organization").GetProperty("name").GetString(),
                                          disambiguatedOrganizationIdentifier: disambiguatedOrganizationIdentifier,
                                          disambiguationSource: disambiguationSource,
                                          departmentName: invitedPositionsSummaryElement.GetProperty("department-name").GetString(),
                                          name: name,
                                          startDate: GetOrcidDate(invitedPositionsSummaryElement.GetProperty("start-date")),
                                          endDate: GetOrcidDate(invitedPositionsSummaryElement.GetProperty("end-date")),
                                          putCode: this.GetOrcidPutCode(invitedPositionsSummaryElement),
                                          url: url
                                      )
                                  );
                                }
                            }
                        }
                    }
                }

                // Memberships
                JsonElement membershipsElement = document.RootElement.GetProperty("activities-summary").GetProperty("memberships");
                if (membershipsElement.TryGetProperty("affiliation-group", out JsonElement affiliationGroupsElement_memberships))
                {
                    foreach (JsonElement affiliationGroupElement in affiliationGroupsElement_memberships.EnumerateArray())
                    {
                        if (affiliationGroupElement.TryGetProperty("summaries", out JsonElement summariesElement))
                        {
                            foreach (JsonElement summaryElement in summariesElement.EnumerateArray())
                            {
                                if (summaryElement.TryGetProperty("membership-summary", out JsonElement membershipSummaryElement))
                                {
                                    string disambiguatedOrganizationIdentifier = "";
                                    string disambiguationSource = "";
                                    if (membershipSummaryElement.GetProperty("organization").TryGetProperty("disambiguated-organization", out JsonElement disambiguatedOrganizationElement))
                                    {
                                        if (disambiguatedOrganizationElement.ValueKind != JsonValueKind.Null)
                                        {
                                            disambiguatedOrganizationIdentifier = disambiguatedOrganizationElement.GetProperty("disambiguated-organization-identifier").GetString();
                                            disambiguationSource = disambiguatedOrganizationElement.GetProperty("disambiguation-source").GetString();
                                        }
                                    }

                                    string url = (membershipSummaryElement.GetProperty("url").ValueKind == JsonValueKind.Null) ?
                                        "" : membershipSummaryElement.GetProperty("url").GetProperty("value").GetString();

                                    string name = (membershipSummaryElement.GetProperty("role-title").ValueKind == JsonValueKind.Null) ?
                                        "" : membershipSummaryElement.GetProperty("role-title").GetString();
                                    name = name.Length > 255 ? name.Substring(0, 255) : name; // Database size 255

                                    profileOnlyResearchActivityItems.Add(
                                      new OrcidResearchActivity(
                                          dimReferencedataCodeValue: Constants.OrcidResearchActivity_To_ReferenceDataCodeValue.MEMBERSHIP,
                                          workType: "",
                                          organizationName: membershipSummaryElement.GetProperty("organization").GetProperty("name").GetString(),
                                          disambiguatedOrganizationIdentifier: disambiguatedOrganizationIdentifier,
                                          disambiguationSource: disambiguationSource,
                                          departmentName: membershipSummaryElement.GetProperty("department-name").GetString(),
                                          name: name,
                                          startDate: GetOrcidDate(membershipSummaryElement.GetProperty("start-date")),
                                          endDate: GetOrcidDate(membershipSummaryElement.GetProperty("end-date")),
                                          putCode: this.GetOrcidPutCode(membershipSummaryElement),
                                          url: url
                                      )
                                  );
                                }
                            }
                        }
                    }
                }

                // Peer reviews
                JsonElement peerReviewsElement = document.RootElement.GetProperty("activities-summary").GetProperty("peer-reviews");
                if (peerReviewsElement.TryGetProperty("group", out JsonElement groupsElement)) {
                    foreach (JsonElement groupElement in groupsElement.EnumerateArray())
                    {
                        if (groupElement.TryGetProperty("peer-review-group", out JsonElement peerReviewGroupsElement))
                        {
                            foreach (JsonElement peerReviewGroupElement in peerReviewGroupsElement.EnumerateArray())
                            {
                                if (peerReviewGroupElement.TryGetProperty("peer-review-summary", out JsonElement peerReviewSummariesElement))
                                {
                                    foreach (JsonElement peerReviewSummaryElement in peerReviewSummariesElement.EnumerateArray())
                                    {
                                        string disambiguatedOrganizationIdentifier = "";
                                        string disambiguationSource = "";
                                        if (peerReviewSummaryElement.GetProperty("convening-organization").TryGetProperty("disambiguated-organization", out JsonElement disambiguatedOrganizationElement))
                                        {
                                            if (disambiguatedOrganizationElement.ValueKind != JsonValueKind.Null)
                                            {
                                                disambiguatedOrganizationIdentifier = disambiguatedOrganizationElement.GetProperty("disambiguated-organization-identifier").GetString();
                                                disambiguationSource = disambiguatedOrganizationElement.GetProperty("disambiguation-source").GetString();
                                            }
                                        }

                                        string url = (peerReviewSummaryElement.GetProperty("review-url").ValueKind == JsonValueKind.Null) ?
                                            "" : peerReviewSummaryElement.GetProperty("review-url").GetProperty("value").GetString();

                                        string name = (peerReviewSummaryElement.GetProperty("reviewer-role").ValueKind == JsonValueKind.Null) ?
                                            "" : peerReviewSummaryElement.GetProperty("reviewer-role").GetString();
                                        name = name.Length > 255 ? name.Substring(0, 255) : name; // Database size 255

                                        profileOnlyResearchActivityItems.Add(
                                              new OrcidResearchActivity(
                                                  dimReferencedataCodeValue: Constants.OrcidResearchActivity_To_ReferenceDataCodeValue.PEER_REVIEW,
                                                  workType: "",
                                                  organizationName: peerReviewSummaryElement.GetProperty("convening-organization").GetProperty("name").GetString(),
                                                  disambiguatedOrganizationIdentifier: disambiguatedOrganizationIdentifier,
                                                  disambiguationSource: disambiguationSource,
                                                  departmentName: "",
                                                  name: name,
                                                  startDate: GetOrcidDate(peerReviewSummaryElement.GetProperty("completion-date")),
                                                  endDate: new OrcidDate(),
                                                  putCode: this.GetOrcidPutCode(peerReviewSummaryElement),
                                                  url: url
                                              )
                                          );
                                    }
                                }
                            }
                        }
                    }
                }

                // Qualifications
                JsonElement qualificationsElement = document.RootElement.GetProperty("activities-summary").GetProperty("qualifications");
                if (qualificationsElement.TryGetProperty("affiliation-group", out JsonElement affiliationGroupsElement_qualifications))
                {
                    foreach (JsonElement affiliationGroupElement in affiliationGroupsElement_qualifications.EnumerateArray())
                    {
                        if (affiliationGroupElement.TryGetProperty("summaries", out JsonElement summariesElement))
                        {
                            foreach (JsonElement summaryElement in summariesElement.EnumerateArray())
                            {
                                if (summaryElement.TryGetProperty("qualification-summary", out JsonElement qualificationSummaryElement))
                                {
                                    string disambiguatedOrganizationIdentifier = "";
                                    string disambiguationSource = "";
                                    if (qualificationSummaryElement.GetProperty("organization").TryGetProperty("disambiguated-organization", out JsonElement disambiguatedOrganizationElement))
                                    {
                                        if (disambiguatedOrganizationElement.ValueKind != JsonValueKind.Null)
                                        {
                                            disambiguatedOrganizationIdentifier = disambiguatedOrganizationElement.GetProperty("disambiguated-organization-identifier").GetString();
                                            disambiguationSource = disambiguatedOrganizationElement.GetProperty("disambiguation-source").GetString();
                                        }
                                    }

                                    string url = (qualificationSummaryElement.GetProperty("url").ValueKind == JsonValueKind.Null) ?
                                        "" : qualificationSummaryElement.GetProperty("url").GetProperty("value").GetString();

                                    string name = (qualificationSummaryElement.GetProperty("role-title").ValueKind == JsonValueKind.Null) ?
                                        "" : qualificationSummaryElement.GetProperty("role-title").GetString();
                                    name = name.Length > 255 ? name.Substring(0, 255) : name; // Database size 255

                                    profileOnlyResearchActivityItems.Add(
                                      new OrcidResearchActivity(
                                          dimReferencedataCodeValue: Constants.OrcidResearchActivity_To_ReferenceDataCodeValue.QUALIFICATION,
                                          workType: "",
                                          organizationName: qualificationSummaryElement.GetProperty("organization").GetProperty("name").GetString(),
                                          disambiguatedOrganizationIdentifier: disambiguatedOrganizationIdentifier,
                                          disambiguationSource: disambiguationSource,
                                          departmentName: qualificationSummaryElement.GetProperty("department-name").GetString(),
                                          name: name,
                                          startDate: GetOrcidDate(qualificationSummaryElement.GetProperty("start-date")),
                                          endDate: GetOrcidDate(qualificationSummaryElement.GetProperty("end-date")),
                                          putCode: this.GetOrcidPutCode(qualificationSummaryElement),
                                          url: url
                                      )
                                  );
                                }
                            }
                        }
                    }
                }

                // Services
                JsonElement servicesElement = document.RootElement.GetProperty("activities-summary").GetProperty("services");
                if (servicesElement.TryGetProperty("affiliation-group", out JsonElement affiliationGroupsElement_services))
                {
                    foreach (JsonElement affiliationGroupElement in affiliationGroupsElement_services.EnumerateArray())
                    {
                        if (affiliationGroupElement.TryGetProperty("summaries", out JsonElement summariesElement))
                        {
                            foreach (JsonElement summaryElement in summariesElement.EnumerateArray())
                            {
                                if (summaryElement.TryGetProperty("service-summary", out JsonElement serviceSummaryElement))
                                {
                                    string disambiguatedOrganizationIdentifier = "";
                                    string disambiguationSource = "";
                                    if (serviceSummaryElement.GetProperty("organization").TryGetProperty("disambiguated-organization", out JsonElement disambiguatedOrganizationElement))
                                    {
                                        if (disambiguatedOrganizationElement.ValueKind != JsonValueKind.Null)
                                        {
                                            disambiguatedOrganizationIdentifier = disambiguatedOrganizationElement.GetProperty("disambiguated-organization-identifier").GetString();
                                            disambiguationSource = disambiguatedOrganizationElement.GetProperty("disambiguation-source").GetString();
                                        }
                                    }

                                    string url = (serviceSummaryElement.GetProperty("url").ValueKind == JsonValueKind.Null) ?
                                        "" : serviceSummaryElement.GetProperty("url").GetProperty("value").GetString();

                                    string name = (serviceSummaryElement.GetProperty("role-title").ValueKind == JsonValueKind.Null) ?
                                        "" : serviceSummaryElement.GetProperty("role-title").GetString();
                                    name = name.Length > 255 ? name.Substring(0, 255) : name; // Database size 255

                                    profileOnlyResearchActivityItems.Add(
                                      new OrcidResearchActivity(
                                          dimReferencedataCodeValue: Constants.OrcidResearchActivity_To_ReferenceDataCodeValue.SERVICE,
                                          workType: "",
                                          organizationName: serviceSummaryElement.GetProperty("organization").GetProperty("name").GetString(),
                                          disambiguatedOrganizationIdentifier: disambiguatedOrganizationIdentifier,
                                          disambiguationSource: disambiguationSource,
                                          departmentName: serviceSummaryElement.GetProperty("department-name").GetString(),
                                          name: name,
                                          startDate: GetOrcidDate(serviceSummaryElement.GetProperty("start-date")),
                                          endDate: GetOrcidDate(serviceSummaryElement.GetProperty("end-date")),
                                          putCode: this.GetOrcidPutCode(serviceSummaryElement),
                                          url: url
                                      )
                                  );
                                }
                            }
                        }
                    }
                }
            }
            return profileOnlyResearchActivityItems;
        }

        /*
         * Funding.
         * Maps to profile only funding decision in TTV database (dim_profile_only_funding_decision).
         */
        public List<OrcidFunding> GetFundings(String json)
        {
            List<OrcidFunding> orcidFundings = new() { };

            using (JsonDocument document = JsonDocument.Parse(json))
            {
                JsonElement fundingsElement = document.RootElement.GetProperty("activities-summary").GetProperty("fundings");
                if (fundingsElement.TryGetProperty("group", out JsonElement fundingGroupsElement))
                {
                    foreach (JsonElement fundingElement in fundingGroupsElement.EnumerateArray())
                    {
                        if (fundingElement.TryGetProperty("funding-summary", out JsonElement fundingSummariesElement))
                        {
                            foreach (JsonElement fundingSummaryElement in fundingSummariesElement.EnumerateArray())
                            {
                                orcidFundings.Add(GetFundingDetailFromJsonElement(fundingElement: fundingSummaryElement));
                            }
                        }
                    }
                }
            }
            return orcidFundings;
        }

        /*
         * Funding detail.
         */
        private OrcidFunding GetFundingDetailFromJsonElement(JsonElement fundingElement)
        {
            string amount = "";
            string currencyCode = "";
            string description = "";
            string disambiguatedOrganizationIdentifier = "";
            string disambiguationSource = "";
            string url = "";

            // Parse amount
            if (fundingElement.TryGetProperty("amount", out JsonElement amountElement))
            {
                if (amountElement.ValueKind != JsonValueKind.Null)
                {
                    amount = amountElement.GetProperty("value").ToString();
                    currencyCode = amountElement.GetProperty("currency-code").ToString();
                }
            }

            // Parse description
            if (fundingElement.TryGetProperty("short-description", out JsonElement descriptionElement))
            {
                if (descriptionElement.ValueKind != JsonValueKind.Null)
                {
                    description = descriptionElement.GetString();
                }
            }

            // Parse disambiguated organization
            if (fundingElement.GetProperty("organization").TryGetProperty("disambiguated-organization", out JsonElement disambiguatedOrganizationElement))
            {
                if (disambiguatedOrganizationElement.ValueKind != JsonValueKind.Null)
                {
                    disambiguatedOrganizationIdentifier = disambiguatedOrganizationElement.GetProperty("disambiguated-organization-identifier").GetString();
                    disambiguationSource = disambiguatedOrganizationElement.GetProperty("disambiguation-source").GetString();
                }
            }

            // Parse name
            string name = fundingElement.GetProperty("title").GetProperty("title").GetProperty("value").GetString();

            // Parse URL
            url = (fundingElement.GetProperty("url").ValueKind == JsonValueKind.Null) ?
                "" : fundingElement.GetProperty("url").GetProperty("value").GetString();

            return new(
                type: fundingElement.GetProperty("type").GetString(),
                name: name.Length > 255 ? $"{name.Substring(0, 252)}..." : name, // Database size 255
                description: description,
                amount: amount,
                currencyCode: currencyCode,
                organizationName: fundingElement.GetProperty("organization").GetProperty("name").GetString(),
                disambiguatedOrganizationIdentifier: disambiguatedOrganizationIdentifier,
                disambiguationSource: disambiguationSource,
                startDate: GetOrcidDate(fundingElement.GetProperty("start-date")),
                endDate: GetOrcidDate(fundingElement.GetProperty("end-date")),
                putCode: this.GetOrcidPutCode(fundingElement),
                url: url,
                path: fundingElement.GetProperty("path").GetString()
            );
        }

        public OrcidFunding GetFundingDetail(string fundingDetailJson)
        {
            using (JsonDocument document = JsonDocument.Parse(fundingDetailJson))
            {
                return GetFundingDetailFromJsonElement(fundingElement: document.RootElement);
            }
        }
    }
}