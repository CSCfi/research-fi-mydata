using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.Controllers;
using api.Models.Log;
using ResearchFi.ProfilePublicApi;
using api.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using Xunit;

namespace api.Tests
{
    [Collection("Public API controller tests")]
    public class PublicApiControllerTests
    {
        private static PublicApiController CreateController(string configuredToken, string headerToken, out Mock<IPublicApiService> publicApiServiceMock, ILogger<PublicApiController> logger = null)
        {
            Dictionary<string, string> inMemorySettings = new Dictionary<string, string> { { "PUBLICAPITOKEN", configuredToken } };
            IConfiguration configuration = new ConfigurationBuilder().AddInMemoryCollection(inMemorySettings).Build();

            publicApiServiceMock = new Mock<IPublicApiService>();
            publicApiServiceMock.Setup(s => s.GetProfileDataForPublicApi(It.IsAny<ProfileDataRequest>()))
                .ReturnsAsync(new ProfileDataResponse { ProfileFound = true });

            PublicApiController controller = new PublicApiController(logger ?? NullLogger<PublicApiController>.Instance, publicApiServiceMock.Object, configuration)
            {
                ControllerContext = new ControllerContext { HttpContext = new DefaultHttpContext() }
            };
            if (headerToken != null)
            {
                controller.HttpContext.Request.Headers["publicapitoken"] = headerToken;
            }
            return controller;
        }

        private static ProfileDataRequest CreateRequest(string pidContent)
        {
            return new ProfileDataRequest
            {
                PersonIdentifiers = new List<Identifier> { new Identifier { PidType = "ORCID", PidContent = pidContent } },
                ControllerROR = "029pk6x14"
            };
        }

        private static void AssertBadRequest(IActionResult result, string expectedMessage)
        {
            BadRequestObjectResult badRequest = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal(expectedMessage, badRequest.Value);
        }

        [Fact(DisplayName = "GetProfileForPublicApi - returns 401 when token header is missing")]
        public async Task GetProfileForPublicApi_01()
        {
            PublicApiController controller = CreateController(configuredToken: "secret", headerToken: null, out var serviceMock);

            IActionResult result = await controller.GetProfileData(CreateRequest("Alice"));

            Assert.IsType<UnauthorizedResult>(result);
            serviceMock.Verify(s => s.GetProfileDataForPublicApi(It.IsAny<ProfileDataRequest>()), Times.Never);
        }

        [Fact(DisplayName = "GetProfileForPublicApi - returns 401 when token header does not match")]
        public async Task GetProfileForPublicApi_02()
        {
            PublicApiController controller = CreateController(configuredToken: "secret", headerToken: "wrong", out var serviceMock);

            IActionResult result = await controller.GetProfileData(CreateRequest("Alice"));

            Assert.IsType<UnauthorizedResult>(result);
            serviceMock.Verify(s => s.GetProfileDataForPublicApi(It.IsAny<ProfileDataRequest>()), Times.Never);
        }

        [Fact(DisplayName = "GetProfileForPublicApi - passes response through when token matches")]
        public async Task GetProfileForPublicApi_03()
        {
            PublicApiController controller = CreateController(configuredToken: "secret", headerToken: "secret", out var serviceMock);
            ProfileDataRequest request = CreateRequest("0000-0000-0000-0001");
            ProfileDataResponse expectedResponse = new ProfileDataResponse { ProfileFound = true };
            serviceMock.Setup(s => s.GetProfileDataForPublicApi(request)).ReturnsAsync(expectedResponse);

            IActionResult result = await controller.GetProfileData(request);

            OkObjectResult okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Same(expectedResponse, okResult.Value);
            serviceMock.Verify(s => s.GetProfileDataForPublicApi(request), Times.Once);
        }

        [Fact(DisplayName = "GetProfileForPublicApi - returns 400 when request does not match query schema")]
        public async Task GetProfileForPublicApi_04()
        {
            PublicApiController controller = CreateController(configuredToken: "secret", headerToken: "secret", out var serviceMock);

            AssertBadRequest(await controller.GetProfileData(null), "A request body is required.");
            AssertBadRequest(await controller.GetProfileData(new ProfileDataRequest { ControllerROR = "x" }), "At least one person identifier is required.");
            AssertBadRequest(await controller.GetProfileData(new ProfileDataRequest
            {
                PersonIdentifiers = new List<Identifier>(),
                ControllerROR = "ror"
            }), "At least one person identifier is required.");
            AssertBadRequest(await controller.GetProfileData(new ProfileDataRequest
            {
                PersonIdentifiers = new List<Identifier> { new Identifier { PidType = "ORCID", PidContent = "x" } }
            }), "ControllerROR is required.");
            AssertBadRequest(await controller.GetProfileData(new ProfileDataRequest
            {
                PersonIdentifiers = new List<Identifier> { new Identifier { PidType = "ORCID", PidContent = " " } },
                ControllerROR = "ror"
            }), "PidContent is required for each person identifier.");
            AssertBadRequest(await controller.GetProfileData(new ProfileDataRequest
            {
                PersonIdentifiers = new List<Identifier> { new Identifier { PidType = "ORCID", PidContent = "x" } },
                ControllerROR = " "
            }), "ControllerROR is required.");
            AssertBadRequest(await controller.GetProfileData(new ProfileDataRequest
            {
                PersonIdentifiers = new List<Identifier> { null },
                ControllerROR = "ror"
            }), "Each person identifier must be provided.");
            AssertBadRequest(await controller.GetProfileData(new ProfileDataRequest
            {
                PersonIdentifiers = new List<Identifier> { new Identifier { PidType = "OTHER", PidContent = "x" } },
                ControllerROR = "ror"
            }), "PidType must be either ORCID or HETU.");
            AssertBadRequest(await controller.GetProfileData(new ProfileDataRequest
            {
                PersonIdentifiers = Enumerable.Range(0, 3).Select(_ => new Identifier { PidType = "HETU", PidContent = "x" }).ToList(),
                ControllerROR = "ror"
            }), "No more than two person identifiers may be provided.");
            serviceMock.Verify(s => s.GetProfileDataForPublicApi(It.IsAny<ProfileDataRequest>()), Times.Never);
        }

        [Fact(DisplayName = "GetProfileForPublicApi - returns unauthorized when configured token is missing")]
        public async Task GetProfileForPublicApi_05()
        {
            PublicApiController controller = CreateController(configuredToken: null, headerToken: "secret", out var serviceMock);

            Assert.IsType<UnauthorizedResult>(await controller.GetProfileData(CreateRequest("0000-0000-0000-0001")));
            serviceMock.Verify(s => s.GetProfileDataForPublicApi(It.IsAny<ProfileDataRequest>()), Times.Never);
        }

        [Theory]
        [InlineData("orcid", "0000-0000-0000-0001", null, "0000-0000-0000-0001")]
        [InlineData("ORCID", "0000-0000-0000-0001", "0000-0000-0000-0002", "0000-0000-0000-0002")]
        [InlineData("HETU", "sensitive-hetu", "0000-0000-0000-0002", "0000-0000-0000-0002")]
        [InlineData("HETU", "sensitive-hetu", null, "")]
        public async Task GetProfileData_LogsCompletionWithOrcidAndClientId(string pidType, string pidContent, string responseOrcid, string expectedOrcid)
        {
            Mock<ILogger<PublicApiController>> logger = new Mock<ILogger<PublicApiController>>();
            PublicApiController controller = CreateController("secret", "secret", out var serviceMock, logger.Object);
            controller.HttpContext.Request.Headers["public-api-clientid"] = "test-client";
            ProfileDataRequest request = CreateRequest(pidContent);
            request.PersonIdentifiers[0].PidType = pidType;
            serviceMock.Setup(s => s.GetProfileDataForPublicApi(request)).ReturnsAsync(new ProfileDataResponse
            {
                ProfileData = responseOrcid == null ? null : new PersonProfileData
                {
                    PersonIdentifier = new Identifier { PidType = "ORCID", PidContent = responseOrcid }
                }
            });

            Assert.IsType<OkObjectResult>(await controller.GetProfileData(request));

            var completion = Assert.Single(logger.Invocations
                .Where(invocation => invocation.Method.Name == nameof(ILogger.Log))
                .Select(invocation => (IReadOnlyList<KeyValuePair<string, object>>)invocation.Arguments[2]),
                state => state.Any(item => item.Value is LogApiInfo info && info.State == LogContent.ActionState.COMPLETE));
            LogUserIdentification user = Assert.IsType<LogUserIdentification>(completion.Single(item => item.Key == "@LogUserIdentification").Value);
            LogApiInfo apiInfo = Assert.IsType<LogApiInfo>(completion.Single(item => item.Key == "@LogApiInfo").Value);
            Assert.Equal(expectedOrcid, user.Orcid);
            Assert.Equal(LogContent.Action.PUBLIC_API_PROFILE_REQUEST, apiInfo.Action);
            Assert.Contains("test-client", apiInfo.Message);
            Assert.DoesNotContain("sensitive-hetu", apiInfo.Message);
        }

        [Fact]
        public async Task GetProfileData_DoesNotLogCompletionWhenServiceFails()
        {
            Mock<ILogger<PublicApiController>> logger = new Mock<ILogger<PublicApiController>>();
            PublicApiController controller = CreateController("secret", "secret", out var serviceMock, logger.Object);
            serviceMock.Setup(s => s.GetProfileDataForPublicApi(It.IsAny<ProfileDataRequest>()))
                .ThrowsAsync(new InvalidOperationException("Service failed"));

            await Assert.ThrowsAsync<InvalidOperationException>(() => controller.GetProfileData(CreateRequest("0000-0000-0000-0001")));

            Assert.DoesNotContain(logger.Invocations
                .Where(invocation => invocation.Method.Name == nameof(ILogger.Log))
                .Select(invocation => (IReadOnlyList<KeyValuePair<string, object>>)invocation.Arguments[2]),
                state => state.Any(item => item.Value is LogApiInfo info && info.State == LogContent.ActionState.COMPLETE));
        }
    }
}
