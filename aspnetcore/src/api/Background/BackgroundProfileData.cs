using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.Models;
using api.Models.Ttv;
using api.Models.Elasticsearch;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using api.Models.Common;

namespace api.Services
{
    /*
     * BackgroundProfiledata gets user profile data and constructs an entry for Elasticsearch person index.
     * In normal controller code the request context has access to database via ttvContext.
     * In a background task that is not available, since it is disposed when the response is sent.
     * Here a local scope is created and database context can be taken from that scope.
     */
    public class BackgroundProfiledata
    {
        private readonly IServiceScopeFactory _serviceScopeFactory;

        public BackgroundProfiledata(IServiceScopeFactory serviceScopeFactory)
        {
            _serviceScopeFactory = serviceScopeFactory;
        }


        /*
         * Get userprofile data from TTV database and construct entry for Elasticsearch person index.
         */ 
        public async Task<Person> GetProfiledataForElasticsearch(string orcidId, int userprofileId)
        {
            // Create a scope and get TtvContext for data query.
            using var scope = _serviceScopeFactory.CreateScope();
            var localTtvContext = scope.ServiceProvider.GetRequiredService<TtvContext>();
            var localLanguageService = scope.ServiceProvider.GetRequiredService<LanguageService>();

            // Get DimFieldDisplaySettings and related entities
            var dimFieldDisplaySettings = await localTtvContext.DimFieldDisplaySettings.Where(dfds => dfds.DimUserProfileId == userprofileId && dfds.FactFieldValues.Any(ffv => ffv.Show == true))
                .Include(dfds => dfds.BrFieldDisplaySettingsDimRegisteredDataSources)
                    .ThenInclude(br => br.DimRegisteredDataSource)
                        .ThenInclude(drds => drds.DimOrganization).AsNoTracking()
                // DimName
                .Include(dfds => dfds.FactFieldValues.Where(ffv => ffv.Show == true))
                    .ThenInclude(ffv => ffv.DimName).AsNoTracking()
                // DimWebLink
                .Include(dfds => dfds.FactFieldValues.Where(ffv => ffv.Show == true))
                    .ThenInclude(ffv => ffv.DimWebLink).AsNoTracking()
                // DimEmailAddress
                .Include(dfds => dfds.FactFieldValues.Where(ffv => ffv.Show == true))
                    .ThenInclude(ffv => ffv.DimEmailAddrress).AsNoTracking()
                // DimPublication
                .Include(dfds => dfds.FactFieldValues.Where(ffv => ffv.Show == true))
                    .ThenInclude(ffv => ffv.DimPublication).AsNoTracking()
                // DimOrcidPublication
                .Include(dfds => dfds.FactFieldValues.Where(ffv => ffv.Show == true))
                    .ThenInclude(ffv => ffv.DimOrcidPublication).AsNoTracking()
                // DimEducation
                .Include(dfds => dfds.FactFieldValues.Where(ffv => ffv.Show == true))
                    .ThenInclude(ffv => ffv.DimEducation)
                        .ThenInclude(de => de.DimStartDateNavigation).AsNoTracking()
                .Include(dfds => dfds.FactFieldValues.Where(ffv => ffv.Show == true))
                    .ThenInclude(ffv => ffv.DimEducation)
                        .ThenInclude(de => de.DimEndDateNavigation).AsNoTracking()
                // DimAffiliation
                .Include(dfds => dfds.FactFieldValues.Where(ffv => ffv.Show == true))
                    .ThenInclude(ffv => ffv.DimAffiliation)
                        .ThenInclude(da => da.StartDateNavigation).AsNoTracking()
                .Include(dfds => dfds.FactFieldValues.Where(ffv => ffv.Show == true))
                    .ThenInclude(ffv => ffv.DimAffiliation)
                        .ThenInclude(da => da.EndDateNavigation).AsNoTracking()
                .Include(dfds => dfds.FactFieldValues.Where(ffv => ffv.Show == true))
                    .ThenInclude(ffv => ffv.DimAffiliation)
                        .ThenInclude(da => da.DimOrganization).AsNoTracking()
                .Include(dfds => dfds.FactFieldValues.Where(ffv => ffv.Show == true))
                    .ThenInclude(ffv => ffv.DimAffiliation)
                        .ThenInclude(da => da.AffiliationTypeNavigation).AsNoTracking()
                // DimTelephoneNumber
                .Include(dfds => dfds.FactFieldValues.Where(ffv => ffv.Show == true))
                    .ThenInclude(ffv => ffv.DimTelephoneNumber).AsNoTracking()
                // DimResearcherDescription
                .Include(dfds => dfds.FactFieldValues.Where(ffv => ffv.Show == true))
                    .ThenInclude(ffv => ffv.DimResearcherDescription).AsNoTracking()
                // DimKeyword
                .Include(dfds => dfds.FactFieldValues.Where(ffv => ffv.Show == true))
                    .ThenInclude(ffv => ffv.DimKeyword).AsNoTracking()
                // DimFieldOfScience
                .Include(dfds => dfds.FactFieldValues.Where(ffv => ffv.Show == true))
                    .ThenInclude(ffv => ffv.DimFieldOfScience).AsNoTracking()
                .ToListAsync();

            var person = new Person(orcidId)
            {
            };


            foreach (DimFieldDisplaySetting dfds in dimFieldDisplaySettings)
            {
                // Source object containing registered data source and organization name.
                var source = new Source()
                {
                    RegisteredDataSource = dfds.BrFieldDisplaySettingsDimRegisteredDataSources.First().DimRegisteredDataSource.Name,
                    Organization = localLanguageService.getOrganization(
                        nameFi: dfds.BrFieldDisplaySettingsDimRegisteredDataSources.First().DimRegisteredDataSource.DimOrganization.NameFi,
                        nameEn: dfds.BrFieldDisplaySettingsDimRegisteredDataSources.First().DimRegisteredDataSource.DimOrganization.NameEn,
                        nameSv: dfds.BrFieldDisplaySettingsDimRegisteredDataSources.First().DimRegisteredDataSource.DimOrganization.NameSv
                    )
                };

                // FieldIdentifier defines what type of data the field contains.
                switch (dfds.FieldIdentifier)
                {
                    // Name
                    case Constants.FieldIdentifiers.PERSON_NAME:
                        var nameGroup = new GroupName()
                        {
                            source = source,
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
                    // Other name
                    case Constants.FieldIdentifiers.PERSON_OTHER_NAMES:
                        var otherNameGroup = new GroupOtherName()
                        {
                            source = source,
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
                    // Researcher description
                    case Constants.FieldIdentifiers.PERSON_RESEARCHER_DESCRIPTION:
                        var researcherDescriptionGroup = new GroupResearcherDescription()
                        {
                            source = source,
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
                    // Web link
                    case Constants.FieldIdentifiers.PERSON_WEB_LINK:
                        var webLinkGroup = new GroupWebLink()
                        {
                            source = source,
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
                    // Email address
                    case Constants.FieldIdentifiers.PERSON_EMAIL_ADDRESS:
                        var emailGroup = new GroupEmail()
                        {
                            source = source,
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
                    // Telephone number
                    case Constants.FieldIdentifiers.PERSON_TELEPHONE_NUMBER:
                        var telephoneNumberGroup = new GroupTelephoneNumber()
                        {
                            source = source,
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
                    // Field of science
                    case Constants.FieldIdentifiers.PERSON_FIELD_OF_SCIENCE:
                        var fieldOfScienceGroup = new GroupFieldOfScience()
                        {
                            source = source,
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
                    // Keyword
                    case Constants.FieldIdentifiers.PERSON_KEYWORD:
                        var keywordGroup = new GroupKeyword()
                        {
                            source = source,
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
                    // External identifier
                    case Constants.FieldIdentifiers.PERSON_EXTERNAL_IDENTIFIER:
                        var externalIdentifierGroup = new GroupExternalIdentifier()
                        {
                            source = source,
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
                    // Affiliation
                    case Constants.FieldIdentifiers.ACTIVITY_AFFILIATION:
                        var affiliationGroup = new GroupAffiliation()
                        {
                            source = source,
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
                    // Education
                    case Constants.FieldIdentifiers.ACTIVITY_EDUCATION:
                        var educationGroup = new GroupEducation()
                        {
                            source = source,
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
                    // Publication
                    case Constants.FieldIdentifiers.ACTIVITY_PUBLICATION:
                        var publicationGroup = new GroupPublication()
                        {
                            source = source,
                            items = new List<ItemPublication>() { }
                        };
                        foreach (FactFieldValue ffv in dfds.FactFieldValues)
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
                        if (publicationGroup.items.Count > 0)
                        {
                            person.activity.publicationGroups.Add(publicationGroup);
                        }
                        break;
                    // Publication (ORCID)
                    case Constants.FieldIdentifiers.ACTIVITY_PUBLICATION_ORCID:
                        var orcidPublicationGroup = new GroupPublication()
                        {
                            source = source,
                            items = new List<ItemPublication>() { }
                        };
                        foreach (FactFieldValue ffv in dfds.FactFieldValues)
                        {
                            orcidPublicationGroup.items.Add(
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
                        if (orcidPublicationGroup.items.Count > 0)
                        {
                            person.activity.publicationGroups.Add(orcidPublicationGroup);
                        }
                        break;
                    default:
                        break;
                }  
            }
            return person;
        }
    }
}