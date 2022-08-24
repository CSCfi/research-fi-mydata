using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.Models;
using api.Models.Ttv;
using api.Models.Elasticsearch;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using api.Models.Common;
using api.Models.ProfileEditor;
using Dapper;
using Microsoft.AspNetCore.Http;

namespace api.Services
{
    /*
     * BackgroundProfiledata gets user profile data and constructs an entry for Elasticsearch person index.
     *
     * In normal controller code the request context has access to database via ttvContext.
     * In a background task that is not available, since it is disposed when the response is sent.
     * Here a local scope is created and database context can be taken from that scope.
     */
    public class BackgroundProfiledata : IBackgroundProfiledata
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
            ILanguageService localLanguageService = scope.ServiceProvider.GetRequiredService<ILanguageService>();
            IOrganizationHandlerService localOrganizationHandlerService = scope.ServiceProvider.GetRequiredService<IOrganizationHandlerService>();
            ITtvSqlService localTtvSqlService = scope.ServiceProvider.GetRequiredService<ITtvSqlService>();

            // Get SQL statement for profile data query
            string profileDataSql = localTtvSqlService.GetSqlQuery_ProfileData(userprofileId);

            // Execute SQL statemen using Dapper
            var connection = localTtvContext.Database.GetDbConnection();
            List<ProfileDataRaw> profileDataRaws = (await connection.QueryAsync<ProfileDataRaw>(profileDataSql)).ToList();

            // Group response by DimRegisteredDataSource_Id
            IEnumerable<IGrouping<int, ProfileDataRaw>> profileDataRaw_GroupedBy_DataSourceId = profileDataRaws.GroupBy(p => p.DimRegisteredDataSource_Id);

            Person person = new(orcidId)
            {
            };

            // Loop data source groups
            foreach (IGrouping<int, ProfileDataRaw> profileDataGroup in profileDataRaw_GroupedBy_DataSourceId)
            {
                // Set data source object. It can be taken from the first object in list, because all object in the group have the same data source.

                // Organization name translation
                NameTranslation nameTranslationSourceOrganization = localLanguageService.GetNameTranslation(
                    nameFi: profileDataGroup.First().DimRegisteredDataSource_DimOrganization_NameFi,
                    nameEn: profileDataGroup.First().DimRegisteredDataSource_DimOrganization_NameEn,
                    nameSv: profileDataGroup.First().DimRegisteredDataSource_DimOrganization_NameSv
                );

                // Source object containing registered data source and organization name.
                Source profileEditorSource = new()
                {
                    RegisteredDataSource = profileDataGroup.First().DimRegisteredDataSource_Name,
                    Organization = new Organization()
                    {
                        NameFi = nameTranslationSourceOrganization.NameFi,
                        NameEn = nameTranslationSourceOrganization.NameEn,
                        NameSv = nameTranslationSourceOrganization.NameSv
                    }
                };

                // Additionally, group the items by DimFieldDisplaySettings_FieldIdentifier.
                // FieldIdentifier indicates what type of data the field contains (name, other name, weblink, etc).
                IEnumerable<IGrouping<int, ProfileDataRaw>> profileDataGroups2 = profileDataGroup.GroupBy(q => q.DimFieldDisplaySettings_FieldIdentifier);

                // Loop field identifier groups.
                foreach (IGrouping<int, ProfileDataRaw> profileDataGroup2 in profileDataGroups2)
                {
                    // Data items are collected into own lists
                    List<ItemName> nameItems = new();
                    List<ItemName> otherNameItems = new();
                    List<ItemWebLink> weblinkItems = new();
                    List<ItemResearcherDescription> researcherDescriptionItems = new();
                    List<ItemEmail> emailItems = new();
                    List<ItemTelephoneNumber> telephoneNumberItems = new();
                    List<ItemFieldOfScience> fieldOfScienceItems = new();
                    List<ItemKeyword> keywordItems = new();
                    List<ItemExternalIdentifier> externalIdentifierItems = new();
                    List<ItemAffiliation> affiliationItems = new();
                    List<ItemEducation> educationItems = new();
                    List<ItemPublication> publicationItems = new();

                    // Loop items in a field identifier group
                    foreach (ProfileDataRaw profileData2 in profileDataGroup2)
                    {
                        // FieldIdentifier defines what type of data the field contains.
                        switch (profileData2.DimFieldDisplaySettings_FieldIdentifier)
                        {
                            // Name
                            case Constants.FieldIdentifiers.PERSON_NAME:
                                nameItems.Add(
                                    new ItemName()
                                    {
                                        FirstNames = profileData2.DimName_FirstNames,
                                        LastName = profileData2.DimName_LastName
                                    }
                                );
                                break;

                            // Other name
                            case Constants.FieldIdentifiers.PERSON_OTHER_NAMES:
                                otherNameItems.Add(
                                    new ItemName()
                                    {
                                        FullName = profileData2.DimName_FullName
                                    }
                                );
                                break;

                            // Web link
                            case Constants.FieldIdentifiers.PERSON_WEB_LINK:
                                weblinkItems.Add(
                                    new ItemWebLink()
                                    {
                                        Url = profileData2.DimWebLink_Url,
                                        LinkLabel = profileData2.DimWebLink_LinkLabel
                                    }
                                );
                                break;

                            // ResearcherDescription
                            case Constants.FieldIdentifiers.PERSON_RESEARCHER_DESCRIPTION:
                                // Researcher description name translation
                                NameTranslation nameTranslationResearcherDescription = localLanguageService.GetNameTranslation(
                                    nameFi: profileData2.DimResearcherDescription_ResearchDescriptionFi,
                                    nameEn: profileData2.DimResearcherDescription_ResearchDescriptionEn,
                                    nameSv: profileData2.DimResearcherDescription_ResearchDescriptionSv
                                );
                                researcherDescriptionItems.Add(
                                    new ItemResearcherDescription()
                                    {
                                        ResearchDescriptionFi = nameTranslationResearcherDescription.NameFi,
                                        ResearchDescriptionEn = nameTranslationResearcherDescription.NameEn,
                                        ResearchDescriptionSv = nameTranslationResearcherDescription.NameSv
                                    }
                                );
                                break;

                            // Email
                            case Constants.FieldIdentifiers.PERSON_EMAIL_ADDRESS:
                                emailItems.Add(
                                    new ItemEmail()
                                    {
                                        Value = profileData2.DimEmailAddrress_Email
                                    }
                                );
                                break;

                            // Telephone number
                            case Constants.FieldIdentifiers.PERSON_TELEPHONE_NUMBER:
                                telephoneNumberItems.Add(
                                    new ItemTelephoneNumber()
                                    {
                                        Value = profileData2.DimTelephoneNumber_TelephoneNumber
                                    }
                                );
                                break;

                            // Field of science
                            case Constants.FieldIdentifiers.PERSON_FIELD_OF_SCIENCE:
                                // Field of science name translation
                                NameTranslation nameTranslationFieldOfScience = localLanguageService.GetNameTranslation(
                                    nameFi: profileData2.DimFieldOfScience_NameFi,
                                    nameEn: profileData2.DimFieldOfScience_NameEn,
                                    nameSv: profileData2.DimFieldOfScience_NameSv
                                );

                                fieldOfScienceItems.Add(
                                    new ItemFieldOfScience()
                                    {
                                        NameFi = nameTranslationFieldOfScience.NameFi,
                                        NameEn = nameTranslationFieldOfScience.NameEn,
                                        NameSv = nameTranslationFieldOfScience.NameSv
                                    }
                                );
                                break;

                            // Keyword
                            case Constants.FieldIdentifiers.PERSON_KEYWORD:
                                keywordItems.Add(
                                    new ItemKeyword()
                                    {
                                        Value = profileData2.DimKeyword_Keyword
                                    }
                                );
                                break;

                            // External identifier
                            case Constants.FieldIdentifiers.PERSON_EXTERNAL_IDENTIFIER:
                                externalIdentifierItems.Add(
                                    new ItemExternalIdentifier()
                                    {
                                        PidContent = profileData2.DimPid_PidContent,
                                        PidType = profileData2.DimPid_PidType
                                    }
                                );
                                break;

                            // Affiliation
                            case Constants.FieldIdentifiers.ACTIVITY_AFFILIATION:
                                // Get affiliation organization name from related DimOrganization (ffv.DimAffiliation.DimOrganization), if exists.
                                // Otherwise from DimIdentifierlessData (ffv.DimIdentifierlessData).
                                // Name translation service ensures that none of the language fields is empty.
                                NameTranslation nameTranslationAffiliationOrganization = new();
                                if (profileData2.DimAffiliation_DimOrganization_Id > 0)
                                {
                                    nameTranslationAffiliationOrganization = localLanguageService.GetNameTranslation(
                                        nameFi: profileData2.DimAffiliation_DimOrganization_NameFi,
                                        nameEn: profileData2.DimAffiliation_DimOrganization_NameEn,
                                        nameSv: profileData2.DimAffiliation_DimOrganization_NameSv
                                    );
                                }
                                else if (profileData2.FactFieldValues_DimIdentifierlessDataId > -1 &&
                                    profileData2.DimIdentifierlessData_Type == Constants.IdentifierlessDataTypes.ORGANIZATION_NAME)
                                {
                                    nameTranslationAffiliationOrganization = localLanguageService.GetNameTranslation(
                                        nameFi: profileData2.DimIdentifierlessData_ValueFi,
                                        nameEn: profileData2.DimIdentifierlessData_ValueEn,
                                        nameSv: profileData2.DimIdentifierlessData_ValueSv
                                    );
                                }

                                // Name translation for position name
                                NameTranslation nameTranslationPositionName = localLanguageService.GetNameTranslation(
                                    nameFi: profileData2.DimAffiliation_PositionNameFi,
                                    nameEn: profileData2.DimAffiliation_PositionNameEn,
                                    nameSv: profileData2.DimAffiliation_PositionNameSv
                                );

                                // Name translation for department name
                                NameTranslation nameTranslationAffiliationDepartment = new();
                                if (profileData2.DimIdentifierlessData_Type != null && profileData2.DimIdentifierlessData_Type == Constants.IdentifierlessDataTypes.ORGANIZATION_UNIT)
                                {
                                    nameTranslationAffiliationDepartment = localLanguageService.GetNameTranslation(
                                        nameFi: profileData2.DimIdentifierlessData_ValueFi,
                                        nameEn: profileData2.DimIdentifierlessData_ValueEn,
                                        nameSv: profileData2.DimIdentifierlessData_ValueSv
                                    );
                                }
                                else if (profileData2.DimIdentifierlessData_Child_Type != null && profileData2.DimIdentifierlessData_Child_Type == Constants.IdentifierlessDataTypes.ORGANIZATION_UNIT)
                                {
                                    nameTranslationAffiliationDepartment = localLanguageService.GetNameTranslation(
                                        nameFi: profileData2.DimIdentifierlessData_Child_ValueFi,
                                        nameEn: profileData2.DimIdentifierlessData_Child_ValueEn,
                                        nameSv: profileData2.DimIdentifierlessData_Child_ValueSv
                                    );
                                }

                                affiliationItems.Add(
                                    new()
                                    {
                                        OrganizationNameFi = nameTranslationAffiliationOrganization.NameFi,
                                        OrganizationNameEn = nameTranslationAffiliationOrganization.NameEn,
                                        OrganizationNameSv = nameTranslationAffiliationOrganization.NameSv,
                                        DepartmentNameFi = nameTranslationAffiliationDepartment.NameFi,
                                        DepartmentNameEn = nameTranslationAffiliationDepartment.NameSv,
                                        DepartmentNameSv = nameTranslationAffiliationDepartment.NameEn,
                                        PositionNameFi = nameTranslationPositionName.NameFi,
                                        PositionNameEn = nameTranslationPositionName.NameEn,
                                        PositionNameSv = nameTranslationPositionName.NameSv,
                                        Type = profileData2.DimAffiliation_DimReferenceData_NameFi,
                                        StartDate = new ItemDate()
                                        {
                                            Year = profileData2.DimAffiliation_StartDate_Year,
                                            Month = profileData2.DimAffiliation_StartDate_Month,
                                            Day = profileData2.DimAffiliation_StartDate_Day
                                        },
                                        EndDate = new ItemDate()
                                        {
                                            Year = profileData2.DimAffiliation_EndDate_Year,
                                            Month = profileData2.DimAffiliation_EndDate_Month,
                                            Day = profileData2.DimAffiliation_EndDate_Day
                                        }
                                    }
                                );
                                break;

                            // Education
                            case Constants.FieldIdentifiers.ACTIVITY_EDUCATION:
                                // Name translation service ensures that none of the language fields is empty.
                                NameTranslation nameTraslationEducation = localLanguageService.GetNameTranslation(
                                    nameFi: profileData2.DimEducation_NameFi,
                                    nameEn: profileData2.DimEducation_NameEn,
                                    nameSv: profileData2.DimEducation_NameSv
                                );

                                educationItems.Add(
                                    new()
                                    {
                                        NameFi = nameTraslationEducation.NameFi,
                                        NameEn = nameTraslationEducation.NameEn,
                                        NameSv = nameTraslationEducation.NameSv,
                                        DegreeGrantingInstitutionName = profileData2.DimEducation_DegreeGrantingInstitutionName,
                                        StartDate = new ItemDate()
                                        {
                                            Year = profileData2.DimEducation_StartDate_Year,
                                            Month = profileData2.DimEducation_StartDate_Month,
                                            Day = profileData2.DimEducation_StartDate_Day
                                        },
                                        EndDate = new ItemDate()
                                        {
                                            Year = profileData2.DimEducation_EndDate_Year,
                                            Month = profileData2.DimEducation_EndDate_Month,
                                            Day = profileData2.DimEducation_EndDate_Day
                                        }
                                    }
                                );
                                break;

                            // Publication
                            case Constants.FieldIdentifiers.ACTIVITY_PUBLICATION:
                                publicationItems.Add(
                                    new ItemPublication()
                                    {
                                        PublicationId = profileData2.DimPublication_PublicationId,
                                        PublicationName = profileData2.DimPublication_PublicationName,
                                        PublicationYear = profileData2.DimPublication_PublicationYear,
                                        Doi = profileData2.DimPublication_Doi,
                                        TypeCode = profileData2.DimPublication_PublicationTypeCode
                                    }
                                );

                                break;

                            // Publication(ORCID)
                            case Constants.FieldIdentifiers.ACTIVITY_PUBLICATION_ORCID:
                                publicationItems.Add(
                                    new ItemPublication()
                                    {
                                        PublicationId = profileData2.DimOrcidPublication_PublicationId,
                                        PublicationName = profileData2.DimOrcidPublication_PublicationName,
                                        PublicationYear = profileData2.DimOrcidPublication_PublicationYear,
                                        Doi = profileData2.DimOrcidPublication_Doi,
                                        TypeCode = ""
                                    }
                                );
                                break;
                        }
                    }

                    // Name groups
                    if (nameItems.Count > 0)
                    {
                        person.personal.nameGroups.Add(
                            new()
                            {
                                source = profileEditorSource,
                                items = nameItems
                            }
                        );
                    }

                    // Other name groups
                    if (otherNameItems.Count > 0)
                    {
                        person.personal.otherNameGroups.Add(
                            new()
                            {
                                source = profileEditorSource,
                                items = otherNameItems
                            }
                        );
                    }

                    // Web link groups
                    if (weblinkItems.Count > 0)
                    {
                        person.personal.webLinkGroups.Add(
                            new()
                            {
                                source = profileEditorSource,
                                items = weblinkItems
                            }
                        );
                    }

                    // Researcher description groups
                    if (researcherDescriptionItems.Count > 0)
                    {
                        person.personal.researcherDescriptionGroups.Add(
                            new()
                            {
                                source = profileEditorSource,
                                items = researcherDescriptionItems
                            }
                        );
                    }

                    // Email groups
                    if (emailItems.Count > 0)
                    {
                        person.personal.emailGroups.Add(
                            new()
                            {
                                source = profileEditorSource,
                                items = emailItems
                            }
                        );
                    }

                    // Telephone number groups
                    if (telephoneNumberItems.Count > 0)
                    {
                        person.personal.telephoneNumberGroups.Add(
                            new()
                            {
                                source = profileEditorSource,
                                items = telephoneNumberItems
                            }
                        );
                    }

                    // Field of science groups
                    if (fieldOfScienceItems.Count > 0)
                    {
                        person.personal.fieldOfScienceGroups.Add(
                            new()
                            {
                                source = profileEditorSource,
                                items = fieldOfScienceItems
                            }
                        );
                    }

                    // Keyword groups
                    if (keywordItems.Count > 0)
                    {
                        person.personal.keywordGroups.Add(
                            new()
                            {
                                source = profileEditorSource,
                                items = keywordItems,
                            }
                        );
                    }

                    // External identifier groups
                    if (externalIdentifierItems.Count > 0)
                    {
                        person.personal.externalIdentifierGroups.Add(
                            new()
                            {
                                source = profileEditorSource,
                                items = externalIdentifierItems
                            }
                        );
                    }

                    // Affiliation groups
                    if (affiliationItems.Count > 0)
                    {
                        person.activity.affiliationGroups.Add(
                            new()
                            {
                                source = profileEditorSource,
                                items = affiliationItems
                            }
                        );
                    }

                    // Education groups
                    if (educationItems.Count > 0)
                    {
                        person.activity.educationGroups.Add(
                            new()
                            {
                                source = profileEditorSource,
                                items = educationItems
                            }
                        );
                    }

                    // Publication groups
                    if (publicationItems.Count > 0)
                    {
                        person.activity.publicationGroups.Add(
                            new()
                            {
                                source = profileEditorSource,
                                items = publicationItems
                            }
                        );
                    }
                }
            }

            return person;
        }
    }
}