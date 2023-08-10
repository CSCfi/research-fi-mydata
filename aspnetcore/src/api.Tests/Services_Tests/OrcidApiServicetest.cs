using Xunit;
using api.Services;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.Collections;

namespace api.Tests
{
    [Collection("ORCID api service tests")]
    public class OrcidApiServiceTests
    {
        /*
         * Helper method for setting up IConfiguration
         */
        public IConfiguration CreateTestConfiguration()
        {
            Dictionary<string, string> inMemorySettings = new Dictionary<string, string> {
                { "SERVICEURL", "https://mydata-api.service.example.fi"},
                { "ORCID:WEBHOOK:ENABLED", "true"},
                { "ORCID:WEBHOOK:API", "https://api.sandbox.orcid.org/"},
                { "ORCID:WEBHOOK:ACCESSTOKEN", "fake20e90b06-0384f05d-3ffc9bd9-31ab06ff" }
            };
            IConfiguration testConfiguration = new ConfigurationBuilder()
                .AddInMemoryCollection(inMemorySettings)
                .Build();
            return testConfiguration;
        }

        /*
         * Create test data for unit test IsOrcidWebhookEnabled_ReturnsIsOrcidWebhookFeatureEnabled
         */
        public class IsOrcidWebhookEnabledTestData : IEnumerable<object[]>
        {
            public IEnumerator<object[]> GetEnumerator()
            {
                // Valid configuration
                yield return new object[] {
                    new Dictionary<string, string> {
                        { "SERVICEURL", "https://mydata-api.service.example.fi"},
                        { "ORCID:WEBHOOK:ENABLED", "true"},
                        { "ORCID:WEBHOOK:API", "https://api.sandbox.orcid.org/"},
                        { "ORCID:WEBHOOK:ACCESSTOKEN", "fake20e90b06-0384f05d-3ffc9bd9-31ab06ff" }
                    },
                    true
                };
                // Required configuration completely missing
                yield return new object[] {
                    new Dictionary<string, string> {},
                    false
                };
                // SERVICEURL is not defined
                yield return new object[] {
                    new Dictionary<string, string> {
                        { "ORCID:WEBHOOK:ENABLED", "false"},
                        { "ORCID:WEBHOOK:API", "https://api.sandbox.orcid.org/"},
                        { "ORCID:WEBHOOK:ACCESSTOKEN", "fake20e90b06-0384f05d-3ffc9bd9-31ab06ff" }
                    },
                    false
                };
                // ORCID:WEBHOOK:ENABLED = false
                yield return new object[] {
                    new Dictionary<string, string> {
                        { "SERVICEURL", "https://mydata-api.service.example.fi"},
                        { "ORCID:WEBHOOK:ENABLED", "false"},
                        { "ORCID:WEBHOOK:API", "https://api.sandbox.orcid.org/"},
                        { "ORCID:WEBHOOK:ACCESSTOKEN", "fake20e90b06-0384f05d-3ffc9bd9-31ab06ff" }
                    },
                    false
                };
                // ORCID:WEBHOOK:ENABLED is not defined
                yield return new object[] {
                    new Dictionary<string, string> {
                        { "SERVICEURL", "https://mydata-api.service.example.fi"},
                        { "ORCID:WEBHOOK:API", "https://api.sandbox.orcid.org/"},
                        { "ORCID:WEBHOOK:ACCESSTOKEN", "fake20e90b06-0384f05d-3ffc9bd9-31ab06ff" }
                    },
                    false
                };
                // ORCID:WEBHOOK:API is not defined
                yield return new object[] {
                    new Dictionary<string, string> {
                        { "SERVICEURL", "https://mydata-api.service.example.fi"},
                        { "ORCID:WEBHOOK:ENABLED", "true"},
                        { "ORCID:WEBHOOK:ACCESSTOKEN", "fake20e90b06-0384f05d-3ffc9bd9-31ab06ff" }
                    },
                    false
                };
                // ORCID:WEBHOOK:ACCESSTOKEN is not defined
                yield return new object[] {
                    new Dictionary<string, string> {
                        { "SERVICEURL", "https://mydata-api.service.example.fi"},
                        { "ORCID:WEBHOOK:ENABLED", "true"},
                        { "ORCID:WEBHOOK:API", "https://api.sandbox.orcid.org/"}
                    },
                    false
                };
            }
            IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
        }

        [Fact(DisplayName = "Get correct path for ORCID record")]
        public void GetOrcidRecordPath_OrcidId_ReturnsCorrectPath()
        {
            // Arrange
            OrcidApiService orcidApiService = new OrcidApiService();
            // Act
            string actualOrcidRecordPath = orcidApiService.GetOrcidRecordPath("1234-5678-8765-4321");
            // Assert
            Assert.Equal("1234-5678-8765-4321/record", actualOrcidRecordPath);
        }

        [Theory(DisplayName = "Check is ORCID webhook feature enabled")]
        [ClassData(typeof(IsOrcidWebhookEnabledTestData))]
        public void IsOrcidWebhookEnabled_ReturnsIsOrcidWebhookFeatureEnabled(Dictionary<string, string> inMemorySettings, bool expectedResult)
        {
            // Arrange
            IConfiguration testConfiguration = new ConfigurationBuilder()
                .AddInMemoryCollection(inMemorySettings)
                .Build();
            OrcidApiService orcidApiService = new OrcidApiService(testConfiguration);
            // Act
            bool actualIsOrcidWebhookEnabled = orcidApiService.IsOrcidWebhookEnabled();
            // Assert
            Assert.Equal(expectedResult, actualIsOrcidWebhookEnabled);
        }

        [Fact(DisplayName = "Get ORCID webhook callback URI for ORCID ID")]
        public void GetOrcidWebhookCallbackUri_OrcidId_ReturnsCorrectCallbackUri()
        {
            // Arrange
            IConfiguration testConfiguration = CreateTestConfiguration();
            OrcidApiService orcidApiService = new OrcidApiService(testConfiguration);
            // Act
            string actualOrcidWebhookCallbackUri = orcidApiService.GetOrcidWebhookCallbackUri(orcidId: "0000-0001-2000-3456");
            // Assert
            Assert.Equal(
                "https://mydata-api.service.example.fi/api/webhook/orcid/0000-0001-2000-3456",
                actualOrcidWebhookCallbackUri
            );
        }

        [Fact(DisplayName = "Get URL encoded string")]
        public void GetUrlEncodedString_URI_ReturnsUrlEncodedString()
        {
            // Arrange
            OrcidApiService orcidApiService = new OrcidApiService();
            // Act
            string actualUrlEncodedString =
                orcidApiService.GetUrlEncodedString("https://mydata-api.service.example.fi/api/webhook/orcid/0000-0001-2000-3456");
            // Assert
            Assert.Equal(
                "https%3a%2f%2fmydata-api.service.example.fi%2fapi%2fwebhook%2forcid%2f0000-0001-2000-3456",
                actualUrlEncodedString
            );
        }

        [Fact(DisplayName = "Get ORCID webhook registration URL for ORCID ID")]
        public void GetOrcidWebhookRegistrationUri_OrcidId_ReturnsCorrectRegistrationUri()
        {
            // Arrange
            IConfiguration testConfiguration = CreateTestConfiguration();
            OrcidApiService orcidApiService = new OrcidApiService(testConfiguration);
            // Act
            string actualOrcidWebhookRegistrationUri = orcidApiService.GetOrcidWebhookRegistrationUri("0000-0001-2000-3456");
            // Assert
            Assert.Equal(
                "https://api.sandbox.orcid.org/0000-0001-2000-3456/webhook/https%3a%2f%2fmydata-api.service.example.fi%2fapi%2fwebhook%2forcid%2f0000-0001-2000-3456",
                actualOrcidWebhookRegistrationUri
            );
        }

        [Fact(DisplayName = "Get ORCID webhook access token from configuration")]
        public void GetOrcidWebhookAccessToken_ReturnsOrcidWebhookAccessTokenFromConfiguration()
        {
            // Arrange
            IConfiguration testConfiguration = CreateTestConfiguration();
            OrcidApiService orcidApiService = new OrcidApiService(testConfiguration);
            // Act
            string actualOrcidWebhookAccessToken = orcidApiService.GetOrcidWebhookAccessToken();
            // Assert
            Assert.Equal(
                "fake20e90b06-0384f05d-3ffc9bd9-31ab06ff",
                actualOrcidWebhookAccessToken
            );
        }
    }
}