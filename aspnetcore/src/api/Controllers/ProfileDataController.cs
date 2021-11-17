using api.Services;
using api.Models;
using api.Models.Ttv;
using api.Models.ProfileEditor;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Memory;
using System;
using Microsoft.Extensions.Logging;
using Nest;
using Microsoft.AspNetCore.Http;

namespace api.Controllers
{
    /*
     * ProfileDataController implements profile editor API commands, such as getting editor data and setting data visibility.
     */
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ProfileDataController : TtvControllerBase
    {
        private readonly TtvContext _ttvContext;
        private readonly UserProfileService _userProfileService;
        private readonly ElasticsearchService _elasticsearchService;
        private readonly TtvSqlService _ttvSqlService;
        private readonly IMemoryCache _cache;
        private readonly ILogger<UserProfileController> _logger;
        private readonly BackgroundElasticsearchPersonUpdateQueue _backgroundElasticsearchPersonUpdateQueue;
        private readonly BackgroundProfiledata _backgroundProfiledata;

        public ProfileDataController(TtvContext ttvContext, UserProfileService userProfileService, ElasticsearchService elasticsearchService, TtvSqlService ttvSqlService, IMemoryCache memoryCache, ILogger<UserProfileController> logger, BackgroundElasticsearchPersonUpdateQueue backgroundElasticsearchPersonUpdateQueue, BackgroundProfiledata backgroundProfiledata)
        {
            _ttvContext = ttvContext;
            _userProfileService = userProfileService;
            _cache = memoryCache;
            _elasticsearchService = elasticsearchService;
            _ttvSqlService = ttvSqlService;
            _backgroundElasticsearchPersonUpdateQueue = backgroundElasticsearchPersonUpdateQueue;
            _backgroundProfiledata = backgroundProfiledata;
            _logger = logger;
        }

        /// <summary>
        /// Get profile data.
        /// </summary>
        [HttpGet]
        [ProducesResponseType(typeof(ApiResponseProfileDataGet), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
        public async Task<IActionResult> Get()
        {
            // Check that user profile exists.
            var orcidId = this.GetOrcidId();
            var userprofileId = await _userProfileService.GetUserprofileId(orcidId);
            if (userprofileId == -1)
            {
                return Ok(new ApiResponse(success: false, reason: "profile not found"));
            }

            // Send cached response, if exists. Cache key is ORCID ID
            ProfileEditorDataResponse cachedResponse;
            if (_cache.TryGetValue(orcidId, out cachedResponse))
            {
                return Ok(new ApiResponseProfileDataGet(success: true, reason: "", data: cachedResponse, fromCache: true));
            }


            // Get DimFieldDisplaySettings and related entities
            var dimFieldDisplaySettings = await _ttvContext.DimFieldDisplaySettings.Where(dfds => dfds.DimUserProfileId == userprofileId && dfds.FactFieldValues.Count() > 0)
                .Include(dfds => dfds.BrFieldDisplaySettingsDimRegisteredDataSources)
                    .ThenInclude(br => br.DimRegisteredDataSource)
                        .ThenInclude(drds => drds.DimOrganization).AsNoTracking()
                // DimName
                .Include(dfds => dfds.FactFieldValues)
                    .ThenInclude(ffv => ffv.DimName).AsNoTracking()
                // DimWebLink
                .Include(dfds => dfds.FactFieldValues)
                    .ThenInclude(ffv => ffv.DimWebLink).AsNoTracking()
                // DimFundingDecision
                .Include(dfds => dfds.FactFieldValues)
                    .ThenInclude(ffv => ffv.DimFundingDecision).AsNoTracking()
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
                .Include(dfds => dfds.FactFieldValues)
                    .ThenInclude(ffv => ffv.DimIdentifierlessData).AsNoTracking()
                // DimOrcidPublication
                .Include(dfds => dfds.FactFieldValues)
                    .ThenInclude(ffv => ffv.DimOrcidPublication).AsNoTracking()
                // DimKeyword
                .Include(dfds => dfds.FactFieldValues)
                    .ThenInclude(ffv => ffv.DimKeyword).AsNoTracking()
                // DimFieldOfScience
                .Include(dfds => dfds.FactFieldValues)
                    .ThenInclude(ffv => ffv.DimFieldOfScience).AsNoTracking()
                .ToListAsync();

            var profileDataResponse = new ProfileEditorDataResponse() {};

            // Collect data from DimFieldDisplaySettings and FactFieldValues entities
            foreach (DimFieldDisplaySetting dfds in dimFieldDisplaySettings)
            {
                // Source object containing registered data source and organization name.
                var source = new ProfileEditorSource()
                {
                    Id = dfds.BrFieldDisplaySettingsDimRegisteredDataSources.First().DimRegisteredDataSource.Id,
                    RegisteredDataSource = dfds.BrFieldDisplaySettingsDimRegisteredDataSources.First().DimRegisteredDataSource.Name,
                    Organization = new ProfileEditorSourceOrganization()
                    {
                        NameFi = dfds.BrFieldDisplaySettingsDimRegisteredDataSources.First().DimRegisteredDataSource.DimOrganization.NameFi,
                        NameEn = dfds.BrFieldDisplaySettingsDimRegisteredDataSources.First().DimRegisteredDataSource.DimOrganization.NameEn,
                        NameSv = dfds.BrFieldDisplaySettingsDimRegisteredDataSources.First().DimRegisteredDataSource.DimOrganization.NameSv
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
                            fieldOfScienceGroup.items.Add(
                                new ProfileEditorItemFieldOfScience()
                                {
                                    NameFi = ffv.DimFieldOfScience.NameFi,
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
                            var affiliation = new ProfileEditorItemAffiliation()
                            {
                                // TODO: DimOrganization handling
                                OrganizationNameFi = ffv.DimAffiliation.DimOrganization.NameFi,
                                OrganizationNameEn = ffv.DimAffiliation.DimOrganization.NameEn,
                                OrganizationNameSv = ffv.DimAffiliation.DimOrganization.NameSv,
                                PositionNameFi = ffv.DimAffiliation.PositionNameFi,
                                PositionNameEn = ffv.DimAffiliation.PositionNameEn,
                                PositionNameSv = ffv.DimAffiliation.PositionNameSv,
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
                            var education = new ProfileEditorItemEducation()
                            {
                                NameFi = ffv.DimEducation.NameFi,
                                NameEn = ffv.DimEducation.NameEn,
                                NameSv = ffv.DimEducation.NameSv,
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
                                    Doi = ffv.DimPublication.Doi,
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
                    default:
                        break;
                }
            }

            // Save response in cache
            var cacheEntryOptions = new MemoryCacheEntryOptions()
                // Keep in cache for this time, reset time if accessed.
                .SetSlidingExpiration(TimeSpan.FromSeconds(60));

            // Save data in cache. Cache key is ORCID ID.
            _cache.Set(orcidId, profileDataResponse, cacheEntryOptions);

            return Ok(new ApiResponseProfileDataGet(success: true, reason: "", data: profileDataResponse, fromCache: false));
        }



        /// <summary>
        /// Modify profile data.
        /// </summary>
        [HttpPatch]
        [ProducesResponseType(typeof(ApiResponseProfileDataPatch), StatusCodes.Status200OK)]
        public async Task<IActionResult> PatchMany([FromBody] ProfileEditorDataModificationRequest profileEditorDataModificationRequest)
        {
            // Return immediately if there is nothing to change.
            if (profileEditorDataModificationRequest.groups.Count == 0 && profileEditorDataModificationRequest.items.Count == 0)
            {
                return Ok(new ApiResponse(success: true));
            }

            // Check that user profile exists.
            var orcidId = this.GetOrcidId();
            var userprofileId = await _userProfileService.GetUserprofileId(orcidId);
            if (userprofileId == -1)
            {
                return Ok(new ApiResponse(success: false, reason: "profile not found"));
            }

            // Remove cached profile data response. Cache key is ORCID ID.
            _cache.Remove(orcidId);


            // Collect information about updated items to a response object, which will be sent in response.
            var profileEditorDataModificationResponse = new ProfileEditorDataModificationResponse();

            // Set 'Show' and 'PrimaryValue' in FactFieldValues
            foreach (ProfileEditorItemMeta profileEditorItemMeta in profileEditorDataModificationRequest.items.ToList())
            {
                var updateSql = _ttvSqlService.getSqlQuery_Update_FactFieldValues(userprofileId, profileEditorItemMeta);
                await _ttvContext.Database.ExecuteSqlRawAsync(updateSql);
                profileEditorDataModificationResponse.items.Add(profileEditorItemMeta);
            }

            // Update Elasticsearch index in a background task.
            _backgroundElasticsearchPersonUpdateQueue.QueueBackgroundWorkItem(async token =>
            {
                _logger.LogInformation($"Background task for updating {orcidId} started at {DateTime.UtcNow}");
                // Get Elasticsearch person entry from profile data.
                var person = await _backgroundProfiledata.GetProfiledataForElasticsearch(orcidId, userprofileId);
                // Update Elasticsearch person index.
                await _elasticsearchService.UpdateEntryInElasticsearchPersonIndex(orcidId, person);
                _logger.LogInformation($"Background task for updating {orcidId} ended at {DateTime.UtcNow}");
            });

            return Ok(new ApiResponseProfileDataPatch(success: true, reason: "", data: profileEditorDataModificationResponse, fromCache: false));
        }
    }
}