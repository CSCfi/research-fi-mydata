using api.Services;
using api.Models;
using api.Models.ProfileEditor;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Http;

namespace api.Controllers
{
    /*
     * CooperationDemoController implements profile editor API for setting co-operation choices.
     * TODO: This is a draft version for demo, as there is not yet database support for saving selections.
     */
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class CooperationDemoController : TtvControllerBase
    {
        private readonly UserProfileService _userProfileService;
        private readonly TtvSqlService _ttvSqlService;
        private readonly ILogger<UserProfileController> _logger;

        public CooperationDemoController(UserProfileService userProfileService, TtvSqlService ttvSqlService, ILogger<UserProfileController> logger)
        {
            _userProfileService = userProfileService;
            _ttvSqlService = ttvSqlService;
            _logger = logger;
        }

        /// <summary>
        /// Get cooperation selections.
        /// </summary>
        [HttpGet]
        [ProducesResponseType(typeof(ApiResponseCooperationDemoGet), StatusCodes.Status200OK)]
        public async Task<IActionResult> Get()
        {
            // Check that user profile exists.
            var orcidId = this.GetOrcidId();
            var userprofileId = await _userProfileService.GetUserprofileId(orcidId);
            if (userprofileId == -1)
            {
                return Ok(new ApiResponse(success: false, reason: "profile not found"));
            }

            // TODO: Cooperation choices must be read from the DB when the model is available. This is a demo implementation.

            var cooperationItems = new List<ProfileEditorCooperationItem>()
            {
                new ProfileEditorCooperationItem()
                {
                    Id = 1,
                    NameFi = "Olen kiinnostunut tiedotusvälineiden yhteydenotoista",
                    NameSv = "",
                    NameEn = "I am interested in media contacts",
                    Selected = false
                },
                new ProfileEditorCooperationItem()
                {
                    Id = 2,
                    NameFi = "Olen kiinnostunut yhteistyöstä muiden tutkijoiden ja tutkimusryhmien kanssa",
                    NameSv = "",
                    NameEn = "I am interested in cooperation with other researchers and research groups",
                    Selected = false
                },
                new ProfileEditorCooperationItem()
                {
                    Id = 3,
                    NameFi = "Olen kiinnostunut yhteistyöstä yritysten kanssa",
                    NameSv = "",
                    NameEn = "I am interested in working with companies",
                    Selected = false
                },
                new ProfileEditorCooperationItem()
                {
                    Id = 4,
                    NameFi = "Olen kiinnostunut toimimaan tieteellisten julkaisujen vertaisarvioijana",
                    NameSv = "",
                    NameEn = "I am interested in being a peer reviewer for scientific publications",
                    Selected = false
                }
            };

            return Ok(new ApiResponseCooperationDemoGet(success: true, reason: "", data: cooperationItems, fromCache: false));
        }


        /// <summary>
        /// Modify cooperation selections.
        /// </summary>
        [HttpPatch]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
        public async Task<IActionResult> PatchMany([FromBody] List<ProfileEditorCooperationItem> profileEditorCooperationItems)
        {
            // Return immediately if there is nothing to change.
            if (profileEditorCooperationItems.Count == 0)
            {
                return Ok(new ApiResponse(success: true));
            }

            // Check that user profile exists.
            var orcidId = this.GetOrcidId();
            var userprofileId = await _userProfileService.GetUserprofileId(orcidId);
            if (userprofileId == -1)
            {
                return Ok(new ApiResponse(success: false, reason: "profile not found"));
            }

            // Save cooperation selections
            foreach (ProfileEditorCooperationItem profileEditorCooperationItem in profileEditorCooperationItems)
            {
                // TODO: Save cooperation selections
            }

            return Ok(new ApiResponse(success: true));
        }
    }
}