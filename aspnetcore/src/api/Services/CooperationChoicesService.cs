
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using api.Models.ProfileEditor;
using api.Models.Ttv;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;


namespace api.Services
{
    public class CooperationChoicesService : ICooperationChoicesService
    {
        private readonly TtvContext _ttvContext;
        private readonly ILogger<CooperationChoicesService> _logger;

        public CooperationChoicesService(TtvContext ttvContext, ILogger<CooperationChoicesService> logger)
        {
            _ttvContext = ttvContext;
            _logger = logger;
        }

        public async Task<List<ProfileEditorCooperationItem>> GetCooperationChoices(int userprofileId, bool forElasticsearch = false)
        {
            var stopwatch = Stopwatch.StartNew();

            List<ProfileEditorCooperationItem> cooperationItems = await _ttvContext.DimUserChoices
                .Where(c => c.DimUserProfileId == userprofileId && (!forElasticsearch || c.UserChoiceValue))
                .Select(c => new ProfileEditorCooperationItem
                {
                    Id = c.Id,
                    NameFi = c.DimReferencedataIdAsUserChoiceLabelNavigation.NameFi,
                    NameEn = c.DimReferencedataIdAsUserChoiceLabelNavigation.NameEn,
                    NameSv = c.DimReferencedataIdAsUserChoiceLabelNavigation.NameSv,
                    Selected = c.UserChoiceValue,
                    Order = c.DimReferencedataIdAsUserChoiceLabelNavigation.Order
                }).AsNoTracking().ToListAsync();
            
            stopwatch.Stop();
            _logger.LogInformation($"GetCooperationChoices. {cooperationItems.Count} items in {stopwatch.ElapsedMilliseconds}ms.");
            
            return cooperationItems;
        }
    }
}