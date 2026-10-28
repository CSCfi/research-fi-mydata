using System.Collections.Generic;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using api.Controllers;
using api.Models.Api;
using api.Models.Common;
using api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace api.Tests
{
    public class ConsentControllerTests
    {
        private static ConsentController CreateController(
            Mock<IConsentService> consentService,
            Mock<IUserProfileService> userProfileService)
        {
            var controller = new ConsentController(consentService.Object, userProfileService.Object)
            {
                ControllerContext = new ControllerContext
                {
                    HttpContext = new DefaultHttpContext
                    {
                        User = new ClaimsPrincipal(new ClaimsIdentity(new[]
                        {
                            new Claim("orcid", "0000-0000-0000-0001")
                        }, "test"))
                    }
                }
            };
            return controller;
        }

        [Fact]
        public async Task GetConsents_ResolvesProfileFromOrcidClaim()
        {
            var consentService = new Mock<IConsentService>();
            var userProfileService = new Mock<IUserProfileService>();
            userProfileService
                .Setup(service => service.GetUserprofileIdForOrcidId("0000-0000-0000-0001"))
                .ReturnsAsync((true, 42));
            consentService
                .Setup(service => service.GetConsentsAsync(42, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new List<ConsentResponse>());
            var controller = CreateController(consentService, userProfileService);

            var result = Assert.IsType<OkObjectResult>(await controller.GetConsents(CancellationToken.None));

            Assert.IsType<ApiResponseConsentGet>(result.Value);
            userProfileService.Verify(service => service.GetUserprofileIdForOrcidId("0000-0000-0000-0001"), Times.Once);
            consentService.Verify(service => service.GetConsentsAsync(42, It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task GrantConsent_ReturnsFailureForUnknownTarget()
        {
            var consentService = new Mock<IConsentService>();
            var userProfileService = new Mock<IUserProfileService>();
            userProfileService
                .Setup(service => service.GetUserprofileIdForOrcidId("0000-0000-0000-0001"))
                .ReturnsAsync((true, 42));
            consentService
                .Setup(service => service.GrantConsentAsync(42, 999, It.IsAny<CancellationToken>()))
                .ReturnsAsync(ConsentGrantResult.TargetNotFound);
            var controller = CreateController(consentService, userProfileService);

            var result = Assert.IsType<OkObjectResult>(await controller.GrantConsent(
                new CreateConsentRequest { ConsentTargetId = 999 },
                CancellationToken.None));

            var response = Assert.IsType<ApiResponse>(result.Value);
            Assert.False(response.Success);
            Assert.Equal(Constants.ApiResponseReasons.CONSENT_TARGET_NOT_FOUND, response.Reason);
            consentService.Verify(service => service.GrantConsentAsync(42, 999, It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task WithdrawConsent_ReturnsNotFoundForConsentNotOwnedByCaller()
        {
            var consentService = new Mock<IConsentService>();
            var userProfileService = new Mock<IUserProfileService>();
            userProfileService
                .Setup(service => service.GetUserprofileIdForOrcidId("0000-0000-0000-0001"))
                .ReturnsAsync((true, 42));
            consentService
                .Setup(service => service.WithdrawConsentAsync(42, 456, It.IsAny<CancellationToken>()))
                .ReturnsAsync(false);
            var controller = CreateController(consentService, userProfileService);

            var result = Assert.IsType<OkObjectResult>(await controller.WithdrawConsent(456, CancellationToken.None));

            var response = Assert.IsType<ApiResponse>(result.Value);
            Assert.False(response.Success);
            Assert.Equal(Constants.ApiResponseReasons.CONSENT_NOT_FOUND, response.Reason);
            consentService.Verify(service => service.WithdrawConsentAsync(42, 456, It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task GetConsents_ReturnsProfileNotFoundWithoutCallingConsentService()
        {
            var consentService = new Mock<IConsentService>();
            var userProfileService = new Mock<IUserProfileService>();
            userProfileService
                .Setup(service => service.GetUserprofileIdForOrcidId("0000-0000-0000-0001"))
                .ReturnsAsync((false, 0));
            var controller = CreateController(consentService, userProfileService);

            var result = Assert.IsType<OkObjectResult>(await controller.GetConsents(CancellationToken.None));

            var response = Assert.IsType<ApiResponse>(result.Value);
            Assert.False(response.Success);
            Assert.Equal(Constants.ApiResponseReasons.PROFILE_NOT_FOUND, response.Reason);
            consentService.Verify(service => service.GetConsentsAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public void ControllerRequiresExistingProfileEditorAuthorizationPolicy()
        {
            var attribute = Assert.Single(typeof(ConsentController).GetCustomAttributes(typeof(AuthorizeAttribute), inherit: true));

            Assert.Equal("RequireScopeApi1AndClaimOrcid", ((AuthorizeAttribute)attribute).Policy);
        }
    }
}
