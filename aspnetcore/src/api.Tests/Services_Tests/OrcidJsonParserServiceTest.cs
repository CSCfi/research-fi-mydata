using System;
using Xunit;
using api.Services;
using api.Models.Common;
using api.Models.Orcid;
using System.IO;
using System.Collections.Generic;

namespace api.Tests
{
    [Collection("Parsing data from ORCID record json")]
    public class OrcidJsonParserServiceTests
    {
        // Get ORCID record.
        // Test file is based on ORCID's sandbox record https://pub.sandbox.orcid.org/v3.0/0000-0002-9227-8514/record
        private string getOrcidJsonRecord()
        {
            var filePath = $@"{Directory.GetParent(Environment.CurrentDirectory).Parent.Parent.FullName}/Infrastructure/orcidSandbox_0000-0002-9227-8514_record.json";
            var reader = new StreamReader(filePath);
            return reader.ReadToEnd();
        }

        // Get ORCID record which does not contain name or other name.
        private string getOrcidJsonRecord_NoNames()
        {
            var filePath = $@"{Directory.GetParent(Environment.CurrentDirectory).Parent.Parent.FullName}/Infrastructure/orcidSandbox_0000-0002-9227-8514_record_no_names.json";
            var reader = new StreamReader(filePath);
            return reader.ReadToEnd();
        }

        // Get ORCID personal details.
        // Test file is based on ORCID's sandbox record https://pub.sandbox.orcid.org/v3.0/0000-0002-9227-8514/personal-details
        private string getOrcidJsonPersonalDetails()
        {
            var filePath = $@"{Directory.GetParent(Environment.CurrentDirectory).Parent.Parent.FullName}/Infrastructure/orcidSandbox_0000-0002-9227-8514_personal-details.json";
            var reader = new StreamReader(filePath);
            return reader.ReadToEnd();
        }

        [Fact(DisplayName = "Get given names from full ORCID record")]
        public void TestGetGivenNamesFromRecord()
        {
            var orcidJsonParserService = new OrcidJsonParserService();
            var jsonStr = getOrcidJsonRecord();
            var expectedGivenNames = new OrcidGivenNames("Sofia");
            var actualGivenNames = orcidJsonParserService.GetGivenNames(jsonStr);
            Assert.Equal(expectedGivenNames.Value, actualGivenNames.Value);
        }

        [Fact(DisplayName = "Get given names from full ORCID record - handle missing name")]
        public void TestGetGivenNames_NameIsNull()
        {
            var orcidJsonParserService = new OrcidJsonParserService();
            var jsonStr = getOrcidJsonRecord_NoNames();
            var expectedGivenNames = new OrcidGivenNames("");
            var actualGivenNames = orcidJsonParserService.GetGivenNames(jsonStr);
            Assert.Equal(expectedGivenNames.Value, actualGivenNames.Value);
        }

        [Fact(DisplayName = "Get given names from personal details")]
        public void TestGetGivenNamesFromPersonalDetails()
        {
            var orcidJsonParserService = new OrcidJsonParserService();
            var jsonStr = getOrcidJsonPersonalDetails();
            var expectedGivenNames = new OrcidGivenNames("Sofia");
            var actualGivenNames = orcidJsonParserService.GetGivenNames(jsonStr);
            Assert.Equal(expectedGivenNames.Value, actualGivenNames.Value);
        }

        [Fact(DisplayName = "Get family name from full ORCID record")]
        public void TestGetFamilyNameFromRecord()
        {
            var orcidJsonParserService = new OrcidJsonParserService();
            var jsonStr = getOrcidJsonRecord();
            var expectedFamilyName = new OrcidFamilyName("Garcia");
            var actualFamilyName = orcidJsonParserService.GetFamilyName(jsonStr);
            Assert.Equal(expectedFamilyName.Value, actualFamilyName.Value);
        }

        [Fact(DisplayName = "Get family name from full ORCID record - handle missing name")]
        public void TestGetFamilyName_NameIsNull()
        {
            var orcidJsonParserService = new OrcidJsonParserService();
            var jsonStr = getOrcidJsonRecord_NoNames();
            var expectedFamilyName = new OrcidFamilyName("");
            var actualFamilyName = orcidJsonParserService.GetFamilyName(jsonStr);
            Assert.Equal(expectedFamilyName.Value, actualFamilyName.Value);
        }

        [Fact(DisplayName = "Get family name from personal details")]
        public void TestGetFamilyNameFromPersonalDetails()
        {
            var orcidJsonParserService = new OrcidJsonParserService();
            var jsonStr = getOrcidJsonPersonalDetails();
            var expectedFamilyName = new OrcidFamilyName("Garcia");
            var actualFamilyName = orcidJsonParserService.GetFamilyName(jsonStr);
            Assert.Equal(expectedFamilyName.Value, actualFamilyName.Value);
        }

        [Fact(DisplayName = "Get credit name from full ORCID record")]
        public void TestGetCreditNameFromRecord()
        {
            var orcidJsonParserService = new OrcidJsonParserService();
            var jsonStr = getOrcidJsonRecord();
            var expectedCreditName = new OrcidCreditName("Sofia Maria Hernandez Garcia");
            var actualCreditName = orcidJsonParserService.GetCreditName(jsonStr);
            Assert.Equal(expectedCreditName.Value, actualCreditName.Value);
        }

        [Fact(DisplayName = "Get credit name from full ORCID record - handle missing name")]
        public void TestGetCreditName_NameIsNull()
        {
            var orcidJsonParserService = new OrcidJsonParserService();
            var jsonStr = getOrcidJsonRecord_NoNames();
            var expectedCreditName = new OrcidCreditName("");
            var actualCreditName = orcidJsonParserService.GetCreditName(jsonStr);
            Assert.Equal(expectedCreditName.Value, actualCreditName.Value);
        }

        [Fact(DisplayName = "Get credit name from personal details")]
        public void TestGetCreditNameFromPersonalDetails()
        {
            var orcidJsonParserService = new OrcidJsonParserService();
            var jsonStr = getOrcidJsonPersonalDetails();
            var expectedCreditName = new OrcidCreditName("Sofia Maria Hernandez Garcia");
            var actualCreditName = orcidJsonParserService.GetCreditName(jsonStr);
            Assert.Equal(expectedCreditName.Value, actualCreditName.Value);
        }

        [Fact(DisplayName = "Get other names from full ORCID record")]
        public void TestGetOtherNamesFromRecord()
        {
            var jsonStr = getOrcidJsonRecord();
            var orcidJsonParserService = new OrcidJsonParserService();
            var actualOtherNames = orcidJsonParserService.GetOtherNames(jsonStr);
            var expectedOtherNames = new List<OrcidOtherName> { };
            expectedOtherNames.Add(
                new OrcidOtherName(
                    "Sofia Maria Garcia",
                    new OrcidPutCode(15812)
                )
            );
            expectedOtherNames.Add(
                new OrcidOtherName(
                    "София Мария Эрнандес Гарсия",
                    new OrcidPutCode(15813)
                )
            );
            expectedOtherNames.Add(
                new OrcidOtherName(
                    "索菲亚玛丽亚 加西亚",
                    new OrcidPutCode(15814)
                )
            );
            Assert.Equal(expectedOtherNames[0].Value, actualOtherNames[0].Value);
            Assert.Equal(expectedOtherNames[1].Value, actualOtherNames[1].Value);
            Assert.Equal(expectedOtherNames[2].Value, actualOtherNames[2].Value);
            Assert.Equal(expectedOtherNames[0].PutCode.Value, actualOtherNames[0].PutCode.Value);
            Assert.Equal(expectedOtherNames[1].PutCode.Value, actualOtherNames[1].PutCode.Value);
            Assert.Equal(expectedOtherNames[2].PutCode.Value, actualOtherNames[2].PutCode.Value);
        }

        [Fact(DisplayName = "Get other names from personal details")]
        public void TestGetOtherNamesFromPersonalDetails()
        {
            var jsonStr = getOrcidJsonPersonalDetails();
            var orcidJsonParserService = new OrcidJsonParserService();
            var actualOtherNames = orcidJsonParserService.GetOtherNames(jsonStr);
            var expectedOtherNames = new List<OrcidOtherName> { };
            expectedOtherNames.Add(
                new OrcidOtherName(
                    "Sofia Maria Garcia",
                    new OrcidPutCode(15812)
                )
            );
            expectedOtherNames.Add(
                new OrcidOtherName(
                    "София Мария Эрнандес Гарсия",
                    new OrcidPutCode(15813)
                )
            );
            expectedOtherNames.Add(
                new OrcidOtherName(
                    "索菲亚玛丽亚 加西亚",
                    new OrcidPutCode(15814)
                )
            );
            Assert.Equal(expectedOtherNames[0].Value, actualOtherNames[0].Value);
            Assert.Equal(expectedOtherNames[1].Value, actualOtherNames[1].Value);
            Assert.Equal(expectedOtherNames[2].Value, actualOtherNames[2].Value);
            Assert.Equal(expectedOtherNames[0].PutCode.Value, actualOtherNames[0].PutCode.Value);
            Assert.Equal(expectedOtherNames[1].PutCode.Value, actualOtherNames[1].PutCode.Value);
            Assert.Equal(expectedOtherNames[2].PutCode.Value, actualOtherNames[2].PutCode.Value);
        }

        [Fact(DisplayName = "Get biography from full ORCID record")]
        public void TestGetBiography()
        {
            var orcidJsonParserService = new OrcidJsonParserService();
            var jsonStr = getOrcidJsonRecord();
            var expectedBiography = new OrcidBiography(
                "Sofia Maria Hernandez Garcia is the researcher that is used as an example ORCID record holder."
            );
            var actualBiography = orcidJsonParserService.GetBiography(jsonStr);
            Assert.Equal(expectedBiography.Value, actualBiography.Value);
        }

        [Fact(DisplayName = "Get biography from personal details")]
        public void TestGetBiographyFromPersonalDetails()
        {
            var orcidJsonParserService = new OrcidJsonParserService();
            var jsonStr = getOrcidJsonPersonalDetails();
            var expectedBiography = new OrcidBiography(
                "Sofia Maria Hernandez Garcia is the researcher that is used as an example ORCID record holder."
            );
            var actualBiography = orcidJsonParserService.GetBiography(jsonStr);
            Assert.Equal(expectedBiography.Value, actualBiography.Value);
        }

        [Fact(DisplayName = "Get researcher urls")]
        public void TestGetResearcherUrls()
        {
            var orcidJsonParserService = new OrcidJsonParserService();
            var jsonStr = getOrcidJsonRecord();
            var expectedResearcherUrls = new List<OrcidResearcherUrl> {
                new OrcidResearcherUrl(
                    "Twitter",
                    "https://twitter.com/ORCIDsofia",
                    new OrcidPutCode(41387)
                )
            };
            var actualResearcherUrls = orcidJsonParserService.GetResearcherUrls(jsonStr);

            Assert.Equal(expectedResearcherUrls[0].UrlName, actualResearcherUrls[0].UrlName);
            Assert.Equal(expectedResearcherUrls[0].Url, actualResearcherUrls[0].Url);
            Assert.Equal(expectedResearcherUrls[0].PutCode.Value, actualResearcherUrls[0].PutCode.Value);
        }

        [Fact(DisplayName = "Get emails")]
        public void TestGetEmails()
        {
            var orcidJsonParserService = new OrcidJsonParserService();
            var jsonStr = getOrcidJsonRecord();
            var expectedEmails = new List<OrcidEmail> {
                new OrcidEmail(
                    "s.garcia@orcid.org",
                    new OrcidPutCode(null)
                )
            };
            var actualEmails = orcidJsonParserService.GetEmails(jsonStr);

            Assert.Equal(expectedEmails[0].Value, actualEmails[0].Value);
            Assert.Equal(expectedEmails[0].PutCode.Value, actualEmails[0].PutCode.Value);
        }

        [Fact(DisplayName = "Get keywords")]
        public void TestGetKeywords()
        {
            var orcidJsonParserService = new OrcidJsonParserService();
            var jsonStr = getOrcidJsonRecord();
            var expectedKeywords = new List<OrcidKeyword> {
                new OrcidKeyword(
                    "QA and testing",
                    new OrcidPutCode(4504)
                ),
                // "QA and testing" is intentionally twice, because the example record has a duplicate
                new OrcidKeyword(
                    "QA and testing",
                    new OrcidPutCode(4603)
                ),
                new OrcidKeyword(
                    "Additional keyword",
                    new OrcidPutCode(4604)
                )
            };
            var actualKeywords = orcidJsonParserService.GetKeywords(jsonStr);

            Assert.Equal(expectedKeywords[0].Value, actualKeywords[0].Value);
            Assert.Equal(expectedKeywords[1].Value, actualKeywords[1].Value);
            Assert.Equal(expectedKeywords[2].Value, actualKeywords[2].Value);
            Assert.Equal(expectedKeywords[0].PutCode.Value, actualKeywords[0].PutCode.Value);
            Assert.Equal(expectedKeywords[1].PutCode.Value, actualKeywords[1].PutCode.Value);
            Assert.Equal(expectedKeywords[2].PutCode.Value, actualKeywords[2].PutCode.Value);
        }

        [Fact(DisplayName = "Get external identifiers")]
        public void TestGetExternalIdentifiers()
        {
            var orcidJsonParserService = new OrcidJsonParserService();
            var jsonStr = getOrcidJsonRecord();
            var expectedExternalIdentifiers = new List<OrcidExternalIdentifier> {
                new OrcidExternalIdentifier(
                    "Loop profile",
                    "558",
                    "http://loop.frontiers-sandbox-int.info/people/559/overview?referrer=orcid_profile",
                    new OrcidPutCode(3193)
                ),

                new OrcidExternalIdentifier(
                    "Personal External Identifier",
                    "506",
                    "www.6.com",
                    new OrcidPutCode(3294)
                )
            };
            var actualExternalIdentifiers = orcidJsonParserService.GetExternalIdentifiers(jsonStr);

            Assert.Equal(expectedExternalIdentifiers[0].ExternalIdType, actualExternalIdentifiers[0].ExternalIdType);
            Assert.Equal(expectedExternalIdentifiers[0].ExternalIdValue, actualExternalIdentifiers[0].ExternalIdValue);
            Assert.Equal(expectedExternalIdentifiers[0].ExternalIdUrl, actualExternalIdentifiers[0].ExternalIdUrl);
            Assert.Equal(expectedExternalIdentifiers[0].PutCode.Value, actualExternalIdentifiers[0].PutCode.Value);
            Assert.Equal(expectedExternalIdentifiers[1].ExternalIdType, actualExternalIdentifiers[1].ExternalIdType);
            Assert.Equal(expectedExternalIdentifiers[1].ExternalIdValue, actualExternalIdentifiers[1].ExternalIdValue);
            Assert.Equal(expectedExternalIdentifiers[1].ExternalIdUrl, actualExternalIdentifiers[1].ExternalIdUrl);
            Assert.Equal(expectedExternalIdentifiers[1].PutCode.Value, actualExternalIdentifiers[1].PutCode.Value);
        }

        [Fact(DisplayName = "Get educations")]
        public void TestGetEducations()
        {
            var orcidJsonParserService = new OrcidJsonParserService();
            var jsonStr = getOrcidJsonRecord();
            var actualEducations = orcidJsonParserService.GetEducations(jsonStr);
            Assert.True(actualEducations.Count == 2, "Educations: should parse 2 education");

            Assert.Equal("Massachusetts Institute of Technology", actualEducations[0].OrganizationName);
            Assert.Equal("Testing Department", actualEducations[0].DepartmentName);
            Assert.Equal("BA", actualEducations[0].RoleTitle);
            Assert.Equal(1997, actualEducations[0].StartDate.Year);
            Assert.Equal(9, actualEducations[0].StartDate.Month);
            Assert.Equal(2, actualEducations[0].StartDate.Day);
            Assert.Equal(2001, actualEducations[0].EndDate.Year);
            Assert.Equal(5, actualEducations[0].EndDate.Month);
            Assert.Equal(15, actualEducations[0].EndDate.Day);
            Assert.Equal(new OrcidPutCode(22423).Value, actualEducations[0].PutCode.Value);

            Assert.Equal("Test university without disambiguated organization", actualEducations[1].OrganizationName);
            Assert.Equal("Managing Department", actualEducations[1].DepartmentName);
            Assert.Equal("MSc", actualEducations[1].RoleTitle);
            Assert.Equal(1998, actualEducations[1].StartDate.Year);
            Assert.Equal(10, actualEducations[1].StartDate.Month);
            Assert.Equal(4, actualEducations[1].StartDate.Day);
            Assert.Equal(2002, actualEducations[1].EndDate.Year);
            Assert.Equal(6, actualEducations[1].EndDate.Month);
            Assert.Equal(16, actualEducations[1].EndDate.Day);
            Assert.Equal(new OrcidPutCode(42345).Value, actualEducations[1].PutCode.Value);
        }

        [Fact(DisplayName = "Get employments")]
        public void TestGetEmployments()
        {
            var orcidJsonParserService = new OrcidJsonParserService();
            var jsonStr = getOrcidJsonRecord();
            var actualEmployments = orcidJsonParserService.GetEmployments(jsonStr);

            Assert.True(actualEmployments.Count == 2, "Educations: should parse 2 employments");

            Assert.Equal("ORCID", actualEmployments[0].OrganizationName);
            Assert.Equal("385488", actualEmployments[0].DisambiguatedOrganizationIdentifier);
            Assert.Equal("RINGGOLD", actualEmployments[0].DisambiguationSource);
            Assert.Equal("QA and Testing", actualEmployments[0].DepartmentName);
            Assert.Equal("Test account holder", actualEmployments[0].RoleTitle);
            Assert.Equal(2012, actualEmployments[0].StartDate.Year);
            Assert.Equal(10, actualEmployments[0].StartDate.Month);
            Assert.Equal(0, actualEmployments[0].StartDate.Day);
            Assert.Equal(0, actualEmployments[0].EndDate.Year);
            Assert.Equal(0, actualEmployments[0].EndDate.Month);
            Assert.Equal(0, actualEmployments[0].EndDate.Day);
            Assert.Equal(new OrcidPutCode(22411).Value, actualEmployments[0].PutCode.Value);

            Assert.Equal("Test university without disambiguated organization", actualEmployments[1].OrganizationName);
            Assert.Equal("", actualEmployments[1].DisambiguatedOrganizationIdentifier);
            Assert.Equal("Astrophysics", actualEmployments[1].DepartmentName);
            Assert.Equal("Professor", actualEmployments[1].RoleTitle);
            Assert.Equal(2018, actualEmployments[1].StartDate.Year);
            Assert.Equal(1, actualEmployments[1].StartDate.Month);
            Assert.Equal(21, actualEmployments[1].StartDate.Day);
            Assert.Equal(2019, actualEmployments[1].EndDate.Year);
            Assert.Equal(12, actualEmployments[1].EndDate.Month);
            Assert.Equal(31, actualEmployments[1].EndDate.Day);
            Assert.Equal(new OrcidPutCode(47431).Value, actualEmployments[1].PutCode.Value);
        }

        [Fact(DisplayName = "Get publications")]
        public void TestGetPublications()
        {
            var orcidJsonParserService = new OrcidJsonParserService();
            var jsonStr = getOrcidJsonRecord();
            var actualPublications = orcidJsonParserService.GetPublications(jsonStr);
            Assert.True(actualPublications.Count == 3, "Publications: should parse 3 publications");

            Assert.Equal("ORCID: a system to uniquely identify researchers", actualPublications[0].PublicationName);
            Assert.Equal(new OrcidPutCode(1022665).Value, actualPublications[0].PutCode.Value);
            Assert.Equal(2019, actualPublications[0].PublicationYear);
            Assert.Equal("", actualPublications[0].Doi);
            Assert.Equal("journal-article", actualPublications[0].Type);

            Assert.Equal("My research paper", actualPublications[1].PublicationName);
            Assert.Equal(new OrcidPutCode(1045646).Value, actualPublications[1].PutCode.Value);
            Assert.Equal(2019, actualPublications[1].PublicationYear);
            Assert.Equal("10.1111/test.12241", actualPublications[1].Doi);
            Assert.Equal("journal-article B", actualPublications[1].Type);

            Assert.Equal("Another publication", actualPublications[2].PublicationName);
            Assert.Equal(new OrcidPutCode(733536).Value, actualPublications[2].PutCode.Value);
            Assert.Null(actualPublications[2].PublicationYear);
            Assert.Equal("10.1087/20120404", actualPublications[2].Doi);
            Assert.Equal("journal-article C", actualPublications[2].Type);
        }

        [Fact(DisplayName = "Get invited positions, distinctions, memberships and services")]
        public void TestGetInvitedPositionsDistinctionsMembershipsAndServices()
        {
            var orcidJsonParserService = new OrcidJsonParserService();
            var jsonStr = getOrcidJsonRecord();
            var actual = orcidJsonParserService.GetInvitedPositionsDistinctionsMembershipsAndServices(jsonStr);
            Assert.True(actual.Count == 7, "Should parse 7 items");

            Assert.Equal(Constants.OrcidResearchActivityTypes.INVITED_POSITION, actual[0].OrcidActivityType);
            Assert.Equal(new OrcidPutCode(29778).Value, actual[0].PutCode.Value);
            Assert.Equal("University of Michigan", actual[0].OrganizationName);
            Assert.Equal("1259", actual[0].DisambiguatedOrganizationIdentifier);
            Assert.Equal("RINGGOLD", actual[0].DisambiguationSource);
            Assert.Equal("Dept Name", actual[0].DepartmentName);
            Assert.Equal("Invited Position Title", actual[0].RoleTitle);
            Assert.Equal(2018, actual[0].StartDate.Year);
            Assert.Equal(2, actual[0].StartDate.Month);
            Assert.Equal(2, actual[0].StartDate.Day);
            Assert.Equal(0, actual[0].EndDate.Year);
            Assert.Equal(0, actual[0].EndDate.Month);
            Assert.Equal(0, actual[0].EndDate.Day);
            Assert.Equal("http://orcid.org/umich", actual[0].Url);

            Assert.Equal(Constants.OrcidResearchActivityTypes.DISTINCTION, actual[1].OrcidActivityType);
            Assert.Equal(new OrcidPutCode(29770).Value, actual[1].PutCode.Value);
            Assert.Equal("Test organization 1", actual[1].OrganizationName);
            Assert.Equal("http://dx.doi.org/10.13039/321321", actual[1].DisambiguatedOrganizationIdentifier);
            Assert.Equal("FUNDREF", actual[1].DisambiguationSource);
            Assert.Equal("Test Department", actual[1].DepartmentName);
            Assert.Equal("Test Distinction", actual[1].RoleTitle);
            Assert.Equal(2012, actual[1].StartDate.Year);
            Assert.Equal(7, actual[1].StartDate.Month);
            Assert.Equal(1, actual[1].StartDate.Day);
            Assert.Equal(0, actual[1].EndDate.Year);
            Assert.Equal(0, actual[1].EndDate.Month);
            Assert.Equal(0, actual[1].EndDate.Day);
            Assert.Equal("", actual[1].Url);

            Assert.Equal(Constants.OrcidResearchActivityTypes.DISTINCTION, actual[2].OrcidActivityType);
            Assert.Equal(new OrcidPutCode(29771).Value, actual[2].PutCode.Value);
            Assert.Equal("Test organization 2", actual[2].OrganizationName);
            Assert.Equal("53455", actual[2].DisambiguatedOrganizationIdentifier);
            Assert.Equal("RINGGOLD", actual[2].DisambiguationSource);
            Assert.Equal("Test Department 2", actual[2].DepartmentName);
            Assert.Equal("Test Distinction 2", actual[2].RoleTitle);
            Assert.Equal(2014, actual[2].StartDate.Year);
            Assert.Equal(10, actual[2].StartDate.Month);
            Assert.Equal(21, actual[2].StartDate.Day);
            Assert.Equal(0, actual[2].EndDate.Year);
            Assert.Equal(0, actual[2].EndDate.Month);
            Assert.Equal(0, actual[2].EndDate.Day);
            Assert.Equal("https://www.testdomain.test", actual[2].Url);

            Assert.Equal(Constants.OrcidResearchActivityTypes.MEMBERSHIP, actual[3].OrcidActivityType);
            Assert.Equal(new OrcidPutCode(54008).Value, actual[3].PutCode.Value);
            Assert.Equal("Joensuun lyseon lukio", actual[3].OrganizationName);
            Assert.Equal("101236", actual[3].DisambiguatedOrganizationIdentifier);
            Assert.Equal("RINGGOLD", actual[3].DisambiguationSource);
            Assert.Equal("Test Membership Department 2", actual[3].DepartmentName);
            Assert.Equal("Test Membership Type 2", actual[3].RoleTitle);
            Assert.Equal(2016, actual[3].StartDate.Year);
            Assert.Equal(1, actual[3].StartDate.Month);
            Assert.Equal(13, actual[3].StartDate.Day);
            Assert.Equal(2019, actual[3].EndDate.Year);
            Assert.Equal(2, actual[3].EndDate.Month);
            Assert.Equal(20, actual[3].EndDate.Day);
            Assert.Equal("https://www.joensuu.fi/", actual[3].Url);

            Assert.Equal(Constants.OrcidResearchActivityTypes.MEMBERSHIP, actual[4].OrcidActivityType);
            Assert.Equal(new OrcidPutCode(54007).Value, actual[4].PutCode.Value);
            Assert.Equal("University of Oulu", actual[4].OrganizationName);
            Assert.Equal("https://ror.org/03yj89h83", actual[4].DisambiguatedOrganizationIdentifier);
            Assert.Equal("ROR", actual[4].DisambiguationSource);
            Assert.Equal("Test Membership Department", actual[4].DepartmentName);
            Assert.Equal("Test Membership Type", actual[4].RoleTitle);
            Assert.Equal(1989, actual[4].StartDate.Year);
            Assert.Equal(1, actual[4].StartDate.Month);
            Assert.Equal(24, actual[4].StartDate.Day);
            Assert.Equal(2010, actual[4].EndDate.Year);
            Assert.Equal(2, actual[4].EndDate.Month);
            Assert.Equal(26, actual[4].EndDate.Day);
            Assert.Equal("https://www.oulu.fi/en", actual[4].Url);

            Assert.Equal(Constants.OrcidResearchActivityTypes.SERVICE, actual[5].OrcidActivityType);
            Assert.Equal(new OrcidPutCode(54009).Value, actual[5].PutCode.Value);
            Assert.Equal("Lahden kaupunki", actual[5].OrganizationName);
            Assert.Equal("86631", actual[5].DisambiguatedOrganizationIdentifier);
            Assert.Equal("RINGGOLD", actual[5].DisambiguationSource);
            Assert.Equal("Test Service Department", actual[5].DepartmentName);
            Assert.Equal("Test Service Role", actual[5].RoleTitle);
            Assert.Equal(2015, actual[5].StartDate.Year);
            Assert.Equal(2, actual[5].StartDate.Month);
            Assert.Equal(27, actual[5].StartDate.Day);
            Assert.Equal(2018, actual[5].EndDate.Year);
            Assert.Equal(3, actual[5].EndDate.Month);
            Assert.Equal(14, actual[5].EndDate.Day);
            Assert.Equal("https://www.google.fi", actual[5].Url);

            Assert.Equal(Constants.OrcidResearchActivityTypes.SERVICE, actual[6].OrcidActivityType);
            Assert.Equal(new OrcidPutCode(54010).Value, actual[6].PutCode.Value);
            Assert.Equal("Rovaniemen kaupunki", actual[6].OrganizationName);
            Assert.Equal("86672", actual[6].DisambiguatedOrganizationIdentifier);
            Assert.Equal("RINGGOLD", actual[6].DisambiguationSource);
            Assert.Equal("Test Service Department 2", actual[6].DepartmentName);
            Assert.Equal("Test Service Role 2", actual[6].RoleTitle);
            Assert.Equal(2011, actual[6].StartDate.Year);
            Assert.Equal(1, actual[6].StartDate.Month);
            Assert.Equal(15, actual[6].StartDate.Day);
            Assert.Equal(2016, actual[6].EndDate.Year);
            Assert.Equal(2, actual[6].EndDate.Month);
            Assert.Equal(17, actual[6].EndDate.Day);
            Assert.Equal("https://www.rovaniemi.fi", actual[6].Url);
        }
    }
}