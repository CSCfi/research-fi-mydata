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

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            // Check that user profile exists.
            var orcidId = this.GetOrcidId();
            var userprofileId = await _userProfileService.GetUserprofileId(orcidId);
            if (userprofileId == -1)
            {
                return Ok(new ApiResponse(success: false, reason: "profile not found"));
            }

            // TODO: this is a demo implementation, modify according to database model

            var cooperationItems = new List<ProfileEditorCooperationItem>()
            {
                new ProfileEditorCooperationItem()
                {
                    Id = 1,
                    NameFi = "foo1_fi",
                    NameSv = "foo1_sv",
                    NameEn = "foo1_en",
                    Selected = false
                },
                new ProfileEditorCooperationItem()
                {
                    Id = 2,
                    NameFi = "foo2_fi",
                    NameSv = "foo2_sv",
                    NameEn = "foo2_en",
                    Selected = false
                },
                new ProfileEditorCooperationItem()
                {
                    Id = 3,
                    NameFi = "foo3_fi",
                    NameSv = "foo3_sv",
                    NameEn = "foo3_en",
                    Selected = false
                },
                new ProfileEditorCooperationItem()
                {
                    Id = 4,
                    NameFi = "foo3_fi",
                    NameSv = "foo3_sv",
                    NameEn = "foo3_en",
                    Selected = false
                }
            };

            return Ok(new ApiResponse(success: true, data: cooperationItems, fromCache: false));
        }



        [HttpPatch]
        public async Task<IActionResult> PatchMany([FromBody] ProfileEditorCooperationModificationRequest profileEditorCooperationModificationRequest)
        {
            // Return immediately if there is nothing to change.
            if (profileEditorCooperationModificationRequest.items.Count == 0)
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

            // Collect information about updated items to a response object, which will be sent in response.
            var profileEditorCooperationModificationResponse = new ProfileEditorCooperationModificationResponse();

            // Set 'Show' and 'PrimaryValue' in FactFieldValues
            foreach (ProfileEditorCooperationItem profileEditorCooperationItem in profileEditorCooperationModificationRequest.items.ToList())
            {
                // TODO: Save cooperation selections
                profileEditorCooperationModificationResponse.items.Add(profileEditorCooperationItem);
            }

            return Ok(new ApiResponse(success: true, data: profileEditorCooperationModificationResponse));
        }
    }
}