using System.Collections.Generic;
using api.Controllers;
using api.PublicApiContracts;
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
            publicApiServiceMock.Setup(s => s.GetHelloMessage(It.IsAny<string>()))
                .Returns((string username) => new PublicApiHelloResponse { Message = $"Hello {username}" });

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

        [Fact(DisplayName = "GetDataForPublicApi - returns 401 when token header is missing")]
        public void GetDataForPublicApi_01()
        {
            PublicApiController controller = CreateController(configuredToken: "secret", headerToken: null);

            IActionResult result = controller.GetDataForPublicApi(new PublicApiHelloRequest { Username = "matti" });

            Assert.IsType<UnauthorizedResult>(result);
        }

        [Fact(DisplayName = "GetDataForPublicApi - returns 401 when token header does not match")]
        public void GetDataForPublicApi_02()
        {
            PublicApiController controller = CreateController(configuredToken: "secret", headerToken: "wrong");

            IActionResult result = controller.GetDataForPublicApi(new PublicApiHelloRequest { Username = "matti" });

            Assert.IsType<UnauthorizedResult>(result);
        }

        [Fact(DisplayName = "GetDataForPublicApi - returns greeting when token header matches")]
        public void GetDataForPublicApi_03()
        {
            PublicApiController controller = CreateController(configuredToken: "secret", headerToken: "secret");

            IActionResult result = controller.GetDataForPublicApi(new PublicApiHelloRequest { Username = "matti" });

            OkObjectResult okResult = Assert.IsType<OkObjectResult>(result);
            PublicApiHelloResponse response = Assert.IsType<PublicApiHelloResponse>(okResult.Value);
            Assert.Equal("Hello matti", response.Message);
        }
    }
}
