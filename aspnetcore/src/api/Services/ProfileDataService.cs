using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.Models.Ttv;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using api.Models.Common;
using api.Models.ProfileEditor.Items;
using Nest;
using System;

namespace api.Services
{
    public class ProfileDataService : IProfileDataService
    {
        private readonly TtvContext _ttvContext;
        private readonly IDataSourceHelperService _dataSourceHelperService;
        private readonly ILanguageService _languageService;
        private readonly IUtilityService _utilityService;
        private readonly ILogger<ProfileDataService> _logger;


        public ProfileDataService(
            TtvContext ttvContext,
            IDataSourceHelperService dataSourceHelperService,
            ILanguageService languageService,
            IUtilityService utilityService,
            ILogger<ProfileDataService> logger)
        {
            _ttvContext = ttvContext;
            _dataSourceHelperService = dataSourceHelperService;
            _languageService = languageService;
            _utilityService = utilityService;
            _logger = logger;
        }

        /*
         * Names
         */
        public async Task<List<ProfileEditorName>> GetProfileEditorNames(int userprofileId)
        {
            List<ProfileEditorName> names = await _ttvContext.FactFieldValues.Where(ffv => ffv.DimUserProfileId == userprofileId
                && ffv.DimNameId > 0
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

            // Postprocessing. Translate data source organizaton names.
            foreach (ProfileEditorName name in names)
            {
                foreach (ProfileEditorSource dataSource in name.DataSources)
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

            return names;
        }

        /*
         * Other Names
         */
        public async Task<List<ProfileEditorName>> GetProfileEditorOtherNames(int userprofileId)
        {
            List<ProfileEditorName> otherNames = await _ttvContext.FactFieldValues.Where(ffv => ffv.DimUserProfileId == userprofileId
                && ffv.DimNameId > 0
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

            // Postprocessing. Translate data source organizaton names.
            foreach (ProfileEditorName otherName in otherNames)
            {
                foreach (ProfileEditorSource dataSource in otherName.DataSources)
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

            return otherNames;
        }

        /*
         * Emails
         */
        public async Task<List<ProfileEditorEmail>> GetProfileEditorEmails(int userprofileId)
        {
            List<ProfileEditorEmail> emails = await _ttvContext.FactFieldValues.Where(ffv => ffv.DimUserProfileId == userprofileId
                && ffv.DimEmailAddrressId > 0
                && ffv.DimFieldDisplaySettings.FieldIdentifier == Constants.FieldIdentifiers.PERSON_EMAIL_ADDRESS)
                .Select(ffv => new ProfileEditorEmail()
                {
                    Value = ffv.DimEmailAddrress.Email,
                    itemMeta = new ProfileEditorItemMeta(
                        ffv.DimEmailAddrressId,
                        Constants.ItemMetaTypes.PERSON_EMAIL_ADDRESS,
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

            // Postprocessing. Translate data source organizaton names.
            foreach (ProfileEditorEmail email in emails)
            {
                foreach (ProfileEditorSource dataSource in email.DataSources)
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

            return emails;
        }

        /*
         * Telephone Numbers
         */
        public async Task<List<ProfileEditorTelephoneNumber>> GetProfileEditorTelephoneNumbers(int userprofileId)
        {
            List<ProfileEditorTelephoneNumber> telephoneNumbers = await _ttvContext.FactFieldValues.Where(ffv => ffv.DimUserProfileId == userprofileId
                && ffv.DimTelephoneNumberId > 0
                && ffv.DimFieldDisplaySettings.FieldIdentifier == Constants.FieldIdentifiers.PERSON_TELEPHONE_NUMBER)
                .Select(ffv => new ProfileEditorTelephoneNumber()
                {
                    Value = ffv.DimTelephoneNumber.TelephoneNumber,
                    itemMeta = new ProfileEditorItemMeta(
                        ffv.DimTelephoneNumberId,
                        Constants.ItemMetaTypes.PERSON_TELEPHONE_NUMBER,
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

            // Postprocessing. Translate data source organizaton names.
            foreach (ProfileEditorTelephoneNumber telephoneNumber in telephoneNumbers)
            {
                foreach (ProfileEditorSource dataSource in telephoneNumber.DataSources)
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

            return telephoneNumbers;
        }

        /*
         * Web links
         */
        public async Task<List<ProfileEditorWebLink>> GetProfileEditorWebLinks(int userprofileId)
        {
            List<ProfileEditorWebLink> webLinks = await _ttvContext.FactFieldValues.Where(ffv => ffv.DimUserProfileId == userprofileId
                && ffv.DimWebLinkId > 0
                && ffv.DimFieldDisplaySettings.FieldIdentifier == Constants.FieldIdentifiers.PERSON_WEB_LINK)
                .Select(ffv => new ProfileEditorWebLink()
                {
                    Url = ffv.DimWebLink.Url,
                    LinkLabel = ffv.DimWebLink.LinkLabel,
                    LinkType = ffv.DimWebLink.LinkType,
                    itemMeta = new ProfileEditorItemMeta(
                        ffv.DimWebLinkId,
                        Constants.ItemMetaTypes.PERSON_WEB_LINK,
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

            // Postprocessing. Translate data source organizaton names.
            foreach (ProfileEditorWebLink webLink in webLinks)
            {
                foreach (ProfileEditorSource dataSource in webLink.DataSources)
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

            return webLinks;
        }

        /*
         * Keywords
         */
        public async Task<List<ProfileEditorKeyword>> GetProfileEditorKeywords(int userprofileId)
        {
            List<ProfileEditorKeyword> keywords = await _ttvContext.FactFieldValues.Where(ffv => ffv.DimUserProfileId == userprofileId
                && ffv.DimKeywordId > 0
                && ffv.DimFieldDisplaySettings.FieldIdentifier == Constants.FieldIdentifiers.PERSON_KEYWORD)
                .Select(ffv => new ProfileEditorKeyword()
                {
                    Value = ffv.DimKeyword.Keyword,
                    itemMeta = new ProfileEditorItemMeta(
                        ffv.DimKeywordId,
                        Constants.ItemMetaTypes.PERSON_KEYWORD,
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

            // Postprocessing. Translate data source organizaton names.
            foreach (ProfileEditorKeyword keyword in keywords)
            {
                foreach (ProfileEditorSource dataSource in keyword.DataSources)
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

            return keywords;
        }

        /*
         * Researcher descriptions
         */
        public async Task<List<ProfileEditorResearcherDescription>> GetProfileEditorResearcherDescriptions(int userprofileId)
        {
            List<ProfileEditorResearcherDescription> researcherDescriptions = await _ttvContext.FactFieldValues.Where(ffv => ffv.DimUserProfileId == userprofileId
                && ffv.DimResearcherDescriptionId > 0
                && ffv.DimFieldDisplaySettings.FieldIdentifier == Constants.FieldIdentifiers.PERSON_RESEARCHER_DESCRIPTION)
                .Select(ffv => new ProfileEditorResearcherDescription()
                {
                    ResearchDescriptionFi = ffv.DimResearcherDescription.ResearchDescriptionFi,
                    ResearchDescriptionEn = ffv.DimResearcherDescription.ResearchDescriptionEn,
                    ResearchDescriptionSv = ffv.DimResearcherDescription.ResearchDescriptionSv,
                    itemMeta = new ProfileEditorItemMeta(
                        ffv.DimResearcherDescriptionId,
                        Constants.ItemMetaTypes.PERSON_RESEARCHER_DESCRIPTION,
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

                /*
                 * Postprocessing. Translate data source organizaton names and researcher description names.
                 * Researcher description name translation must be skipped when data source is tiedejatutkimus.fi.
                 * That indicates user generated value (from AI assisted researcher description feature), whose translation is handled in the frontend.
                 */
                foreach (ProfileEditorResearcherDescription researcherDescription in researcherDescriptions)
                {
                    // Translate data source organization names.
                    foreach (ProfileEditorSource dataSource in researcherDescription.DataSources)
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

                    // Translate researcher description names if data source is not tiedejatutkimus.fi.
                    if (researcherDescription.DataSources[0].RegisteredDataSource != _dataSourceHelperService.DimRegisteredDataSourceName_TTV)
                    {
                        NameTranslation nameTranslationResearcherDescription = _languageService.GetNameTranslation(
                            nameFi: researcherDescription.ResearchDescriptionFi,
                            nameEn: researcherDescription.ResearchDescriptionEn,
                            nameSv: researcherDescription.ResearchDescriptionSv
                        );
                        researcherDescription.ResearchDescriptionFi = nameTranslationResearcherDescription.NameFi;
                        researcherDescription.ResearchDescriptionEn = nameTranslationResearcherDescription.NameEn;
                        researcherDescription.ResearchDescriptionSv = nameTranslationResearcherDescription.NameSv;
                    }
                }
            return researcherDescriptions;
        }

        /*
         * External identifiers
         */
         public async Task<List<ProfileEditorExternalIdentifier>> GetProfileEditorExternalIdentifiers(int userprofileId)
         {
             List<ProfileEditorExternalIdentifier> externalIdentifiers = await _ttvContext.FactFieldValues.Where(ffv => ffv.DimUserProfileId == userprofileId
                && ffv.DimPidId > 0
                && ffv.DimFieldDisplaySettings.FieldIdentifier == Constants.FieldIdentifiers.PERSON_EXTERNAL_IDENTIFIER)
                .Select(ffv => new ProfileEditorExternalIdentifier()
                {
                    PidContent = ffv.DimPid.PidContent,
                    PidType = ffv.DimPid.PidType,
                    itemMeta = new ProfileEditorItemMeta(
                        ffv.DimPidId,
                        Constants.ItemMetaTypes.PERSON_EXTERNAL_IDENTIFIER,
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

            // Postprocessing. Translate data source organizaton names.
            foreach (ProfileEditorExternalIdentifier externalIdentifier in externalIdentifiers)
            {
                foreach (ProfileEditorSource dataSource in externalIdentifier.DataSources)
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

            return externalIdentifiers;
        }

        /*
         * Educations
         */
        public async Task<List<ProfileEditorEducation>> GetProfileEditorEducations(int userprofileId)
        {
            List<ProfileEditorEducation> educations = await _ttvContext.FactFieldValues.Where(ffv => ffv.DimUserProfileId == userprofileId
                && ffv.DimEducationId > 0
                && ffv.DimFieldDisplaySettings.FieldIdentifier == Constants.FieldIdentifiers.ACTIVITY_EDUCATION)
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
            public int? DimIdentifierlessData_Id { get; set; }
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
            List<AffiliationDto> affiliationDtos = await _ttvContext.FactFieldValues.Where(ffv => ffv.DimUserProfileId == userprofileId
                && ffv.DimAffiliationId > 0
                && ffv.DimFieldDisplaySettings.FieldIdentifier == Constants.FieldIdentifiers.ACTIVITY_AFFILIATION)
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
                    DimIdentifierlessData_Child_Type = ffv.DimIdentifierlessData.DimIdentifierlessData.Type,
                    DimIdentifierlessData_Child_ValueFi = ffv.DimIdentifierlessData.DimIdentifierlessData.ValueFi,
                    DimIdentifierlessData_Child_ValueEn = ffv.DimIdentifierlessData.DimIdentifierlessData.ValueEn,
                    DimIdentifierlessData_Child_ValueSv = ffv.DimIdentifierlessData.DimIdentifierlessData.ValueSv,
                    DimAffiliation_PositionNameFi = ffv.DimAffiliation.PositionNameFi,
                    DimAffiliation_PositionNameEn = ffv.DimAffiliation.PositionNameEn,
                    DimAffiliation_PositionNameSv = ffv.DimAffiliation.PositionNameSv,
                    DimAffiliation_AffiliationTypeFi = ffv.DimAffiliation.AffiliationTypeFi,
                    DimAffiliation_AffiliationTypeEn = ffv.DimAffiliation.AffiliationTypeEn,
                    DimAffiliation_AffiliationTypeSv = ffv.DimAffiliation.AffiliationTypeSv,
                    StartDate_Year = ffv.DimAffiliation.StartDateNavigation.Year,
                    StartDate_Month = ffv.DimAffiliation.StartDateNavigation.Month,
                    StartDate_Day = ffv.DimAffiliation.StartDateNavigation.Day,
                    EndDate_Year = ffv.DimAffiliation.EndDateNavigation.Year,
                    EndDate_Month = ffv.DimAffiliation.EndDateNavigation.Month,
                    EndDate_Day = ffv.DimAffiliation.EndDateNavigation.Day
                }).AsNoTracking().ToListAsync();

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
                        affiliationDto.DimAffiliationId,
                        Constants.ItemMetaTypes.ACTIVITY_AFFILIATION,
                        affiliationDto.Show,
                        affiliationDto.PrimaryValue
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

            return affiliations;
        }

        // Publication query DTO class
        public class PublicationDto
        {
        }

        // ProfileOnlyPublication query DTO class
        public class ProfileOnlyPublicationDto
        {
        }

        /*
         * Publications
         */
        public async Task<List<ProfileEditorPublication>> GetProfileEditorPublications(int userprofileId)
        {
            // DimPublication DTOs
            List<PublicationDto> publicationDtos = await _ttvContext.FactFieldValues
                .Where(ffv => ffv.DimUserProfileId == userprofileId && ffv.DimPublicationId > 0
                    && ffv.DimFieldDisplaySettings.FieldIdentifier == Constants.FieldIdentifiers.ACTIVITY_PUBLICATION)
                .Select(ffv => new PublicationDto()
                {
                }).AsNoTracking().ToListAsync();

            // DimProfileOnlyPublication DTOs
            List<ProfileOnlyPublicationDto> profileOnlyPublicationsDtos = await _ttvContext.FactFieldValues
                .Where(ffv => ffv.DimUserProfileId == userprofileId && ffv.DimProfileOnlyPublicationId > 0
                    && ffv.DimFieldDisplaySettings.FieldIdentifier == Constants.FieldIdentifiers.ACTIVITY_PUBLICATION_PROFILE_ONLY)
                .Select(ffv => new ProfileOnlyPublicationDto()
                {
                }).AsNoTracking().ToListAsync();


            // Postprocessing.

            List<ProfileEditorPublication> publications = new List<ProfileEditorPublication>();

            return publications;
        }
    }
}