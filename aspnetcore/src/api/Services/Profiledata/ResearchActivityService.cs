using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.Models.Ttv;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using api.Models.Common;
using api.Models.ProfileEditor.Items;
using System;

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

        public class ResearchActivityDto
        {
            public bool IsProfileOnlyResearchActivity { get; set; }
            public int Id { get; set; }
            public bool? Show { get; set; }
            public bool? PrimaryValue { get; set; }
            public int DimRegisteredDatasource_Id { get; set; }
            public string? DimRegisteredDatasource_Name { get; set; }
            public string? DimRegisteredDatasource_DimOrganization_NameFi { get; set; }
            public string? DimRegisteredDatasource_DimOrganization_NameEn { get; set; }
            public string? DimRegisteredDatasource_DimOrganization_NameSv { get; set; }
            public string? DimRegisteredDatasource_DimOrganization_DimSector_SectorId { get; set; }
            public int DimResearchActivity_DimOrganization_Id { get; set; }
            public string DimResearchActivity_DimOrganization_NameFi { get; set; }
            public string DimResearchActivity_DimOrganization_NameEn { get; set; }
            public string DimResearchActivity_DimOrganization_NameSv { get; set; }
            public string DimResearchActivity_DimOrganization_DimSector_NameFi { get; set; }
            public string DimResearchActivity_DimOrganization_DimSector_NameEn { get; set; }
            public string DimResearchActivity_DimOrganization_DimSector_NameSv { get; set; }
            public int? DimResearchActivity_DimOrganizationBroader_Id { get; set; }
            public string DimResearchActivity_DimOrganizationBroader_NameFi { get; set; }
            public string DimResearchActivity_DimOrganizationBroader_NameEn { get; set; }
            public string DimResearchActivity_DimOrganizationBroader_NameSv { get; set; }
            public string DimResearchActivity_DimOrganizationBroader_DimSector_NameFi { get; set; }
            public string DimResearchActivity_DimOrganizationBroader_DimSector_NameEn { get; set; }
            public string DimResearchActivity_DimOrganizationBroader_DimSector_NameSv { get; set; }
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
            public string DimResearchActivity_ActivityType_CodeValue { get; set; }
            public string DimResearchActivity_ActivityType_NameFi { get; set; }
            public string DimResearchActivity_ActivityType_NameEn { get; set; }
            public string DimResearchActivity_ActivityType_NameSv { get; set; }
            public string DimResearchActivity_Role_CodeValue { get; set; }
            public string DimResearchActivity_Role_NameFi { get; set; }
            public string DimResearchActivity_Role_NameEn { get; set; }
            public string DimResearchActivity_Role_NameSv { get; set; }
            public string DimResearchActivity_ActivityType_From_Role_Parent_CodeValue { get; set; }
            public string DimResearchActivity_ActivityType_From_Role_Parent_NameFi { get; set; }
            public string DimResearchActivity_ActivityType_From_Role_Parent_NameEn { get; set; }
            public string DimResearchActivity_ActivityType_From_Role_Parent_NameSv { get; set; }
            public string DimResearchActivity_NameFi { get; set; }
            public string DimResearchActivity_NameEn { get; set; }
            public string DimResearchActivity_NameSv { get; set; }
            public string DimResearchActivity_DescriptionFi { get; set; }
            public string DimResearchActivity_DescriptionEn { get; set; }
            public string DimResearchActivity_DescriptionSv { get; set; }
            public int DimResearchActivity_StartDate_Year { get; set; }
            public int DimResearchActivity_StartDate_Month { get; set; }
            public int DimResearchActivity_StartDate_Day { get; set; }
            public int DimResearchActivity_EndDate_Year { get; set; }
            public int DimResearchActivity_EndDate_Month { get; set; }
            public int DimResearchActivity_EndDate_Day { get; set; }
            public bool? DimResearchActivity_InternationalCollaboration { get; set; }
            public string DimResearchActivity_DimWebLink_Url { get; set; }
            public List<ProfileEditorWebLink_WithoutItemMeta> WebLinks { get; set; }
        }

        /*
         * Research activities.
         */
        public async Task<List<ProfileEditorActivityAndReward>> GetProfileEditorActiviesAndRewards(int userprofileId, bool forElasticsearch = false)
        {
            /*
             * DimResearchActivity => DTO
             */
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
                    DimResearchActivity_DimOrganization_Id = ffv.DimResearchActivity.DimOrganizationId,
                    DimResearchActivity_DimOrganization_NameFi = ffv.DimResearchActivity.DimOrganizationId > 0 ? ffv.DimResearchActivity.DimOrganization.NameFi : null,
                    DimResearchActivity_DimOrganization_NameEn = ffv.DimResearchActivity.DimOrganizationId > 0 ? ffv.DimResearchActivity.DimOrganization.NameEn : null,
                    DimResearchActivity_DimOrganization_NameSv = ffv.DimResearchActivity.DimOrganizationId > 0 ? ffv.DimResearchActivity.DimOrganization.NameSv : null,
                    DimResearchActivity_DimOrganization_DimSector_NameFi = ffv.DimResearchActivity.DimOrganizationId > 0 ? ffv.DimResearchActivity.DimOrganization.DimSector.NameFi : null,
                    DimResearchActivity_DimOrganization_DimSector_NameEn = ffv.DimResearchActivity.DimOrganizationId > 0 ? ffv.DimResearchActivity.DimOrganization.DimSector.NameEn : null,
                    DimResearchActivity_DimOrganization_DimSector_NameSv = ffv.DimResearchActivity.DimOrganizationId > 0 ? ffv.DimResearchActivity.DimOrganization.DimSector.NameSv : null,
                    DimResearchActivity_DimOrganizationBroader_Id = ffv.DimResearchActivity.DimOrganizationId > 0 && ffv.DimResearchActivity.DimOrganization.DimOrganizationBroader.HasValue ? ffv.DimResearchActivity.DimOrganization.DimOrganizationBroader.Value : null,
                    DimResearchActivity_DimOrganizationBroader_NameFi = ffv.DimResearchActivity.DimOrganizationId > 0 && ffv.DimResearchActivity.DimOrganization.DimOrganizationBroader.HasValue ? ffv.DimResearchActivity.DimOrganization.DimOrganizationBroaderNavigation.NameFi : null,
                    DimResearchActivity_DimOrganizationBroader_NameEn = ffv.DimResearchActivity.DimOrganizationId > 0 && ffv.DimResearchActivity.DimOrganization.DimOrganizationBroader.HasValue ? ffv.DimResearchActivity.DimOrganization.DimOrganizationBroaderNavigation.NameEn : null,
                    DimResearchActivity_DimOrganizationBroader_NameSv = ffv.DimResearchActivity.DimOrganizationId > 0 && ffv.DimResearchActivity.DimOrganization.DimOrganizationBroader.HasValue ? ffv.DimResearchActivity.DimOrganization.DimOrganizationBroaderNavigation.NameSv : null,
                    DimResearchActivity_DimOrganizationBroader_DimSector_NameFi = ffv.DimResearchActivity.DimOrganizationId > 0 && ffv.DimResearchActivity.DimOrganization.DimOrganizationBroader.HasValue ? ffv.DimResearchActivity.DimOrganization.DimOrganizationBroaderNavigation.DimSector.NameFi : null,
                    DimResearchActivity_DimOrganizationBroader_DimSector_NameEn = ffv.DimResearchActivity.DimOrganizationId > 0 && ffv.DimResearchActivity.DimOrganization.DimOrganizationBroader.HasValue ? ffv.DimResearchActivity.DimOrganization.DimOrganizationBroaderNavigation.DimSector.NameEn : null,
                    DimResearchActivity_DimOrganizationBroader_DimSector_NameSv = ffv.DimResearchActivity.DimOrganizationId > 0 && ffv.DimResearchActivity.DimOrganization.DimOrganizationBroader.HasValue ? ffv.DimResearchActivity.DimOrganization.DimOrganizationBroaderNavigation.DimSector.NameSv : null,
                    DimIdentifierlessData_Id = ffv.DimIdentifierlessDataId,
                    DimIdentifierlessData_Type = ffv.DimIdentifierlessData.Type,
                    DimIdentifierlessData_ValueFi = ffv.DimIdentifierlessData.ValueFi,
                    DimIdentifierlessData_ValueEn = ffv.DimIdentifierlessData.ValueEn,
                    DimIdentifierlessData_ValueSv = ffv.DimIdentifierlessData.ValueSv,
                    DimIdentifierlessData_Child_Type = ffv.DimIdentifierlessData.DimIdentifierlessData.Type,
                    DimIdentifierlessData_Child_ValueFi = ffv.DimIdentifierlessData.DimIdentifierlessData.ValueFi,
                    DimIdentifierlessData_Child_ValueEn = ffv.DimIdentifierlessData.DimIdentifierlessData.ValueEn,
                    DimIdentifierlessData_Child_ValueSv = ffv.DimIdentifierlessData.DimIdentifierlessData.ValueSv,
                    DimResearchActivity_StartDate_Month = ffv.DimResearchActivity.DimStartDateNavigation != null ? ffv.DimResearchActivity.DimStartDateNavigation.Month : 0,
                    DimResearchActivity_StartDate_Year = ffv.DimResearchActivity.DimStartDateNavigation != null ? ffv.DimResearchActivity.DimStartDateNavigation.Year : 0,
                    DimResearchActivity_EndDate_Day = ffv.DimResearchActivity.DimEndDateNavigation != null ? ffv.DimResearchActivity.DimEndDateNavigation.Day : 0,
                    DimResearchActivity_EndDate_Month = ffv.DimResearchActivity.DimEndDateNavigation != null ? ffv.DimResearchActivity.DimEndDateNavigation.Month : 0,
                    DimResearchActivity_EndDate_Year = ffv.DimResearchActivity.DimEndDateNavigation != null ? ffv.DimResearchActivity.DimEndDateNavigation.Year : 0,
                    DimResearchActivity_NameFi = ffv.DimResearchActivity.NameFi,
                    DimResearchActivity_NameEn = ffv.DimResearchActivity.NameEn,
                    DimResearchActivity_NameSv = ffv.DimResearchActivity.NameSv,
                    DimResearchActivity_DescriptionFi = ffv.DimResearchActivity.DescriptionFi,
                    DimResearchActivity_DescriptionEn = ffv.DimResearchActivity.DescriptionEn,
                    DimResearchActivity_DescriptionSv = ffv.DimResearchActivity.DescriptionSv,
                    DimResearchActivity_InternationalCollaboration = ffv.DimResearchActivity.InternationalCollaboration,
                    DimResearchActivity_DimWebLink_Url = ffv.DimResearchActivity.DimWebLinks.FirstOrDefault().Url,
                    WebLinks = ffv.DimResearchActivity.DimWebLinks.Select(wl => new ProfileEditorWebLink_WithoutItemMeta()
                    {
                        Url = wl.Url,
                        LinkLabel = wl.LinkLabel,
                        LinkType = wl.LinkType
                    }).ToList() 
                }).AsNoTracking().ToListAsync();

            /*
             * DimProfileOnlyResearchActivity => DTO
             */
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
                    DimRegisteredDatasource_DimOrganization_DimSector_SectorId = ffv.DimRegisteredDataSource.DimOrganization.DimSector.SectorId
                }).AsNoTracking().ToListAsync();

            List<ProfileEditorActivityAndReward> activitiesAndRewards = new();

            /*
             * Process DimResearchActivity DTOs
             */
            foreach (ResearchActivityDto researchActivityDto in researchActivityDtos)
            {
                // Research activity organization search order:
                // 1. DimResearchActivity_DimOrganizationBroader_Id
                // 2. DimResearchActivity_DimOrganization_Id
                // 3. DimIdentifierlessData

                NameTranslation nameTranslationResearchActivityOrganization = new();
                NameTranslation nameTranslationResearchActivityOrganizationSector = new();
                NameTranslation nameTranslationResearchActivityDepartment = new();

                // Organization name
                if (researchActivityDto.DimResearchActivity_DimOrganizationBroader_Id > 0)
                {
                    nameTranslationResearchActivityOrganization = _languageService.GetNameTranslation(
                        nameFi: researchActivityDto.DimResearchActivity_DimOrganizationBroader_NameFi,
                        nameEn: researchActivityDto.DimResearchActivity_DimOrganizationBroader_NameEn,
                        nameSv: researchActivityDto.DimResearchActivity_DimOrganizationBroader_NameSv
                    );

                    nameTranslationResearchActivityOrganizationSector = _languageService.GetNameTranslation(
                        nameFi: researchActivityDto.DimResearchActivity_DimOrganizationBroader_DimSector_NameFi,
                        nameEn: researchActivityDto.DimResearchActivity_DimOrganizationBroader_DimSector_NameEn,
                        nameSv: researchActivityDto.DimResearchActivity_DimOrganizationBroader_DimSector_NameSv
                    );
                }
                else if (researchActivityDto.DimResearchActivity_DimOrganization_Id > 0)
                {
                    nameTranslationResearchActivityOrganization = _languageService.GetNameTranslation(
                        nameFi: researchActivityDto.DimResearchActivity_DimOrganization_NameFi,
                        nameEn: researchActivityDto.DimResearchActivity_DimOrganization_NameEn,
                        nameSv: researchActivityDto.DimResearchActivity_DimOrganization_NameSv
                    );

                    nameTranslationResearchActivityOrganizationSector = _languageService.GetNameTranslation(
                        nameFi: researchActivityDto.DimResearchActivity_DimOrganization_DimSector_NameFi,
                        nameEn: researchActivityDto.DimResearchActivity_DimOrganization_DimSector_NameEn,
                        nameSv: researchActivityDto.DimResearchActivity_DimOrganization_DimSector_NameSv
                    );
                }
                else if (researchActivityDto.DimIdentifierlessData_Id > -1 &&
                    researchActivityDto.DimIdentifierlessData_Type == Constants.IdentifierlessDataTypes.ORGANIZATION_NAME)
                {
                    nameTranslationResearchActivityOrganization = _languageService.GetNameTranslation(
                        nameFi: researchActivityDto.DimIdentifierlessData_ValueFi,
                        nameEn: researchActivityDto.DimIdentifierlessData_ValueEn,
                        nameSv: researchActivityDto.DimIdentifierlessData_ValueSv
                    );
                }

                // Department name
                if (researchActivityDto.DimResearchActivity_DimOrganizationBroader_Id > 0)
                {
                    // When DimOrganizationBroader is available, it contains the organization name and DimOrganization contains department name.
                    nameTranslationResearchActivityDepartment = _languageService.GetNameTranslation(
                        nameFi: researchActivityDto.DimResearchActivity_DimOrganization_NameFi,
                        nameEn: researchActivityDto.DimResearchActivity_DimOrganization_NameEn,
                        nameSv: researchActivityDto.DimResearchActivity_DimOrganization_NameSv
                    );
                }
                else if (researchActivityDto.DimIdentifierlessData_Type != null && researchActivityDto.DimIdentifierlessData_Type == Constants.IdentifierlessDataTypes.ORGANIZATION_UNIT)
                {
                    nameTranslationResearchActivityDepartment = _languageService.GetNameTranslation(
                        nameFi: researchActivityDto.DimIdentifierlessData_ValueFi,
                        nameEn: researchActivityDto.DimIdentifierlessData_ValueEn,
                        nameSv: researchActivityDto.DimIdentifierlessData_ValueSv
                    );
                }
                else if (researchActivityDto.DimIdentifierlessData_Child_Type != null && researchActivityDto.DimIdentifierlessData_Child_Type == Constants.IdentifierlessDataTypes.ORGANIZATION_UNIT)
                {
                    nameTranslationResearchActivityDepartment = _languageService.GetNameTranslation(
                        nameFi: researchActivityDto.DimIdentifierlessData_Child_ValueFi,
                        nameEn: researchActivityDto.DimIdentifierlessData_Child_ValueEn,
                        nameSv: researchActivityDto.DimIdentifierlessData_Child_ValueSv
                    );
                }


                NameTranslation nameTraslationResearchActivityName = _languageService.GetNameTranslation(
                    nameFi: researchActivityDto.DimResearchActivity_NameFi,
                    nameEn: researchActivityDto.DimResearchActivity_NameEn,
                    nameSv: researchActivityDto.DimResearchActivity_NameSv
                );
                NameTranslation nameTraslationResearchActivityDescription = _languageService.GetNameTranslation(
                    nameFi: researchActivityDto.DimResearchActivity_DescriptionFi,
                    nameEn: researchActivityDto.DimResearchActivity_DescriptionEn,
                    nameSv: researchActivityDto.DimResearchActivity_DescriptionSv
                );
                NameTranslation nameTraslationResearchActivityTypeName = _languageService.GetNameTranslation(
                    nameFi: researchActivityDto.DimResearchActivity_ActivityType_NameFi,
                    nameEn: researchActivityDto.DimResearchActivity_ActivityType_NameEn,
                    nameSv: researchActivityDto.DimResearchActivity_ActivityType_NameSv
                );
                NameTranslation nameTraslationResearchActivityRoleName = _languageService.GetNameTranslation(
                    nameFi: researchActivityDto.DimResearchActivity_Role_NameFi,
                    nameEn: researchActivityDto.DimResearchActivity_Role_NameEn,
                    nameSv: researchActivityDto.DimResearchActivity_Role_NameSv
                );
                NameTranslation nameTraslationResearchActivityTypeFromRoleParentName = _languageService.GetNameTranslation(
                    nameFi: researchActivityDto.DimResearchActivity_ActivityType_From_Role_Parent_NameFi,
                    nameEn: researchActivityDto.DimResearchActivity_ActivityType_From_Role_Parent_NameEn,
                    nameSv: researchActivityDto.DimResearchActivity_ActivityType_From_Role_Parent_NameSv
                );

                ProfileEditorActivityAndReward activityAndReward = new()
                {
                    NameFi = nameTraslationResearchActivityName.NameFi,
                    NameEn = nameTraslationResearchActivityName.NameEn,
                    NameSv = nameTraslationResearchActivityName.NameSv,
                    DescriptionFi = nameTraslationResearchActivityDescription.NameFi,
                    DescriptionEn = nameTraslationResearchActivityDescription.NameEn,
                    DescriptionSv = nameTraslationResearchActivityDescription.NameSv,
                    InternationalCollaboration = researchActivityDto.DimResearchActivity_InternationalCollaboration,
                    StartDate = new ProfileEditorDate()
                    {
                        Year = researchActivityDto.DimResearchActivity_StartDate_Year,
                        Month = researchActivityDto.DimResearchActivity_StartDate_Month,
                        Day = researchActivityDto.DimResearchActivity_StartDate_Day
                    },
                    EndDate = new ProfileEditorDate()
                    {
                        Year = researchActivityDto.DimResearchActivity_EndDate_Year,
                        Month = researchActivityDto.DimResearchActivity_EndDate_Month,
                        Day = researchActivityDto.DimResearchActivity_EndDate_Day
                    },
                    // If activity type is not defined, use values from role parent
                    ActivityTypeCode = !string.IsNullOrWhiteSpace(researchActivityDto.DimResearchActivity_ActivityType_CodeValue)
                        ? researchActivityDto.DimResearchActivity_ActivityType_CodeValue
                        : researchActivityDto.DimResearchActivity_ActivityType_From_Role_Parent_CodeValue,
                    ActivityTypeNameFi = !string.IsNullOrWhiteSpace(nameTraslationResearchActivityTypeName.NameFi)
                        ? nameTraslationResearchActivityTypeName.NameFi
                        : nameTraslationResearchActivityTypeFromRoleParentName.NameFi,
                    ActivityTypeNameEn = !string.IsNullOrWhiteSpace(nameTraslationResearchActivityTypeName.NameEn)
                        ? nameTraslationResearchActivityTypeName.NameEn
                        : nameTraslationResearchActivityTypeFromRoleParentName.NameEn,
                    ActivityTypeNameSv = !string.IsNullOrWhiteSpace(nameTraslationResearchActivityTypeName.NameSv)
                        ? nameTraslationResearchActivityTypeName.NameSv
                        : nameTraslationResearchActivityTypeFromRoleParentName.NameSv,
                    RoleCode = researchActivityDto.DimResearchActivity_Role_CodeValue,
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
                        id: researchActivityDto.Id,
                        type: Constants.ItemMetaTypes.ACTIVITY_RESEARCH_ACTIVITY,
                        show: researchActivityDto.Show,
                        primaryValue: researchActivityDto.PrimaryValue
                    ),
                    DataSources = new List<ProfileEditorSource>
                    {
                        new ProfileEditorSource() {
                            Id = researchActivityDto.DimRegisteredDatasource_Id,
                            RegisteredDataSource = researchActivityDto.DimRegisteredDatasource_Name,
                            Organization = new Organization() {
                                NameFi = researchActivityDto.DimRegisteredDatasource_DimOrganization_NameFi,
                                NameEn = researchActivityDto.DimRegisteredDatasource_DimOrganization_NameEn,
                                NameSv = researchActivityDto.DimRegisteredDatasource_DimOrganization_NameSv,
                                SectorId = researchActivityDto.DimRegisteredDatasource_DimOrganization_DimSector_SectorId
                            }
                        }
                    },
                    Url = researchActivityDto.DimResearchActivity_DimWebLink_Url,
                    WebLinks = researchActivityDto.WebLinks
                };

                // // Add Elasticsearch person index related data.
                // if (forElasticsearch && !String.IsNullOrWhiteSpace(researchActivityDto.DimResearchActivity_DimOrganization_DimSector_SectorId))
                // {
                //     activityAndReward.sector = new List<ProfileEditorSector>
                //     {
                //         new ProfileEditorSector()
                //         {
                //             sectorId = p.DimResearchActivity_DimOrganization_DimSector_SectorId,
                //             nameFiSector = nameTranslationResearchActivityOrganizationSector.NameFi,
                //             nameEnSector = nameTranslationResearchActivityOrganizationSector.NameEn,
                //             nameSvSector = nameTranslationResearchActivityOrganizationSector.NameSv,
                //             organization = new List<ProfileEditorSectorOrganization>() {
                //                 new ProfileEditorSectorOrganization()
                //                 {
                //                     organizationId = p.DimResearchActivity_DimOrganization_OrganizationId,
                //                     OrganizationNameFi = nameTranslationResearchActivityOrganization.NameFi,
                //                     OrganizationNameEn = nameTranslationResearchActivityOrganization.NameEn,
                //                     OrganizationNameSv = nameTranslationResearchActivityOrganization.NameSv
                //                 }
                //             }
                //         }
                //     };
                // }
                activitiesAndRewards.Add(activityAndReward);
            }

            /*
             * Process DimProfileOnlyResearchActivity DTOs
             */
            foreach (ResearchActivityDto researchActivityDto in profileOnlyResearchActivityDtos)
            {
            }

            return activitiesAndRewards;
        }
    }
}