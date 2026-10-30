using System.Collections.Generic;
using System.Threading.Tasks;
using api.Controllers;
using api.Models.Api;
using api.Models.Common;
using api.Models.Log;
using api.Models.Ttv;
using api.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using ResearchFi.ProfilePublicApi;
using Xunit;

namespace api.Tests
{
    public class DebugControllerTests
    {
        [Fact]
        public void ProfileDataForApi_UsesGetRouteWithOrcidId()
        {
            var method = typeof(DebugController).GetMethod(nameof(DebugController.GetProfileDataForPublicApiFromOrcidId));

            Assert.NotNull(method);
            Assert.NotNull(System.Attribute.GetCustomAttribute(method, typeof(HttpGetAttribute)));
            var route = (RouteAttribute)System.Attribute.GetCustomAttribute(method, typeof(RouteAttribute));
            Assert.Equal("/[controller]/profiledata-for-public-api/{orcidId}", route.Template);
        }

        private static DebugController CreateController(
            string headerToken,
            out Mock<IUserProfileService> userProfileService,
            out Mock<IPublicApiService> publicApiService)
        {
            IConfiguration configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(new Dictionary<string, string> { ["ADMINTOKEN"] = "secret" })
                .Build();
            userProfileService = new Mock<IUserProfileService>(MockBehavior.Strict);
            publicApiService = new Mock<IPublicApiService>(MockBehavior.Strict);
            var controller = new DebugController(
                configuration, null, userProfileService.Object,
                NullLogger<UserProfileController>.Instance, null, null, null, publicApiService.Object)
            {
                ControllerContext = new ControllerContext { HttpContext = new DefaultHttpContext() }
            };
            if (headerToken != null)
            {
                controller.Request.Headers["admintoken"] = headerToken;
            }
            return controller;
        }

        [Theory]
        [InlineData(null)]
        [InlineData("wrong")]
        public async Task ProfileDataForApi_RequiresAdminToken(string token)
        {
            var controller = CreateController(token, out var userProfileService, out var publicApiService);

            var result = await controller.GetProfileDataForPublicApiFromOrcidId("0000-0000-0000-0001");

            Assert.IsType<UnauthorizedResult>(result);
            userProfileService.VerifyNoOtherCalls();
            publicApiService.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ProfileDataForApi_RejectsInvalidModel()
        {
            var controller = CreateController("secret", out var userProfileService, out var publicApiService);
            controller.ModelState.AddModelError("orcidId", "Invalid value.");

            var result = await controller.GetProfileDataForPublicApiFromOrcidId("");

            Assert.IsType<BadRequestObjectResult>(result);
            userProfileService.VerifyNoOtherCalls();
            publicApiService.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ProfileDataForApi_ReturnsPersonProfileDataForHiddenProfile()
        {
            var controller = CreateController("secret", out var userProfileService, out var publicApiService);
            var expectedData = new PersonProfileData { IsNamedBy = new PersonName { FirstName = "Matti" } };
            userProfileService.Setup(s => s.GetUserprofile("0000-0000-0000-0001"))
                .ReturnsAsync(new DimUserProfile { Id = 1, OrcidId = "0000-0000-0000-0001", Hidden = true });
            publicApiService.Setup(s => s.GetPersonProfileData(1, "0000-0000-0000-0001")).ReturnsAsync(expectedData);

            var result = await controller.GetProfileDataForPublicApiFromOrcidId("0000-0000-0000-0001");

            Assert.Same(expectedData, Assert.IsType<OkObjectResult>(result).Value);
            userProfileService.Verify(s => s.GetUserprofile("0000-0000-0000-0001"), Times.Once);
            publicApiService.Verify(s => s.GetPersonProfileData(1, "0000-0000-0000-0001"), Times.Once);
            userProfileService.Verify(s => s.GetProfileData(It.IsAny<int>(), It.IsAny<LogUserIdentification>(), It.IsAny<bool>()), Times.Never);
            publicApiService.Verify(s => s.GetProfileDataForPublicApi(It.IsAny<ProfileDataRequest>()), Times.Never);
            publicApiService.Verify(s => s.GetUsernameFromNationalIdentificationNumber(It.IsAny<string>()), Times.Never);
        }

        [Fact]
        public async Task ProfileDataForApi_PreservesMissingProfileResponse()
        {
            var controller = CreateController("secret", out var userProfileService, out var publicApiService);
            userProfileService.Setup(s => s.GetUserprofile("0000-0000-0000-0001")).ReturnsAsync((DimUserProfile)null);

            var result = await controller.GetProfileDataForPublicApiFromOrcidId("0000-0000-0000-0001");

            var response = Assert.IsType<ApiResponse>(Assert.IsType<OkObjectResult>(result).Value);
            Assert.False(response.Success);
            Assert.Equal(Constants.ApiResponseReasons.PROFILE_NOT_FOUND, response.Reason);
            publicApiService.Verify(s => s.GetPersonProfileData(It.IsAny<int>(), It.IsAny<string>()), Times.Never);
            publicApiService.Verify(s => s.GetUsernameFromNationalIdentificationNumber(It.IsAny<string>()), Times.Never);
        }
    }
}
