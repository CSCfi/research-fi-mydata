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
            var dimFieldDisplaySettings = await localTtvContext.DimFieldDisplaySettings.Where(dfds => dfds.DimUserProfileId == userprofileId && dfds.FactFieldValues.Count() > 0)
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

            var person = new Person(orcidId)
            {
            };

            // Collect data from DimFieldDisplaySettings and FactFieldValues entities
            foreach (DimFieldDisplaySetting dfds in dimFieldDisplaySettings)
            {
                // Group FactFieldValues by DimRegisteredDataSourceId
                var factFieldValues_GroupedBy_DataSourceId = dfds.FactFieldValues.GroupBy(ffv => ffv.DimRegisteredDataSourceId);

                // Loop groups
                foreach (var factFieldValueGroup in factFieldValues_GroupedBy_DataSourceId)
                {
                    var dimRegisteredDataSource = factFieldValueGroup.First().DimRegisteredDataSource;

                    // Organization name translation
                    var nameTranslationSourceOrganization = localLanguageService.getNameTranslation(
                        nameFi: dimRegisteredDataSource.DimOrganization.NameFi,
                        nameEn: dimRegisteredDataSource.DimOrganization.NameEn,
                        nameSv: dimRegisteredDataSource.DimOrganization.NameSv
                    );

                    // Source object containing registered data source and organization name.
                    var profileEditorSource = new Source()
                    {
                        RegisteredDataSource = dimRegisteredDataSource.Name,
                        Organization = new Organization()
                        {
                            NameFi = nameTranslationSourceOrganization.NameFi,
                            NameEn = nameTranslationSourceOrganization.NameEn,
                            NameSv = nameTranslationSourceOrganization.NameSv
                        }
                    };


                    // FieldIdentifier defines what type of data the field contains.
                    switch (dfds.FieldIdentifier)
                    {
                        // Name
                        case Constants.FieldIdentifiers.PERSON_NAME:
                            var nameGroup = new GroupName()
                            {
                                source = profileEditorSource,
                                items = new List<ItemName>() { }
                            };
                            foreach (FactFieldValue ffv in factFieldValueGroup)
                            {
                                nameGroup.items.Add(
                                    new ItemName()
                                    {
                                        FirstNames = ffv.DimName.FirstNames,
                                        LastName = ffv.DimName.LastName,
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
                                source = profileEditorSource,
                                items = new List<ItemName>() { }
                            };
                            foreach (FactFieldValue ffv in factFieldValueGroup)
                            {
                                otherNameGroup.items.Add(
                                    new ItemName()
                                    {
                                        FullName = ffv.DimName.FullName
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
                                source = profileEditorSource,
                                items = new List<ItemResearcherDescription>() { }
                            };
                            foreach (FactFieldValue ffv in factFieldValueGroup)
                            {
                                researcherDescriptionGroup.items.Add(
                                    new ItemResearcherDescription()
                                    {
                                        ResearchDescriptionEn = ffv.DimResearcherDescription.ResearchDescriptionEn,
                                        ResearchDescriptionFi = ffv.DimResearcherDescription.ResearchDescriptionFi,
                                        ResearchDescriptionSv = ffv.DimResearcherDescription.ResearchDescriptionSv,
          
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
                                source = profileEditorSource,
                                items = new List<ItemWebLink>() { }
                            };
                            foreach (FactFieldValue ffv in factFieldValueGroup)
                            {
                                webLinkGroup.items.Add(
                                    new ItemWebLink()
                                    {
                                        Url = ffv.DimWebLink.Url,
                                        LinkLabel = ffv.DimWebLink.LinkLabel
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
                                source = profileEditorSource,
                                items = new List<ItemEmail>() { }
                            };
                            foreach (FactFieldValue ffv in factFieldValueGroup)
                            {
                                emailGroup.items.Add(
                                    new ItemEmail()
                                    {
                                        Value = ffv.DimEmailAddrress.Email
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
                                source = profileEditorSource,
                                items = new List<ItemTelephoneNumber>() { }
                            };
                            foreach (FactFieldValue ffv in factFieldValueGroup)
                            {
                                telephoneNumberGroup.items.Add(
                                    new ItemTelephoneNumber()
                                    {
                                        Value = ffv.DimTelephoneNumber.TelephoneNumber
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
                                source = profileEditorSource,
                                items = new List<ItemFieldOfScience>() { }
                            };
                            foreach (FactFieldValue ffv in factFieldValueGroup)
                            {
                                // Name translation service ensures that none of the language fields is empty.
                                var nameTranslationFieldOfScience = localLanguageService.getNameTranslation(
                                    nameFi: ffv.DimFieldOfScience.NameFi,
                                    nameEn: ffv.DimFieldOfScience.NameEn,
                                    nameSv: ffv.DimFieldOfScience.NameSv
                                );

                                fieldOfScienceGroup.items.Add(
                                    new ItemFieldOfScience()
                                    {
                                        NameFi = nameTranslationFieldOfScience.NameFi,
                                        NameEn = nameTranslationFieldOfScience.NameEn,
                                        NameSv = nameTranslationFieldOfScience.NameSv
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
                                source = profileEditorSource,
                                items = new List<ItemKeyword>() { }
                            };
                            foreach (FactFieldValue ffv in factFieldValueGroup)
                            {
                                keywordGroup.items.Add(
                                    new ItemKeyword()
                                    {
                                        Value = ffv.DimKeyword.Keyword
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
                                source = profileEditorSource,
                                items = new List<ItemExternalIdentifier>() { }
                            };
                            foreach (FactFieldValue ffv in factFieldValueGroup)
                            {
                                externalIdentifierGroup.items.Add(
                                    new ItemExternalIdentifier()
                                    {
                                        PidContent = ffv.DimPid.PidContent,
                                        PidType = ffv.DimPid.PidType
                                    }
                                );
                            }
                            if (externalIdentifierGroup.items.Count > 0)
                            {
                                person.personal.externalIdentifierGroups.Add(externalIdentifierGroup);
                            }
                            break;

                        // Role in researcher community
                        case Constants.FieldIdentifiers.ACTIVITY_ROLE_IN_RESERCH_COMMUNITY:
                            // TODO
                            break;

                        // Affiliation
                        case Constants.FieldIdentifiers.ACTIVITY_AFFILIATION:
                            var affiliationGroup = new GroupAffiliation()
                            {
                                source = profileEditorSource,
                                items = new List<ItemAffiliation>() { }
                            };
                            foreach (FactFieldValue ffv in factFieldValueGroup)
                            {
                                // Name translation service ensures that none of the language fields is empty.
                                var nameTranslationAffiliationOrganization = localLanguageService.getNameTranslation(
                                    nameFi: ffv.DimAffiliation.DimOrganization.NameFi,
                                    nameEn: ffv.DimAffiliation.DimOrganization.NameEn,
                                    nameSv: ffv.DimAffiliation.DimOrganization.NameSv
                                );
                                var nameTranslationPositionName = localLanguageService.getNameTranslation(
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
                                if (ffv.DimAffiliation.DimOrganization.SourceId == Constants.SourceIdentifiers.PROFILE_API)
                                {
                                    nameTranslationAffiliationDepartment = localLanguageService.getNameTranslation(
                                        "",
                                        nameEn: ffv.DimAffiliation.DimOrganization.NameUnd,
                                        ""
                                    );
                                }

                                var affiliation = new ItemAffiliation()
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
                                    StartDate = new ItemDate()
                                    {
                                        Year = ffv.DimAffiliation.StartDateNavigation.Year,
                                        Month = ffv.DimAffiliation.StartDateNavigation.Month,
                                        Day = ffv.DimAffiliation.StartDateNavigation.Day
                                    }
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
                                source = profileEditorSource,
                                items = new List<ItemEducation>() { }
                            };
                            foreach (FactFieldValue ffv in factFieldValueGroup)
                            {
                                // Name translation service ensures that none of the language fields is empty.
                                var nameTraslationEducation = localLanguageService.getNameTranslation(
                                    nameFi: ffv.DimEducation.NameFi,
                                    nameEn: ffv.DimEducation.NameEn,
                                    nameSv: ffv.DimEducation.NameSv
                                );

                                var education = new ItemEducation()
                                {
                                    NameFi = nameTraslationEducation.NameFi,
                                    NameEn = nameTraslationEducation.NameEn,
                                    NameSv = nameTraslationEducation.NameSv,
                                    DegreeGrantingInstitutionName = ffv.DimEducation.DegreeGrantingInstitutionName
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
                                source = profileEditorSource,
                                items = new List<ItemPublication>() { }
                            };
                            foreach (FactFieldValue ffv in factFieldValueGroup)
                            {
                                publicationGroup.items.Add(
                                    new ItemPublication()
                                    {
                                        PublicationId = ffv.DimPublication.PublicationId,
                                        PublicationName = ffv.DimPublication.PublicationName,
                                        PublicationYear = ffv.DimPublication.PublicationYear,
                                        Doi = ffv.DimPublication.DoiHandle,
                                        TypeCode = ffv.DimPublication.PublicationTypeCode
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
                                source = profileEditorSource,
                                items = new List<ItemPublication>() { }
                            };
                            foreach (FactFieldValue ffv in factFieldValueGroup)
                            {
                                orcidPublicationGroup.items.Add(

                                    new ItemPublication()
                                    {
                                        PublicationId = ffv.DimOrcidPublication.PublicationId,
                                        PublicationName = ffv.DimOrcidPublication.PublicationName,
                                        PublicationYear = ffv.DimOrcidPublication.PublicationYear,
                                        Doi = ffv.DimOrcidPublication.DoiHandle,
                                        TypeCode = "" // TODO: ORCID publication type code handling
                                    }
                                );
                            }
                            if (orcidPublicationGroup.items.Count > 0)
                            {
                                person.activity.publicationGroups.Add(orcidPublicationGroup);
                            }
                            break;

                        // Funding decision
                        case Constants.FieldIdentifiers.ACTIVITY_FUNDING_DECISION:
                            var fundingDecisionGroup = new GroupFundingDecision()
                            {
                                source = profileEditorSource,
                                items = new List<ItemFundingDecision>() { }
                            };
                            foreach (FactFieldValue ffv in factFieldValueGroup)
                            {
                                // Name translation service ensures that none of the language fields is empty.
                                var nameTraslationFundingDecision_ProjectName = localLanguageService.getNameTranslation(
                                    nameFi: ffv.DimFundingDecision.NameFi,
                                    nameSv: ffv.DimFundingDecision.NameSv,
                                    nameEn: ffv.DimFundingDecision.NameEn
                                );
                                var nameTraslationFundingDecision_ProjectDescription = localLanguageService.getNameTranslation(
                                    nameFi: ffv.DimFundingDecision.DescriptionFi,
                                    nameSv: ffv.DimFundingDecision.DescriptionSv,
                                    nameEn: ffv.DimFundingDecision.DescriptionEn
                                );
                                var nameTraslationFundingDecision_FunderName = localLanguageService.getNameTranslation(
                                    nameFi: ffv.DimFundingDecision.DimOrganizationIdFunderNavigation.NameFi,
                                    nameSv: ffv.DimFundingDecision.DimOrganizationIdFunderNavigation.NameSv,
                                    nameEn: ffv.DimFundingDecision.DimOrganizationIdFunderNavigation.NameEn
                                );
                                var nameTranslationFundingDecision_TypeOfFunding = localLanguageService.getNameTranslation(
                                    nameFi: ffv.DimFundingDecision.DimTypeOfFunding.NameFi,
                                    nameSv: ffv.DimFundingDecision.DimTypeOfFunding.NameSv,
                                    nameEn: ffv.DimFundingDecision.DimTypeOfFunding.NameEn
                                );
                                var nameTranslationFundingDecision_CallProgramme = localLanguageService.getNameTranslation(
                                    nameFi: ffv.DimFundingDecision.DimCallProgramme.NameFi,
                                    nameSv: ffv.DimFundingDecision.DimCallProgramme.NameSv,
                                    nameEn: ffv.DimFundingDecision.DimCallProgramme.NameEn
                                );

                                var fundingDecision = new ItemFundingDecision()
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
                                    AmountInEur = ffv.DimFundingDecision.AmountInEur
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
                                person.activity.fundingDecisionGroups.Add(fundingDecisionGroup);
                            }
                            break;

                        // Research dataset
                        case Constants.FieldIdentifiers.ACTIVITY_RESEARCH_DATASET:
                            var researchDatasetGroup = new GroupResearchDataset()
                            {
                                source = profileEditorSource,
                                items = new List<ItemResearchDataset>() { }
                            };
                            foreach (FactFieldValue ffv in factFieldValueGroup)
                            {
                                // Name translation service ensures that none of the language fields is empty.
                                var nameTraslationResearchDataset_Name = localLanguageService.getNameTranslation(
                                    nameFi: ffv.DimResearchDataset.NameFi,
                                    nameSv: ffv.DimResearchDataset.NameSv,
                                    nameEn: ffv.DimResearchDataset.NameEn
                                );
                                var nameTraslationResearchDataset_Description = localLanguageService.getNameTranslation(
                                    nameFi: ffv.DimResearchDataset.DescriptionFi,
                                    nameSv: ffv.DimResearchDataset.DescriptionSv,
                                    nameEn: ffv.DimResearchDataset.DescriptionEn
                                );

                                // Get values from DimPid. There is no FK between DimResearchDataset and DimPid,
                                // so the query must be done separately.
                                var dimPids = await localTtvContext.DimPids.Where(dp => dp.DimResearchDatasetId == ffv.DimResearchDatasetId && ffv.DimResearchDatasetId > -1).AsNoTracking().ToListAsync();

                                var preferredIdentifiers = new List<PreferredIdentifier>();
                                foreach (DimPid dimPid in dimPids)
                                {
                                    preferredIdentifiers.Add(
                                        new PreferredIdentifier()
                                        {
                                            PidType = dimPid.PidType,
                                            PidContent = dimPid.PidContent
                                        }
                                    );
                                }

                                // TODO: add properties according to ElasticSearch index
                                var researchDataset = new ItemResearchDataset()
                                {
                                    Actor = new List<Actor>(),
                                    Identifier = ffv.DimResearchDataset.LocalIdentifier,
                                    NameFi = nameTraslationResearchDataset_Name.NameFi,
                                    NameSv = nameTraslationResearchDataset_Name.NameSv,
                                    NameEn = nameTraslationResearchDataset_Name.NameEn,
                                    DescriptionFi = nameTraslationResearchDataset_Description.NameFi,
                                    DescriptionSv = nameTraslationResearchDataset_Description.NameSv,
                                    DescriptionEn = nameTraslationResearchDataset_Description.NameEn,
                                    PreferredIdentifiers = preferredIdentifiers
                                };

                                // DatasetCreated
                                if (ffv.DimResearchDataset.DatasetCreated != null)
                                {
                                    researchDataset.DatasetCreated = ffv.DimResearchDataset.DatasetCreated.Value.Year;
                                }

                                // Fill actors list
                                foreach (FactContribution fc in ffv.DimResearchDataset.FactContributions)
                                {
                                    researchDataset.Actor.Add(
                                        new Actor()
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
                                person.activity.researchDatasetGroups.Add(researchDatasetGroup);
                            }
                            break;

                        default:
                            break;
                    }
                }
            }

            return person;
        }
    }
}