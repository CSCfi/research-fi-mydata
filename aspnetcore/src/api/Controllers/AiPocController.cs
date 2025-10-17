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

namespace api.Controllers
{
    /*
     * AiPocController implements experimental MVC functionality for testin AI feature
     */
    public class AiPocController : Controller
    {
        private readonly ChatClient _chatClient;
        private readonly AiPocService _aiPocService;
        private readonly ILogger<AiPocController> _logger;

        public AiPocController(ChatClient chatClient, AiPocService aiPocService, ILogger<AiPocController> logger)
        {
            _chatClient = chatClient;
            _aiPocService = aiPocService;
            _logger = logger;
        }

        /// <summary>
        /// Display the name input form
        /// </summary>
        public IActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// Get prompt data
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> GetProfileDataForPrompt(string orcid)
        {
            LogUserIdentification logUserIdentification = new LogUserIdentification(
                keycloakId: "",
                orcid: "",
                ip: HttpContext.Connection.RemoteIpAddress?.ToString()
            );
            _logger.LogInformation(
                LogContent.MESSAGE_TEMPLATE,
                logUserIdentification,
                new LogApiInfo(
                    action: LogContent.Action.AI_GET_PROFILE_DATA,
                    state: LogContent.ActionState.START));

            AittaModel? profileDataForPromt = await _aiPocService.GetProfileDataForPromt(orcid);
            return Content(
                JsonSerializer.Serialize(
                    profileDataForPromt,
                    new JsonSerializerOptions
                    {
                        DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull,
                        Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping
                    }
                )
            );
        }

        /// <summary>
        /// Query AI model
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> QueryAiModel(string systemPrompt, string profileData, int maxOutputTokenCount)
        {
            LogUserIdentification logUserIdentification = new LogUserIdentification(
                keycloakId: "",
                orcid: "",
                ip: HttpContext.Connection.RemoteIpAddress?.ToString()
            );
            _logger.LogInformation(
                LogContent.MESSAGE_TEMPLATE,
                logUserIdentification,
                new LogApiInfo(
                    action: LogContent.Action.AI_QUERY_MODEL,
                    state: LogContent.ActionState.START));

            if (string.IsNullOrWhiteSpace(systemPrompt) && string.IsNullOrWhiteSpace(profileData))
            {
                return BadRequest(new { error = "Prompt cannot be empty." });
            }

            // Append to system prompt
            systemPrompt += "\nRespond in markdown format.";

            try
            {
                ChatCompletionOptions options = new()
                {
                    MaxOutputTokenCount = maxOutputTokenCount > 0 ? maxOutputTokenCount : 200,
                    // Temperature = 0.5f,
                    // TopP = 0.9f
                };

                var chatMessages = new List<ChatMessage>
                {
                    new SystemChatMessage(systemPrompt),
                    new UserChatMessage(profileData)
                };

                ChatCompletion completion = await _chatClient.CompleteChatAsync(chatMessages, options);
                return Content(completion.Content[0].Text);
            }
            catch (ArgumentException ex)
            {
                // Handle invalid arguments passed to the ChatClient
                return BadRequest(new { error = "Invalid prompt provided.", details = ex.Message });
            }
            catch (HttpRequestException ex)
            {
                // Handle network-related issues
                return StatusCode(503, new { error = "Service unavailable. Please try again later.", details = ex.Message });
            }
            catch (Exception ex)
            {
                // Handle any other unexpected errors
                return StatusCode(500, new { error = "An unexpected error occurred.", details = ex.Message });
            }
        }
    }
}