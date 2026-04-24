
using System.Diagnostics;
using System.Threading.Tasks;
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
            _logger.LogInformation($"GetProfileSettings retrieved in {stopwatch.ElapsedMilliseconds} ms");
            
            return profileSettings;
        }
    }
}