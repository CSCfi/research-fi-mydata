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
    public class AffiliationService : IAffiliationService
    {
        private readonly TtvContext _ttvContext;
        private readonly ILanguageService _languageService;
        private readonly IUtilityService _utilityService;
        private readonly ILogger<AffiliationService> _logger;


        public AffiliationService(
            TtvContext ttvContext,
            ILanguageService languageService,
            IUtilityService utilityService,
            ILogger<AffiliationService> logger)
        {
            _ttvContext = ttvContext;
            _languageService = languageService;
            _utilityService = utilityService;
            _logger = logger;
        }

        // Affiliation query DTO class
        public class AffiliationDto
        {
            public int DimAffiliationId { get; set; }
            public bool? Show { get; set; }
            public bool? PrimaryValue { get; set; }
            public int DimRegisteredDatasource_Id { get; set; }
            public string? DimRegisteredDatasource_Name { get; set; }
            public string? DimRegisteredDatasource_DimOrganization_NameFi { get; set; }
            public string? DimRegisteredDatasource_DimOrganization_NameEn { get; set; }
            public string? DimRegisteredDatasource_DimOrganization_NameSv { get; set; }
            public string? DimregisteredDatasource_DimOrganization_DimSector_SectorId { get; set; }
            public int? DimOrganizationBroader_Id { get; set; }
            public string? DimOrganizationBroader_NameFi { get; set; }
            public string? DimOrganizationBroader_NameEn { get; set; }
            public string? DimOrganizationBroader_NameSv { get; set; }
            public string? DimOrganizationBroader_DimSector_SectorId { get; set; }
            public string? DimOrganizationBroader_DimSector_NameFi { get; set; }
            public string? DimOrganizationBroader_DimSector_NameEn { get; set; }
            public string? DimOrganizationBroader_DimSector_NameSv { get; set; }
            public int? DimOrganization_Id { get; set; }
            public string? DimOrganization_OrganizationId { get; set; }
            public string? DimOrganization_NameFi { get; set; }
            public string? DimOrganization_NameEn { get; set; }
            public string? DimOrganization_NameSv { get; set; }
            public string? DimOrganization_DimSector_SectorId { get; set; }
            public string? DimOrganization_DimSector_NameFi { get; set; }
            public string? DimOrganization_DimSector_NameEn { get; set; }
            public string? DimOrganization_DimSector_NameSv { get; set; }
            public int DimIdentifierlessData_Id { get; set; }
            public string? DimIdentifierlessData_Type { get; set; }
            public string? DimIdentifierlessData_ValueFi { get; set; }
            public string? DimIdentifierlessData_ValueEn { get; set; }
            public string? DimIdentifierlessData_ValueSv { get; set; }
            public string? DimIdentifierlessData_Child_Type { get; set; }
            public string? DimIdentifierlessData_Child_ValueFi { get; set; }
            public string? DimIdentifierlessData_Child_ValueEn { get; set; }
            public string? DimIdentifierlessData_Child_ValueSv { get; set; }
            public string? DimAffiliation_PositionNameFi { get; set; }
            public string? DimAffiliation_PositionNameEn { get; set; }
            public string? DimAffiliation_PositionNameSv { get; set; }
            public string? DimAffiliation_AffiliationTypeFi { get; set; }
            public string? DimAffiliation_AffiliationTypeEn { get; set; }
            public string? DimAffiliation_AffiliationTypeSv { get; set; }
            public int StartDate_Year { get; set; }
            public int StartDate_Month { get; set; }
            public int StartDate_Day { get; set; }
            public int EndDate_Year { get; set; }
            public int EndDate_Month { get; set; }
            public int EndDate_Day { get; set; }
        }

        /*
         * Affiliations
         */
        public async Task<List<ProfileEditorAffiliation>> GetProfileEditorAffiliations(int userprofileId, bool forElasticsearch = false)
        {
            var stopwatch = Stopwatch.StartNew();
            List<AffiliationDto> affiliationDtos = await _ttvContext.FactFieldValues.Where(ffv => ffv.DimUserProfileId == userprofileId
                && ffv.DimAffiliationId > 0
                && ffv.DimFieldDisplaySettings.FieldIdentifier == Constants.FieldIdentifiers.ACTIVITY_AFFILIATION
                && (forElasticsearch ? ffv.Show == true : true))
                .Select(ffv => new AffiliationDto()
                {
                    DimAffiliationId = ffv.DimAffiliationId,
                    Show = ffv.Show,
                    PrimaryValue = ffv.PrimaryValue,
                    DimRegisteredDatasource_Id = ffv.DimRegisteredDataSourceId,
                    DimRegisteredDatasource_Name = ffv.DimRegisteredDataSource.Name,
                    DimRegisteredDatasource_DimOrganization_NameFi = ffv.DimRegisteredDataSource.DimOrganization.NameFi,
                    DimRegisteredDatasource_DimOrganization_NameEn = ffv.DimRegisteredDataSource.DimOrganization.NameEn,
                    DimRegisteredDatasource_DimOrganization_NameSv = ffv.DimRegisteredDataSource.DimOrganization.NameSv,
                    DimregisteredDatasource_DimOrganization_DimSector_SectorId = ffv.DimRegisteredDataSource.DimOrganization.DimSector.SectorId,
                    DimOrganizationBroader_Id = ffv.DimAffiliation.DimOrganization.DimOrganizationBroader,
                    DimOrganizationBroader_NameFi = ffv.DimAffiliation.DimOrganization.DimOrganizationBroaderNavigation.NameFi,
                    DimOrganizationBroader_NameEn = ffv.DimAffiliation.DimOrganization.DimOrganizationBroaderNavigation.NameEn,
                    DimOrganizationBroader_NameSv = ffv.DimAffiliation.DimOrganization.DimOrganizationBroaderNavigation.NameSv,
                    DimOrganizationBroader_DimSector_SectorId = ffv.DimAffiliation.DimOrganization.DimOrganizationBroaderNavigation.DimSector.SectorId,
                    DimOrganizationBroader_DimSector_NameFi = ffv.DimAffiliation.DimOrganization.DimOrganizationBroaderNavigation.DimSector.NameFi,
                    DimOrganizationBroader_DimSector_NameEn = ffv.DimAffiliation.DimOrganization.DimOrganizationBroaderNavigation.DimSector.NameEn,
                    DimOrganizationBroader_DimSector_NameSv = ffv.DimAffiliation.DimOrganization.DimOrganizationBroaderNavigation.DimSector.NameSv,
                    DimOrganization_Id = ffv.DimAffiliation.DimOrganizationId,
                    DimOrganization_OrganizationId = ffv.DimAffiliation.DimOrganization.OrganizationId,
                    DimOrganization_NameFi = ffv.DimAffiliation.DimOrganization.NameFi,
                    DimOrganization_NameEn = ffv.DimAffiliation.DimOrganization.NameEn,
                    DimOrganization_NameSv = ffv.DimAffiliation.DimOrganization.NameSv,
                    DimOrganization_DimSector_SectorId = ffv.DimAffiliation.DimOrganization.DimSector.SectorId,
                    DimOrganization_DimSector_NameFi = ffv.DimAffiliation.DimOrganization.DimSector.NameFi,
                    DimOrganization_DimSector_NameEn = ffv.DimAffiliation.DimOrganization.DimSector.NameEn,
                    DimOrganization_DimSector_NameSv = ffv.DimAffiliation.DimOrganization.DimSector.NameSv,
                    DimIdentifierlessData_Id = ffv.DimIdentifierlessDataId,
                    DimIdentifierlessData_Type = ffv.DimIdentifierlessData.Type,
                    DimIdentifierlessData_ValueFi = ffv.DimIdentifierlessData.ValueFi,
                    DimIdentifierlessData_ValueEn = ffv.DimIdentifierlessData.ValueEn,
                    DimIdentifierlessData_ValueSv = ffv.DimIdentifierlessData.ValueSv,
                    DimIdentifierlessData_Child_Type = null, // To improve query performance, child values are batch loaded later, see below.
                    DimIdentifierlessData_Child_ValueFi = null,
                    DimIdentifierlessData_Child_ValueEn = null,
                    DimIdentifierlessData_Child_ValueSv = null,
                    DimAffiliation_PositionNameFi = ffv.DimAffiliation.PositionNameFi,
                    DimAffiliation_PositionNameEn = ffv.DimAffiliation.PositionNameEn,
                    DimAffiliation_PositionNameSv = ffv.DimAffiliation.PositionNameSv,
                    DimAffiliation_AffiliationTypeFi = ffv.DimAffiliation.AffiliationTypeFi,
                    DimAffiliation_AffiliationTypeEn = ffv.DimAffiliation.AffiliationTypeEn,
                    DimAffiliation_AffiliationTypeSv = ffv.DimAffiliation.AffiliationTypeSv,
                    StartDate_Year = ffv.DimAffiliation.StartDateNavigation.Year > 1900 ? ffv.DimAffiliation.StartDateNavigation.Year : 0,
                    StartDate_Month = ffv.DimAffiliation.StartDateNavigation.Year > 1900 ? ffv.DimAffiliation.StartDateNavigation.Month : 0,
                    StartDate_Day = ffv.DimAffiliation.StartDateNavigation.Year > 1900 ? ffv.DimAffiliation.StartDateNavigation.Day : 0,
                    EndDate_Year = ffv.DimAffiliation.EndDateNavigation.Year > 1900 ? ffv.DimAffiliation.EndDateNavigation.Year : 0,
                    EndDate_Month = ffv.DimAffiliation.EndDateNavigation.Year > 1900 ? ffv.DimAffiliation.EndDateNavigation.Month : 0,
                    EndDate_Day = ffv.DimAffiliation.EndDateNavigation.Year > 1900 ? ffv.DimAffiliation.EndDateNavigation.Day : 0
                }).AsNoTracking().ToListAsync();

            /*
            * Batch load child identifierless data.
            * One query replaces 4 correlated subqueries per row across both DTO lists.
            */
            List<int> parentIds = affiliationDtos
                .Where(d => d.DimIdentifierlessData_Id > 0)
                .Select(d => d.DimIdentifierlessData_Id)
                .Distinct()
                .ToList();
            if (parentIds.Count > 0)
            {
                var childRows = await _ttvContext.DimIdentifierlessData
                    .Where(c => c.DimIdentifierlessDataId.HasValue && parentIds.Contains(c.DimIdentifierlessDataId.Value))
                    .OrderBy(c => c.Id)
                    .Select(c => new { c.DimIdentifierlessDataId, c.Type, c.ValueFi, c.ValueEn, c.ValueSv })
                    .AsNoTracking()
                    .ToListAsync();

                Dictionary<int, (string Type, string ValueFi, string ValueEn, string ValueSv)> childByParentId = new();
                foreach (var child in childRows)
                {
                    if (child.DimIdentifierlessDataId.HasValue && !childByParentId.ContainsKey(child.DimIdentifierlessDataId.Value))
                        childByParentId[child.DimIdentifierlessDataId.Value] = (child.Type, child.ValueFi, child.ValueEn, child.ValueSv);
                }

                foreach (AffiliationDto dto in affiliationDtos)
                {
                    if (dto.DimIdentifierlessData_Id > 0 && childByParentId.TryGetValue(dto.DimIdentifierlessData_Id, out var child))
                    {
                        dto.DimIdentifierlessData_Child_Type = child.Type;
                        dto.DimIdentifierlessData_Child_ValueFi = child.ValueFi;
                        dto.DimIdentifierlessData_Child_ValueEn = child.ValueEn;
                        dto.DimIdentifierlessData_Child_ValueSv = child.ValueSv;
                    }
                }
            }

            List<ProfileEditorAffiliation> affiliations = new List<ProfileEditorAffiliation>();
            foreach (AffiliationDto affiliationDto in affiliationDtos)
            {
                /*
                 * Affiliation organization search order:
                 * 1. DimAffiliation_DimOrganizationBroader_Id
                 * 2. DimAffiliation_DimOrganization_Id
                 * 3. DimIdentifierlessData
                 */

                NameTranslation nameTranslationAffiliationOrganization = new();
                NameTranslation nameTranslationAffiliationOrganizationSector = new();
                NameTranslation nameTranslationAffiliationDepartment = new();

                // Organization name
                if (affiliationDto.DimOrganizationBroader_Id > 0)
                {
                    nameTranslationAffiliationOrganization = _languageService.GetNameTranslation(
                        nameFi: affiliationDto.DimOrganizationBroader_NameFi,
                        nameEn: affiliationDto.DimOrganizationBroader_NameEn,
                        nameSv: affiliationDto.DimOrganizationBroader_NameSv
                    );

                    nameTranslationAffiliationOrganizationSector = _languageService.GetNameTranslation(
                        nameFi: affiliationDto.DimOrganizationBroader_DimSector_NameFi,
                        nameEn: affiliationDto.DimOrganizationBroader_DimSector_NameEn,
                        nameSv: affiliationDto.DimOrganizationBroader_DimSector_NameSv
                    );
                }
                else if (affiliationDto.DimOrganization_Id > 0)
                {
                    nameTranslationAffiliationOrganization = _languageService.GetNameTranslation(
                        nameFi: affiliationDto.DimOrganization_NameFi,
                        nameEn: affiliationDto.DimOrganization_NameEn,
                        nameSv: affiliationDto.DimOrganization_NameSv
                    );

                    nameTranslationAffiliationOrganizationSector = _languageService.GetNameTranslation(
                        nameFi: affiliationDto.DimOrganization_DimSector_NameFi,
                        nameEn: affiliationDto.DimOrganization_DimSector_NameEn,
                        nameSv: affiliationDto.DimOrganization_DimSector_NameSv
                    );
                }
                else if (affiliationDto.DimIdentifierlessData_Id > -1 &&
                    affiliationDto.DimIdentifierlessData_Type == Constants.IdentifierlessDataTypes.ORGANIZATION_NAME)
                {
                    nameTranslationAffiliationOrganization = _languageService.GetNameTranslation(
                        nameFi: affiliationDto.DimIdentifierlessData_ValueFi,
                        nameEn: affiliationDto.DimIdentifierlessData_ValueEn,
                        nameSv: affiliationDto.DimIdentifierlessData_ValueSv
                    );
                }

                // Department name
                if (affiliationDto.DimOrganizationBroader_Id > 0)
                {
                    // When DimOrganizationBroader is available, it contains the organization name and DimOrganization contains department name.
                    nameTranslationAffiliationDepartment = _languageService.GetNameTranslation(
                        nameFi: affiliationDto.DimOrganization_NameFi,
                        nameEn: affiliationDto.DimOrganization_NameEn,
                        nameSv: affiliationDto.DimOrganization_NameSv
                    );
                }
                else if (affiliationDto.DimIdentifierlessData_Type != null && affiliationDto.DimIdentifierlessData_Type == Constants.IdentifierlessDataTypes.ORGANIZATION_UNIT)
                {
                    nameTranslationAffiliationDepartment = _languageService.GetNameTranslation(
                        nameFi: affiliationDto.DimIdentifierlessData_ValueFi,
                        nameEn: affiliationDto.DimIdentifierlessData_ValueEn,
                        nameSv: affiliationDto.DimIdentifierlessData_ValueSv
                    );
                }
                else if (affiliationDto.DimIdentifierlessData_Child_Type != null && affiliationDto.DimIdentifierlessData_Child_Type == Constants.IdentifierlessDataTypes.ORGANIZATION_UNIT)
                {
                    nameTranslationAffiliationDepartment = _languageService.GetNameTranslation(
                        nameFi: affiliationDto.DimIdentifierlessData_Child_ValueFi,
                        nameEn: affiliationDto.DimIdentifierlessData_Child_ValueEn,
                        nameSv: affiliationDto.DimIdentifierlessData_Child_ValueSv
                    );
                }

                // Name translation for position name
                NameTranslation nameTranslationPositionName = _languageService.GetNameTranslation(
                    nameFi: affiliationDto.DimAffiliation_PositionNameFi,
                    nameEn: affiliationDto.DimAffiliation_PositionNameEn,
                    nameSv: affiliationDto.DimAffiliation_PositionNameSv
                );

                // Name translation for affiliation type
                NameTranslation nameTranslationAffiliationType = _languageService.GetNameTranslation(
                    nameFi: affiliationDto.DimAffiliation_AffiliationTypeFi,
                    nameEn: affiliationDto.DimAffiliation_AffiliationTypeEn,
                    nameSv: affiliationDto.DimAffiliation_AffiliationTypeSv
                );

                // Name translation for registered data source organization name
                NameTranslation nameTranslationRegisteredDataSourceOrganization = _languageService.GetNameTranslation(
                    nameFi: affiliationDto.DimRegisteredDatasource_DimOrganization_NameFi,
                    nameEn: affiliationDto.DimRegisteredDatasource_DimOrganization_NameEn,
                    nameSv: affiliationDto.DimRegisteredDatasource_DimOrganization_NameSv
                );

                ProfileEditorAffiliation affiliation = new ProfileEditorAffiliation()
                {
                    OrganizationNameFi = nameTranslationAffiliationOrganization.NameFi,
                    OrganizationNameEn = nameTranslationAffiliationOrganization.NameEn,
                    OrganizationNameSv = nameTranslationAffiliationOrganization.NameSv,
                    DepartmentNameFi = nameTranslationAffiliationDepartment.NameFi,
                    DepartmentNameEn = nameTranslationAffiliationDepartment.NameEn,
                    DepartmentNameSv = nameTranslationAffiliationDepartment.NameSv,
                    PositionNameFi = _utilityService.CapitalizeFirstLetter(nameTranslationPositionName.NameFi),
                    PositionNameEn = _utilityService.CapitalizeFirstLetter(nameTranslationPositionName.NameEn),
                    PositionNameSv = _utilityService.CapitalizeFirstLetter(nameTranslationPositionName.NameSv),
                    AffiliationTypeFi = nameTranslationAffiliationType.NameFi,
                    AffiliationTypeEn = nameTranslationAffiliationType.NameEn,
                    AffiliationTypeSv = nameTranslationAffiliationType.NameSv,
                    StartDate = new ProfileEditorDate() {
                        Year = affiliationDto.StartDate_Year,
                        Month = affiliationDto.StartDate_Month,
                        Day = affiliationDto.StartDate_Day
                    },
                    EndDate = new ProfileEditorDate() {
                        Year = affiliationDto.EndDate_Year,
                        Month = affiliationDto.EndDate_Month,
                        Day = affiliationDto.EndDate_Day
                    },
                    itemMeta = new ProfileEditorItemMeta(
                        id: affiliationDto.DimAffiliationId,
                        type: Constants.ItemMetaTypes.ACTIVITY_AFFILIATION,
                        show: affiliationDto.Show,
                        primaryValue: affiliationDto.PrimaryValue
                    ),
                    DataSources = new List<ProfileEditorSource> {
                        new ProfileEditorSource() {
                            Id = affiliationDto.DimRegisteredDatasource_Id,
                            RegisteredDataSource = affiliationDto.DimRegisteredDatasource_Name,
                            Organization = new Organization() {
                                NameFi = nameTranslationRegisteredDataSourceOrganization.NameFi,
                                NameEn = nameTranslationRegisteredDataSourceOrganization.NameEn,
                                NameSv = nameTranslationRegisteredDataSourceOrganization.NameSv,
                                SectorId = affiliationDto.DimregisteredDatasource_DimOrganization_DimSector_SectorId
                            }
                        }
                    }
                };

                // Add Elasticsearch person index related data
                if (forElasticsearch && !String.IsNullOrWhiteSpace(affiliationDto.DimOrganization_DimSector_SectorId))
                {
                    affiliation.sector = new List<ProfileEditorSector>
                    {
                        new ProfileEditorSector()
                        {
                            sectorId = affiliationDto.DimOrganization_DimSector_SectorId,
                            nameFiSector = nameTranslationAffiliationOrganizationSector.NameFi,
                            nameEnSector = nameTranslationAffiliationOrganizationSector.NameEn,
                            nameSvSector = nameTranslationAffiliationOrganizationSector.NameSv,
                            organization = new List<ProfileEditorSectorOrganization>() {
                                new ProfileEditorSectorOrganization()
                                {
                                    organizationId = affiliationDto.DimOrganization_OrganizationId,
                                    OrganizationNameFi = nameTranslationAffiliationOrganization.NameFi,
                                    OrganizationNameEn = nameTranslationAffiliationOrganization.NameEn,
                                    OrganizationNameSv = nameTranslationAffiliationOrganization.NameSv
                                }
                            }
                        }
                    };
                }
                affiliations.Add(affiliation);
            }

            stopwatch.Stop();
            if (stopwatch.ElapsedMilliseconds > Constants.LoggingParameters.SLOW_OPERATION_MS_THRESHOLD)
            {
                _logger.LogWarning($"GetProfileEditorAffiliations is slow. userprofileId={userprofileId}, forElasticsearch={forElasticsearch}, {affiliations.Count} items in {stopwatch.ElapsedMilliseconds}ms.");
            }
            
            return affiliations;
        }
    }
}