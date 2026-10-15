using System.Collections.Generic;
using System.Threading.Tasks;
using api.Controllers;
using ResearchFi.PersonPublicApi;
using api.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using Xunit;

namespace api.Tests
{
    [Collection("Public API controller tests")]
    public class PublicApiControllerTests
    {
        private static PublicApiController CreateController(string configuredToken, string headerToken)
        {
            Dictionary<string, string> inMemorySettings = new Dictionary<string, string> { { "PUBLICAPITOKEN", configuredToken } };
            IConfiguration configuration = new ConfigurationBuilder().AddInMemoryCollection(inMemorySettings).Build();

            Mock<IPublicApiService> publicApiServiceMock = new Mock<IPublicApiService>();
            publicApiServiceMock.Setup(s => s.GetProfileDataForPublicApi(It.IsAny<ProfileDataRequest>()))
                .ReturnsAsync((ProfileDataRequest request) => new ProfileDataResponse
                {
                    ProfileData = new PersonProfileData { IsNamedBy = new PersonName { FirstName = "Test", LastName = request.PersonIdentifier[0].PidContent } }
                });

            PublicApiController controller = new PublicApiController(NullLogger<PublicApiController>.Instance, publicApiServiceMock.Object, configuration)
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
                PersonIdentifier = new List<Identifier> { new Identifier { PidType = "ORCID", PidContent = pidContent } },
                FunderROR = "https://ror.org/funder"
            };
        }

        [Fact(DisplayName = "GetProfileForPublicApi - returns 401 when token header is missing")]
        public async Task GetProfileForPublicApi_01()
        {
            PublicApiController controller = CreateController(configuredToken: "secret", headerToken: null);

            IActionResult result = await controller.GetProfileData(CreateRequest("Alice"));

            Assert.IsType<UnauthorizedResult>(result);
        }

        [Fact(DisplayName = "GetProfileForPublicApi - returns 401 when token header does not match")]
        public async Task GetProfileForPublicApi_02()
        {
            PublicApiController controller = CreateController(configuredToken: "secret", headerToken: "wrong");

            IActionResult result = await controller.GetProfileData(CreateRequest("Alice"));

            Assert.IsType<UnauthorizedResult>(result);
        }

        [Fact(DisplayName = "GetProfileForPublicApi - returns greeting when token header matches")]
        public async Task GetProfileForPublicApi_03()
        {
            PublicApiController controller = CreateController(configuredToken: "secret", headerToken: "secret");

            IActionResult result = await controller.GetProfileData(CreateRequest("Alice"));

            OkObjectResult okResult = Assert.IsType<OkObjectResult>(result);
            ProfileDataResponse response = Assert.IsType<ProfileDataResponse>(okResult.Value);
            Assert.Equal("Alice", response.ProfileData.IsNamedBy.LastName);
        }

        [Fact(DisplayName = "GetProfileForPublicApi - returns 400 when request does not match query schema")]
        public async Task GetProfileForPublicApi_04()
        {
            PublicApiController controller = CreateController(configuredToken: "secret", headerToken: "secret");

            Assert.IsType<BadRequestResult>(await controller.GetProfileData(null));
            Assert.IsType<BadRequestResult>(await controller.GetProfileData(new ProfileDataRequest { FunderROR = "x" }));
            Assert.IsType<BadRequestResult>(await controller.GetProfileData(new ProfileDataRequest { PersonIdentifier = new List<Identifier> { new Identifier() } }));
        }
    }
}
