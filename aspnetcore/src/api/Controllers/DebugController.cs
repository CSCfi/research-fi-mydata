﻿using api.Services;
using api.Models;
using api.Models.Ttv;
using api.Models.Common;
using api.Models.ProfileEditor;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;

namespace api.Controllers
{
    /*
     * DebugController implements API for debugging.
     */
    [ApiController]
    public class DebugController: ControllerBase
    {
        private readonly TtvContext _ttvContext;
        private readonly UserProfileService _userProfileService;
        private readonly TtvSqlService _ttvSqlService;
        private readonly LanguageService _languageService;
        private readonly IMemoryCache _cache;
        private readonly ILogger<UserProfileController> _logger;
        public IConfiguration _configuration { get; }

        public DebugController(IConfiguration configuration, TtvContext ttvContext, UserProfileService userProfileService, TtvSqlService ttvSqlService, LanguageService languageService, IMemoryCache memoryCache, ILogger<UserProfileController> logger)
        {
            _ttvContext = ttvContext;
            _userProfileService = userProfileService;
            _cache = memoryCache;
            _ttvSqlService = ttvSqlService;
            _languageService = languageService;
            _logger = logger;
            _configuration = configuration;
        }


        /// <summary>
        /// Debug: Get number of user profiles. Requires correct "debugtoken" header value.
        /// </summary>
        [HttpGet]
        [Route("/[controller]/profilecount")]
        public async Task<IActionResult> GetNumberOfProfiles()
        {
            // Check that "DEBUGTOKEN" is defined and has a value in configuration and that the request header "debugtoken" matches.
            if (_configuration["DEBUGTOKEN"] == null || _configuration["DEBUGTOKEN"] == "" || Request.Headers["debugtoken"] != _configuration["DEBUGTOKEN"])
            {
                return Unauthorized();
            }

            var dimPids = await _ttvContext.DimPids.Where(dp => dp.PidType == Constants.PidTypes.ORCID && dp.SourceId == Constants.SourceIdentifiers.ORCID).AsNoTracking().ToListAsync();
            return Ok(dimPids.Count);
        }


        /// <summary>
        /// Debug: Get list of ORCID IDs. Requires correct "debugtoken" header value.
        /// </summary>
        [HttpGet]
        [Route("/[controller]/orcids")]
        public async Task<IActionResult> GetListOfORCIDs()
        {
            // Check that "DEBUGTOKEN" is defined and has a value in configuration and that the request header "debugtoken" matches.
            if (_configuration["DEBUGTOKEN"] == null || _configuration["DEBUGTOKEN"] == "" || Request.Headers["debugtoken"] != _configuration["DEBUGTOKEN"])
            {
                return Unauthorized();
            }

            var orcidIds = new List<string>();
            var dimPids = await _ttvContext.DimPids.Where(dp => dp.PidType == Constants.PidTypes.ORCID && dp.SourceId == Constants.SourceIdentifiers.ORCID).AsNoTracking().ToListAsync();

            foreach (DimPid dp in dimPids)
            {
                orcidIds.Add(dp.PidContent);
            }

            return Ok(orcidIds);
        }


        /// <summary>
        /// Debug: Get any profile data. Requires correct "debugtoken" header value.
        /// </summary>
        [HttpGet]
        [Route("/[controller]/profiledata/{orcidId}")]
        [ProducesResponseType(typeof(ApiResponseProfileDataGet), StatusCodes.Status200OK)]
        public async Task<IActionResult> Get(string orcidId)
        {
            // Check that "DEBUGTOKEN" is defined and has a value in configuration and that the request header "debugtoken" matches.
            if (_configuration["DEBUGTOKEN"] == null || _configuration["DEBUGTOKEN"] == "" || Request.Headers["debugtoken"] != _configuration["DEBUGTOKEN"])
            {
                return Unauthorized();
            }

            // Check that user profile exists.
            var userprofileId = await _userProfileService.GetUserprofileId(orcidId);
            if (userprofileId == -1)
            {
                return Ok(new ApiResponse(success: false, reason: "profile not found"));
            }

            // TODO: remove this after Ttv model update
            return Ok(new ApiResponse(success: false, reason: "endpoint temporarily disabled"));

            /*
            // Get DimFieldDisplaySettings and related entities
            var dimFieldDisplaySettings = await _ttvContext.DimFieldDisplaySettings.Where(dfds => dfds.DimUserProfileId == userprofileId && dfds.FactFieldValues.Count() > 0)
                .Include(dfds => dfds.FactFieldValues)
                    .ThenInclude(ffv => ffv.DimRegisteredDataSource)
                        .ThenInclude(drds => drds.DimOrganization).AsNoTracking()
                // DimName
                .Include(dfds => dfds.FactFieldValues)
                    .ThenInclude(ffv => ffv.DimName).AsNoTracking()
                // DimWebLink
                .Include(dfds => dfds.FactFieldValues)
                    .ThenInclude(ffv => ffv.DimWebLink).AsNoTracking()
                // DimFundingDecision
                .Include(dfds => dfds.FactFieldValues)
                    .ThenInclude(ffv => ffv.DimFundingDecision)
                        .ThenInclude(dfd => dfd.DimOrganizationIdFunderNavigation).AsNoTracking() // DimFundingDecision related DimOrganization (funder organization)
                .Include(dfds => dfds.FactFieldValues)
                    .ThenInclude(ffv => ffv.DimFundingDecision)
                        .ThenInclude(dfd => dfd.DimDateIdStartNavigation).AsNoTracking() // DimFundingDecision related start date (DimDate)
                .Include(dfds => dfds.FactFieldValues)
                    .ThenInclude(ffv => ffv.DimFundingDecision)
                        .ThenInclude(dfd => dfd.DimDateIdEndNavigation).AsNoTracking() // DimFundingDecision related end date (DimDate)
                .Include(dfds => dfds.FactFieldValues)
                    .ThenInclude(ffv => ffv.DimFundingDecision)
                        .ThenInclude(dfd => dfd.DimTypeOfFunding).AsNoTracking() // DimFundingDecision related DimTypeOfFunding
                .Include(dfds => dfds.FactFieldValues)
                    .ThenInclude(ffv => ffv.DimFundingDecision)
                        .ThenInclude(dfd => dfd.DimCallProgramme).AsNoTracking() // DimFundingDecision related DimCallProgramme
                // DimPublication
                .Include(dfds => dfds.FactFieldValues)
                    .ThenInclude(ffv => ffv.DimPublication).AsNoTracking()
                // DimPid
                .Include(dfds => dfds.FactFieldValues)
                    .ThenInclude(ffv => ffv.DimPid).AsNoTracking()
                // DimPidIdOrcidPutCodeNavigation
                .Include(dfds => dfds.FactFieldValues)
                    .ThenInclude(ffv => ffv.DimPidIdOrcidPutCodeNavigation).AsNoTracking()
                // DimResearchActivity
                .Include(dfds => dfds.FactFieldValues)
                    .ThenInclude(ffv => ffv.DimResearchActivity).AsNoTracking()
                // DimEvent
                .Include(dfds => dfds.FactFieldValues)
                    .ThenInclude(ffv => ffv.DimEvent).AsNoTracking()
                // DimEducation
                .Include(dfds => dfds.FactFieldValues)
                    .ThenInclude(ffv => ffv.DimEducation)
                        .ThenInclude(de => de.DimStartDateNavigation).AsNoTracking()
                .Include(dfds => dfds.FactFieldValues)
                    .ThenInclude(ffv => ffv.DimEducation)
                        .ThenInclude(de => de.DimEndDateNavigation).AsNoTracking()
                // DimAffiliation
                .Include(dfds => dfds.FactFieldValues)
                   .ThenInclude(ffv => ffv.DimAffiliation)
                       .ThenInclude(da => da.StartDateNavigation).AsNoTracking()
                .Include(dfds => dfds.FactFieldValues)
                   .ThenInclude(ffv => ffv.DimAffiliation)
                       .ThenInclude(da => da.EndDateNavigation).AsNoTracking()
                .Include(dfds => dfds.FactFieldValues)
                   .ThenInclude(ffv => ffv.DimAffiliation)
                       .ThenInclude(da => da.DimOrganization).AsNoTracking()
                .Include(dfds => dfds.FactFieldValues)
                    .ThenInclude(ffv => ffv.DimAffiliation)
                        .ThenInclude(da => da.AffiliationTypeNavigation).AsNoTracking()
                // DimCompetence
                .Include(dfds => dfds.FactFieldValues)
                    .ThenInclude(ffv => ffv.DimCompetence).AsNoTracking()
                // DimResearchCommunity
                .Include(dfds => dfds.FactFieldValues)
                    .ThenInclude(ffv => ffv.DimResearchCommunity).AsNoTracking()
                .Include(dfds => dfds.FactFieldValues)
                    .ThenInclude(ffv => ffv.DimResearcherToResearchCommunity)
                        .ThenInclude(drtrc => drtrc.DimResearchCommunity).AsNoTracking()
                // DimTelephoneNumber
                .Include(dfds => dfds.FactFieldValues)
                    .ThenInclude(ffv => ffv.DimTelephoneNumber).AsNoTracking()
                // DimEmailAddrress
                .Include(dfds => dfds.FactFieldValues)
                    .ThenInclude(ffv => ffv.DimEmailAddrress).AsNoTracking()
                // DimResearcherDescription
                .Include(dfds => dfds.FactFieldValues)
                    .ThenInclude(ffv => ffv.DimResearcherDescription).AsNoTracking()
                // DimIdentifierlessData
                //.Include(dfds => dfds.FactFieldValues)
                //    .ThenInclude(ffv => ffv.DimIdentifierlessData).AsNoTracking() // TODO: update model to match SQL table
                // DimOrcidPublication
                .Include(dfds => dfds.FactFieldValues)
                    .ThenInclude(ffv => ffv.DimOrcidPublication).AsNoTracking()
                // DimKeyword
                .Include(dfds => dfds.FactFieldValues)
                    .ThenInclude(ffv => ffv.DimKeyword).AsNoTracking()
                // DimFieldOfScience
                .Include(dfds => dfds.FactFieldValues)
                    .ThenInclude(ffv => ffv.DimFieldOfScience).AsNoTracking()
                // DimResearchDataset
                .Include(dfds => dfds.FactFieldValues)
                    .ThenInclude(ffv => ffv.DimResearchDataset).AsNoTracking()
                        //.ThenInclude(drd => drd.FactContributions) // FactContribution related to DimResearchDataset
                            //.ThenInclude(fc => fc.DimName).AsNoTracking() // DimName related to FactContribution
                //.Include(dfds => dfds.FactFieldValues)
                //    .ThenInclude(ffv => ffv.DimResearchDataset)
                //        .ThenInclude(drd => drd.FactContributions) // FactContribution related to DimResearchDataset
                //            .ThenInclude(fc => fc.DimReferencedataActorRole).AsNoTracking() // DimName related to DimReferencedataActorRole
                .ToListAsync();

            var profileDataResponse = new ProfileEditorDataResponse() {};

            // Collect data from DimFieldDisplaySettings and FactFieldValues entities
            foreach (DimFieldDisplaySetting dfds in dimFieldDisplaySettings)
            {
                // Organization name translation
                var nameTranslationSource = _languageService.getNameTranslation(
                    nameFi: dfds.BrFieldDisplaySettingsDimRegisteredDataSources.First().DimRegisteredDataSource.DimOrganization.NameFi,
                    nameEn: dfds.BrFieldDisplaySettingsDimRegisteredDataSources.First().DimRegisteredDataSource.DimOrganization.NameEn,
                    nameSv: dfds.BrFieldDisplaySettingsDimRegisteredDataSources.First().DimRegisteredDataSource.DimOrganization.NameSv
                );

                // Source object containing registered data source and organization name.
                var source = new ProfileEditorSource()
                {
                    RegisteredDataSource = dfds.BrFieldDisplaySettingsDimRegisteredDataSources.First().DimRegisteredDataSource.Name,
                    Organization = new Organization()
                    {
                        NameFi = nameTranslationSource.NameFi,
                        NameEn = nameTranslationSource.NameEn,
                        NameSv = nameTranslationSource.NameSv
                    }
                };

                // FieldIdentifier defines what type of data the field contains.
                switch (dfds.FieldIdentifier)
                {
                    // Name
                    case Constants.FieldIdentifiers.PERSON_NAME:
                        var nameGroup = new ProfileEditorGroupName()
                        {
                            source = source,
                            items = new List<ProfileEditorItemName>() { },
                            groupMeta = new ProfileEditorGroupMeta()
                            {
                                Id = dfds.Id,
                                Type = Constants.FieldIdentifiers.PERSON_NAME,
                                Show = dfds.Show
                            }
                        };
                        foreach (FactFieldValue ffv in dfds.FactFieldValues)
                        {
                            nameGroup.items.Add(
                                new ProfileEditorItemName()
                                {
                                    FirstNames = ffv.DimName.FirstNames,
                                    LastName = ffv.DimName.LastName,
                                    itemMeta = new ProfileEditorItemMeta()
                                    {
                                        Id = ffv.DimNameId,
                                        Type = Constants.FieldIdentifiers.PERSON_FIRST_NAMES,
                                        Show = ffv.Show,
                                        PrimaryValue = ffv.PrimaryValue
                                    }
                                }
                            );
                        }
                        if (nameGroup.items.Count > 0)
                        {
                            profileDataResponse.personal.nameGroups.Add(nameGroup);
                        }
                        break;
                    // Other name
                    case Constants.FieldIdentifiers.PERSON_OTHER_NAMES:
                        var otherNameGroup = new ProfileEditorGroupOtherName()
                        {
                            source = source,
                            items = new List<ProfileEditorItemName>() { },
                            groupMeta = new ProfileEditorGroupMeta()
                            {
                                Id = dfds.Id,
                                Type = Constants.FieldIdentifiers.PERSON_OTHER_NAMES,
                                Show = dfds.Show
                            }
                        };
                        foreach (FactFieldValue ffv in dfds.FactFieldValues)
                        {
                            otherNameGroup.items.Add(
                                new ProfileEditorItemName()
                                {
                                    FullName = ffv.DimName.FullName,
                                    itemMeta = new ProfileEditorItemMeta()
                                    {
                                        Id = ffv.DimNameId,
                                        Type = Constants.FieldIdentifiers.PERSON_OTHER_NAMES,
                                        Show = ffv.Show,
                                        PrimaryValue = ffv.PrimaryValue
                                    }
                                }
                            );
                        }
                        if (otherNameGroup.items.Count > 0)
                        {
                            profileDataResponse.personal.otherNameGroups.Add(otherNameGroup);
                        }
                        break;
                    // Researcher description
                    case Constants.FieldIdentifiers.PERSON_RESEARCHER_DESCRIPTION:
                        var researcherDescriptionGroup = new ProfileEditorGroupResearcherDescription()
                        {
                            source = source,
                            items = new List<ProfileEditorItemResearcherDescription>() { },
                            groupMeta = new ProfileEditorGroupMeta()
                            {
                                Id = dfds.Id,
                                Type = Constants.FieldIdentifiers.PERSON_RESEARCHER_DESCRIPTION,
                                Show = dfds.Show
                            }
                        };
                        foreach (FactFieldValue ffv in dfds.FactFieldValues)
                        {
                            researcherDescriptionGroup.items.Add(
                                new ProfileEditorItemResearcherDescription()
                                {
                                    ResearchDescriptionEn = ffv.DimResearcherDescription.ResearchDescriptionEn,
                                    ResearchDescriptionFi = ffv.DimResearcherDescription.ResearchDescriptionFi,
                                    ResearchDescriptionSv = ffv.DimResearcherDescription.ResearchDescriptionSv,
                                    itemMeta = new ProfileEditorItemMeta()
                                    {
                                        Id = ffv.DimResearcherDescriptionId,
                                        Type = Constants.FieldIdentifiers.PERSON_RESEARCHER_DESCRIPTION,
                                        Show = ffv.Show,
                                        PrimaryValue = ffv.PrimaryValue
                                    }
                                }
                            );
                        }
                        if (researcherDescriptionGroup.items.Count > 0)
                        {
                            profileDataResponse.personal.researcherDescriptionGroups.Add(researcherDescriptionGroup);
                        }
                        break;
                    // Web link
                    case Constants.FieldIdentifiers.PERSON_WEB_LINK:
                        var webLinkGroup = new ProfileEditorGroupWebLink()
                        {
                            source = source,
                            items = new List<ProfileEditorItemWebLink>() { },
                            groupMeta = new ProfileEditorGroupMeta()
                            {
                                Id = dfds.Id,
                                Type = Constants.FieldIdentifiers.PERSON_WEB_LINK,
                                Show = dfds.Show
                            }
                        };
                        foreach (FactFieldValue ffv in dfds.FactFieldValues)
                        {
                            webLinkGroup.items.Add(
                                new ProfileEditorItemWebLink()
                                {
                                    Url = ffv.DimWebLink.Url,
                                    LinkLabel = ffv.DimWebLink.LinkLabel,
                                    itemMeta = new ProfileEditorItemMeta()
                                    {
                                        Id = ffv.DimWebLinkId,
                                        Type = Constants.FieldIdentifiers.PERSON_WEB_LINK,
                                        Show = ffv.Show,
                                        PrimaryValue = ffv.PrimaryValue
                                    }
                                }
                            );
                        }
                        if (webLinkGroup.items.Count > 0)
                        {
                            profileDataResponse.personal.webLinkGroups.Add(webLinkGroup);
                        }
                        break;
                    // Email address
                    case Constants.FieldIdentifiers.PERSON_EMAIL_ADDRESS:
                        var emailGroup = new ProfileEditorGroupEmail()
                        {
                            source = source,
                            items = new List<ProfileEditorItemEmail>() { },
                            groupMeta = new ProfileEditorGroupMeta()
                            {
                                Id = dfds.Id,
                                Type = Constants.FieldIdentifiers.PERSON_EMAIL_ADDRESS,
                                Show = dfds.Show
                            }
                        };
                        foreach (FactFieldValue ffv in dfds.FactFieldValues)
                        {
                            emailGroup.items.Add(
                                new ProfileEditorItemEmail()
                                {
                                    Value = ffv.DimEmailAddrress.Email,
                                    itemMeta = new ProfileEditorItemMeta()
                                    {
                                        Id = ffv.DimEmailAddrressId,
                                        Type = Constants.FieldIdentifiers.PERSON_EMAIL_ADDRESS,
                                        Show = ffv.Show,
                                        PrimaryValue = ffv.PrimaryValue
                                    }
                                }
                            );
                        }
                        if (emailGroup.items.Count > 0)
                        {
                            profileDataResponse.personal.emailGroups.Add(emailGroup);
                        }
                        break;
                    // Telephone number
                    case Constants.FieldIdentifiers.PERSON_TELEPHONE_NUMBER:
                        var telephoneNumberGroup = new ProfileEditorGroupTelephoneNumber()
                        {
                            source = source,
                            items = new List<ProfileEditorItemTelephoneNumber>() { },
                            groupMeta = new ProfileEditorGroupMeta()
                            {
                                Id = dfds.Id,
                                Type = Constants.FieldIdentifiers.PERSON_TELEPHONE_NUMBER,
                                Show = dfds.Show
                            }
                        };
                        foreach (FactFieldValue ffv in dfds.FactFieldValues)
                        {
                            telephoneNumberGroup.items.Add(
                                new ProfileEditorItemTelephoneNumber()
                                {
                                    Value = ffv.DimTelephoneNumber.TelephoneNumber,
                                    itemMeta = new ProfileEditorItemMeta()
                                    {
                                        Id = ffv.DimTelephoneNumberId,
                                        Type = Constants.FieldIdentifiers.PERSON_TELEPHONE_NUMBER,
                                        Show = ffv.Show,
                                        PrimaryValue = ffv.PrimaryValue
                                    }
                                }
                            );
                        }
                        if (telephoneNumberGroup.items.Count > 0)
                        {
                            profileDataResponse.personal.telephoneNumberGroups.Add(telephoneNumberGroup);
                        }
                        break;
                    // Field of science
                    case Constants.FieldIdentifiers.PERSON_FIELD_OF_SCIENCE:
                        var fieldOfScienceGroup = new ProfileEditorGroupFieldOfScience()
                        {
                            source = source,
                            items = new List<ProfileEditorItemFieldOfScience>() { },
                            groupMeta = new ProfileEditorGroupMeta()
                            {
                                Id = dfds.Id,
                                Type = Constants.FieldIdentifiers.PERSON_FIELD_OF_SCIENCE,
                                Show = dfds.Show
                            }
                        };
                        foreach (FactFieldValue ffv in dfds.FactFieldValues)
                        {
                            // Name translation service ensures that none of the language fields is empty.
                            var nameTranslationFieldOfScience = _languageService.getNameTranslation(
                                nameFi: ffv.DimFieldOfScience.NameFi,
                                nameEn: ffv.DimFieldOfScience.NameEn,
                                nameSv: ffv.DimFieldOfScience.NameSv
                            );

                            fieldOfScienceGroup.items.Add(
                                new ProfileEditorItemFieldOfScience()
                                {
                                    NameFi = nameTranslationFieldOfScience.NameFi,
                                    NameEn = nameTranslationFieldOfScience.NameEn,
                                    NameSv = nameTranslationFieldOfScience.NameSv,
                                    itemMeta = new ProfileEditorItemMeta()
                                    {
                                        Id = ffv.DimFieldOfScienceId,
                                        Type = Constants.FieldIdentifiers.PERSON_FIELD_OF_SCIENCE,
                                        Show = ffv.Show,
                                        PrimaryValue = ffv.PrimaryValue
                                    }
                                }
                            );
                        }
                        if (fieldOfScienceGroup.items.Count > 0)
                        {
                            profileDataResponse.personal.fieldOfScienceGroups.Add(fieldOfScienceGroup);
                        }
                        break;
                    // Keyword
                    case Constants.FieldIdentifiers.PERSON_KEYWORD:
                        var keywordGroup = new ProfileEditorGroupKeyword()
                        {
                            source = source,
                            items = new List<ProfileEditorItemKeyword>() { },
                            groupMeta = new ProfileEditorGroupMeta()
                            {
                                Id = dfds.Id,
                                Type = Constants.FieldIdentifiers.PERSON_KEYWORD,
                                Show = dfds.Show
                            }
                        };
                        foreach (FactFieldValue ffv in dfds.FactFieldValues)
                        {
                            keywordGroup.items.Add(
                                new ProfileEditorItemKeyword()
                                {
                                    Value = ffv.DimKeyword.Keyword,
                                    itemMeta = new ProfileEditorItemMeta()
                                    {
                                        Id = ffv.DimKeywordId,
                                        Type = Constants.FieldIdentifiers.PERSON_KEYWORD,
                                        Show = ffv.Show,
                                        PrimaryValue = ffv.PrimaryValue
                                    }
                                }
                            );
                        }
                        if (keywordGroup.items.Count > 0)
                        {
                            profileDataResponse.personal.keywordGroups.Add(keywordGroup);
                        }
                        break;
                    // External identifier
                    case Constants.FieldIdentifiers.PERSON_EXTERNAL_IDENTIFIER:
                        var externalIdentifierGroup = new ProfileEditorGroupExternalIdentifier()
                        {
                            source = source,
                            items = new List<ProfileEditorItemExternalIdentifier>() { },
                            groupMeta = new ProfileEditorGroupMeta()
                            {
                                Id = dfds.Id,
                                Type = Constants.FieldIdentifiers.PERSON_EXTERNAL_IDENTIFIER,
                                Show = dfds.Show
                            }
                        };
                        foreach (FactFieldValue ffv in dfds.FactFieldValues)
                        {
                            externalIdentifierGroup.items.Add(
                                new ProfileEditorItemExternalIdentifier()
                                {
                                    PidContent = ffv.DimPid.PidContent,
                                    PidType = ffv.DimPid.PidType,
                                    itemMeta = new ProfileEditorItemMeta()
                                    {
                                        Id = ffv.DimPidId,
                                        Type = Constants.FieldIdentifiers.PERSON_EXTERNAL_IDENTIFIER,
                                        Show = ffv.Show,
                                        PrimaryValue = ffv.PrimaryValue
                                    }
                                }
                            );
                        }
                        if (externalIdentifierGroup.items.Count > 0)
                        {
                            profileDataResponse.personal.externalIdentifierGroups.Add(externalIdentifierGroup);
                        }
                        break;
                    // Role in researcher community
                    case Constants.FieldIdentifiers.ACTIVITY_ROLE_IN_RESERCH_COMMUNITY:
                        // TODO
                        break;
                    // Affiliation
                    case Constants.FieldIdentifiers.ACTIVITY_AFFILIATION:
                        var affiliationGroup = new ProfileEditorGroupAffiliation()
                        {
                            source = source,
                            items = new List<ProfileEditorItemAffiliation>() { },
                            groupMeta = new ProfileEditorGroupMeta()
                            {
                                Id = dfds.Id,
                                Type = Constants.FieldIdentifiers.ACTIVITY_AFFILIATION,
                                Show = dfds.Show
                            }
                        };
                        foreach (FactFieldValue ffv in dfds.FactFieldValues)
                        {
                            // Name translation service ensures that none of the language fields is empty.
                            var nameTranslationAffiliationOrganization = _languageService.getNameTranslation(
                                nameFi: ffv.DimAffiliation.DimOrganization.NameFi,
                                nameEn: ffv.DimAffiliation.DimOrganization.NameEn,
                                nameSv: ffv.DimAffiliation.DimOrganization.NameSv
                            );
                            var nameTranslationPositionName = _languageService.getNameTranslation(
                                nameFi: ffv.DimAffiliation.PositionNameFi,
                                nameEn: ffv.DimAffiliation.PositionNameEn,
                                nameSv: ffv.DimAffiliation.PositionNameSv
                            );

                            // TODO: demo version stores ORDCID affiliation department name in DimOrganization.NameUnd
                            var nameTranslationAffiliationDepartment = new NameTranslation()
                            {
                                NameFi = "",
                                NameEn = "",
                                NameSv = ""
                            };
                            if (ffv.DimAffiliation.DimOrganization.SourceId == Constants.SourceIdentifiers.ORCID)
                            {
                                nameTranslationAffiliationDepartment = _languageService.getNameTranslation(
                                    "",
                                    nameEn: ffv.DimAffiliation.DimOrganization.NameUnd,
                                    ""
                                );
                            }

                            var affiliation = new ProfileEditorItemAffiliation()
                            {
                                // TODO: DimOrganization handling
                                OrganizationNameFi = nameTranslationAffiliationOrganization.NameFi,
                                OrganizationNameEn = nameTranslationAffiliationOrganization.NameEn,
                                OrganizationNameSv = nameTranslationAffiliationOrganization.NameSv,
                                DepartmentNameFi = nameTranslationAffiliationDepartment.NameFi,
                                DepartmentNameEn = nameTranslationAffiliationDepartment.NameEn,
                                DepartmentNameSv = nameTranslationAffiliationDepartment.NameSv,
                                PositionNameFi = nameTranslationPositionName.NameFi,
                                PositionNameEn = nameTranslationPositionName.NameEn,
                                PositionNameSv = nameTranslationPositionName.NameSv,
                                Type = ffv.DimAffiliation.AffiliationTypeNavigation.NameFi,
                                StartDate = new ProfileEditorItemDate()
                                {
                                    Year = ffv.DimAffiliation.StartDateNavigation.Year,
                                    Month = ffv.DimAffiliation.StartDateNavigation.Month,
                                    Day = ffv.DimAffiliation.StartDateNavigation.Day
                                },
                                itemMeta = new ProfileEditorItemMeta()
                                {
                                    Id = ffv.DimAffiliationId,
                                    Type = Constants.FieldIdentifiers.ACTIVITY_AFFILIATION,
                                    Show = ffv.Show,
                                    PrimaryValue = ffv.PrimaryValue
                                }
                            };

                            // Affiliation EndDate can be null
                            if (ffv.DimAffiliation.EndDateNavigation != null)
                            {
                                affiliation.EndDate = new ProfileEditorItemDate()
                                {
                                    Year = ffv.DimAffiliation.EndDateNavigation.Year,
                                    Month = ffv.DimAffiliation.EndDateNavigation.Month,
                                    Day = ffv.DimAffiliation.EndDateNavigation.Day
                                };
                            }
                            affiliationGroup.items.Add(affiliation);
                        }
                        if (affiliationGroup.items.Count > 0)
                        {
                            profileDataResponse.activity.affiliationGroups.Add(affiliationGroup);
                        }
                        break;
                    // Education
                    case Constants.FieldIdentifiers.ACTIVITY_EDUCATION:
                        var educationGroup = new ProfileEditorGroupEducation()
                        {
                            source = source,
                            items = new List<ProfileEditorItemEducation>() { },
                            groupMeta = new ProfileEditorGroupMeta()
                            {
                                Id = dfds.Id,
                                Type = Constants.FieldIdentifiers.ACTIVITY_EDUCATION,
                                Show = dfds.Show
                            }
                        };
                        foreach (FactFieldValue ffv in dfds.FactFieldValues)
                        {
                            // Name translation service ensures that none of the language fields is empty.
                            var nameTraslationEducation = _languageService.getNameTranslation(
                                nameFi: ffv.DimEducation.NameFi,
                                nameEn: ffv.DimEducation.NameEn,
                                nameSv: ffv.DimEducation.NameSv
                            );

                            var education = new ProfileEditorItemEducation()
                            {
                                NameFi = nameTraslationEducation.NameFi,
                                NameEn = nameTraslationEducation.NameEn,
                                NameSv = nameTraslationEducation.NameSv,
                                DegreeGrantingInstitutionName = ffv.DimEducation.DegreeGrantingInstitutionName,
                                itemMeta = new ProfileEditorItemMeta()
                                {
                                    Id = ffv.DimEducationId,
                                    Type = Constants.FieldIdentifiers.ACTIVITY_EDUCATION,
                                    Show = ffv.Show,
                                    PrimaryValue = ffv.PrimaryValue
                                }
                            };
                            // Education StartDate can be null
                            if (ffv.DimEducation.DimStartDateNavigation != null)
                            {
                                education.StartDate = new ProfileEditorItemDate()
                                {
                                    Year = ffv.DimEducation.DimStartDateNavigation.Year,
                                    Month = ffv.DimEducation.DimStartDateNavigation.Month,
                                    Day = ffv.DimEducation.DimStartDateNavigation.Day
                                };
                            }
                            // Education EndDate can be null
                            if (ffv.DimEducation.DimEndDateNavigation != null)
                            {
                                education.EndDate = new ProfileEditorItemDate()
                                {
                                    Year = ffv.DimEducation.DimEndDateNavigation.Year,
                                    Month = ffv.DimEducation.DimEndDateNavigation.Month,
                                    Day = ffv.DimEducation.DimEndDateNavigation.Day
                                };
                            }
                            educationGroup.items.Add(education);
                        }
                        if (educationGroup.items.Count > 0)
                        { 
                            profileDataResponse.activity.educationGroups.Add(educationGroup);
                        }
                        break;
                    // Publication
                    case Constants.FieldIdentifiers.ACTIVITY_PUBLICATION:
                        var publicationGroup = new ProfileEditorGroupPublication()
                        {
                            source = source,
                            items = new List<ProfileEditorItemPublication>() { },
                            groupMeta = new ProfileEditorGroupMeta()
                            {
                                Id = dfds.Id,
                                Type = Constants.FieldIdentifiers.ACTIVITY_PUBLICATION,
                                Show = dfds.Show
                            }
                        };
                        foreach (FactFieldValue ffv in dfds.FactFieldValues)
                        {
                            publicationGroup.items.Add(
                                new ProfileEditorItemPublication()
                                {
                                    PublicationId = ffv.DimPublication.PublicationId,
                                    PublicationName = ffv.DimPublication.PublicationName,
                                    PublicationYear = ffv.DimPublication.PublicationYear,
                                    Doi = ffv.DimPublication.DoiHandle,
                                    itemMeta = new ProfileEditorItemMeta()
                                    {
                                        Id = ffv.DimPublicationId,
                                        Type = Constants.FieldIdentifiers.ACTIVITY_PUBLICATION,
                                        Show = ffv.Show,
                                        PrimaryValue = ffv.PrimaryValue
                                    }
                                }
                            );
                        }
                        if (publicationGroup.items.Count > 0)
                        {
                            profileDataResponse.activity.publicationGroups.Add(publicationGroup);
                        }
                        break;
                    // Publication (ORCID)
                    case Constants.FieldIdentifiers.ACTIVITY_PUBLICATION_ORCID:
                        var orcidPublicationGroup = new ProfileEditorGroupPublication()
                        {
                            source = source,
                            items = new List<ProfileEditorItemPublication>() { },
                            groupMeta = new ProfileEditorGroupMeta()
                            {
                                Id = dfds.Id,
                                Type = Constants.FieldIdentifiers.ACTIVITY_PUBLICATION_ORCID,
                                Show = dfds.Show
                            }
                        };
                        foreach (FactFieldValue ffv in dfds.FactFieldValues)
                        {      
                            orcidPublicationGroup.items.Add(

                                new ProfileEditorItemPublication()
                                {
                                    PublicationId = ffv.DimOrcidPublication.PublicationId,
                                    PublicationName = ffv.DimOrcidPublication.PublicationName,
                                    PublicationYear = ffv.DimOrcidPublication.PublicationYear,
                                    Doi = ffv.DimOrcidPublication.DoiHandle,
                                    itemMeta = new ProfileEditorItemMeta()
                                    {
                                        Id = ffv.DimOrcidPublicationId,
                                        Type = Constants.FieldIdentifiers.ACTIVITY_PUBLICATION_ORCID,
                                        Show = ffv.Show,
                                        PrimaryValue = ffv.PrimaryValue
                                    }
                                }
                            );
                        }
                        if (orcidPublicationGroup.items.Count > 0)
                        {
                            profileDataResponse.activity.publicationGroups.Add(orcidPublicationGroup);
                        }
                        break;
                    // Funding decision
                    case Constants.FieldIdentifiers.ACTIVITY_FUNDING_DECISION:
                        var fundingDecisionGroup = new ProfileEditorGroupFundingDecision()
                        {
                            source = source,
                            items = new List<ProfileEditorItemFundingDecision>() { },
                            groupMeta = new ProfileEditorGroupMeta()
                            {
                                Id = dfds.Id,
                                Type = Constants.FieldIdentifiers.ACTIVITY_FUNDING_DECISION,
                                Show = dfds.Show
                            }
                        };
                        foreach (FactFieldValue ffv in dfds.FactFieldValues)
                        {
                            // Name translation service ensures that none of the language fields is empty.
                            var nameTraslationFundingDecision_ProjectName = _languageService.getNameTranslation(
                                nameFi: ffv.DimFundingDecision.NameFi,
                                nameSv: ffv.DimFundingDecision.NameSv,
                                nameEn: ffv.DimFundingDecision.NameEn
                            );
                            var nameTraslationFundingDecision_ProjectDescription = _languageService.getNameTranslation(
                                nameFi: ffv.DimFundingDecision.DescriptionFi,
                                nameSv: ffv.DimFundingDecision.DescriptionSv,
                                nameEn: ffv.DimFundingDecision.DescriptionEn
                            );
                            var nameTraslationFundingDecision_FunderName = _languageService.getNameTranslation(
                                nameFi: ffv.DimFundingDecision.DimOrganizationIdFunderNavigation.NameFi,
                                nameSv: ffv.DimFundingDecision.DimOrganizationIdFunderNavigation.NameSv,
                                nameEn: ffv.DimFundingDecision.DimOrganizationIdFunderNavigation.NameEn
                            );
                            var nameTranslationFundingDecision_TypeOfFunding = _languageService.getNameTranslation(
                                nameFi: ffv.DimFundingDecision.DimTypeOfFunding.NameFi,
                                nameSv: ffv.DimFundingDecision.DimTypeOfFunding.NameSv,
                                nameEn: ffv.DimFundingDecision.DimTypeOfFunding.NameEn
                            );
                            var nameTranslationFundingDecision_CallProgramme = _languageService.getNameTranslation(
                                nameFi: ffv.DimFundingDecision.DimCallProgramme.NameFi,
                                nameSv: ffv.DimFundingDecision.DimCallProgramme.NameSv,
                                nameEn: ffv.DimFundingDecision.DimCallProgramme.NameEn
                            );

                            var fundingDecision = new ProfileEditorItemFundingDecision()
                            {
                                ProjectId = ffv.DimFundingDecision.Id,
                                ProjectAcronym = ffv.DimFundingDecision.Acronym,
                                ProjectNameFi = nameTraslationFundingDecision_ProjectName.NameFi,
                                ProjectNameSv = nameTraslationFundingDecision_ProjectName.NameSv,
                                ProjectNameEn = nameTraslationFundingDecision_ProjectName.NameEn,
                                ProjectDescriptionFi = nameTraslationFundingDecision_ProjectDescription.NameFi,
                                ProjectDescriptionSv = nameTraslationFundingDecision_ProjectDescription.NameSv,
                                ProjectDescriptionEn = nameTraslationFundingDecision_ProjectDescription.NameEn,
                                FunderNameFi = nameTraslationFundingDecision_FunderName.NameFi,
                                FunderNameSv = nameTraslationFundingDecision_FunderName.NameSv,
                                FunderNameEn = nameTraslationFundingDecision_FunderName.NameEn,
                                FunderProjectNumber = ffv.DimFundingDecision.FunderProjectNumber,
                                TypeOfFundingNameFi = nameTranslationFundingDecision_TypeOfFunding.NameFi,
                                TypeOfFundingNameSv = nameTranslationFundingDecision_TypeOfFunding.NameSv,
                                TypeOfFundingNameEn = nameTranslationFundingDecision_TypeOfFunding.NameEn,
                                CallProgrammeNameFi = nameTranslationFundingDecision_CallProgramme.NameFi,
                                CallProgrammeNameSv = nameTranslationFundingDecision_CallProgramme.NameSv,
                                CallProgrammeNameEn = nameTranslationFundingDecision_CallProgramme.NameEn,
                                AmountInEur = ffv.DimFundingDecision.AmountInEur,
                                itemMeta = new ProfileEditorItemMeta()
                                {
                                    Id = ffv.DimFundingDecisionId,
                                    Type = Constants.FieldIdentifiers.ACTIVITY_FUNDING_DECISION,
                                    Show = ffv.Show,
                                    PrimaryValue = ffv.PrimaryValue
                                }
                            };

                            // Funding decision start year
                            if (ffv.DimFundingDecision.DimDateIdStartNavigation != null && ffv.DimFundingDecision.DimDateIdStartNavigation.Year > 0)
                            {
                                fundingDecision.FundingStartYear = ffv.DimFundingDecision.DimDateIdStartNavigation.Year;
                            }
                            // Funding decision end year
                            if (ffv.DimFundingDecision.DimDateIdEndNavigation != null && ffv.DimFundingDecision.DimDateIdEndNavigation.Year > 0)
                            {
                                fundingDecision.FundingEndYear = ffv.DimFundingDecision.DimDateIdEndNavigation.Year;
                            }

                            fundingDecisionGroup.items.Add(fundingDecision);
                        }
                        if (fundingDecisionGroup.items.Count > 0)
                        {
                            profileDataResponse.activity.fundingDecisionGroups.Add(fundingDecisionGroup);
                        }
                        break;
                    // Research dataset
                    case Constants.FieldIdentifiers.ACTIVITY_RESEARCH_DATASET:
                        var researchDatasetGroup = new ProfileEditorGroupResearchDataset()
                        {
                            source = source,
                            items = new List<ProfileEditorItemResearchDataset>() { },
                            groupMeta = new ProfileEditorGroupMeta()
                            {
                                Id = dfds.Id,
                                Type = Constants.FieldIdentifiers.ACTIVITY_RESEARCH_DATASET,
                                Show = dfds.Show
                            }
                        };
                        foreach (FactFieldValue ffv in dfds.FactFieldValues)
                        {
                            // Name translation service ensures that none of the language fields is empty.
                            var nameTraslationResearchDataset_Name = _languageService.getNameTranslation(
                                nameFi: ffv.DimResearchDataset.NameFi,
                                nameSv: ffv.DimResearchDataset.NameSv,
                                nameEn: ffv.DimResearchDataset.NameEn
                            );
                            var nameTraslationResearchDataset_Description = _languageService.getNameTranslation(
                                nameFi: ffv.DimResearchDataset.DescriptionFi,
                                nameSv: ffv.DimResearchDataset.DescriptionSv,
                                nameEn: ffv.DimResearchDataset.DescriptionEn
                            );

                            // Get values from DimPid. There is no FK between DimResearchDataset and DimPid,
                            // so the query must be done separately.
                            var dimPids = await _ttvContext.DimPids.Where(dp => dp.DimResearchDatasetId == ffv.DimResearchDatasetId && ffv.DimResearchDatasetId > -1).AsNoTracking().ToListAsync();

                            var preferredIdentifiers = new List<ProfileEditorPreferredIdentifier>();
                            foreach (DimPid dimPid in dimPids)
                            {
                                preferredIdentifiers.Add(
                                    new ProfileEditorPreferredIdentifier()
                                    {
                                        PidType = dimPid.PidType,
                                        PidContent = dimPid.PidContent
                                    }
                                );
                            }

                            // TODO: add properties according to ElasticSearch index
                            var researchDataset = new ProfileEditorItemResearchDataset()
                            {
                                Actor = new List<ProfileEditorActor>(),
                                Identifier = ffv.DimResearchDataset.LocalIdentifier,
                                NameFi = nameTraslationResearchDataset_Name.NameFi,
                                NameSv = nameTraslationResearchDataset_Name.NameSv,
                                NameEn = nameTraslationResearchDataset_Name.NameEn,
                                DescriptionFi = nameTraslationResearchDataset_Description.NameFi,
                                DescriptionSv = nameTraslationResearchDataset_Description.NameSv,
                                DescriptionEn = nameTraslationResearchDataset_Description.NameEn,
                                PreferredIdentifiers = preferredIdentifiers,
                                itemMeta = new ProfileEditorItemMeta()
                                {
                                    Id = ffv.DimResearchDatasetId,
                                    Type = Constants.FieldIdentifiers.ACTIVITY_RESEARCH_DATASET,
                                    Show = ffv.Show,
                                    PrimaryValue = ffv.PrimaryValue
                                }
                            };

                            // DatasetCreated
                            if (ffv.DimResearchDataset.DatasetCreated != null)
                            {
                                researchDataset.DatasetCreated = ffv.DimResearchDataset.DatasetCreated.Value.Year;
                            }

                            // Fill actors list
                            foreach(FactContribution fc in ffv.DimResearchDataset.FactContributions)
                            {
                                researchDataset.Actor.Add(
                                    new ProfileEditorActor()
                                    {
                                        actorRole = int.Parse(fc.DimReferencedataActorRole.CodeValue),
                                        actorRoleNameFi = fc.DimReferencedataActorRole.NameFi,
                                        actorRoleNameSv = fc.DimReferencedataActorRole.NameSv,
                                        actorRoleNameEn = fc.DimReferencedataActorRole.NameEn
                                    }
                                );
                            }

                            researchDatasetGroup.items.Add(researchDataset);
                        }
                        if (researchDatasetGroup.items.Count > 0)
                        {
                            profileDataResponse.activity.researchDatasetGroups.Add(researchDatasetGroup);
                        }
                        break;
                    default:
                        break;
                }
            }

            return Ok(new ApiResponseProfileDataGet(success: true, reason: "", data: profileDataResponse, fromCache: false));
            */
        }
    }
}