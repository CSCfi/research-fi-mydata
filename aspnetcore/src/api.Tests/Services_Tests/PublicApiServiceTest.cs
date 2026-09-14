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