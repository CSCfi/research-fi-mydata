using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.Models.Ttv;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using api.Models.Common;
using api.Models.ProfileEditor.Items;
using System;
using System.Diagnostics;

namespace api.Services.Profiledata
{
    public class ResearchActivityService : IResearchActivityService
    {
        private readonly TtvContext _ttvContext;
        private readonly ILanguageService _languageService;
        private readonly ILogger<ResearchActivityService> _logger;


        public ResearchActivityService(
            TtvContext ttvContext,
            ILanguageService languageService,
            ILogger<ResearchActivityService> logger)
        {
            _ttvContext = ttvContext;
            _languageService = languageService;
            _logger = logger;
        }

        /*
         * Compute hash key for research activity based on start year and name FI, EN, SV.
         */
        public string ComputeKey(int startYear, string nameFi, string nameEn, string nameSv)
        {
            return $"{startYear}_{nameFi}_{nameEn}_{nameSv}".Trim().ToLowerInvariant().Replace(" ", "");
        }

        public class ResearchActivityDto
        {
            public bool IsProfileOnlyResearchActivity { get; set; }
            public bool IsDuplicate { get; set; } = false;
            public int Id { get; set; }
            public bool? Show { get; set; }
            public bool? PrimaryValue { get; set; }
            public int DimRegisteredDatasource_Id { get; set; }
            public string? DimRegisteredDatasource_Name { get; set; }
            public string? DimRegisteredDatasource_DimOrganization_NameFi { get; set; }
            public string? DimRegisteredDatasource_DimOrganization_NameEn { get; set; }
            public string? DimRegisteredDatasource_DimOrganization_NameSv { get; set; }
            public string? DimRegisteredDatasource_DimOrganization_DimSector_SectorId { get; set; }
            public int ResearchActivity_DimOrganization_Id { get; set; }
            public string ResearchActivity_DimOrganization_OrganizationId { get; set; }
            public string ResearchActivity_DimOrganization_NameFi { get; set; }
            public string ResearchActivity_DimOrganization_NameEn { get; set; }
            public string ResearchActivity_DimOrganization_NameSv { get; set; }
            public string? ResearchActivity_DimOrganization_DimSector_SectorId { get; set; }
            public string ResearchActivity_DimOrganization_DimSector_NameFi { get; set; }
            public string ResearchActivity_DimOrganization_DimSector_NameEn { get; set; }
            public string ResearchActivity_DimOrganization_DimSector_NameSv { get; set; }
            public int? ResearchActivity_DimOrganizationBroader_Id { get; set; }
            public string ResearchActivity_DimOrganizationBroader_NameFi { get; set; }
            public string ResearchActivity_DimOrganizationBroader_NameEn { get; set; }
            public string ResearchActivity_DimOrganizationBroader_NameSv { get; set; }
            public string ResearchActivity_DimOrganizationBroader_DimSector_NameFi { get; set; }
            public string ResearchActivity_DimOrganizationBroader_DimSector_NameEn { get; set; }
            public string ResearchActivity_DimOrganizationBroader_DimSector_NameSv { get; set; }
            public int DimIdentifierlessData_Id { get; set; }  
            public string DimIdentifierlessData_Type { get; set; }
            public string DimIdentifierlessData_ValueFi { get; set; }
            public string DimIdentifierlessData_ValueEn { get; set; }
            public string DimIdentifierlessData_ValueSv { get; set; }
            public int DimIdentifierlessData_Child_Id { get; set; }
            public string DimIdentifierlessData_Child_Type { get; set; }
            public string DimIdentifierlessData_Child_ValueFi { get; set; }
            public string DimIdentifierlessData_Child_ValueEn { get; set; }
            public string DimIdentifierlessData_Child_ValueSv { get; set; }
            public string ResearchActivity_ActivityType_CodeValue { get; set; }
            public string ResearchActivity_ActivityType_NameFi { get; set; }
            public string ResearchActivity_ActivityType_NameEn { get; set; }
            public string ResearchActivity_ActivityType_NameSv { get; set; }
            public string ResearchActivity_Role_CodeValue { get; set; }
            public string ResearchActivity_Role_NameFi { get; set; }
            public string ResearchActivity_Role_NameEn { get; set; }
            public string ResearchActivity_Role_NameSv { get; set; }
            public string ResearchActivity_ActivityType_From_Role_Parent_CodeValue { get; set; }
            public string ResearchActivity_ActivityType_From_Role_Parent_NameFi { get; set; }
            public string ResearchActivity_ActivityType_From_Role_Parent_NameEn { get; set; }
            public string ResearchActivity_ActivityType_From_Role_Parent_NameSv { get; set; }
            public string ResearchActivity_NameFi { get; set; }
            public string ResearchActivity_NameEn { get; set; }
            public string ResearchActivity_NameSv { get; set; }
            public string ResearchActivity_DescriptionFi { get; set; }
            public string ResearchActivity_DescriptionEn { get; set; }
            public string ResearchActivity_DescriptionSv { get; set; }
            public int ResearchActivity_StartDate_Year { get; set; }
            public int ResearchActivity_StartDate_Month { get; set; }
            public int ResearchActivity_StartDate_Day { get; set; }
            public int ResearchActivity_EndDate_Year { get; set; }
            public int ResearchActivity_EndDate_Month { get; set; }
            public int ResearchActivity_EndDate_Day { get; set; }
            public bool? ResearchActivity_InternationalCollaboration { get; set; }
            public string ResearchActivity_DimWebLink_Url { get; set; }
            public List<ProfileEditorWebLink_WithoutItemMeta> WebLinks { get; set; }
        }

        /*
         * Research activities.
         */
        public async Task<List<ProfileEditorActivityAndReward>> GetProfileEditorActiviesAndRewards(int userprofileId, bool forElasticsearch = false)
        {
            var stopwatch = Stopwatch.StartNew();

            /*
             * DimResearchActivity => DTO
             */
            var stopwatch_researchActivityDtos = Stopwatch.StartNew();
            List<ResearchActivityDto> researchActivityDtos = await _ttvContext.FactFieldValues.Where(ffv => ffv.DimUserProfileId == userprofileId
                && ffv.DimResearchActivityId > 0
                && ffv.DimFieldDisplaySettings.FieldIdentifier == Constants.FieldIdentifiers.ACTIVITY_RESEARCH_ACTIVITY
                && (forElasticsearch ? ffv.Show == true : true))
                .Select(ffv => new ResearchActivityDto()
                {
                    IsProfileOnlyResearchActivity = false,
                    Id = ffv.DimResearchActivityId,
                    Show = ffv.Show,
                    PrimaryValue = ffv.PrimaryValue,
                    DimRegisteredDatasource_Id = ffv.DimRegisteredDataSourceId,
                    DimRegisteredDatasource_Name = ffv.DimRegisteredDataSource.Name,
                    DimRegisteredDatasource_DimOrganization_NameFi = ffv.DimRegisteredDataSource.DimOrganization.NameFi,
                    DimRegisteredDatasource_DimOrganization_NameEn = ffv.DimRegisteredDataSource.DimOrganization.NameEn,
                    DimRegisteredDatasource_DimOrganization_NameSv = ffv.DimRegisteredDataSource.DimOrganization.NameSv,
                    DimRegisteredDatasource_DimOrganization_DimSector_SectorId = ffv.DimRegisteredDataSource.DimOrganization.DimSector.SectorId,
                    ResearchActivity_DimOrganization_Id = ffv.DimResearchActivity.DimOrganizationId,
                    ResearchActivity_DimOrganization_OrganizationId = ffv.DimResearchActivity.DimOrganizationId > 0 ? ffv.DimResearchActivity.DimOrganization.OrganizationId : null,
                    ResearchActivity_DimOrganization_NameFi = ffv.DimResearchActivity.DimOrganizationId > 0 ? ffv.DimResearchActivity.DimOrganization.NameFi : null,
                    ResearchActivity_DimOrganization_NameEn = ffv.DimResearchActivity.DimOrganizationId > 0 ? ffv.DimResearchActivity.DimOrganization.NameEn : null,
                    ResearchActivity_DimOrganization_NameSv = ffv.DimResearchActivity.DimOrganizationId > 0 ? ffv.DimResearchActivity.DimOrganization.NameSv : null,
                    ResearchActivity_DimOrganization_DimSector_SectorId = ffv.DimResearchActivity.DimOrganizationId > 0 ? ffv.DimResearchActivity.DimOrganization.DimSector.SectorId : null,
                    ResearchActivity_DimOrganization_DimSector_NameFi = ffv.DimResearchActivity.DimOrganizationId > 0 ? ffv.DimResearchActivity.DimOrganization.DimSector.NameFi : null,
                    ResearchActivity_DimOrganization_DimSector_NameEn = ffv.DimResearchActivity.DimOrganizationId > 0 ? ffv.DimResearchActivity.DimOrganization.DimSector.NameEn : null,
                    ResearchActivity_DimOrganization_DimSector_NameSv = ffv.DimResearchActivity.DimOrganizationId > 0 ? ffv.DimResearchActivity.DimOrganization.DimSector.NameSv : null,
                    ResearchActivity_DimOrganizationBroader_Id = ffv.DimResearchActivity.DimOrganizationId > 0 && ffv.DimResearchActivity.DimOrganization.DimOrganizationBroader.HasValue ? ffv.DimResearchActivity.DimOrganization.DimOrganizationBroader.Value : null,
                    ResearchActivity_DimOrganizationBroader_NameFi = ffv.DimResearchActivity.DimOrganizationId > 0 && ffv.DimResearchActivity.DimOrganization.DimOrganizationBroader.HasValue ? ffv.DimResearchActivity.DimOrganization.DimOrganizationBroaderNavigation.NameFi : null,
                    ResearchActivity_DimOrganizationBroader_NameEn = ffv.DimResearchActivity.DimOrganizationId > 0 && ffv.DimResearchActivity.DimOrganization.DimOrganizationBroader.HasValue ? ffv.DimResearchActivity.DimOrganization.DimOrganizationBroaderNavigation.NameEn : null,
                    ResearchActivity_DimOrganizationBroader_NameSv = ffv.DimResearchActivity.DimOrganizationId > 0 && ffv.DimResearchActivity.DimOrganization.DimOrganizationBroader.HasValue ? ffv.DimResearchActivity.DimOrganization.DimOrganizationBroaderNavigation.NameSv : null,
                    ResearchActivity_DimOrganizationBroader_DimSector_NameFi = ffv.DimResearchActivity.DimOrganizationId > 0 && ffv.DimResearchActivity.DimOrganization.DimOrganizationBroader.HasValue ? ffv.DimResearchActivity.DimOrganization.DimOrganizationBroaderNavigation.DimSector.NameFi : null,
                    ResearchActivity_DimOrganizationBroader_DimSector_NameEn = ffv.DimResearchActivity.DimOrganizationId > 0 && ffv.DimResearchActivity.DimOrganization.DimOrganizationBroader.HasValue ? ffv.DimResearchActivity.DimOrganization.DimOrganizationBroaderNavigation.DimSector.NameEn : null,
                    ResearchActivity_DimOrganizationBroader_DimSector_NameSv = ffv.DimResearchActivity.DimOrganizationId > 0 && ffv.DimResearchActivity.DimOrganization.DimOrganizationBroader.HasValue ? ffv.DimResearchActivity.DimOrganization.DimOrganizationBroaderNavigation.DimSector.NameSv : null,
                    DimIdentifierlessData_Id = ffv.DimIdentifierlessDataId,
                    DimIdentifierlessData_Type = ffv.DimIdentifierlessData.Type,
                    DimIdentifierlessData_ValueFi = ffv.DimIdentifierlessData.ValueFi,
                    DimIdentifierlessData_ValueEn = ffv.DimIdentifierlessData.ValueEn,
                    DimIdentifierlessData_ValueSv = ffv.DimIdentifierlessData.ValueSv,
                    DimIdentifierlessData_Child_Type = ffv.DimIdentifierlessData.InverseDimIdentifierlessData.FirstOrDefault().Type,
                    DimIdentifierlessData_Child_ValueFi = ffv.DimIdentifierlessData.InverseDimIdentifierlessData.FirstOrDefault().ValueFi,
                    DimIdentifierlessData_Child_ValueEn = ffv.DimIdentifierlessData.InverseDimIdentifierlessData.FirstOrDefault().ValueEn,
                    DimIdentifierlessData_Child_ValueSv = ffv.DimIdentifierlessData.InverseDimIdentifierlessData.FirstOrDefault().ValueSv,
                    // Activity type.
                    ResearchActivity_ActivityType_CodeValue = ffv.DimResearchActivity.FactContributions
                        .Where(fc => fc.ContributionType == Constants.FactContributionTypes.ACTIVITY_TYPE)
                        .Select(fc => fc.DimReferencedataActorRole.CodeValue).FirstOrDefault(),
                    ResearchActivity_ActivityType_NameFi = ffv.DimResearchActivity.FactContributions
                        .Where(fc => fc.ContributionType == Constants.FactContributionTypes.ACTIVITY_TYPE)
                        .Select(fc => fc.DimReferencedataActorRole.NameFi).FirstOrDefault(),
                    ResearchActivity_ActivityType_NameEn = ffv.DimResearchActivity.FactContributions
                        .Where(fc => fc.ContributionType == Constants.FactContributionTypes.ACTIVITY_TYPE)
                        .Select(fc => fc.DimReferencedataActorRole.NameEn).FirstOrDefault(),
                    ResearchActivity_ActivityType_NameSv = ffv.DimResearchActivity.FactContributions
                        .Where(fc => fc.ContributionType == Constants.FactContributionTypes.ACTIVITY_TYPE)
                        .Select(fc => fc.DimReferencedataActorRole.NameSv).FirstOrDefault(),
                    // Activity role.
                    ResearchActivity_Role_CodeValue = ffv.DimResearchActivity.FactContributions
                        .Where(fc => fc.ContributionType == Constants.FactContributionTypes.RESEARCHER_NAME_ACTIVITY)
                        .Select(fc => fc.DimReferencedataActorRole.CodeValue).FirstOrDefault(),
                    ResearchActivity_Role_NameFi = ffv.DimResearchActivity.FactContributions
                        .Where(fc => fc.ContributionType == Constants.FactContributionTypes.RESEARCHER_NAME_ACTIVITY)
                        .Select(fc => fc.DimReferencedataActorRole.NameFi).FirstOrDefault(),
                    ResearchActivity_Role_NameEn = ffv.DimResearchActivity.FactContributions
                        .Where(fc => fc.ContributionType == Constants.FactContributionTypes.RESEARCHER_NAME_ACTIVITY)
                        .Select(fc => fc.DimReferencedataActorRole.NameEn).FirstOrDefault(),
                    ResearchActivity_Role_NameSv = ffv.DimResearchActivity.FactContributions
                        .Where(fc => fc.ContributionType == Constants.FactContributionTypes.RESEARCHER_NAME_ACTIVITY)
                        .Select(fc => fc.DimReferencedataActorRole.NameSv).FirstOrDefault(),
                    // Activity type from role parent.
                    ResearchActivity_ActivityType_From_Role_Parent_CodeValue = ffv.DimResearchActivity.FactContributions
                        .Where(fc => fc.ContributionType == Constants.FactContributionTypes.RESEARCHER_NAME_ACTIVITY)
                        .Select(fc => fc.DimReferencedataActorRole.DimReferencedata.CodeValue).FirstOrDefault(),
                    ResearchActivity_ActivityType_From_Role_Parent_NameFi = ffv.DimResearchActivity.FactContributions
                        .Where(fc => fc.ContributionType == Constants.FactContributionTypes.RESEARCHER_NAME_ACTIVITY)
                        .Select(fc => fc.DimReferencedataActorRole.DimReferencedata.NameFi).FirstOrDefault(),
                    ResearchActivity_ActivityType_From_Role_Parent_NameEn = ffv.DimResearchActivity.FactContributions
                        .Where(fc => fc.ContributionType == Constants.FactContributionTypes.RESEARCHER_NAME_ACTIVITY)
                        .Select(fc => fc.DimReferencedataActorRole.DimReferencedata.NameEn).FirstOrDefault(),
                    ResearchActivity_ActivityType_From_Role_Parent_NameSv = ffv.DimResearchActivity.FactContributions
                        .Where(fc => fc.ContributionType == Constants.FactContributionTypes.RESEARCHER_NAME_ACTIVITY)
                        .Select(fc => fc.DimReferencedataActorRole.DimReferencedata.NameSv).FirstOrDefault(),
                    ResearchActivity_StartDate_Day = ffv.DimResearchActivity.DimStartDateNavigation != null ? ffv.DimResearchActivity.DimStartDateNavigation.Day : 0,
                    ResearchActivity_StartDate_Month = ffv.DimResearchActivity.DimStartDateNavigation != null ? ffv.DimResearchActivity.DimStartDateNavigation.Month : 0,
                    ResearchActivity_StartDate_Year = ffv.DimResearchActivity.DimStartDateNavigation != null ? ffv.DimResearchActivity.DimStartDateNavigation.Year : 0,
                    ResearchActivity_EndDate_Day = ffv.DimResearchActivity.DimEndDateNavigation != null ? ffv.DimResearchActivity.DimEndDateNavigation.Day : 0,
                    ResearchActivity_EndDate_Month = ffv.DimResearchActivity.DimEndDateNavigation != null ? ffv.DimResearchActivity.DimEndDateNavigation.Month : 0,
                    ResearchActivity_EndDate_Year = ffv.DimResearchActivity.DimEndDateNavigation != null ? ffv.DimResearchActivity.DimEndDateNavigation.Year : 0,
                    ResearchActivity_NameFi = ffv.DimResearchActivity.NameFi,
                    ResearchActivity_NameEn = ffv.DimResearchActivity.NameEn,
                    ResearchActivity_NameSv = ffv.DimResearchActivity.NameSv,
                    ResearchActivity_DescriptionFi = ffv.DimResearchActivity.DescriptionFi,
                    ResearchActivity_DescriptionEn = ffv.DimResearchActivity.DescriptionEn,
                    ResearchActivity_DescriptionSv = ffv.DimResearchActivity.DescriptionSv,
                    ResearchActivity_InternationalCollaboration = ffv.DimResearchActivity.InternationalCollaboration,
                    ResearchActivity_DimWebLink_Url = ffv.DimResearchActivity.DimWebLinks.FirstOrDefault().Url,
                    WebLinks = ffv.DimResearchActivity.DimWebLinks.Select(wl => new ProfileEditorWebLink_WithoutItemMeta()
                    {
                        Url = wl.Url,
                        LinkLabel = wl.LinkLabel,
                        LinkType = wl.LinkType
                    }).ToList() 
                }).AsNoTracking().ToListAsync();
            stopwatch_researchActivityDtos.Stop();
            _logger.LogInformation($"GetProfileEditorResearchActivities. SQL query for researchActivityDtos got {researchActivityDtos.Count} items and took {stopwatch_researchActivityDtos.ElapsedMilliseconds}ms.");

            /*
             * DimProfileOnlyResearchActivity => DTO
             */
            var stopwatch_profileOnlyResearchActivityDtos = Stopwatch.StartNew();
            List<ResearchActivityDto> profileOnlyResearchActivityDtos = await _ttvContext.FactFieldValues.Where(ffv => ffv.DimUserProfileId == userprofileId
                && ffv.DimProfileOnlyResearchActivityId > 0
                && ffv.DimFieldDisplaySettings.FieldIdentifier == Constants.FieldIdentifiers.ACTIVITY_RESEARCH_ACTIVITY
                && (forElasticsearch ? ffv.Show == true : true))
                .Select(ffv => new ResearchActivityDto()
                {
                    IsProfileOnlyResearchActivity = true,
                    Id = ffv.DimProfileOnlyResearchActivityId,
                    Show = ffv.Show,
                    PrimaryValue = ffv.PrimaryValue,
                    DimRegisteredDatasource_Id = ffv.DimRegisteredDataSourceId,
                    DimRegisteredDatasource_Name = ffv.DimRegisteredDataSource.Name,
                    DimRegisteredDatasource_DimOrganization_NameFi = ffv.DimRegisteredDataSource.DimOrganization.NameFi,
                    DimRegisteredDatasource_DimOrganization_NameEn = ffv.DimRegisteredDataSource.DimOrganization.NameEn,
                    DimRegisteredDatasource_DimOrganization_NameSv = ffv.DimRegisteredDataSource.DimOrganization.NameSv,
                    DimRegisteredDatasource_DimOrganization_DimSector_SectorId = ffv.DimRegisteredDataSource.DimOrganization.DimSector.SectorId,
                    ResearchActivity_DimOrganization_Id = ffv.DimProfileOnlyResearchActivity.DimOrganizationId,
                    ResearchActivity_DimOrganization_OrganizationId = ffv.DimProfileOnlyResearchActivity.DimOrganizationId > 0 ? ffv.DimProfileOnlyResearchActivity.DimOrganization.OrganizationId : null,
                    ResearchActivity_DimOrganization_NameFi = ffv.DimProfileOnlyResearchActivity.DimOrganizationId > 0 ? ffv.DimProfileOnlyResearchActivity.DimOrganization.NameFi : null,
                    ResearchActivity_DimOrganization_NameEn = ffv.DimProfileOnlyResearchActivity.DimOrganizationId > 0 ? ffv.DimProfileOnlyResearchActivity.DimOrganization.NameEn : null,
                    ResearchActivity_DimOrganization_NameSv = ffv.DimProfileOnlyResearchActivity.DimOrganizationId > 0 ? ffv.DimProfileOnlyResearchActivity.DimOrganization.NameSv : null,
                    ResearchActivity_DimOrganization_DimSector_SectorId = ffv.DimProfileOnlyResearchActivity.DimOrganizationId > 0 ? ffv.DimProfileOnlyResearchActivity.DimOrganization.DimSector.SectorId : null,
                    ResearchActivity_DimOrganization_DimSector_NameFi = ffv.DimProfileOnlyResearchActivity.DimOrganizationId > 0 ? ffv.DimProfileOnlyResearchActivity.DimOrganization.DimSector.NameFi : null,
                    ResearchActivity_DimOrganization_DimSector_NameEn = ffv.DimProfileOnlyResearchActivity.DimOrganizationId > 0 ? ffv.DimProfileOnlyResearchActivity.DimOrganization.DimSector.NameEn : null,
                    ResearchActivity_DimOrganization_DimSector_NameSv = ffv.DimProfileOnlyResearchActivity.DimOrganizationId > 0 ? ffv.DimProfileOnlyResearchActivity.DimOrganization.DimSector.NameSv : null,
                    ResearchActivity_DimOrganizationBroader_Id = ffv.DimProfileOnlyResearchActivity.DimOrganizationId > 0 && ffv.DimProfileOnlyResearchActivity.DimOrganization.DimOrganizationBroader.HasValue ? ffv.DimProfileOnlyResearchActivity.DimOrganization.DimOrganizationBroader.Value : null,
                    ResearchActivity_DimOrganizationBroader_NameFi = ffv.DimProfileOnlyResearchActivity.DimOrganizationId > 0 && ffv.DimProfileOnlyResearchActivity.DimOrganization.DimOrganizationBroader.HasValue ? ffv.DimProfileOnlyResearchActivity.DimOrganization.DimOrganizationBroaderNavigation.NameFi : null,
                    ResearchActivity_DimOrganizationBroader_NameEn = ffv.DimProfileOnlyResearchActivity.DimOrganizationId > 0 && ffv.DimProfileOnlyResearchActivity.DimOrganization.DimOrganizationBroader.HasValue ? ffv.DimProfileOnlyResearchActivity.DimOrganization.DimOrganizationBroaderNavigation.NameEn : null,
                    ResearchActivity_DimOrganizationBroader_NameSv = ffv.DimProfileOnlyResearchActivity.DimOrganizationId > 0 && ffv.DimProfileOnlyResearchActivity.DimOrganization.DimOrganizationBroader.HasValue ? ffv.DimProfileOnlyResearchActivity.DimOrganization.DimOrganizationBroaderNavigation.NameSv : null,
                    ResearchActivity_DimOrganizationBroader_DimSector_NameFi = ffv.DimProfileOnlyResearchActivity.DimOrganizationId > 0 && ffv.DimProfileOnlyResearchActivity.DimOrganization.DimOrganizationBroader.HasValue ? ffv.DimProfileOnlyResearchActivity.DimOrganization.DimOrganizationBroaderNavigation.DimSector.NameFi : null,
                    ResearchActivity_DimOrganizationBroader_DimSector_NameEn = ffv.DimProfileOnlyResearchActivity.DimOrganizationId > 0 && ffv.DimProfileOnlyResearchActivity.DimOrganization.DimOrganizationBroader.HasValue ? ffv.DimProfileOnlyResearchActivity.DimOrganization.DimOrganizationBroaderNavigation.DimSector.NameEn : null,
                    ResearchActivity_DimOrganizationBroader_DimSector_NameSv = ffv.DimProfileOnlyResearchActivity.DimOrganizationId > 0 && ffv.DimProfileOnlyResearchActivity.DimOrganization.DimOrganizationBroader.HasValue ? ffv.DimProfileOnlyResearchActivity.DimOrganization.DimOrganizationBroaderNavigation.DimSector.NameSv : null,
                    // DimIdentifierlessData_Id = ffv.DimIdentifierlessDataId,
                    // DimIdentifierlessData_Type = ffv.DimIdentifierlessData.Type,
                    // DimIdentifierlessData_ValueFi = ffv.DimIdentifierlessData.ValueFi,
                    // DimIdentifierlessData_ValueEn = ffv.DimIdentifierlessData.ValueEn,
                    // DimIdentifierlessData_ValueSv = ffv.DimIdentifierlessData.ValueSv,
                    // DimIdentifierlessData_Child_Type = ffv.DimIdentifierlessData.InverseDimIdentifierlessData.FirstOrDefault().Type,
                    // DimIdentifierlessData_Child_ValueFi = ffv.DimIdentifierlessData.InverseDimIdentifierlessData.FirstOrDefault().ValueFi,
                    // DimIdentifierlessData_Child_ValueEn = ffv.DimIdentifierlessData.InverseDimIdentifierlessData.FirstOrDefault().ValueEn,
                    // DimIdentifierlessData_Child_ValueSv = ffv.DimIdentifierlessData.InverseDimIdentifierlessData.FirstOrDefault().ValueSv,
                    ResearchActivity_ActivityType_CodeValue = ffv.DimReferencedataActorRole.CodeValue, // Activity type from DimReferencedata via FactFieldValue.
                    ResearchActivity_ActivityType_NameFi = ffv.DimReferencedataActorRole.NameFi,
                    ResearchActivity_ActivityType_NameEn = ffv.DimReferencedataActorRole.NameEn,
                    ResearchActivity_ActivityType_NameSv = ffv.DimReferencedataActorRole.NameSv,
                    ResearchActivity_Role_CodeValue = "", // Role not available in DimProfileOnlyResearchActivity.
                    ResearchActivity_Role_NameFi = "",
                    ResearchActivity_Role_NameEn = "",
                    ResearchActivity_Role_NameSv = "",
                    ResearchActivity_ActivityType_From_Role_Parent_CodeValue = "", // Role parent not available in DimProfileOnlyResearchActivity.
                    ResearchActivity_ActivityType_From_Role_Parent_NameFi = "",
                    ResearchActivity_ActivityType_From_Role_Parent_NameEn = "",
                    ResearchActivity_ActivityType_From_Role_Parent_NameSv = "",
                    ResearchActivity_StartDate_Day = ffv.DimProfileOnlyResearchActivity.DimDateIdStartNavigation != null ? ffv.DimProfileOnlyResearchActivity.DimDateIdStartNavigation.Day : 0,
                    ResearchActivity_StartDate_Month = ffv.DimProfileOnlyResearchActivity.DimDateIdStartNavigation != null ? ffv.DimProfileOnlyResearchActivity.DimDateIdStartNavigation.Month : 0,
                    ResearchActivity_StartDate_Year = ffv.DimProfileOnlyResearchActivity.DimDateIdStartNavigation != null ? ffv.DimProfileOnlyResearchActivity.DimDateIdStartNavigation.Year : 0,
                    ResearchActivity_EndDate_Day = ffv.DimProfileOnlyResearchActivity.DimDateIdEndNavigation != null ? ffv.DimProfileOnlyResearchActivity.DimDateIdEndNavigation.Day : 0,
                    ResearchActivity_EndDate_Month = ffv.DimProfileOnlyResearchActivity.DimDateIdEndNavigation != null ? ffv.DimProfileOnlyResearchActivity.DimDateIdEndNavigation.Month : 0,
                    ResearchActivity_EndDate_Year = ffv.DimProfileOnlyResearchActivity.DimDateIdEndNavigation != null ? ffv.DimProfileOnlyResearchActivity.DimDateIdEndNavigation.Year : 0,
                    ResearchActivity_NameFi = ffv.DimProfileOnlyResearchActivity.NameFi,
                    ResearchActivity_NameEn = ffv.DimProfileOnlyResearchActivity.NameEn,
                    ResearchActivity_NameSv = ffv.DimProfileOnlyResearchActivity.NameSv,
                    ResearchActivity_DescriptionFi = ffv.DimProfileOnlyResearchActivity.DescriptionFi,
                    ResearchActivity_DescriptionEn = ffv.DimProfileOnlyResearchActivity.DescriptionEn,
                    ResearchActivity_DescriptionSv = ffv.DimProfileOnlyResearchActivity.DescriptionSv,
                    ResearchActivity_InternationalCollaboration = null, // Not available in DimProfileOnlyResearchActivity.
                    ResearchActivity_DimWebLink_Url = ffv.DimProfileOnlyResearchActivity.DimWebLinks.FirstOrDefault().Url,
                    WebLinks = ffv.DimProfileOnlyResearchActivity.DimWebLinks.Select(wl => new ProfileEditorWebLink_WithoutItemMeta()
                    {
                        Url = wl.Url,
                        LinkLabel = wl.LinkLabel,
                        LinkType = wl.LinkType
                    }).ToList() 
                }).AsNoTracking().ToListAsync();
            stopwatch_profileOnlyResearchActivityDtos.Stop();
            _logger.LogInformation($"GetProfileEditorResearchActivities. SQL query for profileOnlyResearchActivityDtos got {profileOnlyResearchActivityDtos.Count} items and took {stopwatch_profileOnlyResearchActivityDtos.ElapsedMilliseconds}ms.");

            /*
             * Research activity deduplication.
             * Deduplication is based on start year and name properties.
             * Remove items from profileOnlyResearchActivityDtos which duplicate items from researchActivityDtos.
             * Comparison is done by computing a key for each research activity based on start year and translated name FI, and comparing the keys.
             */
            var stopwatch_deduplicateDtos = Stopwatch.StartNew();
            HashSet<string> uniqueKeys = new();
            foreach (ResearchActivityDto researchActivityDto in researchActivityDtos)
            {
                NameTranslation nameTraslationResearchActivity_Name = _languageService.GetNameTranslation(
                    nameFi: researchActivityDto.ResearchActivity_NameFi,
                    nameEn: researchActivityDto.ResearchActivity_NameEn,
                    nameSv: researchActivityDto.ResearchActivity_NameSv
                );

                uniqueKeys.Add(
                    ComputeKey(
                        startYear: researchActivityDto.ResearchActivity_StartDate_Year,
                        nameFi: nameTraslationResearchActivity_Name.NameFi,
                        nameEn: nameTraslationResearchActivity_Name.NameEn,
                        nameSv: nameTraslationResearchActivity_Name.NameSv
                    )
                );
            }
            foreach (ResearchActivityDto profileOnlyResearchActivityDto in profileOnlyResearchActivityDtos)
            {
                NameTranslation nameTraslationProfileOnlyResearchActivity_Name = _languageService.GetNameTranslation(
                    nameFi: profileOnlyResearchActivityDto.ResearchActivity_NameFi,
                    nameEn: profileOnlyResearchActivityDto.ResearchActivity_NameEn,
                    nameSv: profileOnlyResearchActivityDto.ResearchActivity_NameSv
                );

                string key = ComputeKey(
                    startYear: profileOnlyResearchActivityDto.ResearchActivity_StartDate_Year,
                    nameFi: nameTraslationProfileOnlyResearchActivity_Name.NameFi,
                    nameEn: nameTraslationProfileOnlyResearchActivity_Name.NameEn,
                    nameSv: nameTraslationProfileOnlyResearchActivity_Name.NameSv
                );

                if (uniqueKeys.Contains(key))
                {
                    // Duplicate found, mark profile only research activity as duplicate.
                    profileOnlyResearchActivityDto.IsDuplicate = true;
                    break;
                }
            }
            _logger.LogInformation($"GetProfileEditorResearchActivities. Deduplication took {stopwatch_deduplicateDtos.ElapsedMilliseconds}ms.");

            /*
             * Process DTOs
             * Source:
             *   - researchActivityDtos
             *   - profileOnlyResearchActivityDtos (after removing duplicates)
             * Destination:
             *   - List<ProfileEditorActivityAndReward>
             */
            var stopwatch_processDtos = Stopwatch.StartNew();
            List<ProfileEditorActivityAndReward> activitiesAndRewards = new();
            foreach (ResearchActivityDto dto in researchActivityDtos.Concat(profileOnlyResearchActivityDtos).Where(d => !d.IsDuplicate))
            {
                // Research activity organization search order:
                // 1. DimResearchActivity_DimOrganizationBroader_Id
                // 2. DimResearchActivity_DimOrganization_Id
                // 3. DimIdentifierlessData

                NameTranslation nameTranslationResearchActivityOrganization = new();
                NameTranslation nameTranslationResearchActivityOrganizationSector = new();
                NameTranslation nameTranslationResearchActivityDepartment = new();

                // Organization name
                if (dto.ResearchActivity_DimOrganizationBroader_Id > 0)
                {
                    nameTranslationResearchActivityOrganization = _languageService.GetNameTranslation(
                        nameFi: dto.ResearchActivity_DimOrganizationBroader_NameFi,
                        nameEn: dto.ResearchActivity_DimOrganizationBroader_NameEn,
                        nameSv: dto.ResearchActivity_DimOrganizationBroader_NameSv
                    );

                    nameTranslationResearchActivityOrganizationSector = _languageService.GetNameTranslation(
                        nameFi: dto.ResearchActivity_DimOrganizationBroader_DimSector_NameFi,
                        nameEn: dto.ResearchActivity_DimOrganizationBroader_DimSector_NameEn,
                        nameSv: dto.ResearchActivity_DimOrganizationBroader_DimSector_NameSv
                    );
                }
                else if (dto.ResearchActivity_DimOrganization_Id > 0)
                {
                    nameTranslationResearchActivityOrganization = _languageService.GetNameTranslation(
                        nameFi: dto.ResearchActivity_DimOrganization_NameFi,
                        nameEn: dto.ResearchActivity_DimOrganization_NameEn,
                        nameSv: dto.ResearchActivity_DimOrganization_NameSv
                    );

                    nameTranslationResearchActivityOrganizationSector = _languageService.GetNameTranslation(
                        nameFi: dto.ResearchActivity_DimOrganization_DimSector_NameFi,
                        nameEn: dto.ResearchActivity_DimOrganization_DimSector_NameEn,
                        nameSv: dto.ResearchActivity_DimOrganization_DimSector_NameSv
                    );
                }
                else if (dto.DimIdentifierlessData_Id > -1 &&
                    dto.DimIdentifierlessData_Type == Constants.IdentifierlessDataTypes.ORGANIZATION_NAME)
                {
                    nameTranslationResearchActivityOrganization = _languageService.GetNameTranslation(
                        nameFi: dto.DimIdentifierlessData_ValueFi,
                        nameEn: dto.DimIdentifierlessData_ValueEn,
                        nameSv: dto.DimIdentifierlessData_ValueSv
                    );
                }

                // Department name
                if (dto.ResearchActivity_DimOrganizationBroader_Id > 0)
                {
                    // When DimOrganizationBroader is available, it contains the organization name and DimOrganization contains department name.
                    nameTranslationResearchActivityDepartment = _languageService.GetNameTranslation(
                        nameFi: dto.ResearchActivity_DimOrganization_NameFi,
                        nameEn: dto.ResearchActivity_DimOrganization_NameEn,
                        nameSv: dto.ResearchActivity_DimOrganization_NameSv
                    );
                }
                else if (dto.DimIdentifierlessData_Type != null && dto.DimIdentifierlessData_Type == Constants.IdentifierlessDataTypes.ORGANIZATION_UNIT)
                {
                    nameTranslationResearchActivityDepartment = _languageService.GetNameTranslation(
                        nameFi: dto.DimIdentifierlessData_ValueFi,
                        nameEn: dto.DimIdentifierlessData_ValueEn,
                        nameSv: dto.DimIdentifierlessData_ValueSv
                    );
                }
                else if (dto.DimIdentifierlessData_Child_Type != null && dto.DimIdentifierlessData_Child_Type == Constants.IdentifierlessDataTypes.ORGANIZATION_UNIT)
                {
                    nameTranslationResearchActivityDepartment = _languageService.GetNameTranslation(
                        nameFi: dto.DimIdentifierlessData_Child_ValueFi,
                        nameEn: dto.DimIdentifierlessData_Child_ValueEn,
                        nameSv: dto.DimIdentifierlessData_Child_ValueSv
                    );
                }


                NameTranslation nameTraslationResearchActivityName = _languageService.GetNameTranslation(
                    nameFi: dto.ResearchActivity_NameFi,
                    nameEn: dto.ResearchActivity_NameEn,
                    nameSv: dto.ResearchActivity_NameSv
                );
                NameTranslation nameTraslationResearchActivityDescription = _languageService.GetNameTranslation(
                    nameFi: dto.ResearchActivity_DescriptionFi,
                    nameEn: dto.ResearchActivity_DescriptionEn,
                    nameSv: dto.ResearchActivity_DescriptionSv
                );
                NameTranslation nameTraslationResearchActivityTypeName = _languageService.GetNameTranslation(
                    nameFi: dto.ResearchActivity_ActivityType_NameFi,
                    nameEn: dto.ResearchActivity_ActivityType_NameEn,
                    nameSv: dto.ResearchActivity_ActivityType_NameSv
                );
                NameTranslation nameTraslationResearchActivityRoleName = _languageService.GetNameTranslation(
                    nameFi: dto.ResearchActivity_Role_NameFi,
                    nameEn: dto.ResearchActivity_Role_NameEn,
                    nameSv: dto.ResearchActivity_Role_NameSv
                );
                NameTranslation nameTraslationResearchActivityTypeFromRoleParentName = _languageService.GetNameTranslation(
                    nameFi: dto.ResearchActivity_ActivityType_From_Role_Parent_NameFi,
                    nameEn: dto.ResearchActivity_ActivityType_From_Role_Parent_NameEn,
                    nameSv: dto.ResearchActivity_ActivityType_From_Role_Parent_NameSv
                );

                // Name translation for registered data source organization name
                NameTranslation nameTranslationRegisteredDataSourceOrganization = _languageService.GetNameTranslation(
                    nameFi: dto.DimRegisteredDatasource_DimOrganization_NameFi,
                    nameEn: dto.DimRegisteredDatasource_DimOrganization_NameEn,
                    nameSv: dto.DimRegisteredDatasource_DimOrganization_NameSv
                );

                ProfileEditorActivityAndReward activityAndReward = new()
                {
                    NameFi = nameTraslationResearchActivityName.NameFi,
                    NameEn = nameTraslationResearchActivityName.NameEn,
                    NameSv = nameTraslationResearchActivityName.NameSv,
                    DescriptionFi = nameTraslationResearchActivityDescription.NameFi,
                    DescriptionEn = nameTraslationResearchActivityDescription.NameEn,
                    DescriptionSv = nameTraslationResearchActivityDescription.NameSv,
                    InternationalCollaboration = dto.ResearchActivity_InternationalCollaboration,
                    StartDate = new ProfileEditorDate()
                    {
                        Year = dto.ResearchActivity_StartDate_Year,
                        Month = dto.ResearchActivity_StartDate_Month,
                        Day = dto.ResearchActivity_StartDate_Day
                    },
                    EndDate = new ProfileEditorDate()
                    {
                        Year = dto.ResearchActivity_EndDate_Year,
                        Month = dto.ResearchActivity_EndDate_Month,
                        Day = dto.ResearchActivity_EndDate_Day
                    },
                    // If activity type is not defined, use values from role parent
                    ActivityTypeCode = !string.IsNullOrWhiteSpace(dto.ResearchActivity_ActivityType_CodeValue)
                        ? dto.ResearchActivity_ActivityType_CodeValue
                        : dto.ResearchActivity_ActivityType_From_Role_Parent_CodeValue,
                    ActivityTypeNameFi = !string.IsNullOrWhiteSpace(nameTraslationResearchActivityTypeName.NameFi)
                        ? nameTraslationResearchActivityTypeName.NameFi
                        : nameTraslationResearchActivityTypeFromRoleParentName.NameFi,
                    ActivityTypeNameEn = !string.IsNullOrWhiteSpace(nameTraslationResearchActivityTypeName.NameEn)
                        ? nameTraslationResearchActivityTypeName.NameEn
                        : nameTraslationResearchActivityTypeFromRoleParentName.NameEn,
                    ActivityTypeNameSv = !string.IsNullOrWhiteSpace(nameTraslationResearchActivityTypeName.NameSv)
                        ? nameTraslationResearchActivityTypeName.NameSv
                        : nameTraslationResearchActivityTypeFromRoleParentName.NameSv,
                    RoleCode = dto.ResearchActivity_Role_CodeValue,
                    RoleNameFi = nameTraslationResearchActivityRoleName.NameFi,
                    RoleNameEn = nameTraslationResearchActivityRoleName.NameEn,
                    RoleNameSv = nameTraslationResearchActivityRoleName.NameSv,
                    OrganizationNameFi = nameTranslationResearchActivityOrganization.NameFi,
                    OrganizationNameEn = nameTranslationResearchActivityOrganization.NameEn,
                    OrganizationNameSv = nameTranslationResearchActivityOrganization.NameSv,
                    DepartmentNameFi = nameTranslationResearchActivityDepartment.NameFi,
                    DepartmentNameEn = nameTranslationResearchActivityDepartment.NameEn,
                    DepartmentNameSv = nameTranslationResearchActivityDepartment.NameSv,
                    itemMeta = new ProfileEditorItemMeta(
                        id: dto.Id,
                        type: dto.IsProfileOnlyResearchActivity ? Constants.ItemMetaTypes.ACTIVITY_RESEARCH_ACTIVITY_PROFILE_ONLY : Constants.ItemMetaTypes.ACTIVITY_RESEARCH_ACTIVITY,
                        show: dto.Show,
                        primaryValue: dto.PrimaryValue
                    ),
                    DataSources = new List<ProfileEditorSource>
                    {
                        new ProfileEditorSource() {
                            Id = dto.DimRegisteredDatasource_Id,
                            RegisteredDataSource = dto.DimRegisteredDatasource_Name,
                            Organization = new Organization() {
                                NameFi = nameTranslationRegisteredDataSourceOrganization.NameFi,
                                NameEn = nameTranslationRegisteredDataSourceOrganization.NameEn,
                                NameSv = nameTranslationRegisteredDataSourceOrganization.NameSv,
                                SectorId = dto.DimRegisteredDatasource_DimOrganization_DimSector_SectorId
                            }
                        }
                    },
                    Url = dto.ResearchActivity_DimWebLink_Url,
                    WebLinks = dto.WebLinks
                };

                // Add Elasticsearch person index related data.
                if (forElasticsearch && !String.IsNullOrWhiteSpace(dto.ResearchActivity_DimOrganization_DimSector_SectorId))
                {
                    activityAndReward.sector = new List<ProfileEditorSector>
                    {
                        new ProfileEditorSector()
                        {
                            sectorId = dto.ResearchActivity_DimOrganization_DimSector_SectorId,
                            nameFiSector = nameTranslationResearchActivityOrganizationSector.NameFi,
                            nameEnSector = nameTranslationResearchActivityOrganizationSector.NameEn,
                            nameSvSector = nameTranslationResearchActivityOrganizationSector.NameSv,
                            organization = new List<ProfileEditorSectorOrganization>() {
                                new ProfileEditorSectorOrganization()
                                {
                                    organizationId = dto.ResearchActivity_DimOrganization_OrganizationId,
                                    OrganizationNameFi = nameTranslationResearchActivityOrganization.NameFi,
                                    OrganizationNameEn = nameTranslationResearchActivityOrganization.NameEn,
                                    OrganizationNameSv = nameTranslationResearchActivityOrganization.NameSv
                                }
                            }
                        }
                    };
                }
                activitiesAndRewards.Add(activityAndReward);
            }
            stopwatch_processDtos.Stop();
            _logger.LogInformation($"GetProfileEditorResearchActivities. Processing DTOs took {stopwatch_processDtos.ElapsedMilliseconds}ms.");

            stopwatch.Stop();
            _logger.LogInformation($"GetProfileEditorResearchActivities. {activitiesAndRewards.Count} items in {stopwatch.ElapsedMilliseconds}ms.");

            return activitiesAndRewards;
        }
    }
}