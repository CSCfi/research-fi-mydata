using api.Services;
using api.Models;
using api.Models.Ttv;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

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

        public ProfileDataController(TtvContext ttvContext, UserProfileService userProfileService)
        {
            _ttvContext = ttvContext;
            _userProfileService = userProfileService;
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            // Get userprofile
            var orcidId = this.GetOrcidId();
            var userprofileId = await _userProfileService.GetUserprofileId(orcidId);
            if (userprofileId == -1)
            {
                return Ok(new ApiResponse(success: false, reason: "profile not found"));
            }

            // Get DimUserProfile and related entities
            var dimUserProfile = await _ttvContext.DimUserProfiles
                .Include(dup => dup.DimFieldDisplaySettings)
                    .ThenInclude(dfds => dfds.BrFieldDisplaySettingsDimRegisteredDataSources)
                        .ThenInclude(br => br.DimRegisteredDataSource)
                            .ThenInclude(drds => drds.DimOrganization).AsNoTracking()
                .Include(dup => dup.DimFieldDisplaySettings)
                    .ThenInclude(dfds => dfds.FactFieldValues)
                        .ThenInclude(ffv => ffv.DimName).AsNoTracking()
                .Include(dup => dup.DimFieldDisplaySettings)
                    .ThenInclude(dfds => dfds.FactFieldValues)
                        .ThenInclude(ffv => ffv.DimWebLink).AsNoTracking()
                .Include(dup => dup.DimFieldDisplaySettings)
                    .ThenInclude(dfds => dfds.FactFieldValues)
                        .ThenInclude(ffv => ffv.DimFundingDecision).AsNoTracking()
                .Include(dup => dup.DimFieldDisplaySettings)
                    .ThenInclude(dfds => dfds.FactFieldValues)
                        .ThenInclude(ffv => ffv.DimPublication).AsNoTracking()
                .Include(dup => dup.DimFieldDisplaySettings)
                    .ThenInclude(dfds => dfds.FactFieldValues)
                        .ThenInclude(ffv => ffv.DimPid).AsNoTracking()
                .Include(dup => dup.DimFieldDisplaySettings)
                    .ThenInclude(dfds => dfds.FactFieldValues)
                        .ThenInclude(ffv => ffv.DimPidIdOrcidPutCodeNavigation).AsNoTracking()
                .Include(dup => dup.DimFieldDisplaySettings)
                    .ThenInclude(dfds => dfds.FactFieldValues)
                        .ThenInclude(ffv => ffv.DimResearchActivity).AsNoTracking()
                .Include(dup => dup.DimFieldDisplaySettings)
                    .ThenInclude(dfds => dfds.FactFieldValues)
                        .ThenInclude(ffv => ffv.DimEvent).AsNoTracking()
                .Include(dup => dup.DimFieldDisplaySettings)
                    .ThenInclude(dfds => dfds.FactFieldValues)
                        .ThenInclude(ffv => ffv.DimEducation)
                            .ThenInclude(de => de.DimStartDateNavigation).AsNoTracking()
                .Include(dup => dup.DimFieldDisplaySettings)
                    .ThenInclude(dfds => dfds.FactFieldValues)
                        .ThenInclude(ffv => ffv.DimEducation)
                            .ThenInclude(de => de.DimEndDateNavigation).AsNoTracking()
                .Include(dup => dup.DimFieldDisplaySettings)
                    .ThenInclude(dfds => dfds.FactFieldValues)
                        .ThenInclude(ffv => ffv.DimAffiliation)
                            .ThenInclude(da => da.StartDateNavigation).AsNoTracking()
                .Include(dup => dup.DimFieldDisplaySettings)
                    .ThenInclude(dfds => dfds.FactFieldValues)
                        .ThenInclude(ffv => ffv.DimAffiliation)
                            .ThenInclude(da => da.EndDateNavigation).AsNoTracking()
                .Include(dup => dup.DimFieldDisplaySettings)
                    .ThenInclude(dfds => dfds.FactFieldValues)
                        .ThenInclude(ffv => ffv.DimAffiliation)
                            .ThenInclude(da => da.DimOrganization).AsNoTracking()
                .Include(dup => dup.DimFieldDisplaySettings)
                    .ThenInclude(dfds => dfds.FactFieldValues)
                        .ThenInclude(ffv => ffv.DimAffiliation)
                            .ThenInclude(da => da.AffiliationTypeNavigation).AsNoTracking()
                .Include(dup => dup.DimFieldDisplaySettings)
                    .ThenInclude(dfds => dfds.FactFieldValues)
                        .ThenInclude(ffv => ffv.DimCompetence).AsNoTracking()
                .Include(dup => dup.DimFieldDisplaySettings)
                    .ThenInclude(dfds => dfds.FactFieldValues)
                        .ThenInclude(ffv => ffv.DimResearchCommunity).AsNoTracking()
                .Include(dup => dup.DimFieldDisplaySettings)
                    .ThenInclude(dfds => dfds.FactFieldValues)
                        .ThenInclude(ffv => ffv.DimResearcherToResearchCommunity)
                            .ThenInclude(drtrc => drtrc.DimResearchCommunity).AsNoTracking()
                .Include(dup => dup.DimFieldDisplaySettings)
                    .ThenInclude(dfds => dfds.FactFieldValues)
                        .ThenInclude(ffv => ffv.DimTelephoneNumber).AsNoTracking()
                .Include(dup => dup.DimFieldDisplaySettings)
                    .ThenInclude(dfds => dfds.FactFieldValues)
                        .ThenInclude(ffv => ffv.DimEmailAddrress).AsNoTracking()
                .Include(dup => dup.DimFieldDisplaySettings)
                    .ThenInclude(dfds => dfds.FactFieldValues)
                        .ThenInclude(ffv => ffv.DimResearcherDescription).AsNoTracking()
                .Include(dup => dup.DimFieldDisplaySettings)
                    .ThenInclude(dfds => dfds.FactFieldValues)
                        .ThenInclude(ffv => ffv.DimIdentifierlessData).AsNoTracking()
                .Include(dup => dup.DimFieldDisplaySettings)
                    .ThenInclude(dfds => dfds.FactFieldValues)
                        .ThenInclude(ffv => ffv.DimOrcidPublication).AsNoTracking()
                .Include(dup => dup.DimFieldDisplaySettings)
                    .ThenInclude(dfds => dfds.FactFieldValues)
                        .ThenInclude(ffv => ffv.DimKeyword).AsNoTracking()
                .Include(dup => dup.DimFieldDisplaySettings)
                    .ThenInclude(dfds => dfds.FactFieldValues)
                        .ThenInclude(ffv => ffv.DimFieldOfScience).AsNoTracking().AsSplitQuery().FirstOrDefaultAsync(up => up.Id == userprofileId);

            var profileDataResponse = new ProfileEditorDataResponse() {};

            // Collect data from DimFieldDisplaySettings and FactFieldValues entities
            foreach (DimFieldDisplaySetting dfds in dimUserProfile.DimFieldDisplaySettings)
            {
                // FieldIdentifier defines what type of data the field contains.
                switch (dfds.FieldIdentifier)
                {
                    case Constants.FieldIdentifiers.PERSON_NAME:
                        var nameGroup = new ProfileEditorGroupName()
                        {
                            source = new ProfileEditorSource()
                            {
                                Id = dfds.BrFieldDisplaySettingsDimRegisteredDataSources.First().DimRegisteredDataSource.Id,
                                RegisteredDataSource = dfds.BrFieldDisplaySettingsDimRegisteredDataSources.First().DimRegisteredDataSource.Name,
                                Organization = new ProfileEditorSourceOrganization()
                                {
                                    NameFi = dfds.BrFieldDisplaySettingsDimRegisteredDataSources.First().DimRegisteredDataSource.DimOrganization.NameFi,
                                    NameEn = dfds.BrFieldDisplaySettingsDimRegisteredDataSources.First().DimRegisteredDataSource.DimOrganization.NameEn,
                                    NameSv = dfds.BrFieldDisplaySettingsDimRegisteredDataSources.First().DimRegisteredDataSource.DimOrganization.NameSv
                                }
                            },
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
                    case Constants.FieldIdentifiers.PERSON_OTHER_NAMES:
                        var otherNameGroup = new ProfileEditorGroupOtherName()
                        {
                            source = new ProfileEditorSource()
                            {
                                Id = dfds.BrFieldDisplaySettingsDimRegisteredDataSources.First().DimRegisteredDataSource.Id,
                                RegisteredDataSource = dfds.BrFieldDisplaySettingsDimRegisteredDataSources.First().DimRegisteredDataSource.Name,
                                Organization = new ProfileEditorSourceOrganization()
                                {
                                    NameFi = dfds.BrFieldDisplaySettingsDimRegisteredDataSources.First().DimRegisteredDataSource.DimOrganization.NameFi,
                                    NameEn = dfds.BrFieldDisplaySettingsDimRegisteredDataSources.First().DimRegisteredDataSource.DimOrganization.NameEn,
                                    NameSv = dfds.BrFieldDisplaySettingsDimRegisteredDataSources.First().DimRegisteredDataSource.DimOrganization.NameSv
                                }
                            },
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
                    case Constants.FieldIdentifiers.PERSON_RESEARCHER_DESCRIPTION:
                        var researcherDescriptionGroup = new ProfileEditorGroupResearcherDescription()
                        {
                            source = new ProfileEditorSource()
                            {
                                Id = dfds.BrFieldDisplaySettingsDimRegisteredDataSources.First().DimRegisteredDataSource.Id,
                                RegisteredDataSource = dfds.BrFieldDisplaySettingsDimRegisteredDataSources.First().DimRegisteredDataSource.Name,
                                Organization = new ProfileEditorSourceOrganization()
                                {
                                    NameFi = dfds.BrFieldDisplaySettingsDimRegisteredDataSources.First().DimRegisteredDataSource.DimOrganization.NameFi,
                                    NameEn = dfds.BrFieldDisplaySettingsDimRegisteredDataSources.First().DimRegisteredDataSource.DimOrganization.NameEn,
                                    NameSv = dfds.BrFieldDisplaySettingsDimRegisteredDataSources.First().DimRegisteredDataSource.DimOrganization.NameSv
                                }
                            },
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
                    case Constants.FieldIdentifiers.PERSON_WEB_LINK:
                        var webLinkGroup = new ProfileEditorGroupWebLink()
                        {
                            source = new ProfileEditorSource()
                            {
                                Id = dfds.BrFieldDisplaySettingsDimRegisteredDataSources.First().DimRegisteredDataSource.Id,
                                RegisteredDataSource = dfds.BrFieldDisplaySettingsDimRegisteredDataSources.First().DimRegisteredDataSource.Name,
                                Organization = new ProfileEditorSourceOrganization()
                                {
                                    NameFi = dfds.BrFieldDisplaySettingsDimRegisteredDataSources.First().DimRegisteredDataSource.DimOrganization.NameFi,
                                    NameEn = dfds.BrFieldDisplaySettingsDimRegisteredDataSources.First().DimRegisteredDataSource.DimOrganization.NameEn,
                                    NameSv = dfds.BrFieldDisplaySettingsDimRegisteredDataSources.First().DimRegisteredDataSource.DimOrganization.NameSv
                                }
                            },
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
                    case Constants.FieldIdentifiers.PERSON_EMAIL_ADDRESS:
                        var emailGroup = new ProfileEditorGroupEmail()
                        {
                            source = new ProfileEditorSource()
                            {
                                Id = dfds.BrFieldDisplaySettingsDimRegisteredDataSources.First().DimRegisteredDataSource.Id,
                                RegisteredDataSource = dfds.BrFieldDisplaySettingsDimRegisteredDataSources.First().DimRegisteredDataSource.Name,
                                Organization = new ProfileEditorSourceOrganization()
                                {
                                    NameFi = dfds.BrFieldDisplaySettingsDimRegisteredDataSources.First().DimRegisteredDataSource.DimOrganization.NameFi,
                                    NameEn = dfds.BrFieldDisplaySettingsDimRegisteredDataSources.First().DimRegisteredDataSource.DimOrganization.NameEn,
                                    NameSv = dfds.BrFieldDisplaySettingsDimRegisteredDataSources.First().DimRegisteredDataSource.DimOrganization.NameSv
                                }
                            },
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
                    case Constants.FieldIdentifiers.PERSON_TELEPHONE_NUMBER:
                        var telephoneNumberGroup = new ProfileEditorGroupTelephoneNumber()
                        {
                            source = new ProfileEditorSource()
                            {
                                Id = dfds.BrFieldDisplaySettingsDimRegisteredDataSources.First().DimRegisteredDataSource.Id,
                                RegisteredDataSource = dfds.BrFieldDisplaySettingsDimRegisteredDataSources.First().DimRegisteredDataSource.Name,
                                Organization = new ProfileEditorSourceOrganization()
                                {
                                    NameFi = dfds.BrFieldDisplaySettingsDimRegisteredDataSources.First().DimRegisteredDataSource.DimOrganization.NameFi,
                                    NameEn = dfds.BrFieldDisplaySettingsDimRegisteredDataSources.First().DimRegisteredDataSource.DimOrganization.NameEn,
                                    NameSv = dfds.BrFieldDisplaySettingsDimRegisteredDataSources.First().DimRegisteredDataSource.DimOrganization.NameSv
                                }
                            },
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
                    case Constants.FieldIdentifiers.PERSON_FIELD_OF_SCIENCE:
                        var fieldOfScienceGroup = new ProfileEditorGroupFieldOfScience()
                        {
                            source = new ProfileEditorSource()
                            {
                                Id = dfds.BrFieldDisplaySettingsDimRegisteredDataSources.First().DimRegisteredDataSource.Id,
                                RegisteredDataSource = dfds.BrFieldDisplaySettingsDimRegisteredDataSources.First().DimRegisteredDataSource.Name,
                                Organization = new ProfileEditorSourceOrganization()
                                {
                                    NameFi = dfds.BrFieldDisplaySettingsDimRegisteredDataSources.First().DimRegisteredDataSource.DimOrganization.NameFi,
                                    NameEn = dfds.BrFieldDisplaySettingsDimRegisteredDataSources.First().DimRegisteredDataSource.DimOrganization.NameEn,
                                    NameSv = dfds.BrFieldDisplaySettingsDimRegisteredDataSources.First().DimRegisteredDataSource.DimOrganization.NameSv
                                }
                            },
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
                    case Constants.FieldIdentifiers.PERSON_KEYWORD:
                        var keywordGroup = new ProfileEditorGroupKeyword()
                        {
                            source = new ProfileEditorSource()
                            {
                                Id = dfds.BrFieldDisplaySettingsDimRegisteredDataSources.First().DimRegisteredDataSource.Id,
                                RegisteredDataSource = dfds.BrFieldDisplaySettingsDimRegisteredDataSources.First().DimRegisteredDataSource.Name,
                                Organization = new ProfileEditorSourceOrganization()
                                {
                                    NameFi = dfds.BrFieldDisplaySettingsDimRegisteredDataSources.First().DimRegisteredDataSource.DimOrganization.NameFi,
                                    NameEn = dfds.BrFieldDisplaySettingsDimRegisteredDataSources.First().DimRegisteredDataSource.DimOrganization.NameEn,
                                    NameSv = dfds.BrFieldDisplaySettingsDimRegisteredDataSources.First().DimRegisteredDataSource.DimOrganization.NameSv
                                }
                            },
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
                    case Constants.FieldIdentifiers.PERSON_EXTERNAL_IDENTIFIER:
                        var externalIdentifierGroup = new ProfileEditorGroupExternalIdentifier()
                        {
                            source = new ProfileEditorSource()
                            {
                                Id = dfds.BrFieldDisplaySettingsDimRegisteredDataSources.First().DimRegisteredDataSource.Id,
                                RegisteredDataSource = dfds.BrFieldDisplaySettingsDimRegisteredDataSources.First().DimRegisteredDataSource.Name,
                                Organization = new ProfileEditorSourceOrganization()
                                {
                                    NameFi = dfds.BrFieldDisplaySettingsDimRegisteredDataSources.First().DimRegisteredDataSource.DimOrganization.NameFi,
                                    NameEn = dfds.BrFieldDisplaySettingsDimRegisteredDataSources.First().DimRegisteredDataSource.DimOrganization.NameEn,
                                    NameSv = dfds.BrFieldDisplaySettingsDimRegisteredDataSources.First().DimRegisteredDataSource.DimOrganization.NameSv
                                }
                            },
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
                    case Constants.FieldIdentifiers.ACTIVITY_ROLE_IN_RESERCH_COMMUNITY:
                        // TODO
                        break;
                    case Constants.FieldIdentifiers.ACTIVITY_AFFILIATION:
                        var affiliationGroup = new ProfileEditorGroupAffiliation()
                        {
                            source = new ProfileEditorSource()
                            {
                                Id = dfds.BrFieldDisplaySettingsDimRegisteredDataSources.First().DimRegisteredDataSource.Id,
                                RegisteredDataSource = dfds.BrFieldDisplaySettingsDimRegisteredDataSources.First().DimRegisteredDataSource.Name,
                                Organization = new ProfileEditorSourceOrganization()
                                {
                                    NameFi = dfds.BrFieldDisplaySettingsDimRegisteredDataSources.First().DimRegisteredDataSource.DimOrganization.NameFi,
                                    NameEn = dfds.BrFieldDisplaySettingsDimRegisteredDataSources.First().DimRegisteredDataSource.DimOrganization.NameEn,
                                    NameSv = dfds.BrFieldDisplaySettingsDimRegisteredDataSources.First().DimRegisteredDataSource.DimOrganization.NameSv
                                }
                            },
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
                    case Constants.FieldIdentifiers.ACTIVITY_EDUCATION:
                        var educationGroup = new ProfileEditorGroupEducation()
                        {
                            source = new ProfileEditorSource()
                            {
                                Id = dfds.BrFieldDisplaySettingsDimRegisteredDataSources.First().DimRegisteredDataSource.Id,
                                RegisteredDataSource = dfds.BrFieldDisplaySettingsDimRegisteredDataSources.First().DimRegisteredDataSource.Name,
                                Organization = new ProfileEditorSourceOrganization()
                                {
                                    NameFi = dfds.BrFieldDisplaySettingsDimRegisteredDataSources.First().DimRegisteredDataSource.DimOrganization.NameFi,
                                    NameEn = dfds.BrFieldDisplaySettingsDimRegisteredDataSources.First().DimRegisteredDataSource.DimOrganization.NameEn,
                                    NameSv = dfds.BrFieldDisplaySettingsDimRegisteredDataSources.First().DimRegisteredDataSource.DimOrganization.NameSv
                                }
                            },
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
                    case Constants.FieldIdentifiers.ACTIVITY_PUBLICATION:
                        var publicationGroup = new ProfileEditorGroupPublication()
                        {
                            source = new ProfileEditorSource()
                            {
                                Id = dfds.BrFieldDisplaySettingsDimRegisteredDataSources.First().DimRegisteredDataSource.Id,
                                RegisteredDataSource = dfds.BrFieldDisplaySettingsDimRegisteredDataSources.First().DimRegisteredDataSource.Name,
                                Organization = new ProfileEditorSourceOrganization()
                                {
                                    NameFi = dfds.BrFieldDisplaySettingsDimRegisteredDataSources.First().DimRegisteredDataSource.DimOrganization.NameFi,
                                    NameEn = dfds.BrFieldDisplaySettingsDimRegisteredDataSources.First().DimRegisteredDataSource.DimOrganization.NameEn,
                                    NameSv = dfds.BrFieldDisplaySettingsDimRegisteredDataSources.First().DimRegisteredDataSource.DimOrganization.NameSv
                                }
                            },
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
                            // DimPublication
                            if (ffv.DimPublicationId != -1)
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

                            // DimOrcidPublication
                            if (ffv.DimOrcidPublicationId != -1)
                            {
                                publicationGroup.items.Add(

                                    new ProfileEditorItemPublication()
                                    {
                                        PublicationId = ffv.DimOrcidPublication.PublicationId,
                                        PublicationName = ffv.DimOrcidPublication.PublicationName,
                                        PublicationYear = ffv.DimOrcidPublication.PublicationYear,
                                        Doi = ffv.DimOrcidPublication.DoiHandle,
                                        itemMeta = new ProfileEditorItemMeta()
                                        {
                                            Id = ffv.DimOrcidPublicationId,
                                            Type = Constants.FieldIdentifiers.ACTIVITY_PUBLICATION,
                                            Show = ffv.Show,
                                            PrimaryValue = ffv.PrimaryValue
                                        }
                                    }

                                );
                            }
                        }
                        if (publicationGroup.items.Count > 0)
                        {
                            profileDataResponse.activity.publicationGroups.Add(publicationGroup);
                        }
                        break;
                    default:
                        break;
                }
            }

            return Ok(new ApiResponse(success: true, data: profileDataResponse));
        }



        // PATCH: api/ProfileData/
        [HttpPatch]
        public async Task<IActionResult> PatchMany([FromBody] ProfileEditorDataModificationRequest profileEditorDataModificationRequest)
        {
            // Return immediately if there is nothing to change.
            if (profileEditorDataModificationRequest.groups.Count == 0 && profileEditorDataModificationRequest.items.Count == 0)
            {
                return Ok(new ApiResponse(success: true));
            }

            // Get userprofile
            var orcidId = this.GetOrcidId();
            var userprofileId = await _userProfileService.GetUserprofileId(orcidId);
            if (userprofileId == -1)
            {
                return Ok(new ApiResponse(success: false, reason: "profile not found"));
            }


            var dimUserProfile = await _ttvContext.DimUserProfiles
                .Include(dup => dup.DimFieldDisplaySettings)
                .Include(dup => dup.FactFieldValues).AsSplitQuery().FirstOrDefaultAsync(up => up.Id == userprofileId);


            var profileEditorDataModificationResponse = new ProfileEditorDataModificationResponse();

            // Set 'Show' in DimFieldDisplaySettings
            foreach (ProfileEditorGroupMeta profileEditorGroupMeta in profileEditorDataModificationRequest.groups.ToList())
            {
                var dimFieldDisplaySettings = dimUserProfile.DimFieldDisplaySettings.Where(d => d.Id == profileEditorGroupMeta.Id).FirstOrDefault();
                if (dimFieldDisplaySettings != null)
                {
                    dimFieldDisplaySettings.Show = profileEditorGroupMeta.Show;
                    profileEditorDataModificationResponse.groups.Add(profileEditorGroupMeta);
                }
            }

            // Set 'Show' and 'PrimaryValue' in FactFieldValues
            foreach (ProfileEditorItemMeta profileEditorItemMeta in profileEditorDataModificationRequest.items.ToList())
            {
                FactFieldValue factFieldValue = null;
                switch (profileEditorItemMeta.Type)
                {
                    case Constants.FieldIdentifiers.PERSON_FIRST_NAMES:
                        factFieldValue = dimUserProfile.FactFieldValues.Where(ffv => ffv.DimNameId == profileEditorItemMeta.Id).FirstOrDefault();
                        break;
                    case Constants.FieldIdentifiers.PERSON_LAST_NAME:
                        factFieldValue = dimUserProfile.FactFieldValues.Where(ffv => ffv.DimNameId == profileEditorItemMeta.Id).FirstOrDefault();
                        break;
                    case Constants.FieldIdentifiers.PERSON_OTHER_NAMES:
                        factFieldValue = dimUserProfile.FactFieldValues.Where(ffv => ffv.DimNameId == profileEditorItemMeta.Id).FirstOrDefault();
                        break;
                    case Constants.FieldIdentifiers.PERSON_RESEARCHER_DESCRIPTION:
                        factFieldValue = dimUserProfile.FactFieldValues.Where(ffv => ffv.DimResearcherDescriptionId == profileEditorItemMeta.Id).FirstOrDefault();
                        break;
                    case Constants.FieldIdentifiers.PERSON_WEB_LINK:
                        factFieldValue = dimUserProfile.FactFieldValues.Where(ffv => ffv.DimWebLinkId == profileEditorItemMeta.Id).FirstOrDefault();
                        break;
                    case Constants.FieldIdentifiers.PERSON_EMAIL_ADDRESS:
                        factFieldValue = dimUserProfile.FactFieldValues.Where(ffv => ffv.DimEmailAddrressId == profileEditorItemMeta.Id).FirstOrDefault();
                        break;
                    case Constants.FieldIdentifiers.PERSON_KEYWORD:
                        factFieldValue = dimUserProfile.FactFieldValues.Where(ffv => ffv.DimKeywordId == profileEditorItemMeta.Id).FirstOrDefault();
                        break;
                    case Constants.FieldIdentifiers.PERSON_TELEPHONE_NUMBER:
                        factFieldValue = dimUserProfile.FactFieldValues.Where(ffv => ffv.DimTelephoneNumberId == profileEditorItemMeta.Id).FirstOrDefault();
                        break;
                    case Constants.FieldIdentifiers.ACTIVITY_AFFILIATION:
                        factFieldValue = dimUserProfile.FactFieldValues.Where(ffv => ffv.DimAffiliationId == profileEditorItemMeta.Id).FirstOrDefault();
                        break;
                    case Constants.FieldIdentifiers.ACTIVITY_EDUCATION:
                        factFieldValue = dimUserProfile.FactFieldValues.Where(ffv => ffv.DimEducationId == profileEditorItemMeta.Id).FirstOrDefault();
                        break;
                    case Constants.FieldIdentifiers.ACTIVITY_PUBLICATION:
                        factFieldValue = dimUserProfile.FactFieldValues.Where(ffv => ffv.DimPublicationId == profileEditorItemMeta.Id || ffv.DimOrcidPublicationId == profileEditorItemMeta.Id).FirstOrDefault();
                        break;
                    default:
                        break;
                }

                if (factFieldValue != null)
                {
                    factFieldValue.Show = profileEditorItemMeta.Show;
                    factFieldValue.PrimaryValue = profileEditorItemMeta.PrimaryValue;
                    profileEditorDataModificationResponse.items.Add(profileEditorItemMeta);
                }
            }

            await _ttvContext.SaveChangesAsync();

            return Ok(new ApiResponse(success: true, data: profileEditorDataModificationResponse));
        }
    }
}