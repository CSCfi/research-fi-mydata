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

        [Fact(DisplayName = "Get URL encoded string")]
        public void getUrlEncodedString()
        {
            OrcidApiService orcidApiService = new OrcidApiService();
            Assert.Equal(
                "https%3a%2f%2fmyservice.fi%2fapi%2fwebhook%2forcid%2f0000-0001-2000-3456",
                orcidApiService.GetUrlEncodedString("https://myservice.fi/api/webhook/orcid/0000-0001-2000-3456")
            );
        }

        [Fact(DisplayName = "Get ORCID webhook callback URI")]
        public void getOrcidWebhookCallbackUri()
        {
            OrcidApiService orcidApiService = new OrcidApiService();
            Assert.Equal(
                "abc",
                orcidApiService.GetOrcidWebhookCallbackUri(orcidId: "0000-0001-2000-3456")
            );
        }
    }
}