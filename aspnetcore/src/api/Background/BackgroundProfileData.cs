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
     *
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
            using IServiceScope scope = _serviceScopeFactory.CreateScope();
            TtvContext localTtvContext = scope.ServiceProvider.GetRequiredService<TtvContext>();
            LanguageService localLanguageService = scope.ServiceProvider.GetRequiredService<LanguageService>();
            OrganizationHandlerService localOrganizationHandlerService = scope.ServiceProvider.GetRequiredService<OrganizationHandlerService>();

            // Get DimFieldDisplaySettings and related entities
            List<DimFieldDisplaySetting> dimFieldDisplaySettings = await localTtvContext.DimFieldDisplaySettings.Where(dfds => dfds.DimUserProfileId == userprofileId && dfds.FactFieldValues.Count() > 0)
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
                // DimIdentifierlessData. Can have a child entity.
                .Include(dfds => dfds.FactFieldValues)
                    .ThenInclude(ffv => ffv.DimIdentifierlessData)
                        .ThenInclude(did => did.InverseDimIdentifierlessData).AsNoTracking()
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

            Person person = new(orcidId)
            {
            };

            // Collect data from DimFieldDisplaySettings and FactFieldValues entities
            foreach (DimFieldDisplaySetting dfds in dimFieldDisplaySettings)
            {
                // Group FactFieldValues by DimRegisteredDataSourceId
                IEnumerable<IGrouping<int, FactFieldValue>> factFieldValues_GroupedBy_DataSourceId = dfds.FactFieldValues.GroupBy(ffv => ffv.DimRegisteredDataSourceId);

                // Loop groups
                foreach (IGrouping<int, FactFieldValue> factFieldValueGroup in factFieldValues_GroupedBy_DataSourceId)
                {
                    DimRegisteredDataSource dimRegisteredDataSource = factFieldValueGroup.First().DimRegisteredDataSource;

                    // Organization name translation
                    NameTranslation nameTranslationSourceOrganization = localLanguageService.GetNameTranslation(
                        nameFi: dimRegisteredDataSource.DimOrganization.NameFi,
                        nameEn: dimRegisteredDataSource.DimOrganization.NameEn,
                        nameSv: dimRegisteredDataSource.DimOrganization.NameSv
                    );

                    // Source object containing registered data source and organization name.
                    Source profileEditorSource = new()
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
                            GroupName nameGroup = new()
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
                            GroupOtherName otherNameGroup = new()
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
                            GroupResearcherDescription researcherDescriptionGroup = new()
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
                            GroupWebLink webLinkGroup = new()
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
                            GroupEmail emailGroup = new()
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
                            GroupTelephoneNumber telephoneNumberGroup = new()
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
                            GroupFieldOfScience fieldOfScienceGroup = new()
                            {
                                source = profileEditorSource,
                                items = new List<ItemFieldOfScience>() { }
                            };
                            foreach (FactFieldValue ffv in factFieldValueGroup)
                            {
                                // Name translation service ensures that none of the language fields is empty.
                                NameTranslation nameTranslationFieldOfScience = localLanguageService.GetNameTranslation(
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
                            GroupKeyword keywordGroup = new()
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
                            GroupExternalIdentifier externalIdentifierGroup = new()
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
                            GroupAffiliation affiliationGroup = new()
                            {
                                source = profileEditorSource,
                                items = new List<ItemAffiliation>() { }
                            };
                            foreach (FactFieldValue ffv in factFieldValueGroup)
                            {
                                // Get organization name from related DimOrganization (ffv.DimAffiliation.DimOrganization), if exists.
                                // Otherwise from DimIdentifierlessData (ffv.DimIdentifierlessData).
                                // Name translation service ensures that none of the language fields is empty.
                                NameTranslation nameTranslationAffiliationOrganization = new();
                                if (ffv.DimAffiliation.DimOrganizationId > 0)
                                {
                                    nameTranslationAffiliationOrganization = localLanguageService.GetNameTranslation(
                                        nameFi: ffv.DimAffiliation.DimOrganization.NameFi,
                                        nameEn: ffv.DimAffiliation.DimOrganization.NameEn,
                                        nameSv: ffv.DimAffiliation.DimOrganization.NameSv
                                    );
                                }
                                else if (ffv.DimIdentifierlessDataId > -1 && ffv.DimIdentifierlessData.Type == Constants.IdentifierlessDataTypes.ORGANIZATION_NAME)
                                {
                                    nameTranslationAffiliationOrganization = localLanguageService.GetNameTranslation(
                                        nameFi: ffv.DimIdentifierlessData.ValueFi,
                                        nameEn: ffv.DimIdentifierlessData.ValueEn,
                                        nameSv: ffv.DimIdentifierlessData.ValueSv
                                    );
                                }

                                // Name translation for position name
                                NameTranslation nameTranslationPositionName = localLanguageService.GetNameTranslation(
                                    nameFi: ffv.DimAffiliation.PositionNameFi,
                                    nameEn: ffv.DimAffiliation.PositionNameEn,
                                    nameSv: ffv.DimAffiliation.PositionNameSv
                                );

                                // Name translation for department name
                                NameTranslation nameTranslationAffiliationDepartment = localLanguageService.GetNameTranslation(
                                    nameFi: "",
                                    nameEn: localOrganizationHandlerService.GetAffiliationDepartmentNameFromFactFieldValue(factFieldValue: ffv),
                                    nameSv: ""
                                );

                                ItemAffiliation affiliation = new()
                                {
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
                            GroupEducation educationGroup = new()
                            {
                                source = profileEditorSource,
                                items = new List<ItemEducation>() { }
                            };
                            foreach (FactFieldValue ffv in factFieldValueGroup)
                            {
                                // Name translation service ensures that none of the language fields is empty.
                                NameTranslation nameTraslationEducation = localLanguageService.GetNameTranslation(
                                    nameFi: ffv.DimEducation.NameFi,
                                    nameEn: ffv.DimEducation.NameEn,
                                    nameSv: ffv.DimEducation.NameSv
                                );

                                ItemEducation education = new()
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
                            GroupPublication publicationGroup = new()
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
                                        Doi = ffv.DimPublication.Doi,
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
                            GroupPublication orcidPublicationGroup = new()
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
                            GroupFundingDecision fundingDecisionGroup = new()
                            {
                                source = profileEditorSource,
                                items = new List<ItemFundingDecision>() { }
                            };
                            foreach (FactFieldValue ffv in factFieldValueGroup)
                            {
                                // Name translation service ensures that none of the language fields is empty.
                                NameTranslation nameTraslationFundingDecision_ProjectName = localLanguageService.GetNameTranslation(
                                    nameFi: ffv.DimFundingDecision.NameFi,
                                    nameSv: ffv.DimFundingDecision.NameSv,
                                    nameEn: ffv.DimFundingDecision.NameEn
                                );
                                NameTranslation nameTraslationFundingDecision_ProjectDescription = localLanguageService.GetNameTranslation(
                                    nameFi: ffv.DimFundingDecision.DescriptionFi,
                                    nameSv: ffv.DimFundingDecision.DescriptionSv,
                                    nameEn: ffv.DimFundingDecision.DescriptionEn
                                );
                                NameTranslation nameTraslationFundingDecision_FunderName = localLanguageService.GetNameTranslation(
                                    nameFi: ffv.DimFundingDecision.DimOrganizationIdFunderNavigation.NameFi,
                                    nameSv: ffv.DimFundingDecision.DimOrganizationIdFunderNavigation.NameSv,
                                    nameEn: ffv.DimFundingDecision.DimOrganizationIdFunderNavigation.NameEn
                                );
                                NameTranslation nameTranslationFundingDecision_TypeOfFunding = localLanguageService.GetNameTranslation(
                                    nameFi: ffv.DimFundingDecision.DimTypeOfFunding.NameFi,
                                    nameSv: ffv.DimFundingDecision.DimTypeOfFunding.NameSv,
                                    nameEn: ffv.DimFundingDecision.DimTypeOfFunding.NameEn
                                );
                                NameTranslation nameTranslationFundingDecision_CallProgramme = localLanguageService.GetNameTranslation(
                                    nameFi: ffv.DimFundingDecision.DimCallProgramme.NameFi,
                                    nameSv: ffv.DimFundingDecision.DimCallProgramme.NameSv,
                                    nameEn: ffv.DimFundingDecision.DimCallProgramme.NameEn
                                );

                                ItemFundingDecision fundingDecision = new()
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
                            GroupResearchDataset researchDatasetGroup = new()
                            {
                                source = profileEditorSource,
                                items = new List<ItemResearchDataset>() { }
                            };
                            foreach (FactFieldValue ffv in factFieldValueGroup)
                            {
                                // Name translation service ensures that none of the language fields is empty.
                                NameTranslation nameTraslationResearchDataset_Name = localLanguageService.GetNameTranslation(
                                    nameFi: ffv.DimResearchDataset.NameFi,
                                    nameSv: ffv.DimResearchDataset.NameSv,
                                    nameEn: ffv.DimResearchDataset.NameEn
                                );
                                NameTranslation nameTraslationResearchDataset_Description = localLanguageService.GetNameTranslation(
                                    nameFi: ffv.DimResearchDataset.DescriptionFi,
                                    nameSv: ffv.DimResearchDataset.DescriptionSv,
                                    nameEn: ffv.DimResearchDataset.DescriptionEn
                                );

                                // Get values from DimPid. There is no FK between DimResearchDataset and DimPid,
                                // so the query must be done separately.
                                List<DimPid> dimPids = await localTtvContext.DimPids.Where(dp => dp.DimResearchDatasetId == ffv.DimResearchDatasetId && ffv.DimResearchDatasetId > -1).AsNoTracking().ToListAsync();

                                List<PreferredIdentifier> preferredIdentifiers = new();
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
                                ItemResearchDataset researchDataset = new()
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