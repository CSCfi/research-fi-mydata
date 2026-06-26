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
using System.Net.Http;
using System.ClientModel;

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
        [Route("generate/{targetLanguage}")]
        [ProducesResponseType(typeof(BiographyContent), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status503ServiceUnavailable)]
        public async Task<IActionResult> QueryAiModel(string targetLanguage)
        {
            // Validate request data
            if (!string.IsNullOrEmpty(targetLanguage) && targetLanguage != "fi" && targetLanguage != "en" && targetLanguage != "sv")
            {
                ModelState.AddModelError(nameof(targetLanguage), "Target language must be one of: fi, en, sv");
                return BadRequest(ModelState);
            }

            LogUserIdentification logUserIdentification = this.GetLogUserIdentification();
            string orcidId = logUserIdentification.Orcid;

            _logger.LogInformation(
                LogContent.MESSAGE_TEMPLATE,
                logUserIdentification,
                new LogApiInfo(
                    action: LogContent.Action.PROFILE_BIOGRAPHY_GENERATE_GET_PROFILEDATA,
                    state: LogContent.ActionState.START,
                    message: $"Target language {targetLanguage.ToUpper()}"));

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

            string targetLanguageFull = targetLanguage switch
            {
                "fi" => "Finnish",
                "en" => "English",
                "sv" => "Swedish",
                _ => "unspecified language"
            };
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
                    Create the description in " + targetLanguageFull + @" language.
                    Create the description based on following information as instructed:";

            try
            {
                _logger.LogInformation(
                    LogContent.MESSAGE_TEMPLATE,
                    logUserIdentification,
                    new LogApiInfo(
                        action: LogContent.Action.PROFILE_BIOGRAPHY_GENERATE_QUERY_MODEL,
                        state: LogContent.ActionState.START,
                        message: $"Target language {targetLanguage.ToUpper()}"));

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

                if (completion?.Content == null || completion.Content.Count == 0 || string.IsNullOrWhiteSpace(completion.Content[0].Text))
                {
                    // Handle empty response from AI model
                    _logger.LogError(
                        LogContent.MESSAGE_TEMPLATE,
                        logUserIdentification,
                        new LogApiInfo(
                            action: LogContent.Action.PROFILE_BIOGRAPHY_GENERATE_QUERY_MODEL,
                            state: LogContent.ActionState.FAILED,
                            message: "AI returned empty completion"));
                    return StatusCode(StatusCodes.Status503ServiceUnavailable);
                }

                // Successfully got response from AI model.
                _logger.LogInformation(
                    LogContent.MESSAGE_TEMPLATE,
                    logUserIdentification,
                    new LogApiInfo(
                        action: LogContent.Action.PROFILE_BIOGRAPHY_GENERATE_QUERY_MODEL,
                        state: LogContent.ActionState.COMPLETE,
                        message: $"took {chatCompletionStopwatch.ElapsedMilliseconds}ms"));

                return Ok(new BiographyContent { ContentText = completion.Content[0].Text });
            }
            catch (ClientResultException ex)
            {
                _logger.LogError(
                    LogContent.MESSAGE_TEMPLATE,
                    logUserIdentification,
                    new LogApiInfo(
                        action: LogContent.Action.PROFILE_BIOGRAPHY_GENERATE_QUERY_MODEL,
                        state: LogContent.ActionState.FAILED,
                        message: ex.Message));
                return ex.Status switch
                {
                    400 or 422 => BadRequest("Invalid request"),
                    429 or 500 or 502 or 503 or 504 => StatusCode(StatusCodes.Status503ServiceUnavailable),
                    _ => StatusCode(StatusCodes.Status500InternalServerError)
                };
            }
            catch (OperationCanceledException) when (HttpContext.RequestAborted.IsCancellationRequested)
            {
                _logger.LogError(
                    LogContent.MESSAGE_TEMPLATE,
                    logUserIdentification,
                    new LogApiInfo(
                        action: LogContent.Action.PROFILE_BIOGRAPHY_GENERATE_QUERY_MODEL,
                        state: LogContent.ActionState.FAILED,
                        message: "AI request was canceled by the client"));
                return StatusCode(499);
            }
            catch (OperationCanceledException ex)
            {
                _logger.LogError(
                    LogContent.MESSAGE_TEMPLATE,
                    logUserIdentification,
                    new LogApiInfo(
                        action: LogContent.Action.PROFILE_BIOGRAPHY_GENERATE_QUERY_MODEL,
                        state: LogContent.ActionState.FAILED,
                        message: ex.Message));
                return StatusCode(StatusCodes.Status503ServiceUnavailable);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected AI failure");
                _logger.LogError(
                    LogContent.MESSAGE_TEMPLATE,
                    logUserIdentification,
                    new LogApiInfo(
                        action: LogContent.Action.PROFILE_BIOGRAPHY_GENERATE_QUERY_MODEL,
                        state: LogContent.ActionState.FAILED,
                        message: ex.Message));
                return StatusCode(StatusCodes.Status500InternalServerError);
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
            // Validate request data
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

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


        /// <summary>
        /// Translate biography
        /// </summary>
        [HttpPost]
        [Route("translate")]
        [ProducesResponseType(typeof(BiographyContent), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status503ServiceUnavailable)]
        public async Task<IActionResult> TranslateUsingAiModel([FromBody] TranslateRequest translateRequest)
        {
            // Validate request data
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            LogUserIdentification logUserIdentification = this.GetLogUserIdentification();
            _logger.LogInformation(
                LogContent.MESSAGE_TEMPLATE,
                logUserIdentification,
                new LogApiInfo(
                    action: LogContent.Action.AI_TRANSLATE_TEXT,
                    state: LogContent.ActionState.START));

            // Append to system prompt
            string systemChatMessage = $@"You are an expert, professional translator specializing in academic and biographical content. Your task includes first identifying the source language, which is guaranteed to be one of: **Finnish, Swedish, or English**.
                                    The translation must be performed with absolute fidelity to the original text. You must ensure that the meaning, tone (professional/academic), and factual content of the source text are preserved strictly and completely.
                                    **Output Rule: Your entire response must consist ONLY of the final translated text. Do not include section headings, labels, or any text other than the complete, high-fidelity translation itself.**
                                    **Target Language:** {translateRequest.TargetLanguage}]";

            string userChatMessage = $@"Please perform the translation task now on the following researcher biography:
                                        **Source Text:**
                                        {translateRequest.TextToTranslate}";
            try
            {
                ChatCompletionOptions options = new()
                {
                    MaxOutputTokenCount = 500,
                    Temperature = 0.0f,
                    TopP = 0.1f
                };

                var chatMessages = new List<ChatMessage>
                {
                    new SystemChatMessage(systemChatMessage),
                    new UserChatMessage(userChatMessage)
                };

                ChatCompletion completion = await _chatClient.CompleteChatAsync(chatMessages, options);

                _logger.LogInformation(
                    LogContent.MESSAGE_TEMPLATE,
                    logUserIdentification,
                    new LogApiInfo(
                        action: LogContent.Action.AI_TRANSLATE_TEXT,
                        state: LogContent.ActionState.COMPLETE));

                return Ok(new BiographyContent { ContentText = completion.Content[0].Text });
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    LogContent.MESSAGE_TEMPLATE,
                    logUserIdentification,
                    new LogApiInfo(
                        action: LogContent.Action.AI_TRANSLATE_TEXT,
                        state: LogContent.ActionState.FAILED,
                        message: ex.Message));
                return StatusCode(500);
            }
        }
    }
}