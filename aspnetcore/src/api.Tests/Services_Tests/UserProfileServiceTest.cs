using Xunit;
using api.Services;
using api.Models.Common;
using api.Models.Ttv;
using System.Collections.Generic;
using System;

namespace api.Tests
{
    [Collection("User profile service tests")]
    public class UserProfileServiceTests
    {
        [Fact(DisplayName = "Get FieldIdentifiers")]
        public void getFieldIdentifiers_01()
        {
            // Arrange
            UserProfileService userProfileService = new UserProfileService();
            // Act
            List<int> fieldIdentifiers = userProfileService.GetFieldIdentifiers();
            // Assert
            Assert.Equal(16, fieldIdentifiers.Count);
            Assert.Contains<int>(Constants.FieldIdentifiers.PERSON_EMAIL_ADDRESS, fieldIdentifiers);
            Assert.Contains<int>(Constants.FieldIdentifiers.PERSON_EXTERNAL_IDENTIFIER, fieldIdentifiers);
            Assert.Contains<int>(Constants.FieldIdentifiers.PERSON_FIELD_OF_SCIENCE, fieldIdentifiers);
            Assert.Contains<int>(Constants.FieldIdentifiers.PERSON_KEYWORD, fieldIdentifiers);
            Assert.Contains<int>(Constants.FieldIdentifiers.PERSON_NAME, fieldIdentifiers);
            Assert.Contains<int>(Constants.FieldIdentifiers.PERSON_OTHER_NAMES, fieldIdentifiers);
            Assert.Contains<int>(Constants.FieldIdentifiers.PERSON_RESEARCHER_DESCRIPTION, fieldIdentifiers);
            Assert.Contains<int>(Constants.FieldIdentifiers.PERSON_TELEPHONE_NUMBER, fieldIdentifiers);
            Assert.Contains<int>(Constants.FieldIdentifiers.PERSON_WEB_LINK, fieldIdentifiers);
            Assert.Contains<int>(Constants.FieldIdentifiers.ACTIVITY_AFFILIATION, fieldIdentifiers);
            Assert.Contains<int>(Constants.FieldIdentifiers.ACTIVITY_EDUCATION, fieldIdentifiers);
            Assert.Contains<int>(Constants.FieldIdentifiers.ACTIVITY_PUBLICATION, fieldIdentifiers);
            Assert.Contains<int>(Constants.FieldIdentifiers.ACTIVITY_PUBLICATION_PROFILE_ONLY, fieldIdentifiers);
            Assert.Contains<int>(Constants.FieldIdentifiers.ACTIVITY_FUNDING_DECISION, fieldIdentifiers);
            Assert.Contains<int>(Constants.FieldIdentifiers.ACTIVITY_RESEARCH_DATASET, fieldIdentifiers);
            Assert.Contains<int>(Constants.FieldIdentifiers.ACTIVITY_RESEARCH_ACTIVITY, fieldIdentifiers);
        }

        [Fact(DisplayName = "CanDeleteFactFieldValueRelatedData returns true")]
        public void canDeleteFactFieldValueRelatedData_01()
        {
            // Arrange
            DataSourceHelperService dataSourceHelperService = new DataSourceHelperService();
            dataSourceHelperService.DimRegisteredDataSourceId_ORCID = 12345;
            UserProfileService userProfileService = new UserProfileService(dataSourceHelperService:dataSourceHelperService);
            FactFieldValue ffv = new()
            {
                DimRegisteredDataSourceId = 12345 // Registered data source is the same as DimRegisteredDataSourceId_ORCID
            };
            // Act
            bool actualCanDeleteFactFieldValueRelatedData = userProfileService.CanDeleteFactFieldValueRelatedData(ffv);
            // Assert
            Assert.True(actualCanDeleteFactFieldValueRelatedData);
        }

        [Fact(DisplayName = "CanDeleteFactFieldValueRelatedData returns false")]
        public void canDeleteFactFieldValueRelatedData_02()
        {
            // Arrange
            DataSourceHelperService dataSourceHelperService = new DataSourceHelperService();
            dataSourceHelperService.DimRegisteredDataSourceId_ORCID = 12345;
            UserProfileService userProfileService = new UserProfileService(dataSourceHelperService: dataSourceHelperService);
            FactFieldValue ffv = new()
            {
                DimRegisteredDataSourceId = 54321 // Registered data source is different from DimRegisteredDataSourceId_ORCID
            };
            // Act
            bool actualCanDeleteFactFieldValueRelatedData = userProfileService.CanDeleteFactFieldValueRelatedData(ffv);
            // Assert
            Assert.False(actualCanDeleteFactFieldValueRelatedData);
        }

        [Fact(DisplayName = "Get new DimKnownPerson")]
        public void getDimKnownPerson_01()
        {
            // Arrange
            DataSourceHelperService dataSourceHelperService = new DataSourceHelperService();
            int orcidRegisteredDataSourceId = 789;
            dataSourceHelperService.DimRegisteredDataSourceId_ORCID = orcidRegisteredDataSourceId;
            UserProfileService userProfileService = new UserProfileService(dataSourceHelperService: dataSourceHelperService);
            DateTime testDateTime = DateTime.Now;
            string orcidId = "abcd-ef12-3456-7890";
            // Act
            DimKnownPerson actualDimKnownPerson = userProfileService.GetNewDimKnownPerson(orcidId, testDateTime);
            // Assert
            Assert.Equal(orcidId, actualDimKnownPerson.SourceId);
            Assert.Equal(Constants.SourceDescriptions.PROFILE_API, actualDimKnownPerson.SourceDescription);
            Assert.Equal(orcidRegisteredDataSourceId, actualDimKnownPerson.DimRegisteredDataSourceId);

        }

        [Fact(DisplayName = "Get empty FactFieldValue")]
        public void getEmptyFactFieldValue_01()
        {
            // Arrange
            UtilityService utilityService = new UtilityService();
            UserProfileService userProfileService = new UserProfileService(utilityService: utilityService);
            // Act
            FactFieldValue actualFfv = userProfileService.GetEmptyFactFieldValue();
            // Assert
            Assert.Equal<int>(-1, actualFfv.DimUserProfileId);
            Assert.Equal<int>(-1, actualFfv.DimFieldDisplaySettingsId);
            Assert.Equal<int>(-1, actualFfv.DimNameId);
            Assert.Equal<int>(-1, actualFfv.DimWebLinkId);
            Assert.Equal<int>(-1, actualFfv.DimFundingDecisionId);
            Assert.Equal<int>(-1, actualFfv.DimPublicationId);
            Assert.Equal<int>(-1, actualFfv.DimPidId);
            Assert.Equal<int>(-1, actualFfv.DimPidIdOrcidPutCode);
            Assert.Equal<int>(-1, actualFfv.DimResearchActivityId);
            Assert.Equal<int>(-1, actualFfv.DimEventId);
            Assert.Equal<int>(-1, actualFfv.DimEducationId);
            Assert.Equal<int>(-1, actualFfv.DimCompetenceId);
            Assert.Equal<int>(-1, actualFfv.DimResearchCommunityId);
            Assert.Equal<int>(-1, actualFfv.DimTelephoneNumberId);
            Assert.Equal<int>(-1, actualFfv.DimEmailAddrressId);
            Assert.Equal<int>(-1, actualFfv.DimResearcherDescriptionId);
            Assert.Equal<int>(-1, actualFfv.DimIdentifierlessDataId);
            Assert.Equal<int>(-1, actualFfv.DimProfileOnlyPublicationId);
            Assert.Equal<int>(-1, actualFfv.DimProfileOnlyResearchActivityId);
            Assert.Equal<int>(-1, actualFfv.DimKeywordId);
            Assert.Equal<int>(-1, actualFfv.DimAffiliationId);
            Assert.Equal<int>(-1, actualFfv.DimResearcherToResearchCommunityId);
            Assert.Equal<int>(-1, actualFfv.DimResearchDatasetId);
            Assert.Equal<int>(-1, actualFfv.DimReferencedataFieldOfScienceId);
            Assert.Equal<int>(-1, actualFfv.DimReferencedataActorRoleId);
            Assert.False(actualFfv.Show);
            Assert.False(actualFfv.PrimaryValue);
            Assert.Equal(Constants.SourceIdentifiers.PROFILE_API, actualFfv.SourceId);
            Assert.Equal(Constants.SourceDescriptions.PROFILE_API, actualFfv.SourceDescription);
        }

        [Fact(DisplayName = "Get empty DimProfileOnlyPublication")]
        public void getEmptyDimProfileOnlyPublication_01()
        {
            // Arrange
            UserProfileService userProfileService = new UserProfileService();
            // Act
            DimProfileOnlyPublication actualDimProfileOnlyPublication = userProfileService.GetEmptyDimProfileOnlyPublication();
            // Assert
            Assert.Equal<int>(-1, actualDimProfileOnlyPublication.DimKnownPersonId);
            Assert.Equal<int>(-1, actualDimProfileOnlyPublication.DimRegisteredDataSourceId);
            Assert.Null(actualDimProfileOnlyPublication.DimProfileOnlyPublicationId);
            Assert.Equal<int>(-1, actualDimProfileOnlyPublication.ParentTypeClassificationCode);
            Assert.Equal<int>(-1, actualDimProfileOnlyPublication.TypeClassificationCode);
            Assert.Equal<int>(-1, actualDimProfileOnlyPublication.PublicationFormatCode);
            Assert.Equal<int>(-1, actualDimProfileOnlyPublication.TargetAudienceCode);
            Assert.Null(actualDimProfileOnlyPublication.OrcidWorkType);
            Assert.Equal("", actualDimProfileOnlyPublication.PublicationName);
            Assert.Null(actualDimProfileOnlyPublication.ConferenceName);
            Assert.Null(actualDimProfileOnlyPublication.ShortDescription);
            Assert.Null(actualDimProfileOnlyPublication.PublicationYear);
            Assert.Equal("", actualDimProfileOnlyPublication.PublicationId);
            Assert.Equal("", actualDimProfileOnlyPublication.AuthorsText);
            Assert.Null(actualDimProfileOnlyPublication.NumberOfAuthors);
            Assert.Null(actualDimProfileOnlyPublication.PageNumberText);
            Assert.Null(actualDimProfileOnlyPublication.ArticleNumberText);
            Assert.Null(actualDimProfileOnlyPublication.IssueNumber);
            Assert.Null(actualDimProfileOnlyPublication.Volume);
            Assert.Null(actualDimProfileOnlyPublication.PublicationCountryCode);
            Assert.Null(actualDimProfileOnlyPublication.PublisherName);
            Assert.Null(actualDimProfileOnlyPublication.PublisherLocation);
            Assert.Null(actualDimProfileOnlyPublication.ParentPublicationName);
            Assert.Null(actualDimProfileOnlyPublication.ParentPublicationEditors);
            Assert.Null(actualDimProfileOnlyPublication.LicenseCode);
            Assert.Equal<int>(-1, actualDimProfileOnlyPublication.LanguageCode);
            Assert.Null(actualDimProfileOnlyPublication.OpenAccessCode);
            Assert.Null(actualDimProfileOnlyPublication.OriginalPublicationId);
            Assert.Null(actualDimProfileOnlyPublication.PeerReviewed);
            Assert.Null(actualDimProfileOnlyPublication.Report);
            Assert.Null(actualDimProfileOnlyPublication.ThesisTypeCode);
            Assert.Null(actualDimProfileOnlyPublication.DoiHandle);
            Assert.Equal(Constants.SourceIdentifiers.PROFILE_API, actualDimProfileOnlyPublication.SourceId);
            Assert.Equal(Constants.SourceDescriptions.PROFILE_API, actualDimProfileOnlyPublication.SourceDescription);
        }

        [Fact(DisplayName = "Get empty DimPid")]
        public void getEmptyDimPid_01()
        {
            // Arrange
            UtilityService utilityService = new UtilityService();
            UserProfileService userProfileService = new UserProfileService(utilityService: utilityService);
            // Act
            DimPid actualDimPid = userProfileService.GetEmptyDimPid();
            // Assert
            Assert.Equal("", actualDimPid.PidContent);
            Assert.Equal("", actualDimPid.PidType);
            Assert.Equal<int>(-1, actualDimPid.DimOrganizationId);
            Assert.Equal<int>(-1, actualDimPid.DimKnownPersonId);
            Assert.Equal<int>(-1, actualDimPid.DimPublicationId);
            Assert.Equal<int>(-1, actualDimPid.DimServiceId);
            Assert.Equal<int>(-1, actualDimPid.DimInfrastructureId);
            Assert.Equal<int>(-1, actualDimPid.DimPublicationChannelId);
            Assert.Equal<int>(-1, actualDimPid.DimResearchDatasetId);
            Assert.Equal<int>(-1, actualDimPid.DimFundingDecisionId);
            Assert.Equal<int>(-1, actualDimPid.DimResearchDataCatalogId);
            Assert.Equal<int>(-1, actualDimPid.DimResearchActivityId);
            Assert.Equal<int>(-1, actualDimPid.DimEventId);
            Assert.Equal<int>(-1, actualDimPid.DimProfileOnlyPublicationId);
            Assert.Equal(Constants.SourceIdentifiers.PROFILE_API, actualDimPid.SourceId);
            Assert.Equal(Constants.SourceDescriptions.PROFILE_API, actualDimPid.SourceDescription);
        }

        [Fact(DisplayName = "Check that DimName cannot be included in user profile, when registered data source name is 'virta'")]
        public void canIncludeDimNameInUserProfile_01()
        {
            // Arrange
            UserProfileService userProfileService = new ();
            DimRegisteredDataSource dimRegisteredDataSourceVirta = new() { Name = "virta" };
            DimName dimName = new () { DimRegisteredDataSource = dimRegisteredDataSourceVirta };
            // Act
            bool actualResult = userProfileService.CanIncludeDimNameInUserProfile(dimName);
            // Assert
            Assert.False(actualResult);
        }

        [Fact(DisplayName = "Check that DimName cannot be included in user profile, when registered data source name is 'metax'")]
        public void canIncludeDimNameInUserProfile_02()
        {
            // Arrange
            UserProfileService userProfileService = new();
            DimRegisteredDataSource dimRegisteredDataSourceVirta = new() { Name = "metax" };
            DimName dimName = new() { DimRegisteredDataSource = dimRegisteredDataSourceVirta };
            // Act
            bool actualResult = userProfileService.CanIncludeDimNameInUserProfile(dimName);
            // Assert
            Assert.False(actualResult);
        }

        [Fact(DisplayName = "Check that DimName cannot be included in user profile, when registered data source name is 'sftp_funding'")]
        public void canIncludeDimNameInUserProfile_03()
        {
            // Arrange
            UserProfileService userProfileService = new();
            DimRegisteredDataSource dimRegisteredDataSourceVirta = new() { Name = "sftp_funding" };
            DimName dimName = new() { DimRegisteredDataSource = dimRegisteredDataSourceVirta };
            // Act
            bool actualResult = userProfileService.CanIncludeDimNameInUserProfile(dimName);
            // Assert
            Assert.False(actualResult);
        }

        [Fact(DisplayName = "Check that DimName can be included in user profile, when registered data source is not 'virta', 'metax' or 'sftp_funding'")]
        public void canIncludeDimNameInUserProfile_04()
        {
            // Arrange
            UserProfileService userProfileService = new();
            DimRegisteredDataSource dimRegisteredDataSourceVirta = new() { Name = "yliopisto A" };
            DimName dimName = new() { DimRegisteredDataSource = dimRegisteredDataSourceVirta };
            // Act
            bool actualResult = userProfileService.CanIncludeDimNameInUserProfile(dimName);
            // Assert
            Assert.True(actualResult);
        }
    }
}