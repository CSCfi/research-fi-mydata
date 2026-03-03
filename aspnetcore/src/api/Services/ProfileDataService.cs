using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.Models.Ttv;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using api.Models.Common;
using api.Models.ProfileEditor.Items;

namespace api.Services
{
    public class ProfileDataService : IProfileDataService
    {
        private readonly TtvContext _ttvContext;
        private readonly ILogger<ProfileDataService> _logger;


        public ProfileDataService(
            TtvContext ttvContext,
            ILogger<ProfileDataService> logger)
        {
            _ttvContext = ttvContext;
            _logger = logger;
        }

        public async Task<List<ProfileEditorName>> GetProfileEditorNames(int userprofileId)
        {
            List<ProfileEditorName> names = await _ttvContext.FactFieldValues.Where(ffv => ffv.DimUserProfileId == userprofileId
                && ffv.DimFieldDisplaySettings.FieldIdentifier == Constants.FieldIdentifiers.PERSON_NAME)
                .Select(ffv => new ProfileEditorName()
                {
                    FirstNames = ffv.DimName.FirstNames.Trim(),
                    LastName = ffv.DimName.LastName.Trim(),
                    FullName = $"{ffv.DimName.LastName.Trim()} {ffv.DimName.FirstNames.Trim()}".Trim(), // Populate for Elasticsearch queries
                    itemMeta = new ProfileEditorItemMeta(
                        ffv.DimNameId,
                        Constants.ItemMetaTypes.PERSON_NAME,
                        ffv.Show,
                        ffv.PrimaryValue
                    ),
                    DataSources = new List<ProfileEditorSource> {
                        new ProfileEditorSource() {
                            Id = ffv.DimRegisteredDataSourceId,
                            RegisteredDataSource = ffv.DimRegisteredDataSource.Name,
                            Organization = new Organization() {
                                NameFi = ffv.DimRegisteredDataSource.DimOrganization.NameFi,
                                NameEn = ffv.DimRegisteredDataSource.DimOrganization.NameEn,
                                NameSv = ffv.DimRegisteredDataSource.DimOrganization.NameSv,
                                SectorId = ffv.DimRegisteredDataSource.DimOrganization.DimSector.SectorId
                            }
                        }
                    }
                }).AsNoTracking().ToListAsync();
            return names;
        }

        public async Task<List<ProfileEditorName>> GetProfileEditorOtherNames(int userprofileId)
        {
            List<ProfileEditorName> otherNames = await _ttvContext.FactFieldValues.Where(ffv => ffv.DimUserProfileId == userprofileId
                && ffv.DimFieldDisplaySettings.FieldIdentifier == Constants.FieldIdentifiers.PERSON_OTHER_NAMES)
                .Select(ffv => new ProfileEditorName()
                {
                    FirstNames = "",
                    LastName = "",
                    FullName = ffv.DimName.FullName.Trim(),
                    itemMeta = new ProfileEditorItemMeta(
                        ffv.DimNameId,
                        Constants.ItemMetaTypes.PERSON_OTHER_NAMES,
                        ffv.Show,
                        ffv.PrimaryValue
                    ),
                    DataSources = new List<ProfileEditorSource> {
                        new ProfileEditorSource() {
                            Id = ffv.DimRegisteredDataSourceId,
                            RegisteredDataSource = ffv.DimRegisteredDataSource.Name,
                            Organization = new Organization() {
                                NameFi = ffv.DimRegisteredDataSource.DimOrganization.NameFi,
                                NameEn = ffv.DimRegisteredDataSource.DimOrganization.NameEn,
                                NameSv = ffv.DimRegisteredDataSource.DimOrganization.NameSv,
                                SectorId = ffv.DimRegisteredDataSource.DimOrganization.DimSector.SectorId
                            }
                        }
                    }
                }).AsNoTracking().ToListAsync();
            return otherNames;
        }
    }
}