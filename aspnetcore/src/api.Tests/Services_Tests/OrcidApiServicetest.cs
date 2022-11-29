using Xunit;
using api.Services;

namespace api.Tests
{
    [Collection("ORCID api service tests")]
    public class OrcidApiServiceTests
    {
        [Fact(DisplayName = "Get correct path for ORCID record")]
        public void getCorrectPathForOrcidRecord()
        {
            OrcidApiService orcidApiService = new OrcidApiService();
            Assert.Equal("1234-5678-8765-4321/record", orcidApiService.GetOrcidRecordPath("1234-5678-8765-4321"));
        }
    }
}