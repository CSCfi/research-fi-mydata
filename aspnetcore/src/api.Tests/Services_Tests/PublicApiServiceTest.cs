using System;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Moq;
using Xunit;
using api.Services;

namespace api.Tests
{
    [Collection("Public API service tests")]
    public class PublicApiServiceTests
    {
        [Fact(DisplayName = "Get username from national identification number")]
        public void GetUsernameFromNationalIdentificationNumber_01()
        {
            var publicApiService = new PublicApiService(null, null, null);

            // Value 010170-999R is a test value for Finnish national identification number. No real person is associated with this value.
            Assert.Equal("5f0c2c8d2107f4700fb5aa1ef717ac03", publicApiService.GetUsernameFromNationalIdentificationNumber("010170-999R"));
        }

        [Fact(DisplayName = "GetOrcidIdFromKeycloak: Keycloak response contains user with ORCID")]
        public async Task GetOrcidIdFromKeycloak_UserWithOrcid_ReturnsOrcidId()
        {
            const string json = "[{\"id\":\"14ee8d56-97dc-4e7e-b645-6ef5ccec4b33\",\"username\":\"3b56c0dd67cb1e0af107fb4ccc047359\",\"firstName\":\"Alice\",\"lastName\":\"Smith\",\"emailVerified\":false,\"attributes\":{\"orcid\":[\"0000-1111-2222-3333\"]},\"enabled\":true,\"createdTimestamp\":1734609919788,\"totp\":false,\"disableableCredentialTypes\":[],\"requiredActions\":[],\"notBefore\":0,\"access\":{\"manage\":true}}]";
            var service = CreateServiceWithResponse(json);

            string? result = await service.GetOrcidIdFromKeycloak("3b56c0dd67cb1e0af107fb4ccc047359");

            Assert.Equal("0000-1111-2222-3333", result);
        }

        [Fact(DisplayName = "GetOrcidIdFromKeycloak: Keycloak response contains user without ORCID attribute")]
        public async Task GetOrcidIdFromKeycloak_UserWithoutOrcid_ReturnsNull()
        {
            const string json = "[{\"id\":\"14ee8d56-97dc-4e7e-b645-6ef5ccec4b33\",\"username\":\"3b56c0dd67cb1e0af107fb4ccc047359\",\"firstName\":\"Alice\",\"lastName\":\"Smith\",\"emailVerified\":false,\"enabled\":true,\"createdTimestamp\":1734609919788,\"totp\":false,\"disableableCredentialTypes\":[],\"requiredActions\":[],\"notBefore\":0,\"access\":{\"manage\":true}}]";
            var service = CreateServiceWithResponse(json);

            string? result = await service.GetOrcidIdFromKeycloak("3b56c0dd67cb1e0af107fb4ccc047359");

            Assert.Null(result);
        }

        [Fact(DisplayName = "GetOrcidIdFromKeycloak: Keycloak response does not contain user")]
        public async Task GetOrcidIdFromKeycloak_NoUserFound_ReturnsNull()
        {
            const string json = "[]";
            var service = CreateServiceWithResponse(json);

            string? result = await service.GetOrcidIdFromKeycloak("nonexistent");

            Assert.Null(result);
        }

        private static PublicApiService CreateServiceWithResponse(string jsonResponse)
        {
            var handler = new FakeHttpMessageHandler(jsonResponse);
            var httpClient = new HttpClient(handler)
            {
                BaseAddress = new Uri("https://keycloak.example.com/users")
            };
            var factoryMock = new Mock<IHttpClientFactory>();
            factoryMock.Setup(f => f.CreateClient("keycloakClient")).Returns(httpClient);
            return new PublicApiService(null, factoryMock.Object, null);
        }

        private class FakeHttpMessageHandler : HttpMessageHandler
        {
            private readonly string _responseBody;

            public FakeHttpMessageHandler(string responseBody)
            {
                _responseBody = responseBody;
            }

            protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
            {
                return Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = new StringContent(_responseBody, Encoding.UTF8, "application/json")
                });
            }
        }
    }
}