
using System.Diagnostics;
using System.Threading.Tasks;
using api.Models.Common;
using api.Models.ProfileEditor;
using api.Models.Ttv;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;


namespace api.Services
{
    public class SettingsService : ISettingsService
    {
        private readonly TtvContext _ttvContext;
        private readonly ILogger<SettingsService> _logger;

        public SettingsService(TtvContext ttvContext, ILogger<SettingsService> logger)
        {
            _ttvContext = ttvContext;
            _logger = logger;
        }

        public async Task<ProfileSettings> GetProfileSettings(int userprofileId, bool forElasticsearch = false)
        {
            var stopwatch = Stopwatch.StartNew();

            DimUserProfile dup = await _ttvContext.DimUserProfiles
                .AsNoTracking()
                .FirstOrDefaultAsync(up => up.Id == userprofileId);

            var profileSettings = new ProfileSettings()
            {
                Hidden = dup?.Hidden,
                PublishNewData = dup?.PublishNewOrcidData,
                HighlightOpeness = dup?.HighlightOpeness
            };
            
            stopwatch.Stop();
            if (stopwatch.ElapsedMilliseconds > Constants.LoggingParameters.SLOW_OPERATION_MS_THRESHOLD)
            {
                _logger.LogWarning($"GetProfileSettings is slow. userprofileId={userprofileId}, forElasticsearch={forElasticsearch} in {stopwatch.ElapsedMilliseconds}ms.");
            }
            
            return profileSettings;
        }
    }
}