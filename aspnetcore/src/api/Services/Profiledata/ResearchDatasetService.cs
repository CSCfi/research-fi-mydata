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
    public class ResearchDatasetService : IResearchDatasetService
    {
        private readonly TtvContext _ttvContext;
        private readonly ILanguageService _languageService;
        private readonly ILogger<ResearchDatasetService> _logger;


        public ResearchDatasetService(
            TtvContext ttvContext,
            ILanguageService languageService,
            ILogger<ResearchDatasetService> logger)
        {
            _ttvContext = ttvContext;
            _languageService = languageService;
            _logger = logger;
        }

        public class ResearchDatasetDto
        {
            public bool IsProfileOnlyResearchDataset { get; set; }
            public int Id { get; set; }
            public bool? Show { get; set; }
            public bool? PrimaryValue { get; set; }
            public int DimRegisteredDatasource_Id { get; set; }
            public string? DimRegisteredDatasource_Name { get; set; }
            public string? DimRegisteredDatasource_DimOrganization_NameFi { get; set; }
            public string? DimRegisteredDatasource_DimOrganization_NameEn { get; set; }
            public string? DimRegisteredDatasource_DimOrganization_NameSv { get; set; }
            public string? DimregisteredDatasource_DimOrganization_DimSector_SectorId { get; set; }
            public string? AccessType { get; set; } = null;
            public DateTime? DatasetCreated { get; set; } = null;
            public string DescriptionFi { get; set; } = string.Empty;
            public string DescriptionEn { get; set; } = string.Empty;
            public string DescriptionSv { get; set; } = string.Empty;
            public string LocalIdentifier { get; set; } = string.Empty;
            public string NameFi { get; set; } = string.Empty;
            public string NameEn { get; set; } = string.Empty;
            public string NameSv { get; set; } = string.Empty;
            public string Url { get; set; } = string.Empty;
            public List<ProfileEditorPreferredIdentifier> PreferredIdentifiers { get; set; } = new();
        }

        /*
         * Research Datasets
         */
        public async Task<List<ProfileEditorResearchDataset>> GetProfileEditorResearchDatasets(int userprofileId, bool forElasticsearch = false)
        {
            var stopwatch = Stopwatch.StartNew();

            /*
             * DimResearchDataset => DTO
             */
            List<ResearchDatasetDto> researchDatasetDtos = await _ttvContext.FactFieldValues.Where(ffv => ffv.DimUserProfileId == userprofileId
                && ffv.DimResearchDatasetId > 0
                && ffv.DimFieldDisplaySettings.FieldIdentifier == Constants.FieldIdentifiers.ACTIVITY_RESEARCH_DATASET
                && (forElasticsearch ? ffv.Show == true : true))
                .Select(ffv => new ResearchDatasetDto()
                {
                    IsProfileOnlyResearchDataset = false,
                    Id = ffv.DimResearchDatasetId,
                    Show = ffv.Show,
                    PrimaryValue = ffv.PrimaryValue,
                    DimRegisteredDatasource_Id = ffv.DimRegisteredDataSourceId,
                    DimRegisteredDatasource_Name = ffv.DimRegisteredDataSource.Name,
                    DimRegisteredDatasource_DimOrganization_NameFi = ffv.DimRegisteredDataSource.DimOrganization.NameFi,
                    DimRegisteredDatasource_DimOrganization_NameEn = ffv.DimRegisteredDataSource.DimOrganization.NameEn,
                    DimRegisteredDatasource_DimOrganization_NameSv = ffv.DimRegisteredDataSource.DimOrganization.NameSv,
                    DimregisteredDatasource_DimOrganization_DimSector_SectorId = ffv.DimRegisteredDataSource.DimOrganization.DimSector.SectorId,
                    AccessType = ffv.DimResearchDataset.DimReferencedataAvailabilityNavigation.CodeValue,
                    DatasetCreated = ffv.DimResearchDataset.DatasetCreated,
                    DescriptionFi = ffv.DimResearchDataset.DimDescriptiveItems.Where(di =>
                        di.DescriptiveItemType == Constants.DescriptiveItemTypes.DESCRIPTION &&
                        di.DescriptiveItemLanguage == Constants.DescriptiveItemLanguages.FI).Select(di => di.DescriptiveItem).FirstOrDefault() ?? string.Empty,
                    DescriptionEn = ffv.DimResearchDataset.DimDescriptiveItems.Where(di =>
                        di.DescriptiveItemType == Constants.DescriptiveItemTypes.DESCRIPTION &&
                        di.DescriptiveItemLanguage == Constants.DescriptiveItemLanguages.EN).Select(di => di.DescriptiveItem).FirstOrDefault() ?? string.Empty,
                    DescriptionSv = ffv.DimResearchDataset.DimDescriptiveItems.Where(di =>
                        di.DescriptiveItemType == Constants.DescriptiveItemTypes.DESCRIPTION &&
                        di.DescriptiveItemLanguage == Constants.DescriptiveItemLanguages.SV).Select(di => di.DescriptiveItem).FirstOrDefault() ?? string.Empty,
                    LocalIdentifier = ffv.DimResearchDataset.LocalIdentifier,
                    NameFi = ffv.DimResearchDataset.DimDescriptiveItems.Where(di =>
                        di.DescriptiveItemType == Constants.DescriptiveItemTypes.NAME &&
                        di.DescriptiveItemLanguage == Constants.DescriptiveItemLanguages.FI).Select(di => di.DescriptiveItem).FirstOrDefault() ?? string.Empty,
                    NameEn = ffv.DimResearchDataset.DimDescriptiveItems.Where(di =>
                        di.DescriptiveItemType == Constants.DescriptiveItemTypes.NAME &&
                        di.DescriptiveItemLanguage == Constants.DescriptiveItemLanguages.EN).Select(di => di.DescriptiveItem).FirstOrDefault() ?? string.Empty,
                    NameSv = ffv.DimResearchDataset.DimDescriptiveItems.Where(di =>
                        di.DescriptiveItemType == Constants.DescriptiveItemTypes.NAME &&
                        di.DescriptiveItemLanguage == Constants.DescriptiveItemLanguages.SV).Select(di => di.DescriptiveItem).FirstOrDefault() ?? string.Empty,
                    PreferredIdentifiers = ffv.DimResearchDataset.DimPids.Select(pid => new ProfileEditorPreferredIdentifier()
                    {
                        PidType = pid.PidType,
                        PidContent = pid.PidContent
                    }).ToList()

                }).AsNoTracking().ToListAsync();

            /*
             * DimProfileOnlyDataset => DTO
             */
            List<ResearchDatasetDto> profileOnlyResearchDatasetDtos = await _ttvContext.FactFieldValues.Where(ffv => ffv.DimUserProfileId == userprofileId
                && ffv.DimProfileOnlyDatasetId > 0
                && ffv.DimFieldDisplaySettings.FieldIdentifier == Constants.FieldIdentifiers.ACTIVITY_RESEARCH_DATASET
                && (forElasticsearch ? ffv.Show == true : true))
                .Select(ffv => new ResearchDatasetDto()
                {
                    IsProfileOnlyResearchDataset = true,
                    Id = ffv.DimProfileOnlyDatasetId,
                    Show = ffv.Show,
                    PrimaryValue = ffv.PrimaryValue,
                    DimRegisteredDatasource_Id = ffv.DimRegisteredDataSourceId,
                    DimRegisteredDatasource_Name = ffv.DimRegisteredDataSource.Name,
                    DimRegisteredDatasource_DimOrganization_NameFi = ffv.DimRegisteredDataSource.DimOrganization.NameFi,
                    DimRegisteredDatasource_DimOrganization_NameEn = ffv.DimRegisteredDataSource.DimOrganization.NameEn,
                    DimRegisteredDatasource_DimOrganization_NameSv = ffv.DimRegisteredDataSource.DimOrganization.NameSv,
                    DimregisteredDatasource_DimOrganization_DimSector_SectorId = ffv.DimRegisteredDataSource.DimOrganization.DimSector.SectorId,
                    DatasetCreated = ffv.DimProfileOnlyDataset.DatasetCreated,
                    DescriptionFi = ffv.DimProfileOnlyDataset.DescriptionFi,
                    DescriptionEn = ffv.DimProfileOnlyDataset.DescriptionEn,
                    DescriptionSv = ffv.DimProfileOnlyDataset.DescriptionSv,
                    LocalIdentifier = ffv.DimProfileOnlyDataset.LocalIdentifier,
                    NameFi = ffv.DimProfileOnlyDataset.NameFi,
                    NameEn = ffv.DimProfileOnlyDataset.NameEn,
                    NameSv = ffv.DimProfileOnlyDataset.NameSv,
                    Url = ffv.DimProfileOnlyDataset.DimWebLinks.Count > 0 ? ffv.DimProfileOnlyDataset.DimWebLinks.FirstOrDefault().Url : string.Empty,
                }).AsNoTracking().ToListAsync();


            List<ProfileEditorResearchDataset> researchDatasets = new(); 

            /*
             * Process DimResearchDataset DTOs
             */
            foreach (ResearchDatasetDto researchDatasetDto in researchDatasetDtos)
            {
                // Name translation: research dataset name
                NameTranslation nameTranslationResearchDatasetName = _languageService.GetNameTranslation(
                    nameFi: researchDatasetDto.NameFi,
                    nameEn: researchDatasetDto.NameEn,
                    nameSv: researchDatasetDto.NameSv
                );
                // Name translation: research dataset description
                NameTranslation nameTranslationResearchDatasetDescription = _languageService.GetNameTranslation(
                    nameFi: researchDatasetDto.DescriptionFi,
                    nameEn: researchDatasetDto.DescriptionEn,
                    nameSv: researchDatasetDto.DescriptionSv
                );
                // Name translation: data source organization name
                NameTranslation dataSourceOrganizationName = _languageService.GetNameTranslation(
                    nameFi: researchDatasetDto.DimRegisteredDatasource_DimOrganization_NameFi,
                    nameEn: researchDatasetDto.DimRegisteredDatasource_DimOrganization_NameEn,
                    nameSv: researchDatasetDto.DimRegisteredDatasource_DimOrganization_NameSv
                );
                ProfileEditorResearchDataset researchDataset = new()
                {
                    AccessType = researchDatasetDto.AccessType,
                    // Only year part of datetime is set in DatasetCreated 
                    DatasetCreated = (researchDatasetDto.DatasetCreated != null) ? researchDatasetDto.DatasetCreated.Value.Year : null,
                    DescriptionFi = nameTranslationResearchDatasetDescription.NameFi,
                    DescriptionSv = nameTranslationResearchDatasetDescription.NameSv,
                    DescriptionEn = nameTranslationResearchDatasetDescription.NameEn,
                    FairdataUrl = !string.IsNullOrWhiteSpace(researchDatasetDto.LocalIdentifier) ? $"https://etsin.fairdata.fi/dataset/{researchDatasetDto.LocalIdentifier}" : string.Empty,
                    Identifier = researchDatasetDto.LocalIdentifier,
                    NameFi = nameTranslationResearchDatasetName.NameFi,
                    NameEn = nameTranslationResearchDatasetName.NameEn,
                    NameSv = nameTranslationResearchDatasetName.NameSv,
                    PreferredIdentifiers = researchDatasetDto.PreferredIdentifiers,
                    itemMeta = new ProfileEditorItemMeta(
                        id: researchDatasetDto.Id,
                        type: Constants.ItemMetaTypes.ACTIVITY_RESEARCH_DATASET,
                        show: researchDatasetDto.Show,
                        primaryValue: researchDatasetDto.PrimaryValue
                    ),
                    DataSources = new List<ProfileEditorSource>
                    {
                        new ProfileEditorSource() {
                            Id = researchDatasetDto.DimRegisteredDatasource_Id,
                            RegisteredDataSource = researchDatasetDto.DimRegisteredDatasource_Name,
                            Organization = new Organization() {
                                NameFi = dataSourceOrganizationName.NameFi,
                                NameEn = dataSourceOrganizationName.NameEn,
                                NameSv = dataSourceOrganizationName.NameSv,
                                SectorId = researchDatasetDto.DimregisteredDatasource_DimOrganization_DimSector_SectorId
                            }
                        }
                    }
                };

                researchDatasets.Add(researchDataset);
            }

            /*
             * Process DimProfileOnlyDataset DTOs
             */
            foreach (ResearchDatasetDto profileOnlyResearchDatasetDto in profileOnlyResearchDatasetDtos)
            {
                // Name translation: research dataset name
                NameTranslation nameTranslationResearchDatasetName = _languageService.GetNameTranslation(
                    nameFi: profileOnlyResearchDatasetDto.NameFi,
                    nameEn: profileOnlyResearchDatasetDto.NameEn,
                    nameSv: profileOnlyResearchDatasetDto.NameSv
                );
                // Name translation: research dataset description
                NameTranslation nameTranslationResearchDatasetDescription = _languageService.GetNameTranslation(
                    nameFi: profileOnlyResearchDatasetDto.DescriptionFi,
                    nameEn: profileOnlyResearchDatasetDto.DescriptionEn,
                    nameSv: profileOnlyResearchDatasetDto.DescriptionSv
                );
                // Name translation: data source organization name
                NameTranslation dataSourceOrganizationName = _languageService.GetNameTranslation(
                    nameFi: profileOnlyResearchDatasetDto.DimRegisteredDatasource_DimOrganization_NameFi,
                    nameEn: profileOnlyResearchDatasetDto.DimRegisteredDatasource_DimOrganization_NameEn,
                    nameSv: profileOnlyResearchDatasetDto.DimRegisteredDatasource_DimOrganization_NameSv
                );

                ProfileEditorResearchDataset researchDataset = new()
                {
                    // Only year part of datetime is set in DatasetCreated 
                    DatasetCreated = (profileOnlyResearchDatasetDto.DatasetCreated != null) ? profileOnlyResearchDatasetDto.DatasetCreated.Value.Year : null,
                    DescriptionFi = nameTranslationResearchDatasetDescription.NameFi,
                    DescriptionSv = nameTranslationResearchDatasetDescription.NameSv,
                    DescriptionEn = nameTranslationResearchDatasetDescription.NameEn,
                    Identifier = profileOnlyResearchDatasetDto.LocalIdentifier,
                    NameFi = nameTranslationResearchDatasetName.NameFi,
                    NameEn = nameTranslationResearchDatasetName.NameEn,
                    NameSv = nameTranslationResearchDatasetName.NameSv,
                    Url = profileOnlyResearchDatasetDto.Url,
                    itemMeta = new ProfileEditorItemMeta(
                        id: profileOnlyResearchDatasetDto.Id,
                        type: Constants.ItemMetaTypes.ACTIVITY_RESEARCH_DATASET_PROFILE_ONLY,
                        show: profileOnlyResearchDatasetDto.Show,
                        primaryValue: profileOnlyResearchDatasetDto.PrimaryValue
                    ),
                    DataSources = new List<ProfileEditorSource>
                    {
                        new ProfileEditorSource() {
                            Id = profileOnlyResearchDatasetDto.DimRegisteredDatasource_Id,
                            RegisteredDataSource = profileOnlyResearchDatasetDto.DimRegisteredDatasource_Name,
                            Organization = new Organization() {
                                NameFi = dataSourceOrganizationName.NameFi,
                                NameEn = dataSourceOrganizationName.NameEn,
                                NameSv = dataSourceOrganizationName.NameSv,
                                SectorId = profileOnlyResearchDatasetDto.DimregisteredDatasource_DimOrganization_DimSector_SectorId
                            }
                        }
                    }
                };

                researchDatasets.Add(researchDataset);
            }

            stopwatch.Stop();
            if (stopwatch.ElapsedMilliseconds > Constants.LoggingParameters.SLOW_OPERATION_MS_THRESHOLD)
            {
                _logger.LogWarning($"GetProfileEditorResearchDatasets is slow. userprofileId={userprofileId}, forElasticsearch={forElasticsearch}, {researchDatasets.Count} items in {stopwatch.ElapsedMilliseconds}ms.");
            }

            return researchDatasets;
        }
    }
}