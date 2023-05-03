using api.Services;
using api.Models.Api;
using api.Models.Common;
using api.Models.Ttv;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Collections.Generic;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.AspNetCore.Http;
using api.Models.ProfileEditor;
using System;
using Microsoft.Extensions.Logging;
using api.Models.Log;

namespace api.Controllers
{
    /*
     * ProfileSettingsController implements profile editor settings related APIs.
     */
    [Route("api/settings")]
    [ApiController]
    [Authorize(Policy = "RequireScopeApi1AndClaimOrcid")]
    public class ProfileSettingsController : TtvControllerBase
    {
        private readonly IUserProfileService _userProfileService;
        private readonly IBackgroundTaskQueue _taskQueue;
        private readonly ILogger<UserProfileController> _logger;

        public ProfileSettingsController(IUserProfileService userProfileService, ILogger<UserProfileController> logger, IBackgroundTaskQueue taskQueue)
        {
            _userProfileService = userProfileService;
            _taskQueue = taskQueue;
            _logger = logger;
        }

        /// <summary>
        /// Set profile state to "hidden".
        /// Profile is removed from Elasticsearch index.
        /// </summary>
        [HttpGet]
        [Route("hideprofile")]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
        public async Task<IActionResult> HideProfileFromPortal()
        {
            // Get ORCID id
            string orcidId = GetOrcidId();
            LogUserIdentification logUserIdentification = this.GetLogUserIdentification();

            _logger.LogInformation(
                LogContent.MESSAGE_TEMPLATE,
                logUserIdentification,
                new LogApiInfo(
                    action: LogContent.Action.PROFILE_HIDE,
                    state: LogContent.ActionState.START));

            // Set profile state to "hidden"
            await _userProfileService.HideProfile(orcidId: orcidId, logUserIdentification: logUserIdentification);

            return Ok(new ApiResponse());
        }

        /// <summary>
        /// Reveal profile from state "hidden".
        /// Profile is updated in Elasticsearch index.
        /// </summary>
        [HttpGet]
        [Route("revealprofile")]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
        public async Task<IActionResult> RevealProfileInPortal()
        {
            // Get ORCID id
            string orcidId = GetOrcidId();
            LogUserIdentification logUserIdentification = this.GetLogUserIdentification();

            _logger.LogInformation(
                LogContent.MESSAGE_TEMPLATE,
                logUserIdentification,
                new LogApiInfo(
                    action: LogContent.Action.PROFILE_REVEAL,
                    state: LogContent.ActionState.START));

            // Reveal profile from state "hidden"
            await _userProfileService.RevealProfile(orcidId: orcidId, logUserIdentification: logUserIdentification);

            return Ok(new ApiResponse());
        }
    }
}