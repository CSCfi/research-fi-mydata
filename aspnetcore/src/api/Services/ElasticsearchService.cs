using System;
using System.Net.Http;
using System.Threading.Tasks;
using api.Models;
using api.Models.Ttv;
using api.Models.Elasticsearch;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Nest;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Collections.Generic;

namespace api.Services
{
    /*
     * ElasticsearchService handles person index update.
     */
    public class ElasticsearchService
    {
        public ElasticClient ESclient;
        public IConfiguration _configuration { get; }
        private readonly ILogger<ElasticsearchService> _logger;
        private readonly TtvContext _ttvContext;
        private readonly UserProfileService _userProfileService;

        // Check if Elasticsearch synchronization is enabled and related configuration is valid.
        public Boolean IsElasticsearchSyncEnabled()
        {
            return _configuration["ELASTICSEARCH:ENABLED"] != null
                && _configuration["ELASTICSEARCH:ENABLED"] == "true"
                && _configuration["ELASTICSEARCH:URL"] != null
                && Uri.IsWellFormedUriString(
                    _configuration["ELASTICSEARCH:URL"],
                    UriKind.Absolute
                );
        }

        // Constructor.
        // Do not setup HttpClient unless configuration values are ok.
        public ElasticsearchService(TtvContext ttvContext, IConfiguration configuration, HttpClient client, ILogger<ElasticsearchService> logger, UserProfileService userProfileService)
        {
            _configuration = configuration;
            _logger = logger;
            _ttvContext = ttvContext;
            _userProfileService = userProfileService;

            if (this.IsElasticsearchSyncEnabled())
            {
                var settings = new ConnectionSettings(new Uri(_configuration["ELASTICSEARCH:URL"]))
                    .DefaultIndex("person")
                    .BasicAuthentication(_configuration["ELASTICSEARCH:USERNAME"], _configuration["ELASTICSEARCH:PASSWORD"]);
                ESclient = new ElasticClient(settings);
            }
        }
            

        // Update entry in Elasticsearch person index
        // TODO: When 3rd party sharing feature is implemented, check that TTV share is enabled in user profile.
        public async Task UpdateEntryInElasticsearchPersonIndex(string orcidId, int userprofileId)
        {
            if (!this.IsElasticsearchSyncEnabled())
            {
                return;
            }

            // Get DimUserProfile and related entities
            var dimUserProfile = await _ttvContext.DimUserProfiles
                .Include(dup => dup.DimFieldDisplaySettings.Where(dfds => dfds.FactFieldValues.Count > 0))
                    .ThenInclude(dfds => dfds.BrFieldDisplaySettingsDimRegisteredDataSources)
                        .ThenInclude(br => br.DimRegisteredDataSource)
                            .ThenInclude(drds => drds.DimOrganization).AsNoTracking()
                // DimName
                .Include(dup => dup.DimFieldDisplaySettings)
                    .ThenInclude(dfds => dfds.FactFieldValues.Where(ffv => ffv.Show == true))
                        .ThenInclude(ffv => ffv.DimName).AsNoTracking()
                // DimWebLink
                .Include(dup => dup.DimFieldDisplaySettings)
                    .ThenInclude(dfds => dfds.FactFieldValues.Where(ffv => ffv.Show == true))
                        .ThenInclude(ffv => ffv.DimWebLink).AsNoTracking()
                // DimEmailAddress
                .Include(dup => dup.DimFieldDisplaySettings)
                    .ThenInclude(dfds => dfds.FactFieldValues.Where(ffv => ffv.Show == true))
                        .ThenInclude(ffv => ffv.DimEmailAddrress).AsNoTracking()
                // DimPublication
                .Include(dup => dup.DimFieldDisplaySettings)
                    .ThenInclude(dfds => dfds.FactFieldValues.Where(ffv => ffv.Show == true))
                        .ThenInclude(ffv => ffv.DimPublication).AsNoTracking()
                // DimOrcidPublication
                .Include(dup => dup.DimFieldDisplaySettings)
                    .ThenInclude(dfds => dfds.FactFieldValues.Where(ffv => ffv.Show == true))
                        .ThenInclude(ffv => ffv.DimOrcidPublication).AsNoTracking()
                // DimEducation
                .Include(dup => dup.DimFieldDisplaySettings)
                    .ThenInclude(dfds => dfds.FactFieldValues.Where(ffv => ffv.Show == true))
                        .ThenInclude(ffv => ffv.DimEducation)
                            .ThenInclude(de => de.DimStartDateNavigation).AsNoTracking()
                .Include(dup => dup.DimFieldDisplaySettings)
                    .ThenInclude(dfds => dfds.FactFieldValues.Where(ffv => ffv.Show == true))
                        .ThenInclude(ffv => ffv.DimEducation)
                            .ThenInclude(de => de.DimEndDateNavigation).AsNoTracking()
                // DimAffiliation
                .Include(dup => dup.DimFieldDisplaySettings)
                    .ThenInclude(dfds => dfds.FactFieldValues.Where(ffv => ffv.Show == true))
                        .ThenInclude(ffv => ffv.DimAffiliation)
                            .ThenInclude(da => da.StartDateNavigation).AsNoTracking()
                .Include(dup => dup.DimFieldDisplaySettings)
                    .ThenInclude(dfds => dfds.FactFieldValues.Where(ffv => ffv.Show == true))
                        .ThenInclude(ffv => ffv.DimAffiliation)
                            .ThenInclude(da => da.EndDateNavigation).AsNoTracking()
                .Include(dup => dup.DimFieldDisplaySettings)
                    .ThenInclude(dfds => dfds.FactFieldValues.Where(ffv => ffv.Show == true))
                        .ThenInclude(ffv => ffv.DimAffiliation)
                            .ThenInclude(da => da.DimOrganization).AsNoTracking()
                .Include(dup => dup.DimFieldDisplaySettings)
                    .ThenInclude(dfds => dfds.FactFieldValues.Where(ffv => ffv.Show == true))
                        .ThenInclude(ffv => ffv.DimAffiliation)
                            .ThenInclude(da => da.AffiliationTypeNavigation).AsNoTracking()
                // DimTelephoneNumber
                .Include(dup => dup.DimFieldDisplaySettings)
                    .ThenInclude(dfds => dfds.FactFieldValues.Where(ffv => ffv.Show == true))
                        .ThenInclude(ffv => ffv.DimTelephoneNumber).AsNoTracking()
                // DimResearcherDescription
                .Include(dup => dup.DimFieldDisplaySettings)
                    .ThenInclude(dfds => dfds.FactFieldValues.Where(ffv => ffv.Show == true))
                        .ThenInclude(ffv => ffv.DimResearcherDescription).AsNoTracking()
                // DimKeyword
                .Include(dup => dup.DimFieldDisplaySettings)
                    .ThenInclude(dfds => dfds.FactFieldValues.Where(ffv => ffv.Show == true))
                        .ThenInclude(ffv => ffv.DimKeyword).AsNoTracking()
                // DimFieldOfScience
                .Include(dup => dup.DimFieldDisplaySettings)
                    .ThenInclude(dfds => dfds.FactFieldValues.Where(ffv => ffv.Show == true))
                        .ThenInclude(ffv => ffv.DimFieldOfScience).AsNoTracking()

                .AsSplitQuery().FirstOrDefaultAsync(up => up.Id == userprofileId);

            var person = new Person(orcidId)
            {
            };

            // foreach (DimFieldDisplaySetting dfds in dimUserProfile.DimFieldDisplaySettings)
            foreach (DimFieldDisplaySetting dfds in dimUserProfile.DimFieldDisplaySettings)
            {
                // FieldIdentifier defines what type of data the field contains.
                switch (dfds.FieldIdentifier)
                {
                    case Constants.FieldIdentifiers.PERSON_NAME:
                        var nameGroup = new GroupName()
                        {
                            source = new Source()
                            {
                                RegisteredDataSource = dfds.BrFieldDisplaySettingsDimRegisteredDataSources.First().DimRegisteredDataSource.Name,
                                Organization = new SourceOrganization()
                                {
                                    NameFi = dfds.BrFieldDisplaySettingsDimRegisteredDataSources.First().DimRegisteredDataSource.DimOrganization.NameFi,
                                    NameEn = dfds.BrFieldDisplaySettingsDimRegisteredDataSources.First().DimRegisteredDataSource.DimOrganization.NameEn,
                                    NameSv = dfds.BrFieldDisplaySettingsDimRegisteredDataSources.First().DimRegisteredDataSource.DimOrganization.NameSv
                                }
                            },
                            items = new List<ItemName>() { }
                        };
                        foreach (FactFieldValue ffv in dfds.FactFieldValues)
                        {
                            nameGroup.items.Add(
                                new ItemName()
                                {
                                    FirstNames = ffv.DimName.FirstNames,
                                    LastName = ffv.DimName.LastName,
                                    PrimaryValue = ffv.PrimaryValue
                                }
                            );
                        }
                        if (nameGroup.items.Count > 0)
                        {
                            person.personal.nameGroups.Add(nameGroup);
                        }
                        break;
                    case Constants.FieldIdentifiers.PERSON_OTHER_NAMES:
                        var otherNameGroup = new GroupOtherName()
                        {
                            source = new Source()
                            {
                                RegisteredDataSource = dfds.BrFieldDisplaySettingsDimRegisteredDataSources.First().DimRegisteredDataSource.Name,
                                Organization = new SourceOrganization()
                                {
                                    NameFi = dfds.BrFieldDisplaySettingsDimRegisteredDataSources.First().DimRegisteredDataSource.DimOrganization.NameFi,
                                    NameEn = dfds.BrFieldDisplaySettingsDimRegisteredDataSources.First().DimRegisteredDataSource.DimOrganization.NameEn,
                                    NameSv = dfds.BrFieldDisplaySettingsDimRegisteredDataSources.First().DimRegisteredDataSource.DimOrganization.NameSv
                                }
                            },
                            items = new List<ItemName>() { }
                        };
                        foreach (FactFieldValue ffv in dfds.FactFieldValues)
                        {
                            otherNameGroup.items.Add(
                                new ItemName()
                                {
                                    FullName = ffv.DimName.FullName,
                                    PrimaryValue = ffv.PrimaryValue
                           
                                }
                            );
                        }
                        if (otherNameGroup.items.Count > 0)
                        {
                            person.personal.otherNameGroups.Add(otherNameGroup);
                        }
                        break;
                    case Constants.FieldIdentifiers.PERSON_RESEARCHER_DESCRIPTION:
                        var researcherDescriptionGroup = new GroupResearcherDescription()
                        {
                            source = new Source()
                            {
                                RegisteredDataSource = dfds.BrFieldDisplaySettingsDimRegisteredDataSources.First().DimRegisteredDataSource.Name,
                                Organization = new SourceOrganization()
                                {
                                    NameFi = dfds.BrFieldDisplaySettingsDimRegisteredDataSources.First().DimRegisteredDataSource.DimOrganization.NameFi,
                                    NameEn = dfds.BrFieldDisplaySettingsDimRegisteredDataSources.First().DimRegisteredDataSource.DimOrganization.NameEn,
                                    NameSv = dfds.BrFieldDisplaySettingsDimRegisteredDataSources.First().DimRegisteredDataSource.DimOrganization.NameSv
                                }
                            },
                            items = new List<ItemResearcherDescription>() { }
                        };
                        foreach (FactFieldValue ffv in dfds.FactFieldValues)
                        {
                            researcherDescriptionGroup.items.Add(
                                new ItemResearcherDescription()
                                {
                                    ResearchDescriptionEn = ffv.DimResearcherDescription.ResearchDescriptionEn,
                                    ResearchDescriptionFi = ffv.DimResearcherDescription.ResearchDescriptionFi,
                                    ResearchDescriptionSv = ffv.DimResearcherDescription.ResearchDescriptionSv,
                                    PrimaryValue = ffv.PrimaryValue
                                }
                            );
                        }
                        if (researcherDescriptionGroup.items.Count > 0)
                        {
                            person.personal.researcherDescriptionGroups.Add(researcherDescriptionGroup);
                        }
                        break;
                    case Constants.FieldIdentifiers.PERSON_WEB_LINK:
                        var webLinkGroup = new GroupWebLink()
                        {
                            source = new Source()
                            {
                                RegisteredDataSource = dfds.BrFieldDisplaySettingsDimRegisteredDataSources.First().DimRegisteredDataSource.Name,
                                Organization = new SourceOrganization()
                                {
                                    NameFi = dfds.BrFieldDisplaySettingsDimRegisteredDataSources.First().DimRegisteredDataSource.DimOrganization.NameFi,
                                    NameEn = dfds.BrFieldDisplaySettingsDimRegisteredDataSources.First().DimRegisteredDataSource.DimOrganization.NameEn,
                                    NameSv = dfds.BrFieldDisplaySettingsDimRegisteredDataSources.First().DimRegisteredDataSource.DimOrganization.NameSv
                                }
                            },
                            items = new List<ItemWebLink>() { }
                        };
                        foreach (FactFieldValue ffv in dfds.FactFieldValues)
                        {
                            webLinkGroup.items.Add(
                                new ItemWebLink()
                                {
                                    Url = ffv.DimWebLink.Url,
                                    LinkLabel = ffv.DimWebLink.LinkLabel,
                                    PrimaryValue = ffv.PrimaryValue
                                }
                            );
                        }
                        if (webLinkGroup.items.Count > 0)
                        {
                            person.personal.webLinkGroups.Add(webLinkGroup);
                        }
                        break;
                    case Constants.FieldIdentifiers.PERSON_EMAIL_ADDRESS:
                        var emailGroup = new GroupEmail()
                        {
                            source = new Source()
                            {
                                RegisteredDataSource = dfds.BrFieldDisplaySettingsDimRegisteredDataSources.First().DimRegisteredDataSource.Name,
                                Organization = new SourceOrganization()
                                {
                                    NameFi = dfds.BrFieldDisplaySettingsDimRegisteredDataSources.First().DimRegisteredDataSource.DimOrganization.NameFi,
                                    NameEn = dfds.BrFieldDisplaySettingsDimRegisteredDataSources.First().DimRegisteredDataSource.DimOrganization.NameEn,
                                    NameSv = dfds.BrFieldDisplaySettingsDimRegisteredDataSources.First().DimRegisteredDataSource.DimOrganization.NameSv
                                }
                            },
                            items = new List<ItemEmail>() { },
                        };
                        foreach (FactFieldValue ffv in dfds.FactFieldValues)
                        {
                            emailGroup.items.Add(
                                new ItemEmail()
                                {
                                    Value = ffv.DimEmailAddrress.Email,
                                    PrimaryValue = ffv.PrimaryValue
                                }
                            );
                        }
                        if (emailGroup.items.Count > 0)
                        {
                            person.personal.emailGroups.Add(emailGroup);
                        }
                        break;
                    case Constants.FieldIdentifiers.PERSON_TELEPHONE_NUMBER:
                        var telephoneNumberGroup = new GroupTelephoneNumber()
                        {
                            source = new Source()
                            {
                                RegisteredDataSource = dfds.BrFieldDisplaySettingsDimRegisteredDataSources.First().DimRegisteredDataSource.Name,
                                Organization = new SourceOrganization()
                                {
                                    NameFi = dfds.BrFieldDisplaySettingsDimRegisteredDataSources.First().DimRegisteredDataSource.DimOrganization.NameFi,
                                    NameEn = dfds.BrFieldDisplaySettingsDimRegisteredDataSources.First().DimRegisteredDataSource.DimOrganization.NameEn,
                                    NameSv = dfds.BrFieldDisplaySettingsDimRegisteredDataSources.First().DimRegisteredDataSource.DimOrganization.NameSv
                                }
                            },
                            items = new List<ItemTelephoneNumber>() { }
                        };
                        foreach (FactFieldValue ffv in dfds.FactFieldValues)
                        {
                            telephoneNumberGroup.items.Add(
                                new ItemTelephoneNumber()
                                {
                                    Value = ffv.DimTelephoneNumber.TelephoneNumber,
                                    PrimaryValue = ffv.PrimaryValue
                                }
                            );
                        }
                        if (telephoneNumberGroup.items.Count > 0)
                        {
                            person.personal.telephoneNumberGroups.Add(telephoneNumberGroup);
                        }
                        break;
                    case Constants.FieldIdentifiers.PERSON_FIELD_OF_SCIENCE:
                        var fieldOfScienceGroup = new GroupFieldOfScience()
                        {
                            source = new Source()
                            {
                                RegisteredDataSource = dfds.BrFieldDisplaySettingsDimRegisteredDataSources.First().DimRegisteredDataSource.Name,
                                Organization = new SourceOrganization()
                                {
                                    NameFi = dfds.BrFieldDisplaySettingsDimRegisteredDataSources.First().DimRegisteredDataSource.DimOrganization.NameFi,
                                    NameEn = dfds.BrFieldDisplaySettingsDimRegisteredDataSources.First().DimRegisteredDataSource.DimOrganization.NameEn,
                                    NameSv = dfds.BrFieldDisplaySettingsDimRegisteredDataSources.First().DimRegisteredDataSource.DimOrganization.NameSv
                                }
                            },
                            items = new List<ItemFieldOfScience>() { }
                        };
                        foreach (FactFieldValue ffv in dfds.FactFieldValues)
                        {
                            fieldOfScienceGroup.items.Add(
                                new ItemFieldOfScience()
                                {
                                    NameFi = ffv.DimFieldOfScience.NameFi,
                                    PrimaryValue = ffv.PrimaryValue
                                }
                            );
                        }
                        if (fieldOfScienceGroup.items.Count > 0)
                        {
                            person.personal.fieldOfScienceGroups.Add(fieldOfScienceGroup);
                        }
                        break;
                    case Constants.FieldIdentifiers.PERSON_KEYWORD:
                        var keywordGroup = new GroupKeyword()
                        {
                            source = new Source()
                            {
                                RegisteredDataSource = dfds.BrFieldDisplaySettingsDimRegisteredDataSources.First().DimRegisteredDataSource.Name,
                                Organization = new SourceOrganization()
                                {
                                    NameFi = dfds.BrFieldDisplaySettingsDimRegisteredDataSources.First().DimRegisteredDataSource.DimOrganization.NameFi,
                                    NameEn = dfds.BrFieldDisplaySettingsDimRegisteredDataSources.First().DimRegisteredDataSource.DimOrganization.NameEn,
                                    NameSv = dfds.BrFieldDisplaySettingsDimRegisteredDataSources.First().DimRegisteredDataSource.DimOrganization.NameSv
                                }
                            },
                            items = new List<ItemKeyword>() { }
                        };
                        foreach (FactFieldValue ffv in dfds.FactFieldValues)
                        {
                            keywordGroup.items.Add(
                                new ItemKeyword()
                                {
                                    Value = ffv.DimKeyword.Keyword,
                                    PrimaryValue = ffv.PrimaryValue
                                }
                            );
                        }
                        if (keywordGroup.items.Count > 0)
                        {
                            person.personal.keywordGroups.Add(keywordGroup);
                        }
                        break;
                    case Constants.FieldIdentifiers.PERSON_EXTERNAL_IDENTIFIER:
                        var externalIdentifierGroup = new GroupExternalIdentifier()
                        {
                            source = new Source()
                            {
                                RegisteredDataSource = dfds.BrFieldDisplaySettingsDimRegisteredDataSources.First().DimRegisteredDataSource.Name,
                                Organization = new SourceOrganization()
                                {
                                    NameFi = dfds.BrFieldDisplaySettingsDimRegisteredDataSources.First().DimRegisteredDataSource.DimOrganization.NameFi,
                                    NameEn = dfds.BrFieldDisplaySettingsDimRegisteredDataSources.First().DimRegisteredDataSource.DimOrganization.NameEn,
                                    NameSv = dfds.BrFieldDisplaySettingsDimRegisteredDataSources.First().DimRegisteredDataSource.DimOrganization.NameSv
                                }
                            },
                            items = new List<ItemExternalIdentifier>() { }
                        };
                        foreach (FactFieldValue ffv in dfds.FactFieldValues)
                        {
                            externalIdentifierGroup.items.Add(
                                new ItemExternalIdentifier()
                                {
                                    PidContent = ffv.DimPid.PidContent,
                                    PidType = ffv.DimPid.PidType,
                                    PrimaryValue = ffv.PrimaryValue
                                }
                            );
                        }
                        if (externalIdentifierGroup.items.Count > 0)
                        {
                            person.personal.externalIdentifierGroups.Add(externalIdentifierGroup);
                        }
                        break;
                    case Constants.FieldIdentifiers.ACTIVITY_AFFILIATION:
                        var affiliationGroup = new GroupAffiliation()
                        {
                            source = new Source()
                            {
                                RegisteredDataSource = dfds.BrFieldDisplaySettingsDimRegisteredDataSources.First().DimRegisteredDataSource.Name,
                                Organization = new SourceOrganization()
                                {
                                    NameFi = dfds.BrFieldDisplaySettingsDimRegisteredDataSources.First().DimRegisteredDataSource.DimOrganization.NameFi,
                                    NameEn = dfds.BrFieldDisplaySettingsDimRegisteredDataSources.First().DimRegisteredDataSource.DimOrganization.NameEn,
                                    NameSv = dfds.BrFieldDisplaySettingsDimRegisteredDataSources.First().DimRegisteredDataSource.DimOrganization.NameSv
                                }
                            },
                            items = new List<ItemAffiliation>() { }
                        };
                        foreach (FactFieldValue ffv in dfds.FactFieldValues)
                        {
                            var affiliation = new ItemAffiliation()
                            {
                                // TODO: DimOrganization handling
                                OrganizationNameFi = ffv.DimAffiliation.DimOrganization.NameFi,
                                OrganizationNameEn = ffv.DimAffiliation.DimOrganization.NameEn,
                                OrganizationNameSv = ffv.DimAffiliation.DimOrganization.NameSv,
                                PositionNameFi = ffv.DimAffiliation.PositionNameFi,
                                PositionNameEn = ffv.DimAffiliation.PositionNameEn,
                                PositionNameSv = ffv.DimAffiliation.PositionNameSv,
                                Type = ffv.DimAffiliation.AffiliationTypeNavigation.NameFi,
                                StartDate = new ItemDate()
                                {
                                    Year = ffv.DimAffiliation.StartDateNavigation.Year,
                                    Month = ffv.DimAffiliation.StartDateNavigation.Month,
                                    Day = ffv.DimAffiliation.StartDateNavigation.Day
                                },
                                PrimaryValue = ffv.PrimaryValue
                            };

                            // Affiliation EndDate can be null
                            if (ffv.DimAffiliation.EndDateNavigation != null)
                            {
                                affiliation.EndDate = new ItemDate()
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
                            person.activity.affiliationGroups.Add(affiliationGroup);
                        }
                        break;
                    case Constants.FieldIdentifiers.ACTIVITY_EDUCATION:
                        var educationGroup = new GroupEducation()
                        {
                            source = new Source()
                            {
                                RegisteredDataSource = dfds.BrFieldDisplaySettingsDimRegisteredDataSources.First().DimRegisteredDataSource.Name,
                                Organization = new SourceOrganization()
                                {
                                    NameFi = dfds.BrFieldDisplaySettingsDimRegisteredDataSources.First().DimRegisteredDataSource.DimOrganization.NameFi,
                                    NameEn = dfds.BrFieldDisplaySettingsDimRegisteredDataSources.First().DimRegisteredDataSource.DimOrganization.NameEn,
                                    NameSv = dfds.BrFieldDisplaySettingsDimRegisteredDataSources.First().DimRegisteredDataSource.DimOrganization.NameSv
                                }
                            },
                            items = new List<ItemEducation>() { }
                        };
                        foreach (FactFieldValue ffv in dfds.FactFieldValues)
                        {
                            var education = new ItemEducation()
                            {
                                NameFi = ffv.DimEducation.NameFi,
                                NameEn = ffv.DimEducation.NameEn,
                                NameSv = ffv.DimEducation.NameSv,
                                DegreeGrantingInstitutionName = ffv.DimEducation.DegreeGrantingInstitutionName,
                                PrimaryValue = ffv.PrimaryValue
                            };
                            // Education StartDate can be null
                            if (ffv.DimEducation.DimStartDateNavigation != null)
                            {
                                education.StartDate = new ItemDate()
                                {
                                    Year = ffv.DimEducation.DimStartDateNavigation.Year,
                                    Month = ffv.DimEducation.DimStartDateNavigation.Month,
                                    Day = ffv.DimEducation.DimStartDateNavigation.Day
                                };
                            }
                            // Education EndDate can be null
                            if (ffv.DimEducation.DimEndDateNavigation != null)
                            {
                                education.EndDate = new ItemDate()
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
                            person.activity.educationGroups.Add(educationGroup);
                        }
                        break;
                    case Constants.FieldIdentifiers.ACTIVITY_PUBLICATION:
                        var publicationGroup = new GroupPublication()
                        {
                            source = new Source()
                            {
                                RegisteredDataSource = dfds.BrFieldDisplaySettingsDimRegisteredDataSources.First().DimRegisteredDataSource.Name,
                                Organization = new SourceOrganization()
                                {
                                    NameFi = dfds.BrFieldDisplaySettingsDimRegisteredDataSources.First().DimRegisteredDataSource.DimOrganization.NameFi,
                                    NameEn = dfds.BrFieldDisplaySettingsDimRegisteredDataSources.First().DimRegisteredDataSource.DimOrganization.NameEn,
                                    NameSv = dfds.BrFieldDisplaySettingsDimRegisteredDataSources.First().DimRegisteredDataSource.DimOrganization.NameSv
                                }
                            },
                            items = new List<ItemPublication>() { }
                        };
                        foreach (FactFieldValue ffv in dfds.FactFieldValues)
                        {
                            // DimPublication
                            if (ffv.DimPublicationId != -1)
                            {
                                publicationGroup.items.Add(

                                    new ItemPublication()
                                    {
                                        PublicationId = ffv.DimPublication.PublicationId,
                                        PublicationName = ffv.DimPublication.PublicationName,
                                        PublicationYear = ffv.DimPublication.PublicationYear,
                                        Doi = ffv.DimPublication.Doi,
                                        PrimaryValue = ffv.PrimaryValue
                                    }

                                );
                            }

                            // DimOrcidPublication
                            if (ffv.DimOrcidPublicationId != -1)
                            {
                                publicationGroup.items.Add(

                                    new ItemPublication()
                                    {
                                        PublicationId = ffv.DimOrcidPublication.PublicationId,
                                        PublicationName = ffv.DimOrcidPublication.PublicationName,
                                        PublicationYear = ffv.DimOrcidPublication.PublicationYear,
                                        Doi = ffv.DimOrcidPublication.DoiHandle,
                                        PrimaryValue = ffv.PrimaryValue
                                    }

                                );
                            }
                        }
                        if (publicationGroup.items.Count > 0)
                        {
                            person.activity.publicationGroups.Add(publicationGroup);
                        }
                        break;
                    default:
                        break;
                }
            }


            var asyncIndexResponse = await ESclient.IndexDocumentAsync(person);

            if (!asyncIndexResponse.IsValid)
            {
                _logger.LogInformation("Elasticsearch: ERROR: " + orcidId + ": " + asyncIndexResponse.OriginalException.Message);
            }
            else
            {
                _logger.LogInformation("Elasticsearch: Updated: " + orcidId);
            }
        }

        // Delete entry from Elasticsearch person index
        public async Task DeleteEntryFromElasticsearchPersonIndex(String orcidId)
        {
            if (!this.IsElasticsearchSyncEnabled())
            {
                return;
            }

            var asyncDeleteResponse = await ESclient.DeleteAsync<ElasticPerson>(orcidId);

            if (!asyncDeleteResponse.IsValid)
            {
                _logger.LogInformation("Elasticsearch: ERROR: " + orcidId + ": " + asyncDeleteResponse.OriginalException.Message);
            }
            else
            {
                _logger.LogInformation("Elasticsearch: Deleted: " + orcidId);
            }
        }
    }
}