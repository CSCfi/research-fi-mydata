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
        private readonly IBiographyService _biographyService;
        private readonly ILogger<BiographyController> _logger;
        private readonly IUserProfileService _userProfileService;

        public BiographyController(ChatClient chatClient, IBiographyService biographyService, ILogger<BiographyController> logger, IUserProfileService userProfileService)
        {
            _chatClient = chatClient;
            _biographyService = biographyService;
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
                profileDataForPromt = await _biographyService.GetProfileDataForPromt(orcidId);
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
                @"Act as an expert in RDI and create a max 300 word profile description of your activities and interests.
                    Write in an approachable and understandable manner in a semi formal language. Write in the first person and in present tense. Do not include listings or headers, just prose.
                    Description should include:
                    Information on whether you are a researcher or some other expert, based on your current affiliations.
                    Highlight your areas of expertise, based on other descriptions and keywords provided and recent publications, datasets and granted funding.
                    Do not list past publications and datasets.
                    What you are passionate about based on other descriptions provided.
                    What kind of activities you have been performing.
                    Information about awards if you have received any. Do not mention about awards if I have not received any.
                    An explanation of how your research is situated within the broader field of study, the academic community or society at large  
                    Do not imagine any information about career that is not provided. Do not mention things that you have not done or taken part in. Respond in plain text format.
                    Create the description based on following information as instructed:";

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
                    Temperature = 1.0f,
                    TopP = 0.5f
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
        /// Returns always a Biography object. Object properties are empty, if values are not found in the database..
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
            try
            {
                _logger.LogInformation(
                    LogContent.MESSAGE_TEMPLATE,
                    logUserIdentification,
                    new LogApiInfo(
                        action: LogContent.Action.PROFILE_BIOGRAPHY_GET,
                        state: LogContent.ActionState.START));

                var getBiographyStopwatch = Stopwatch.StartNew();
                Biography biography = await _biographyService.GetBiography(userprofileId);
                getBiographyStopwatch.Stop();

                _logger.LogInformation(
                    LogContent.MESSAGE_TEMPLATE,
                    logUserIdentification,
                    new LogApiInfo(
                        action: LogContent.Action.PROFILE_BIOGRAPHY_GET,
                        state: LogContent.ActionState.COMPLETE,
                        message: $"took {getBiographyStopwatch.ElapsedMilliseconds}ms"));

                return Ok(biography);
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
        }

        /// <summary>
        /// Create or update biography.
        /// </summary>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
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
                bool success = await _biographyService.CreateOrUpdateBiography(userprofileId, biography);
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

            return NoContent();
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
                bool success = await _biographyService.DeleteBiography(userprofileId);
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