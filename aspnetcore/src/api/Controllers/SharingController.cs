using api.Services;
using api.Models;
using api.Models.Ttv;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Collections.Generic;
using Microsoft.Extensions.Caching.Memory;

namespace api.Controllers
{
    /*
     * SharingController implements profile editor API commands for sharing settings.
     */
    [Route("api/sharing")]
    [ApiController]
    [Authorize(Policy = "RequireScopeApi1AndClaimOrcid")]
    public class SharingController : TtvControllerBase
    {
        private readonly TtvContext _ttvContext;
        private readonly UserProfileService _userProfileService;
        private readonly UtilityService _utilityService;
        private readonly DataSourceHelperService _dataSourceHelperService;
        private readonly IMemoryCache _cache;

        public SharingController(TtvContext ttvContext,
            UserProfileService userProfileService,
            UtilityService utilityService,
            DataSourceHelperService dataSourceHelperService,
            IMemoryCache memoryCache)
        {
            _ttvContext = ttvContext;
            _userProfileService = userProfileService;
            _utilityService = utilityService;
            _dataSourceHelperService = dataSourceHelperService;
            _cache = memoryCache;
        }
    }
}