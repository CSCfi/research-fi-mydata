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
    public class FundingDecisionService : IFundingDecisionService
    {
        private readonly TtvContext _ttvContext;
        private readonly ILanguageService _languageService;
        private readonly ILogger<FundingDecisionService> _logger;


        public FundingDecisionService(
            TtvContext ttvContext,
            ILanguageService languageService,
            ILogger<FundingDecisionService> logger)
        {
            _ttvContext = ttvContext;
            _languageService = languageService;
            _logger = logger;
        }

        public class FundingDecisionDto
        {
            public bool IsProfileOnlyFundingDecision { get; set; }
            public int Id { get; set; }
            public bool? Show { get; set; }
            public bool? PrimaryValue { get; set; }
            public int DimRegisteredDatasource_Id { get; set; }
            public string? DimRegisteredDatasource_Name { get; set; }
            public string? DimRegisteredDatasource_DimOrganization_NameFi { get; set; }
            public string? DimRegisteredDatasource_DimOrganization_NameEn { get; set; }
            public string? DimRegisteredDatasource_DimOrganization_NameSv { get; set; }
            public string? DimregisteredDatasource_DimOrganization_DimSector_SectorId { get; set; }
            public string Acronym { get; set; } = string.Empty;
            public decimal AmountInEur { get; set; }
            public decimal? AmountInFundingDecisionCurrency { get; set; } = null;
            public string CallProgramNameFi { get; set; } = string.Empty;
            public string CallProgramNameEn { get; set; } = string.Empty;
            public string CallProgramNameSv { get; set; } = string.Empty;
            public string DescriptionFi { get; set; } = string.Empty;
            public string DescriptionEn { get; set; } = string.Empty;
            public string DescriptionSv { get; set; } = string.Empty;
            public string FunderProjectNumber { get; set; } = string.Empty;
            public string? FundingDecisionCurrencyAbbreviation { get; set; } = null;
            public int? FundingEndYear { get; set; }
            public int? FundingStartYear { get; set; }
            public string FunderNameFi { get; set; } = string.Empty;
            public string FunderNameEn { get; set; } = string.Empty;
            public string FunderNameSv { get; set; } = string.Empty;
            public string NameFi { get; set; } = string.Empty;
            public string NameEn { get; set; } = string.Empty;
            public string NameSv { get; set; } = string.Empty;
            public string TypeOfFundingNameFi { get; set; } = string.Empty;
            public string TypeOfFundingNameEn { get; set; } = string.Empty;
            public string TypeOfFundingNameSv { get; set; } = string.Empty;
            public string Url { get; set; } = string.Empty;
            public int? DimProfileOnlyFundingDecision_DimOrganization_Id { get; set; } = -1;
            public string DimProfileOnlyFundingDecision_DimOrganization_NameFi { get; set; } = string.Empty;
            public string DimProfileOnlyFundingDecision_DimOrganization_NameEn { get; set; } = string.Empty;
            public string DimProfileOnlyFundingDecision_DimOrganization_NameSv { get; set; } = string.Empty;
            public int FactFieldValues_DimIdentifierlessDataId { get; set; } = -1;
            public string FactFieldValues_DimIdentifierlessData_Type { get; set; } = string.Empty;
            public string FactFieldValues_DimIdentifierlessData_ValueFi { get; set; } = string.Empty;
            public string FactFieldValues_DimIdentifierlessData_ValueEn { get; set; } = string.Empty;
            public string FactFieldValues_DimIdentifierlessData_ValueSv { get; set; } = string.Empty;
        }

        /*
         * Funding Decisions
         */
        public async Task<List<ProfileEditorFundingDecision>> GetProfileEditorFundingDecisions(int userprofileId, bool forElasticsearch = false)
        {
            /*
             * DimFundingDecision => DTO
             */
            List<FundingDecisionDto> fundingDecisionsDtos = await _ttvContext.FactFieldValues.Where(ffv => ffv.DimUserProfileId == userprofileId
                && ffv.DimFundingDecisionId > 0
                && ffv.DimFieldDisplaySettings.FieldIdentifier == Constants.FieldIdentifiers.ACTIVITY_FUNDING_DECISION
                && (forElasticsearch ? ffv.Show == true : true))
                .Select(ffv => new FundingDecisionDto()
                {
                    IsProfileOnlyFundingDecision = false,
                    Id = ffv.DimFundingDecisionId,
                    Show = ffv.Show,
                    PrimaryValue = ffv.PrimaryValue,
                    DimRegisteredDatasource_Id = ffv.DimRegisteredDataSourceId,
                    DimRegisteredDatasource_Name = ffv.DimRegisteredDataSource.Name,
                    DimRegisteredDatasource_DimOrganization_NameFi = ffv.DimRegisteredDataSource.DimOrganization.NameFi,
                    DimRegisteredDatasource_DimOrganization_NameEn = ffv.DimRegisteredDataSource.DimOrganization.NameEn,
                    DimRegisteredDatasource_DimOrganization_NameSv = ffv.DimRegisteredDataSource.DimOrganization.NameSv,
                    DimregisteredDatasource_DimOrganization_DimSector_SectorId = ffv.DimRegisteredDataSource.DimOrganization.DimSector.SectorId,
                    Acronym = ffv.DimFundingDecision.Acronym,
                    AmountInEur = ffv.DimFundingDecision.AmountInEur,
                    CallProgramNameFi = ffv.DimFundingDecision.DimCallProgramme.NameFi,
                    CallProgramNameEn = ffv.DimFundingDecision.DimCallProgramme.NameEn,
                    CallProgramNameSv = ffv.DimFundingDecision.DimCallProgramme.NameSv,
                    DescriptionFi = ffv.DimFundingDecision.DescriptionFi,
                    DescriptionEn = ffv.DimFundingDecision.DescriptionEn,
                    DescriptionSv = ffv.DimFundingDecision.DescriptionSv,
                    FunderProjectNumber = ffv.DimFundingDecision.FunderProjectNumber,
                    FundingEndYear = ffv.DimFundingDecision.DimDateIdEndNavigation.Year,
                    FundingStartYear = ffv.DimFundingDecision.DimDateIdStartNavigation.Year,
                    FunderNameFi = ffv.DimFundingDecision.DimOrganizationIdFunderNavigation.NameFi,
                    FunderNameEn = ffv.DimFundingDecision.DimOrganizationIdFunderNavigation.NameEn,
                    FunderNameSv = ffv.DimFundingDecision.DimOrganizationIdFunderNavigation.NameSv,
                    NameFi = ffv.DimFundingDecision.NameFi,
                    NameEn = ffv.DimFundingDecision.NameEn,
                    NameSv = ffv.DimFundingDecision.NameSv,
                    TypeOfFundingNameFi = ffv.DimFundingDecision.DimTypeOfFunding.NameFi,
                    TypeOfFundingNameEn = ffv.DimFundingDecision.DimTypeOfFunding.NameEn,
                    TypeOfFundingNameSv = ffv.DimFundingDecision.DimTypeOfFunding.NameSv,
                }).AsNoTracking().ToListAsync();

            /*
             * DimProfileOnlyFundingDecision => DTO
             */
            List<FundingDecisionDto> profileOnlyFundingDecisionsDtos = await _ttvContext.FactFieldValues.Where(ffv => ffv.DimUserProfileId == userprofileId
                && ffv.DimProfileOnlyFundingDecisionId > 0
                && ffv.DimFieldDisplaySettings.FieldIdentifier == Constants.FieldIdentifiers.ACTIVITY_FUNDING_DECISION
                && (forElasticsearch ? ffv.Show == true : true))
                .Select(ffv => new FundingDecisionDto()
                {
                    IsProfileOnlyFundingDecision = true,
                    Id = ffv.DimProfileOnlyFundingDecisionId,
                    Show = ffv.Show,
                    PrimaryValue = ffv.PrimaryValue,
                    DimRegisteredDatasource_Id = ffv.DimRegisteredDataSourceId,
                    DimRegisteredDatasource_Name = ffv.DimRegisteredDataSource.Name,
                    DimRegisteredDatasource_DimOrganization_NameFi = ffv.DimRegisteredDataSource.DimOrganization.NameFi,
                    DimRegisteredDatasource_DimOrganization_NameEn = ffv.DimRegisteredDataSource.DimOrganization.NameEn,
                    DimRegisteredDatasource_DimOrganization_NameSv = ffv.DimRegisteredDataSource.DimOrganization.NameSv,
                    DimregisteredDatasource_DimOrganization_DimSector_SectorId = ffv.DimRegisteredDataSource.DimOrganization.DimSector.SectorId,
                    Acronym = ffv.DimProfileOnlyFundingDecision.Acronym,
                    AmountInEur = ffv.DimProfileOnlyFundingDecision.AmountInEur,
                    AmountInFundingDecisionCurrency = ffv.DimProfileOnlyFundingDecision.AmountInFundingDecisionCurrency,
                    DescriptionFi = ffv.DimProfileOnlyFundingDecision.DescriptionFi,
                    DescriptionEn = ffv.DimProfileOnlyFundingDecision.DescriptionEn,
                    DescriptionSv = ffv.DimProfileOnlyFundingDecision.DescriptionSv,
                    FundingDecisionCurrencyAbbreviation = ffv.DimProfileOnlyFundingDecision.FundingDecisionCurrencyAbbreviation,
                    FunderProjectNumber = ffv.DimProfileOnlyFundingDecision.FunderProjectNumber,
                    FundingEndYear = ffv.DimProfileOnlyFundingDecision.DimDateIdEndNavigation.Year,
                    FundingStartYear = ffv.DimProfileOnlyFundingDecision.DimDateIdStartNavigation.Year,
                    NameFi = ffv.DimProfileOnlyFundingDecision.NameFi,
                    NameEn = ffv.DimProfileOnlyFundingDecision.NameEn,
                    NameSv = ffv.DimProfileOnlyFundingDecision.NameSv,
                    TypeOfFundingNameFi = ffv.DimReferencedataActorRole.NameFi,
                    TypeOfFundingNameEn = ffv.DimReferencedataActorRole.NameEn,
                    TypeOfFundingNameSv = ffv.DimReferencedataActorRole.NameSv,
                    Url = ffv.DimProfileOnlyFundingDecision.DimWebLinks.Count > 0 ? ffv.DimProfileOnlyFundingDecision.DimWebLinks.FirstOrDefault().Url : string.Empty,
                    DimProfileOnlyFundingDecision_DimOrganization_Id = ffv.DimProfileOnlyFundingDecision.DimOrganizationIdFunder > 0 ? ffv.DimProfileOnlyFundingDecision.DimOrganizationIdFunder : -1,
                    DimProfileOnlyFundingDecision_DimOrganization_NameFi = ffv.DimProfileOnlyFundingDecision.DimOrganizationIdFunder != null && ffv.DimProfileOnlyFundingDecision.DimOrganizationIdFunder > 0 ? ffv.DimProfileOnlyFundingDecision.DimOrganizationIdFunderNavigation.NameFi : string.Empty,
                    DimProfileOnlyFundingDecision_DimOrganization_NameEn = ffv.DimProfileOnlyFundingDecision.DimOrganizationIdFunder != null && ffv.DimProfileOnlyFundingDecision.DimOrganizationIdFunder > 0 ? ffv.DimProfileOnlyFundingDecision.DimOrganizationIdFunderNavigation.NameEn : string.Empty,
                    DimProfileOnlyFundingDecision_DimOrganization_NameSv = ffv.DimProfileOnlyFundingDecision.DimOrganizationIdFunder != null && ffv.DimProfileOnlyFundingDecision.DimOrganizationIdFunder > 0 ? ffv.DimProfileOnlyFundingDecision.DimOrganizationIdFunderNavigation.NameSv : string.Empty,
                    FactFieldValues_DimIdentifierlessDataId = ffv.DimIdentifierlessDataId > 0 ? ffv.DimIdentifierlessDataId : -1,
                    FactFieldValues_DimIdentifierlessData_Type = ffv.DimIdentifierlessDataId > 0 ? ffv.DimIdentifierlessData.Type : string.Empty,
                    FactFieldValues_DimIdentifierlessData_ValueFi = ffv.DimIdentifierlessDataId > 0 ? ffv.DimIdentifierlessData.ValueFi : string.Empty,
                    FactFieldValues_DimIdentifierlessData_ValueEn = ffv.DimIdentifierlessDataId > 0 ? ffv.DimIdentifierlessData.ValueEn : string.Empty,
                    FactFieldValues_DimIdentifierlessData_ValueSv = ffv.DimIdentifierlessDataId > 0 ? ffv.DimIdentifierlessData.ValueSv : string.Empty
                }).AsNoTracking().ToListAsync();

            List<ProfileEditorFundingDecision> fundingDecisions = new List<ProfileEditorFundingDecision>(); 

            /*
             * Process DimFundingDecision DTOs
             */
            foreach (FundingDecisionDto fundingDecisionDto in fundingDecisionsDtos)
            {
                // Name translation: funding decision name
                NameTranslation nameTranslationFundingDecisionName = _languageService.GetNameTranslation(
                    nameFi: fundingDecisionDto.NameFi,
                    nameEn: fundingDecisionDto.NameEn,
                    nameSv: fundingDecisionDto.NameSv
                );
                // Name translation: funding decision description
                NameTranslation nameTranslationFundingDecisionDescription = _languageService.GetNameTranslation(
                    nameFi: fundingDecisionDto.DescriptionFi,
                    nameEn: fundingDecisionDto.DescriptionEn,
                    nameSv: fundingDecisionDto.DescriptionSv
                );
                // Name translation: funder name
                NameTranslation nameTranslationFunderName = _languageService.GetNameTranslation(
                    nameFi: fundingDecisionDto.FunderNameFi,
                    nameEn: fundingDecisionDto.FunderNameEn,
                    nameSv: fundingDecisionDto.FunderNameSv
                );
                // Name translation: call programme
                NameTranslation nameTranslationCallProgramme = _languageService.GetNameTranslation(
                    nameFi: fundingDecisionDto.CallProgramNameFi,
                    nameEn: fundingDecisionDto.CallProgramNameEn,
                    nameSv: fundingDecisionDto.CallProgramNameSv
                );
                // Name translation: type of funding name
                NameTranslation nameTranslationTypeOfFundingName = _languageService.GetNameTranslation(
                    nameFi: fundingDecisionDto.TypeOfFundingNameFi,
                    nameEn: fundingDecisionDto.TypeOfFundingNameEn,
                    nameSv: fundingDecisionDto.TypeOfFundingNameSv
                );
                // Name translation: data source organization name
                NameTranslation dataSourceOrganizationName = _languageService.GetNameTranslation(
                    nameFi: fundingDecisionDto.DimRegisteredDatasource_DimOrganization_NameFi,
                    nameEn: fundingDecisionDto.DimRegisteredDatasource_DimOrganization_NameEn,
                    nameSv: fundingDecisionDto.DimRegisteredDatasource_DimOrganization_NameSv
                );

                ProfileEditorFundingDecision fdFromFundingDecisionDto = new ProfileEditorFundingDecision()
                {
                    ProjectId = fundingDecisionDto.Id,
                    ProjectAcronym = fundingDecisionDto.Acronym,
                    ProjectNameFi = nameTranslationFundingDecisionName.NameFi,
                    ProjectNameEn = nameTranslationFundingDecisionName.NameEn,
                    ProjectNameSv = nameTranslationFundingDecisionName.NameSv,
                    ProjectDescriptionFi = nameTranslationFundingDecisionDescription.NameFi,
                    ProjectDescriptionEn = nameTranslationFundingDecisionDescription.NameEn,
                    ProjectDescriptionSv = nameTranslationFundingDecisionDescription.NameSv,
                    FunderNameFi = nameTranslationFunderName.NameFi,
                    FunderNameEn = nameTranslationFunderName.NameEn,
                    FunderNameSv = nameTranslationFunderName.NameSv,
                    FunderProjectNumber = fundingDecisionDto.FunderProjectNumber,
                    TypeOfFundingNameFi = nameTranslationTypeOfFundingName.NameFi,
                    TypeOfFundingNameEn = nameTranslationTypeOfFundingName.NameEn,
                    TypeOfFundingNameSv = nameTranslationTypeOfFundingName.NameSv,
                    CallProgrammeNameFi = nameTranslationCallProgramme.NameFi,
                    CallProgrammeNameEn = nameTranslationCallProgramme.NameEn,
                    CallProgrammeNameSv = nameTranslationCallProgramme.NameSv,
                    FundingStartYear = fundingDecisionDto.FundingStartYear,
                    FundingEndYear = fundingDecisionDto.FundingEndYear,
                    AmountInEur = fundingDecisionDto.AmountInEur,
                    itemMeta = new ProfileEditorItemMeta(
                        id: fundingDecisionDto.Id,
                        type: Constants.ItemMetaTypes.ACTIVITY_FUNDING_DECISION,
                        show: fundingDecisionDto.Show,
                        primaryValue: fundingDecisionDto.PrimaryValue
                    ),
                    DataSources = new List<ProfileEditorSource>
                    {
                        new ProfileEditorSource() {
                            Id = fundingDecisionDto.DimRegisteredDatasource_Id,
                            RegisteredDataSource = fundingDecisionDto.DimRegisteredDatasource_Name,
                            Organization = new Organization() {
                                NameFi = dataSourceOrganizationName.NameFi,
                                NameEn = dataSourceOrganizationName.NameEn,
                                NameSv = dataSourceOrganizationName.NameSv,
                                SectorId = fundingDecisionDto.DimregisteredDatasource_DimOrganization_DimSector_SectorId
                            }
                        }
                    }
                };
                fundingDecisions.Add(fdFromFundingDecisionDto);
            }

            /*
             * Process DimProfileOnlyFundingDecision DTOs
             */
            foreach (FundingDecisionDto profileOnlyFundingDecisionDto in profileOnlyFundingDecisionsDtos)
            {
                // Name translation: funding decision name
                NameTranslation nameTranslationFundingDecisionName = _languageService.GetNameTranslation(
                    nameFi: profileOnlyFundingDecisionDto.NameFi,
                    nameEn: profileOnlyFundingDecisionDto.NameEn,
                    nameSv: profileOnlyFundingDecisionDto.NameSv
                );
                // Name translation: funding decision description
                NameTranslation nameTranslationFundingDecisionDescription = _languageService.GetNameTranslation(
                    nameFi: profileOnlyFundingDecisionDto.DescriptionFi,
                    nameEn: profileOnlyFundingDecisionDto.DescriptionEn,
                    nameSv: profileOnlyFundingDecisionDto.DescriptionSv
                );
                // Name translation: type of funding name
                NameTranslation nameTranslationTypeOfFundingName = _languageService.GetNameTranslation(
                    nameFi: profileOnlyFundingDecisionDto.TypeOfFundingNameFi,
                    nameEn: profileOnlyFundingDecisionDto.TypeOfFundingNameEn,
                    nameSv: profileOnlyFundingDecisionDto.TypeOfFundingNameSv
                );
                // Name translation: funder name
                NameTranslation nameTranslationFunderName = new();
                // Taken from either related dim_organization or dim_identifierless_data
                if (profileOnlyFundingDecisionDto.DimProfileOnlyFundingDecision_DimOrganization_Id != null && profileOnlyFundingDecisionDto.DimProfileOnlyFundingDecision_DimOrganization_Id > 0)
                {
                    nameTranslationFunderName = _languageService.GetNameTranslation(
                        nameFi: profileOnlyFundingDecisionDto.DimProfileOnlyFundingDecision_DimOrganization_NameFi,
                        nameEn: profileOnlyFundingDecisionDto.DimProfileOnlyFundingDecision_DimOrganization_NameEn,
                        nameSv: profileOnlyFundingDecisionDto.DimProfileOnlyFundingDecision_DimOrganization_NameSv
                    );
                }
                else if (profileOnlyFundingDecisionDto.FactFieldValues_DimIdentifierlessDataId > -1 &&
                    profileOnlyFundingDecisionDto.FactFieldValues_DimIdentifierlessData_Type == Constants.IdentifierlessDataTypes.ORGANIZATION_NAME)
                {
                    nameTranslationFunderName = _languageService.GetNameTranslation(
                        nameFi: profileOnlyFundingDecisionDto.FactFieldValues_DimIdentifierlessData_ValueFi,
                        nameEn: profileOnlyFundingDecisionDto.FactFieldValues_DimIdentifierlessData_ValueEn,
                        nameSv: profileOnlyFundingDecisionDto.FactFieldValues_DimIdentifierlessData_ValueSv
                    );
                }
                // Name translation: data source organization name
                NameTranslation dataSourceOrganizationName = _languageService.GetNameTranslation(
                    nameFi: profileOnlyFundingDecisionDto.DimRegisteredDatasource_DimOrganization_NameFi,
                    nameEn: profileOnlyFundingDecisionDto.DimRegisteredDatasource_DimOrganization_NameEn,
                    nameSv: profileOnlyFundingDecisionDto.DimRegisteredDatasource_DimOrganization_NameSv
                );
                ProfileEditorFundingDecision fdFromProfileOnlyFundingDecisionDto = new ProfileEditorFundingDecision()
                {
                    ProjectId = -1, // Not populated for DimProfileOnlyFundingDecision 
                    ProjectAcronym = profileOnlyFundingDecisionDto.Acronym,
                    ProjectNameFi = nameTranslationFundingDecisionName.NameFi,
                    ProjectNameEn = nameTranslationFundingDecisionName.NameEn,
                    ProjectNameSv = nameTranslationFundingDecisionName.NameSv,
                    ProjectDescriptionFi = nameTranslationFundingDecisionDescription.NameFi,
                    ProjectDescriptionEn = nameTranslationFundingDecisionDescription.NameEn,
                    ProjectDescriptionSv = nameTranslationFundingDecisionDescription.NameSv,
                    FunderNameFi = nameTranslationFunderName.NameFi,
                    FunderNameEn = nameTranslationFunderName.NameEn,
                    FunderNameSv = nameTranslationFunderName.NameSv,
                    FunderProjectNumber = profileOnlyFundingDecisionDto.FunderProjectNumber,
                    TypeOfFundingNameFi = nameTranslationTypeOfFundingName.NameFi,
                    TypeOfFundingNameEn = nameTranslationTypeOfFundingName.NameEn,
                    TypeOfFundingNameSv = nameTranslationTypeOfFundingName.NameSv,
                    CallProgrammeNameFi = "", // Not populated for DimProfileOnlyFundingDecision 
                    CallProgrammeNameEn = "",
                    CallProgrammeNameSv = "",
                    FundingStartYear = profileOnlyFundingDecisionDto.FundingStartYear,
                    FundingEndYear = profileOnlyFundingDecisionDto.FundingEndYear,
                    AmountInEur = profileOnlyFundingDecisionDto.AmountInEur,
                    AmountInFundingDecisionCurrency = profileOnlyFundingDecisionDto.AmountInFundingDecisionCurrency,
                    FundingDecisionCurrencyAbbreviation = profileOnlyFundingDecisionDto.FundingDecisionCurrencyAbbreviation,
                    Url = profileOnlyFundingDecisionDto.Url,
                    itemMeta = new ProfileEditorItemMeta(
                        id: profileOnlyFundingDecisionDto.Id,
                        type: Constants.ItemMetaTypes.ACTIVITY_FUNDING_DECISION_PROFILE_ONLY,
                        show: profileOnlyFundingDecisionDto.Show,
                        primaryValue: profileOnlyFundingDecisionDto.PrimaryValue
                    ),
                    DataSources = new List<ProfileEditorSource>
                    {
                        new ProfileEditorSource() {
                            Id = profileOnlyFundingDecisionDto.DimRegisteredDatasource_Id,
                            RegisteredDataSource = profileOnlyFundingDecisionDto.DimRegisteredDatasource_Name,
                            Organization = new Organization() {
                                NameFi = dataSourceOrganizationName.NameFi,
                                NameEn = dataSourceOrganizationName.NameEn,
                                NameSv = dataSourceOrganizationName.NameSv,
                                SectorId = profileOnlyFundingDecisionDto.DimregisteredDatasource_DimOrganization_DimSector_SectorId
                            }
                        }
                    }
                };
                fundingDecisions.Add(fdFromProfileOnlyFundingDecisionDto);
            }

            return fundingDecisions;
        }
    }
}