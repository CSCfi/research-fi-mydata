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
        public string getOrcidRecordJson()
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

        [Fact(DisplayName = "Get family names")]
        public void TestGetFamilyNames()
        {
            var orcidJsonParserService = new OrcidJsonParserService();
            var expectedFamilyNames = "Smith";
            var jsonStr = getOrcidRecordJson();
            Assert.Equal(expectedFamilyNames, orcidJsonParserService.GetFamilyName(jsonStr));
        }

        [Fact(DisplayName = "Get biography")]
        public void TestGetBiography()
        {
            var orcidJsonParserService = new OrcidJsonParserService();
            var expectedBiography = "Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua.";
            var jsonStr = getOrcidRecordJson();
            Assert.Equal(expectedBiography, orcidJsonParserService.GetBiography(jsonStr));
        }

        [Fact(DisplayName = "Get web links")]
        public void TestGetWebLinks()
        {
            var orcidJsonParserService = new OrcidJsonParserService();
            var expectedWebLinks = new List<(string LinkName, string LinkUrl)> {};
            expectedWebLinks.Add(("Qwerty Consulting", "https://www.qwertyconsulting.fi/"));
            var jsonStr = getOrcidRecordJson();
            Assert.Equal(expectedWebLinks, orcidJsonParserService.GetWebLinks(jsonStr));
        }
    }
}