using System;
using Xunit;
using api.Services;
using api.Models.Orcid;
using System.IO;
using System.Collections.Generic;

namespace api.Tests
{
    [Collection("Parsing data from ORCID record json")]
    public class OrcidJsonParserTests
    {
        // Get ORCID record.
        // Test file is a copy of ORCID's sandbox https://pub.sandbox.orcid.org/v3.0/0000-0002-9227-8514/record
        private string getOrcidJsonRecord()
        {
            var filePath = $@"{Directory.GetParent(Environment.CurrentDirectory).Parent.Parent.FullName}/Infrastructure/orcidSandbox_0000-0002-9227-8514_record.json";
            var reader = new StreamReader(filePath);
            return reader.ReadToEnd();
        }

        // Get ORCID personal details.
        // Test file is a copy of ORCID's sandbox https://pub.sandbox.orcid.org/v3.0/0000-0002-9227-8514/personal-details
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
            Assert.True(actualEducations.Count == 1, "Educations: should parse 1 education");
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
        }

        [Fact(DisplayName = "Get employments")]
        public void TestGetEmployments()
        {
            var orcidJsonParserService = new OrcidJsonParserService();
            var jsonStr = getOrcidJsonRecord();
            var actualEmployments = orcidJsonParserService.GetEmployments(jsonStr);
            Assert.True(actualEmployments.Count == 1, "Educations: should parse 1 employment");
            Assert.Equal("ORCID", actualEmployments[0].OrganizationName);
            Assert.Equal("QA and Testing", actualEmployments[0].DepartmentName);
            Assert.Equal("Test account holder", actualEmployments[0].RoleTitle);
            Assert.Equal(2012, actualEmployments[0].StartDate.Year);
            Assert.Equal(10, actualEmployments[0].StartDate.Month);
            Assert.Equal(0, actualEmployments[0].StartDate.Day);
            Assert.Equal(0, actualEmployments[0].EndDate.Year);
            Assert.Equal(0, actualEmployments[0].EndDate.Month);
            Assert.Equal(0, actualEmployments[0].EndDate.Day);
            Assert.Equal(new OrcidPutCode(22411).Value, actualEmployments[0].PutCode.Value);
        }

        [Fact(DisplayName = "Get publications")]
        public void TestGetPublications()
        {
            var orcidJsonParserService = new OrcidJsonParserService();
            var jsonStr = getOrcidJsonRecord();
            var actualPublications = orcidJsonParserService.GetPublications(jsonStr);
            Assert.True(actualPublications.Count == 4, "Publications: should parse 4 publications");
            Assert.Equal("ORCID: a system to uniquely identify researchers", actualPublications[0].PublicatonName);
            Assert.Equal(new OrcidPutCode(1022665).Value, actualPublications[0].PutCode.Value);
            Assert.Equal(2019, actualPublications[0].PublicationYear);
            Assert.Equal("", actualPublications[0].DoiHandle);
            Assert.Equal("journal-article", actualPublications[0].Type);
            Assert.Equal("ORCID: a system to uniquely identify researchers", actualPublications[1].PublicatonName);
            Assert.Equal(new OrcidPutCode(1045646).Value, actualPublications[1].PutCode.Value);
            Assert.Equal(2019, actualPublications[1].PublicationYear);
            Assert.Equal("10.1111/test.12241", actualPublications[1].DoiHandle);
            Assert.Equal("journal-article", actualPublications[1].Type);
            Assert.Equal("ORCID: a system to uniquely identify researchers", actualPublications[2].PublicatonName);
            Assert.Equal(new OrcidPutCode(733536).Value, actualPublications[2].PutCode.Value);
            Assert.Equal(2012, actualPublications[2].PublicationYear);
            Assert.Equal("10.1087/20120404", actualPublications[2].DoiHandle);
            Assert.Equal("journal-article", actualPublications[2].Type);
            Assert.Equal("ORCID: a system to uniquely identify researchers", actualPublications[3].PublicatonName);
            Assert.Equal(new OrcidPutCode(733535).Value, actualPublications[3].PutCode.Value);
            Assert.Equal(2012, actualPublications[3].PublicationYear);
            Assert.Equal("10.1087/20120404", actualPublications[3].DoiHandle);
            Assert.Equal("journal-article", actualPublications[3].Type);
        }

        //[Fact(DisplayName = "Template")]
        //public void TestTemplate()
        //{
        //    var orcidJsonParserService = new OrcidJsonParserService();
        //    var jsonStr = getOrcidJsonRecord();

        //    var biography = orcidJsonParserService.GetBiography(jsonStr);
        //    var givenNames = orcidJsonParserService.GetGivenNames(jsonStr);
        //    var familyName = orcidJsonParserService.GetFamilyName(jsonStr);
        //    var creditName = orcidJsonParserService.GetCreditName(jsonStr);
        //    var otherNames = orcidJsonParserService.GetOtherNames(jsonStr);
        //    var researcherUrls = orcidJsonParserService.GetResearcherUrls(jsonStr);
        //    var emails = orcidJsonParserService.GetEmails(jsonStr);
        //    var keywords = orcidJsonParserService.GetKeywords(jsonStr);
        //    var externalIdentifiers = orcidJsonParserService.GetExternalIdentifiers(jsonStr);
        //    var educations = orcidJsonParserService.GetEducations(jsonStr);
        //    var employments = orcidJsonParserService.GetEmployments(jsonStr);
        //    Assert.True(employments.Count == 0, "Educations: parsed correct number of employments");
        //}
    }
}