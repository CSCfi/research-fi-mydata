using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.Models.Ttv;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using api.Models.Common;
using api.Models.ProfileEditor.Items;
using Nest;

namespace api.Services
{
    public class ProfileDataService : IProfileDataService
    {
        private readonly TtvContext _ttvContext;
        private readonly IDataSourceHelperService _dataSourceHelperService;
        private readonly ILanguageService _languageService;
        private readonly ILogger<ProfileDataService> _logger;


        public ProfileDataService(
            TtvContext ttvContext,
            IDataSourceHelperService dataSourceHelperService,
            ILanguageService languageService,
            ILogger<ProfileDataService> logger)
        {
            _ttvContext = ttvContext;
            _dataSourceHelperService = dataSourceHelperService;
            _languageService = languageService;
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
    }
}