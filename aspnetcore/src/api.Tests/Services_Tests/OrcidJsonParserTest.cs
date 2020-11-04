using System;
using Xunit;
using api.Services;
using System.IO;

namespace api.Tests
{
    [Collection("Parsing data from ORCID record json")]
    public class OrcidJsonParserTests
    {
        [Fact(DisplayName = "Get biography")]
        public void TestGetBiography()
        {
            var orcidJsonParserService = new OrcidJsonParserService();
            var expectedBiography = "Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua.";
            var filePath = $@"{Directory.GetParent(Environment.CurrentDirectory).Parent.Parent.FullName}/Infrastructure/testDataOrcidRecord.json";
            var reader = new StreamReader(filePath);
            var jsonStr = reader.ReadToEnd();
            Assert.Equal(expectedBiography, orcidJsonParserService.GetBiography(jsonStr));
        }
    }
}