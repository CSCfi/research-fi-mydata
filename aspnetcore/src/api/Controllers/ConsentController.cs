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
    /// <summary>
    /// Grants activate consent; GetConsents returns only active consent. Withdrawal deactivates consent
    /// and records the UTC withdrawal date. Later grants create new rows, preserving inactive history.
    /// </summary>
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
        /// Gets consent targets with a ROR identifier and the signed-in researcher's active consent,
        /// optionally filtered by a bare ROR identifier.
        /// </summary>
        [HttpGet("targets")]
        [ProducesResponseType(typeof(ApiResponseConsentTargetsGet), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetTargets([FromQuery] string rorId, CancellationToken cancellationToken)
        {
            (bool profileExists, int userProfileId) = await _userProfileService.GetUserprofileIdForOrcidId(GetOrcidId());
            var targets = await _consentService.GetTargetsAsync(rorId, profileExists ? userProfileId : null, cancellationToken);
            return Ok(new ApiResponseConsentTargetsGet(success: true, reason: "", data: targets));
        }

        /// <summary>
        /// Gets each ROR-identified target covered by the signed-in researcher's active consents.
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
            if (!ModelState.IsValid || request == null || request.OrgNodeId <= 0)
            {
                return Ok(new ApiResponse(success: false, reason: Constants.ApiResponseReasons.INVALID_REQUEST));
            }

            (bool profileExists, int userProfileId) = await _userProfileService.GetUserprofileIdForOrcidId(GetOrcidId());
            if (!profileExists)
            {
                return Ok(new ApiResponse(success: false, reason: Constants.ApiResponseReasons.PROFILE_NOT_FOUND));
            }

            ConsentGrantResult result = await _consentService.GrantConsentAsync(userProfileId, request.OrgNodeId, cancellationToken);
            return result switch
            {
                ConsentGrantResult.Created => Ok(new ApiResponse(success: true)),
                ConsentGrantResult.TargetNotFound => Ok(new ApiResponse(success: false, reason: Constants.ApiResponseReasons.CONSENT_TARGET_NOT_FOUND)),
                ConsentGrantResult.ConsentAlreadyExists => Ok(new ApiResponse(success: false, reason: Constants.ApiResponseReasons.CONSENT_ALREADY_EXISTS)),
                _ => throw new InvalidOperationException($"Unexpected consent grant result: {result}.")
            };
        }

        /// <summary>
        /// Withdraws the signed-in researcher's consent using an organization node identifier.
        /// </summary>
        [HttpDelete("{orgNodeId:int}")]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
        public async Task<IActionResult> WithdrawConsent(int orgNodeId, CancellationToken cancellationToken)
        {
            if (orgNodeId <= 0)
            {
                return Ok(new ApiResponse(success: false, reason: Constants.ApiResponseReasons.INVALID_REQUEST));
            }

            (bool profileExists, int userProfileId) = await _userProfileService.GetUserprofileIdForOrcidId(GetOrcidId());
            if (!profileExists)
            {
                return Ok(new ApiResponse(success: false, reason: Constants.ApiResponseReasons.PROFILE_NOT_FOUND));
            }

            bool withdrawn = await _consentService.WithdrawConsentAsync(userProfileId, orgNodeId, cancellationToken);
            return withdrawn
                ? Ok(new ApiResponse(success: true))
                : Ok(new ApiResponse(success: false, reason: Constants.ApiResponseReasons.CONSENT_NOT_FOUND));
        }
    }
}
