using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using api.Services;
using Microsoft.AspNetCore.Mvc;
using OpenAI.Chat;
using api.Models.Ai;
using System.Text.Json;
using System.Threading;
using Microsoft.Extensions.Logging;
using api.Models.Log;
using System.Diagnostics;
using System.Reflection.Metadata.Ecma335;

namespace api.Controllers
{
    /*
     * AiController implements experimental MVC functionality for testin AI feature
     */
    public class AiController : TtvControllerBase
    {
        private readonly ChatClient _chatClient;
        private readonly AiService _aiService;
        private readonly ILogger<AiController> _logger;

        public AiController(ChatClient chatClient, AiService aiService, ILogger<AiController> logger)
        {
            _chatClient = chatClient;
            _aiService = aiService;
            _logger = logger;
        }

        /// <summary>
        /// Query AI model
        /// </summary>
        [HttpGet]
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
                        action: LogContent.Action.AI_GET_PROFILE_DATA,
                        state: LogContent.ActionState.START));

                var profileDataStopwatch = Stopwatch.StartNew();
                profileDataForPromt = await _aiService.GetProfileDataForPromt(orcidId);
                profileDataStopwatch.Stop();

                _logger.LogInformation(
                    LogContent.MESSAGE_TEMPLATE,
                    logUserIdentification,
                    new LogApiInfo(
                        action: LogContent.Action.AI_GET_PROFILE_DATA,
                        state: LogContent.ActionState.COMPLETE,
                        message: $"took {profileDataStopwatch.ElapsedMilliseconds}ms"));
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    LogContent.MESSAGE_TEMPLATE,
                    logUserIdentification,
                    new LogApiInfo(
                        action: LogContent.Action.AI_GET_PROFILE_DATA,
                        state: LogContent.ActionState.FAILED,
                        message: ex.Message));
                return StatusCode(500, new { error = "An unexpected error occurred while retrieving profile data." });
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
                        action: LogContent.Action.AI_QUERY_MODEL,
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
                        action: LogContent.Action.AI_QUERY_MODEL,
                        state: LogContent.ActionState.COMPLETE,
                        message: $"took {chatCompletionStopwatch.ElapsedMilliseconds}ms"));

                return Ok(new BiographyFromAi { Biography = completion.Content[0].Text });
            }
            catch (Exception ex)
            {
                // Handle any other unexpected errors
                return StatusCode(500, new { error = "An unexpected error occurred.", details = ex.Message });
            }
        }
    }
}