using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using api.Services;
using Microsoft.AspNetCore.Mvc;
using api.Models.Ai;
using Microsoft.Extensions.Logging;
using api.Models.Log;
using System.Diagnostics;
using Microsoft.AspNetCore.Http;
using api.Models.Api;
using api.Models.Common;

namespace api.Controllers
{
    /*
     * PublicApiController implements endpoint for Public API.
     */
    [Route("api/publicapi")]
    [ApiController]
    public class PublicApiController : TtvControllerBase
    {
        private readonly ILogger<PublicApiController> _logger;
        private readonly IUserProfileService _userProfileService;

        public PublicApiController(ILogger<PublicApiController> logger, IUserProfileService userProfileService)
        {
            _logger = logger;
            _userProfileService = userProfileService;
        }

/*
        /// <summary>
        /// Get profile data.
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetDataForPublicApi()
        {
            LogUserIdentification logUserIdentification = this.GetLogUserIdentification();
        }
    }
*/
    }
}