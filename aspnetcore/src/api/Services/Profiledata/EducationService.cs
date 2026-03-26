using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.Models.Ttv;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using api.Models.Common;
using api.Models.ProfileEditor.Items;

namespace api.Services.Profiledata
{
    public class EducationService : IEducationService
    {
        private readonly TtvContext _ttvContext;
        private readonly ILanguageService _languageService;
        private readonly ILogger<EducationService> _logger;


        public EducationService(
            TtvContext ttvContext,
            ILanguageService languageService,
            ILogger<EducationService> logger)
        {
            _ttvContext = ttvContext;
            _languageService = languageService;
            _logger = logger;
        }

        /*
         * Educations
         */
        public async Task<List<ProfileEditorEducation>> GetProfileEditorEducations(int userprofileId, bool forElasticsearch = false)
        {
            List<ProfileEditorEducation> educations = await _ttvContext.FactFieldValues.Where(ffv => ffv.DimUserProfileId == userprofileId
                && ffv.DimEducationId > 0
                && ffv.DimFieldDisplaySettings.FieldIdentifier == Constants.FieldIdentifiers.ACTIVITY_EDUCATION
                && (forElasticsearch ? ffv.Show == true : true))
                .Select(ffv => new ProfileEditorEducation()
                {
                    NameFi = ffv.DimEducation.NameFi,
                    NameEn = ffv.DimEducation.NameEn,
                    NameSv = ffv.DimEducation.NameSv,
                    DegreeGrantingInstitutionName = ffv.DimEducation.DegreeGrantingInstitutionName,
                    StartDate = new ProfileEditorDate {
                        Year = ffv.DimEducation.DimStartDateNavigation.Year,
                        Month = ffv.DimEducation.DimStartDateNavigation.Month,
                        Day = ffv.DimEducation.DimStartDateNavigation.Day
                    },
                    EndDate = new ProfileEditorDate {
                        Year = ffv.DimEducation.DimEndDateNavigation.Year,
                        Month = ffv.DimEducation.DimEndDateNavigation.Month,
                        Day = ffv.DimEducation.DimEndDateNavigation.Day
                    },
                    itemMeta = new ProfileEditorItemMeta(
                        ffv.DimEducationId,
                        Constants.ItemMetaTypes.ACTIVITY_EDUCATION,
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

            // Postprocessing. Translate education names and data source organizaton names.
            foreach (ProfileEditorEducation education in educations)
            {
                NameTranslation nameTraslationEducation = _languageService.GetNameTranslation(
                    nameFi: education.NameFi,
                    nameEn: education.NameEn,
                    nameSv: education.NameSv
                );
                education.NameFi = nameTraslationEducation.NameFi;
                education.NameEn = nameTraslationEducation.NameEn;
                education.NameSv = nameTraslationEducation.NameSv;

                foreach (ProfileEditorSource dataSource in education.DataSources)
                {
                    NameTranslation dataSourceOrganizationName = _languageService.GetNameTranslation(
                        nameFi: dataSource.Organization.NameFi,
                        nameEn: dataSource.Organization.NameEn,
                        nameSv: dataSource.Organization.NameSv
                    );
                    dataSource.Organization.NameFi = dataSourceOrganizationName.NameFi;
                    dataSource.Organization.NameEn = dataSourceOrganizationName.NameEn;
                    dataSource.Organization.NameSv = dataSourceOrganizationName.NameSv;
                }
            }

            return educations;
        }  
    }
}