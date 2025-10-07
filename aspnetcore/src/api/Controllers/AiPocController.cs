using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using api.Services;
using Microsoft.AspNetCore.Mvc;
using OpenAI.Chat;
using api.Models.Ai;
using System.Text.Json;

namespace api.Controllers
{
    /*
     * AiPocController implements experimental MVC functionality for testin AI feature
     */
    public class AiPocController : Controller
    {
        private readonly ChatClient _chatClient;
        private readonly AiPocService _aiPocService;

        public AiPocController(ChatClient chatClient, AiPocService aiPocService)
        {
            _chatClient = chatClient;
            _aiPocService = aiPocService;
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
        public async Task<IActionResult> QueryAiModel(string systemPrompt, string profileData)
        {
            if (string.IsNullOrWhiteSpace(systemPrompt) && string.IsNullOrWhiteSpace(profileData))
            {
                return BadRequest(new { error = "Prompt cannot be empty." });
            }

            try
            {
                ChatCompletionOptions options = new()
                {
                    MaxOutputTokenCount = 500
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