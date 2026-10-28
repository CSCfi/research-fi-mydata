using System;
using System.Threading;
using System.Threading.Tasks;
using api.Models.Api;
using api.Models.Common;
using api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace api.Controllers
{
    [Route("api/consent")]
    [ApiController]
    [Authorize(Policy = "RequireScopeApi1AndClaimOrcid")]
    public sealed class ConsentController : TtvControllerBase
    {
        private readonly IConsentService _consentService;
        private readonly IUserProfileService _userProfileService;

        public ConsentController(IConsentService consentService, IUserProfileService userProfileService)
        {
            _consentService = consentService;
            _userProfileService = userProfileService;
        }

        /// <summary>
        /// Gets the available consent targets, optionally filtered by a bare ROR identifier.
        /// </summary>
        [HttpGet("targets")]
        [ProducesResponseType(typeof(ApiResponseConsentTargetsGet), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetTargets([FromQuery] string rorId, CancellationToken cancellationToken)
        {
            var targets = await _consentService.GetTargetsAsync(rorId, cancellationToken);
            return Ok(new ApiResponseConsentTargetsGet(success: true, reason: "", data: targets));
        }

        /// <summary>
        /// Gets the signed-in researcher's active consents.
        /// </summary>
        [HttpGet]
        [ProducesResponseType(typeof(ApiResponseConsentGet), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetConsents(CancellationToken cancellationToken)
        {
            (bool profileExists, int userProfileId) = await _userProfileService.GetUserprofileIdForOrcidId(GetOrcidId());
            if (!profileExists)
            {
                return Ok(new ApiResponse(success: false, reason: Constants.ApiResponseReasons.PROFILE_NOT_FOUND));
            }

            var consents = await _consentService.GetConsentsAsync(userProfileId, cancellationToken);
            return Ok(new ApiResponseConsentGet(success: true, reason: "", data: consents));
        }

        /// <summary>
        /// Grants consent for the controller organization associated with the selected target.
        /// </summary>
        [HttpPost]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
        public async Task<IActionResult> GrantConsent([FromBody] CreateConsentRequest request, CancellationToken cancellationToken)
        {
            if (!ModelState.IsValid || request == null || request.ConsentTargetId <= 0)
            {
                return Ok(new ApiResponse(success: false, reason: Constants.ApiResponseReasons.INVALID_REQUEST));
            }

            (bool profileExists, int userProfileId) = await _userProfileService.GetUserprofileIdForOrcidId(GetOrcidId());
            if (!profileExists)
            {
                return Ok(new ApiResponse(success: false, reason: Constants.ApiResponseReasons.PROFILE_NOT_FOUND));
            }

            ConsentGrantResult result = await _consentService.GrantConsentAsync(userProfileId, request.ConsentTargetId, cancellationToken);
            return result switch
            {
                ConsentGrantResult.Created => Ok(new ApiResponse(success: true)),
                ConsentGrantResult.TargetNotFound => Ok(new ApiResponse(success: false, reason: Constants.ApiResponseReasons.CONSENT_TARGET_NOT_FOUND)),
                ConsentGrantResult.ConsentAlreadyExists => Ok(new ApiResponse(success: false, reason: Constants.ApiResponseReasons.CONSENT_ALREADY_EXISTS)),
                _ => throw new InvalidOperationException($"Unexpected consent grant result: {result}.")
            };
        }

        /// <summary>
        /// Withdraws the signed-in researcher's consent using a consent target identifier.
        /// </summary>
        [HttpDelete("{consentTargetId:int}")]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
        public async Task<IActionResult> WithdrawConsent(int consentTargetId, CancellationToken cancellationToken)
        {
            if (consentTargetId <= 0)
            {
                return Ok(new ApiResponse(success: false, reason: Constants.ApiResponseReasons.INVALID_REQUEST));
            }

            (bool profileExists, int userProfileId) = await _userProfileService.GetUserprofileIdForOrcidId(GetOrcidId());
            if (!profileExists)
            {
                return Ok(new ApiResponse(success: false, reason: Constants.ApiResponseReasons.PROFILE_NOT_FOUND));
            }

            bool withdrawn = await _consentService.WithdrawConsentAsync(userProfileId, consentTargetId, cancellationToken);
            return withdrawn
                ? Ok(new ApiResponse(success: true))
                : Ok(new ApiResponse(success: false, reason: Constants.ApiResponseReasons.CONSENT_NOT_FOUND));
        }
    }
}
