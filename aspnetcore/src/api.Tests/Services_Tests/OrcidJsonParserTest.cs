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
        // Get ORCID record json test file
        private string getOrcidRecordJson()
        {
            var filePath = $@"{Directory.GetParent(Environment.CurrentDirectory).Parent.Parent.FullName}/Infrastructure/testDataOrcidRecord.json";
            var reader = new StreamReader(filePath);
            return reader.ReadToEnd();
        }

        [Fact(DisplayName = "Get given names")]
        public void TestGetGivenNames()
        {
            var orcidJsonParserService = new OrcidJsonParserService();
            var expectedGivenNames = "John";
            var jsonStr = getOrcidRecordJson();
            Assert.Equal(expectedGivenNames, orcidJsonParserService.GetGivenNames(jsonStr));
        }

        [Fact(DisplayName = "Get family name")]
        public void TestGetFamilyName()
        {
            var orcidJsonParserService = new OrcidJsonParserService();
            var expectedFamilyName = "Smith";
            var jsonStr = getOrcidRecordJson();
            Assert.Equal(expectedFamilyName, orcidJsonParserService.GetFamilyName(jsonStr));
        }

        [Fact(DisplayName = "Get credit name")]
        public void TestGetCreditName()
        {
            var orcidJsonParserService = new OrcidJsonParserService();
            var expectedCreditName = "Johnson";
            var jsonStr = getOrcidRecordJson();
            Assert.Equal(expectedCreditName, orcidJsonParserService.GetCreditName(jsonStr));
        }

        [Fact(DisplayName = "Get other names")]
        public void TestGetOtherNames()
        {
            var orcidJsonParserService = new OrcidJsonParserService();
            var expectedOtherNames = new List<string> { };
            expectedOtherNames.Add("JS");
            var jsonStr = getOrcidRecordJson();
            Assert.Equal(expectedOtherNames, orcidJsonParserService.GetOtherNames(jsonStr));
        }

        [Fact(DisplayName = "Get biography")]
        public void TestGetBiography()
        {
            var orcidJsonParserService = new OrcidJsonParserService();
            var expectedBiography = "Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua.";
            var jsonStr = getOrcidRecordJson();
            Assert.Equal(expectedBiography, orcidJsonParserService.GetBiography(jsonStr));
        }

        [Fact(DisplayName = "Get researcher urls")]
        public void TestGetResearcherUrls()
        {
            var orcidJsonParserService = new OrcidJsonParserService();
            var expectedResearcherUrls = new List<(string UrlName, string Url)> {};
            expectedResearcherUrls.Add((UrlName: "Qwerty Consulting", Url: "https://www.qwertyconsulting.fi/"));
            var jsonStr = getOrcidRecordJson();
            Assert.Equal(expectedResearcherUrls, orcidJsonParserService.GetResearcherUrls(jsonStr));
        }

        [Fact(DisplayName = "Get emails")]
        public void TestGetEmails()
        {
            var orcidJsonParserService = new OrcidJsonParserService();
            var expectedEmails = new List<string> { };
            expectedEmails.Add("john.smith@qwertyconsulting.fi");
            var jsonStr = getOrcidRecordJson();
            Assert.Equal(expectedEmails, orcidJsonParserService.GetEmails(jsonStr));
        }

        [Fact(DisplayName = "Get keywords")]
        public void TestGetKeywords()
        {
            var orcidJsonParserService = new OrcidJsonParserService();
            var expectedKeywords = new List<string> { };
            expectedKeywords.Add("consulting");
            expectedKeywords.Add("programming");
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
                    externalIdType: "Scopus Author ID",
                    externalIdValue: "123456",
                    externalIdUrl: "http://www.scopus.com/inward/authorDetails.url?authorID=123456&partnerID=ABC"
                )
            );
            var jsonStr = getOrcidRecordJson();
            Assert.Equal(expectedExternalIdentifiers, orcidJsonParserService.GetExternalIdentifiers(jsonStr));
        }
    }
}