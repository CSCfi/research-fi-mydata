using System;
using Xunit;
using api.Services;
using System.IO;
using System.Collections.Generic;

namespace api.Tests
{
    [Collection("Parsing data from ORCID record json")]
    public class OrcidJsonParserTests
    {
        // Get ORCID record json test file.
        // Test file is a copy of ORCID's example record https://sandbox.orcid.org/0000-0002-9227-8514
        private string getOrcidRecordJson()
        {
            var filePath = $@"{Directory.GetParent(Environment.CurrentDirectory).Parent.Parent.FullName}/Infrastructure/orcidRecordSandbox_0000-0002-9227-8514.json";
            var reader = new StreamReader(filePath);
            return reader.ReadToEnd();
        }

        [Fact(DisplayName = "Get given names")]
        public void TestGetGivenNames()
        {
            var orcidJsonParserService = new OrcidJsonParserService();
            var expectedGivenNames = "Sofia";
            var jsonStr = getOrcidRecordJson();
            Assert.Equal(expectedGivenNames, orcidJsonParserService.GetGivenNames(jsonStr));
        }

        [Fact(DisplayName = "Get family name")]
        public void TestGetFamilyName()
        {
            var orcidJsonParserService = new OrcidJsonParserService();
            var expectedFamilyName = "Garcia";
            var jsonStr = getOrcidRecordJson();
            Assert.Equal(expectedFamilyName, orcidJsonParserService.GetFamilyName(jsonStr));
        }

        [Fact(DisplayName = "Get credit name")]
        public void TestGetCreditName()
        {
            var orcidJsonParserService = new OrcidJsonParserService();
            var expectedCreditName = "Sofia Maria Hernandez Garcia";
            var jsonStr = getOrcidRecordJson();
            Assert.Equal(expectedCreditName, orcidJsonParserService.GetCreditName(jsonStr));
        }

        [Fact(DisplayName = "Get other names")]
        public void TestGetOtherNames()
        {
            var orcidJsonParserService = new OrcidJsonParserService();
            var expectedOtherNames = new List<string> { };
            expectedOtherNames.Add("Sofia Maria Garcia");
            expectedOtherNames.Add("София Мария Эрнандес Гарсия");
            expectedOtherNames.Add("索菲亚玛丽亚 加西亚");
            var jsonStr = getOrcidRecordJson();
            Assert.Equal(expectedOtherNames, orcidJsonParserService.GetOtherNames(jsonStr));
        }

        [Fact(DisplayName = "Get biography")]
        public void TestGetBiography()
        {
            var orcidJsonParserService = new OrcidJsonParserService();
            var expectedBiography = "Sofia Maria Hernandez Garcia is the researcher that is used as an example ORCID record holder.";
            var jsonStr = getOrcidRecordJson();
            Assert.Equal(expectedBiography, orcidJsonParserService.GetBiography(jsonStr));
        }

        [Fact(DisplayName = "Get researcher urls")]
        public void TestGetResearcherUrls()
        {
            var orcidJsonParserService = new OrcidJsonParserService();
            var expectedResearcherUrls = new List<(string UrlName, string Url)> {};
            expectedResearcherUrls.Add((UrlName: "Twitter", Url: "https://twitter.com/ORCIDsofia"));
            var jsonStr = getOrcidRecordJson();
            Assert.Equal(expectedResearcherUrls, orcidJsonParserService.GetResearcherUrls(jsonStr));
        }

        [Fact(DisplayName = "Get emails")]
        public void TestGetEmails()
        {
            var orcidJsonParserService = new OrcidJsonParserService();
            var expectedEmails = new List<string> { };
            expectedEmails.Add("s.garcia@orcid.org");
            var jsonStr = getOrcidRecordJson();
            Assert.Equal(expectedEmails, orcidJsonParserService.GetEmails(jsonStr));
        }

        [Fact(DisplayName = "Get keywords")]
        public void TestGetKeywords()
        {
            var orcidJsonParserService = new OrcidJsonParserService();
            var expectedKeywords = new List<string> { };
            expectedKeywords.Add("QA and testing");
            expectedKeywords.Add("QA and testing"); // This is intentionally twice, because the example record has a duplicate
            expectedKeywords.Add("Additional keyword");
            var jsonStr = getOrcidRecordJson();
            Assert.Equal(expectedKeywords, orcidJsonParserService.GetKeywords(jsonStr));
        }

        [Fact(DisplayName = "Get external identifiers")]
        public void TestGetExternalIdentifiers()
        {
            var orcidJsonParserService = new OrcidJsonParserService();
            var expectedExternalIdentifiers = new List<(string externalIdType, string externalIdValue, string externalIdUrl)> { };
            expectedExternalIdentifiers.Add(
                (
                    externalIdType: "Loop profile",
                    externalIdValue: "558",
                    externalIdUrl: "http://loop.frontiers-sandbox-int.info/people/559/overview?referrer=orcid_profile"
                )
            );
            expectedExternalIdentifiers.Add(
                (
                    externalIdType: "Personal External Identifier",
                    externalIdValue: "506",
                    externalIdUrl: "www.6.com"
                )
            );
            var jsonStr = getOrcidRecordJson();
            Assert.Equal(expectedExternalIdentifiers, orcidJsonParserService.GetExternalIdentifiers(jsonStr));
        }

        [Fact(DisplayName = "Get educations")]
        public void TestGetEducations()
        {
            var orcidJsonParserService = new OrcidJsonParserService();
            var expectedEducations = new List<(string organizationName, string departmentName, string roleTitle, UInt16? startYear, UInt16? startMonth, UInt16? startDay, UInt16? endYear, UInt16? endMonth, UInt16? endDay)> { };
            expectedEducations.Add(
                (
                    organizationName: "Massachusetts Institute of Technology",
                    departmentName: "Testing Department",
                    roleTitle: "BA",
                    startYear: 1997,
                    startMonth: 9,
                    startDay: 2,
                    endYear: 2001,
                    endMonth: 5,
                    endDay: 15
                )
            );
            var jsonStr = getOrcidRecordJson();
            Assert.Equal(expectedEducations, orcidJsonParserService.GetEducations(jsonStr));
        }

        [Fact(DisplayName = "Get employments")]
        public void TestGetEmployments()
        {
            var orcidJsonParserService = new OrcidJsonParserService();
            var expectedEmployments = new List<(string organizationName, string departmentName, string roleTitle, UInt16? startYear, UInt16? startMonth, UInt16? startDay, UInt16? endYear, UInt16? endMonth, UInt16? endDay)> { };
            expectedEmployments.Add(
                (
                    organizationName: "ORCID",
                    departmentName: "QA and Testing",
                    roleTitle: "Test account holder",
                    startYear: 2012,
                    startMonth: 10,
                    startDay: null,
                    endYear: null,
                    endMonth: null,
                    endDay: null
                )
            );
            var jsonStr = getOrcidRecordJson();
            Assert.Equal(expectedEmployments, orcidJsonParserService.GetEmployments(jsonStr));
        }
    }
}