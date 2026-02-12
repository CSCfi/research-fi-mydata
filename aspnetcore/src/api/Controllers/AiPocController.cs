using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using api.Services;
using Microsoft.AspNetCore.Mvc;
using OpenAI.Chat;
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
        private readonly IBiographyService _biographyService;
        private readonly ILogger<AiPocController> _logger;

        public AiPocController(ChatClient chatClient, IBiographyService biographyService, ILogger<AiPocController> logger)
        {
            _chatClient = chatClient;
            _biographyService = biographyService;
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
        public async Task<IActionResult> GetProfileDataForPrompt(string orcidId)
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
                    state: LogContent.ActionState.START,
                    message: $"{orcidId}"));

            string? profileDataForPromt = await _biographyService.GetProfileDataForPromt(orcidId);

            return Content(profileDataForPromt);
        }

        /// <summary>
        /// Query AI model
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> QueryAiModel(string systemPrompt, string profileData, int maxOutputTokenCount, float temperature, float topP)
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
                    state: LogContent.ActionState.START,
                    message: $"max: {maxOutputTokenCount}, temperature: {temperature}, topP: {topP}"));

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
                    Temperature = temperature,
                    TopP = topP
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

        /// <summary>
        /// Translate text using AI model
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> TranslateUsingAiModel(string textToTranslate, string targetLanguage, int maxOutputTokenCount)
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
                    action: LogContent.Action.AI_TRANSLATE_TEXT,
                    state: LogContent.ActionState.START,
                    message: $"text to translate: {textToTranslate}"));

            if (string.IsNullOrWhiteSpace(textToTranslate))
            {
                return BadRequest(new { error = "Text to translate cannot be empty." });
            }

            // Append to system prompt
            string systemChatMessage = $@"You are an expert, professional translator specializing in academic and biographical content. Your task includes first identifying the source language, which is guaranteed to be one of: **Finnish, Swedish, or English**.
                                    The translation must be performed with absolute fidelity to the original text. You must ensure that the meaning, tone (professional/academic), and factual content of the source text are preserved strictly and completely.
                                    **Output Rule: Your entire response must consist ONLY of the final translated text. Do not include section headings, labels, or any text other than the complete, high-fidelity translation itself.**
                                    **Target Language:** {targetLanguage}]";

            string userChatMessage = $@"Please perform the translation task now on the following researcher biography:
                                        **Source Text:**
                                        {textToTranslate}";

            try
            {
                ChatCompletionOptions options = new()
                {
                    MaxOutputTokenCount = maxOutputTokenCount > 0 ? maxOutputTokenCount : 200,
                    Temperature = 0.0f,
                    TopP = 0.1f
                };

                var chatMessages = new List<ChatMessage>
                {
                    new SystemChatMessage(systemChatMessage),
                    new UserChatMessage(userChatMessage)
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