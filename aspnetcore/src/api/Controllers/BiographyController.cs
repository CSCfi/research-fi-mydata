using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OpenAI.Chat;
using api.Models.Ai;
using Microsoft.Extensions.Logging;
using api.Models.Log;
using System.Diagnostics;
using Microsoft.AspNetCore.Http;
using api.Models.Api;
using api.Models.Common;
using api.Models.Ttv;
using Elasticsearch.Net;

namespace api.Controllers
{
    /*
     * BiographyController implements AI assisted biography generation API commands
     */
    [Route("api/biography")]
    [ApiController]
    [Authorize(Policy = "RequireScopeApi1AndClaimOrcid")]
    public class BiographyController : TtvControllerBase
    {
        private readonly ChatClient _chatClient;
        private readonly IAiService _aiService;
        private readonly ILogger<BiographyController> _logger;
        private readonly IUserProfileService _userProfileService;

        public BiographyController(ChatClient chatClient, IAiService aiService, ILogger<BiographyController> logger, IUserProfileService userProfileService)
        {
            _chatClient = chatClient;
            _aiService = aiService;
            _logger = logger;
            _userProfileService = userProfileService;
        }

        /// <summary>
        /// Generate biography.
        /// </summary>
        [HttpGet]
        [Route("generate")]
        [ProducesResponseType(typeof(BiographyGenerated), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> QueryAiModel()
        {
            LogUserIdentification logUserIdentification = this.GetLogUserIdentification();
            string orcidId = logUserIdentification.Orcid;

            // Get profile data for prompt
            string? profileDataForPromt = null;
            try
            {
                _logger.LogInformation(
                    LogContent.MESSAGE_TEMPLATE,
                    logUserIdentification,
                    new LogApiInfo(
                        action: LogContent.Action.PROFILE_BIOGRAPHY_GENERATE_GET_PROFILEDATA,
                        state: LogContent.ActionState.START));

                var profileDataStopwatch = Stopwatch.StartNew();
                profileDataForPromt = await _aiService.GetProfileDataForPromt(orcidId);
                profileDataStopwatch.Stop();

                _logger.LogInformation(
                    LogContent.MESSAGE_TEMPLATE,
                    logUserIdentification,
                    new LogApiInfo(
                        action: LogContent.Action.PROFILE_BIOGRAPHY_GENERATE_GET_PROFILEDATA,
                        state: LogContent.ActionState.COMPLETE,
                        message: $"took {profileDataStopwatch.ElapsedMilliseconds}ms"));
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    LogContent.MESSAGE_TEMPLATE,
                    logUserIdentification,
                    new LogApiInfo(
                        action: LogContent.Action.PROFILE_BIOGRAPHY_GENERATE_GET_PROFILEDATA,
                        state: LogContent.ActionState.FAILED,
                        message: ex.Message));
                return StatusCode(500);
            }

            string systemPrompt =
                @"Act as an expert in RDI and create a max 400 word profile description of your activities and interests.
                Write in an approachable and understandable manner in a semi formal language. Write in the first person and in present tense. Do not include listings or headers, just prose.
                Description should include: Information on whether you are a researcher or some other expert, based on your current affiliations.
                Highlight your areas of expertise, based on other descriptions and keywords provided and recent publications, datasets and granted funding.
                What you are passionate about based on other descriptions provided. What kind of activities you have been performing.
                Information about awards. Do not imagine any information about your career that is not provided.
                Respond in plain text format.
                Use the following profile data as your only source of information:";

            try
            {
                _logger.LogInformation(
                    LogContent.MESSAGE_TEMPLATE,
                    logUserIdentification,
                    new LogApiInfo(
                        action: LogContent.Action.PROFILE_BIOGRAPHY_GENERATE_QUERY_MODEL,
                        state: LogContent.ActionState.START));

                ChatCompletionOptions options = new()
                {
                    MaxOutputTokenCount = 500,
                    Temperature = 0.2f,
                    TopP = 1.0f
                };

                var chatMessages = new List<ChatMessage>
                {
                    new SystemChatMessage(systemPrompt),
                    new UserChatMessage(profileDataForPromt)
                };

                var chatCompletionStopwatch = Stopwatch.StartNew();
                ChatCompletion completion = await _chatClient.CompleteChatAsync(chatMessages, options);
                chatCompletionStopwatch.Stop();

                _logger.LogInformation(
                    LogContent.MESSAGE_TEMPLATE,
                    logUserIdentification,
                    new LogApiInfo(
                        action: LogContent.Action.PROFILE_BIOGRAPHY_GENERATE_QUERY_MODEL,
                        state: LogContent.ActionState.COMPLETE,
                        message: $"took {chatCompletionStopwatch.ElapsedMilliseconds}ms"));

                return Ok(new BiographyGenerated { ContentText = completion.Content[0].Text });
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    LogContent.MESSAGE_TEMPLATE,
                    logUserIdentification,
                    new LogApiInfo(
                        action: LogContent.Action.PROFILE_BIOGRAPHY_GENERATE_QUERY_MODEL,
                        state: LogContent.ActionState.COMPLETE,
                        message: ex.Message));
                return StatusCode(500);
            }
        }

        /// <summary>
        /// Get biography.
        /// </summary>
        [HttpGet]
        [ProducesResponseType(typeof(Biography), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetBiography()
        {
            LogUserIdentification logUserIdentification = this.GetLogUserIdentification();
            // Get ORCID id
            string orcidId = GetOrcidId();

            // Check that userprofile exists.
            (bool userprofileExists, int userprofileId) = await _userProfileService.GetUserprofileIdForOrcidId(orcidId);
            if (!userprofileExists)
            {
                return Ok(new ApiResponse(success: false, reason: Constants.ApiResponseReasons.PROFILE_NOT_FOUND));
            }

            // Get Biography
            Biography? biography = null;
            try
            {
                _logger.LogInformation(
                    LogContent.MESSAGE_TEMPLATE,
                    logUserIdentification,
                    new LogApiInfo(
                        action: LogContent.Action.PROFILE_BIOGRAPHY_GET,
                        state: LogContent.ActionState.START));

                var getBiographyStopwatch = Stopwatch.StartNew();
                biography = await _aiService.GetBiography(userprofileId);
                getBiographyStopwatch.Stop();

                _logger.LogInformation(
                    LogContent.MESSAGE_TEMPLATE,
                    logUserIdentification,
                    new LogApiInfo(
                        action: LogContent.Action.PROFILE_BIOGRAPHY_GET,
                        state: LogContent.ActionState.COMPLETE,
                        message: $"took {getBiographyStopwatch.ElapsedMilliseconds}ms"));
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    LogContent.MESSAGE_TEMPLATE,
                    logUserIdentification,
                    new LogApiInfo(
                        action: LogContent.Action.PROFILE_BIOGRAPHY_GET,
                        state: LogContent.ActionState.FAILED,
                        message: ex.Message));
                return StatusCode(500);
            }

            return Ok(biography);
        }

        /// <summary>
        /// Create or update biography.
        /// </summary>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> SetBiography([FromBody] Biography biography)
        {
            LogUserIdentification logUserIdentification = this.GetLogUserIdentification();
            // Get ORCID id
            string orcidId = GetOrcidId();

            // Check that userprofile exists.
            (bool userprofileExists, int userprofileId) = await _userProfileService.GetUserprofileIdForOrcidId(orcidId);
            if (!userprofileExists)
            {
                return Ok(new ApiResponse(success: false, reason: Constants.ApiResponseReasons.PROFILE_NOT_FOUND));
            }

            // Set Biography
            try
            {
                _logger.LogInformation(
                    LogContent.MESSAGE_TEMPLATE,
                    logUserIdentification,
                    new LogApiInfo(
                        action: LogContent.Action.PROFILE_BIOGRAPHY_SET,
                        state: LogContent.ActionState.START));

                var setBiographyStopwatch = Stopwatch.StartNew();
                bool success = await _aiService.CreateOrUpdateBiography(userprofileId, biography);
                setBiographyStopwatch.Stop();

                _logger.LogInformation(
                    LogContent.MESSAGE_TEMPLATE,
                    logUserIdentification,
                    new LogApiInfo(
                        action: LogContent.Action.PROFILE_BIOGRAPHY_SET,
                        state: LogContent.ActionState.COMPLETE,
                        message: $"took {setBiographyStopwatch.ElapsedMilliseconds}ms"));
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    LogContent.MESSAGE_TEMPLATE,
                    logUserIdentification,
                    new LogApiInfo(
                        action: LogContent.Action.PROFILE_BIOGRAPHY_SET,
                        state: LogContent.ActionState.FAILED,
                        message: ex.Message));
                return StatusCode(500);
            }

            return Created();
        }

        /// <summary>
        /// Delete Biography.
        /// </summary>
        [HttpDelete]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> DeleteBiography()
        {
            LogUserIdentification logUserIdentification = this.GetLogUserIdentification();
            // Get ORCID id
            string orcidId = GetOrcidId();

            // Check that userprofile exists.
            (bool userprofileExists, int userprofileId) = await _userProfileService.GetUserprofileIdForOrcidId(orcidId);
            if (!userprofileExists)
            {
                return Ok(new ApiResponse(success: false, reason: Constants.ApiResponseReasons.PROFILE_NOT_FOUND));
            }

            // Delete Biography by setting empty strings
            try
            {
                _logger.LogInformation(
                    LogContent.MESSAGE_TEMPLATE,
                    logUserIdentification,
                    new LogApiInfo(
                        action: LogContent.Action.PROFILE_BIOGRAPHY_DELETE,
                        state: LogContent.ActionState.START));

                var deleteBiographyStopwatch = Stopwatch.StartNew();
                bool success = await _aiService.DeleteBiography(userprofileId);
                deleteBiographyStopwatch.Stop();

                _logger.LogInformation(
                    LogContent.MESSAGE_TEMPLATE,
                    logUserIdentification,
                    new LogApiInfo(
                        action: LogContent.Action.PROFILE_BIOGRAPHY_DELETE,
                        state: LogContent.ActionState.COMPLETE,
                        message: $"took {deleteBiographyStopwatch.ElapsedMilliseconds}ms"));
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    LogContent.MESSAGE_TEMPLATE,
                    logUserIdentification,
                    new LogApiInfo(
                        action: LogContent.Action.PROFILE_BIOGRAPHY_DELETE,
                        state: LogContent.ActionState.FAILED,
                        message: ex.Message));
                return StatusCode(500);
            }

            return NoContent();
        }
    }
}