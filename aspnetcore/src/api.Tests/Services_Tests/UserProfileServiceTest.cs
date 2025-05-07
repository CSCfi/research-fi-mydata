using Xunit;
using api.Services;
using api.Models.Common;
using api.Models.Ttv;
using api.Models.ProfileEditor.Items;
using System.Collections.Generic;
using System;
using api.Models.ProfileEditor;
using api.Models.Log;

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
            Assert.Equal<long>(-1, actualFfv.DimNameId);
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
            Assert.Equal<int>(-1, actualFfv.DimProfileOnlyDatasetId);
            Assert.Equal<int>(-1, actualFfv.DimProfileOnlyFundingDecisionId);
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

        [Fact(DisplayName = "Get empty DimProfileOnlyDataset")]
        public void getEmptyDimProfileOnlyDataset_01()
        {
            // Arrange
            UserProfileService userProfileService = new UserProfileService();
            // Act
            DimProfileOnlyDataset actualDimProfileOnlyDataset = userProfileService.GetEmptyDimProfileOnlyDataset();
            // Assert
            Assert.Null(actualDimProfileOnlyDataset.DimReferencedataIdAvailability);
            Assert.Equal("", actualDimProfileOnlyDataset.OrcidWorkType);
            Assert.Equal("", actualDimProfileOnlyDataset.LocalIdentifier);
            Assert.Equal("", actualDimProfileOnlyDataset.NameFi);
            Assert.Equal("", actualDimProfileOnlyDataset.NameSv);
            Assert.Equal("", actualDimProfileOnlyDataset.NameEn);
            Assert.Equal("", actualDimProfileOnlyDataset.NameUnd);
            Assert.Equal("", actualDimProfileOnlyDataset.DescriptionFi);
            Assert.Equal("", actualDimProfileOnlyDataset.DescriptionSv);
            Assert.Equal("", actualDimProfileOnlyDataset.DescriptionEn);
            Assert.Equal("", actualDimProfileOnlyDataset.DescriptionUnd);
            Assert.Equal("", actualDimProfileOnlyDataset.VersionInfo);
            Assert.Null(actualDimProfileOnlyDataset.DatasetCreated);
            Assert.Equal(Constants.SourceIdentifiers.PROFILE_API, actualDimProfileOnlyDataset.SourceId);
            Assert.Equal(Constants.SourceDescriptions.PROFILE_API, actualDimProfileOnlyDataset.SourceDescription);
        }


        [Fact(DisplayName = "Get empty DimProfileOnlyFundingDecision")]
        public void getEmptyDimProfileOnlyFundingDecision_01()
        {
            // Arrange
            UserProfileService userProfileService = new UserProfileService();
            // Act
            DimProfileOnlyFundingDecision actualDimProfileOnlyFundingDecision = userProfileService.GetEmptyDimProfileOnlyFundingDecision();
            // Assert
            Assert.Equal<int>(-1, actualDimProfileOnlyFundingDecision.DimDateIdApproval);
            Assert.Equal<int>(-1, actualDimProfileOnlyFundingDecision.DimDateIdStart);
            Assert.Equal<int>(-1, actualDimProfileOnlyFundingDecision.DimDateIdEnd);
            Assert.Equal<int>(-1, actualDimProfileOnlyFundingDecision.DimCallProgrammeId);
            Assert.Equal<int>(-1, actualDimProfileOnlyFundingDecision.DimTypeOfFundingId);
            Assert.Null(actualDimProfileOnlyFundingDecision.DimOrganizationIdFunder);
            Assert.Equal("", actualDimProfileOnlyFundingDecision.OrcidWorkType);
            Assert.Equal("", actualDimProfileOnlyFundingDecision.FunderProjectNumber);
            Assert.Equal("", actualDimProfileOnlyFundingDecision.Acronym);
            Assert.Equal("", actualDimProfileOnlyFundingDecision.NameFi);
            Assert.Equal("", actualDimProfileOnlyFundingDecision.NameSv);
            Assert.Equal("", actualDimProfileOnlyFundingDecision.NameEn);
            Assert.Equal("", actualDimProfileOnlyFundingDecision.NameUnd);
            Assert.Equal("", actualDimProfileOnlyFundingDecision.DescriptionFi);
            Assert.Equal("", actualDimProfileOnlyFundingDecision.DescriptionEn);
            Assert.Equal("", actualDimProfileOnlyFundingDecision.DescriptionSv);
            Assert.Equal<decimal>(-1, actualDimProfileOnlyFundingDecision.AmountInEur);
            Assert.Null(actualDimProfileOnlyFundingDecision.AmountInFundingDecisionCurrency);
            Assert.Equal("", actualDimProfileOnlyFundingDecision.FundingDecisionCurrencyAbbreviation);
            Assert.Equal(Constants.SourceIdentifiers.PROFILE_API, actualDimProfileOnlyFundingDecision.SourceId);
            Assert.Equal(Constants.SourceDescriptions.PROFILE_API, actualDimProfileOnlyFundingDecision.SourceDescription);
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
            Assert.Equal<int>(-1, actualDimProfileOnlyPublication.PublicationCountryCode);
            Assert.Null(actualDimProfileOnlyPublication.PublisherName);
            Assert.Null(actualDimProfileOnlyPublication.PublisherLocation);
            Assert.Null(actualDimProfileOnlyPublication.ParentPublicationName);
            Assert.Null(actualDimProfileOnlyPublication.ParentPublicationEditors);
            Assert.Equal<int>(-1, actualDimProfileOnlyPublication.LicenseCode);
            Assert.Equal<int>(-1, actualDimProfileOnlyPublication.LanguageCode);
            Assert.Null(actualDimProfileOnlyPublication.OpenAccessCode);
            Assert.Null(actualDimProfileOnlyPublication.OriginalPublicationId);
            Assert.Null(actualDimProfileOnlyPublication.PeerReviewed);
            Assert.Null(actualDimProfileOnlyPublication.Report);
            Assert.Equal<int>(-1, actualDimProfileOnlyPublication.ThesisTypeCode);
            Assert.Null(actualDimProfileOnlyPublication.DoiHandle);
            Assert.Equal(Constants.SourceIdentifiers.PROFILE_API, actualDimProfileOnlyPublication.SourceId);
            Assert.Equal(Constants.SourceDescriptions.PROFILE_API, actualDimProfileOnlyPublication.SourceDescription);
        }

        [Fact(DisplayName = "Get empty DimProfileOnlyResearchActivity")]
        public void getEmptyDimProfileOnlyResearchActivity_01()
        {
            // Arrange
            UserProfileService userProfileService = new UserProfileService();
            // Act
            DimProfileOnlyResearchActivity actualDimProfileOnlyResearchActivity = userProfileService.GetEmptyDimProfileOnlyResearchActivity();
            // Assert
            Assert.Equal<int>(-1, actualDimProfileOnlyResearchActivity.DimDateIdStart);
            Assert.Equal<int>(-1, actualDimProfileOnlyResearchActivity.DimDateIdEnd);
            Assert.Null(actualDimProfileOnlyResearchActivity.DimGeoIdCountry);
            Assert.Equal<int>(-1, actualDimProfileOnlyResearchActivity.DimOrganizationId);
            Assert.Equal<int>(-1, actualDimProfileOnlyResearchActivity.DimEventId);
            Assert.Equal("", actualDimProfileOnlyResearchActivity.LocalIdentifier);
            Assert.Equal("", actualDimProfileOnlyResearchActivity.OrcidWorkType);
            Assert.Equal("", actualDimProfileOnlyResearchActivity.NameFi);
            Assert.Equal("", actualDimProfileOnlyResearchActivity.NameSv);
            Assert.Equal("", actualDimProfileOnlyResearchActivity.NameEn);
            Assert.Equal("", actualDimProfileOnlyResearchActivity.NameUnd);
            Assert.Equal("", actualDimProfileOnlyResearchActivity.DescriptionFi);
            Assert.Equal("", actualDimProfileOnlyResearchActivity.DescriptionEn);
            Assert.Equal("", actualDimProfileOnlyResearchActivity.DescriptionSv);
            Assert.Equal("", actualDimProfileOnlyResearchActivity.IndentifierlessTargetOrg);
            Assert.Equal<int>(-1, actualDimProfileOnlyResearchActivity.DimRegisteredDataSourceId);
            Assert.Equal(Constants.SourceIdentifiers.PROFILE_API, actualDimProfileOnlyResearchActivity.SourceId);
            Assert.Equal(Constants.SourceDescriptions.PROFILE_API, actualDimProfileOnlyResearchActivity.SourceDescription);
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
            Assert.Equal<int>(-1, (int)actualDimPid.DimResearchProjectId);
            Assert.Equal<int>(-1, (int)actualDimPid.DimResearchCommunityId);
            Assert.Equal<int>(-1, actualDimPid.DimResearchDataCatalogId);
            Assert.Equal<int>(-1, actualDimPid.DimResearchActivityId);
            Assert.Equal<int>(-1, actualDimPid.DimEventId);
            Assert.Equal(-1, actualDimPid.DimProfileOnlyDatasetId);
            Assert.Equal(-1, actualDimPid.DimProfileOnlyFundingDecisionId);
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
            DimName dimName = new () {
                Id = 1,
                DimRegisteredDataSource = dimRegisteredDataSourceVirta
            };
            List<long> existingIds = new() { };
            // Act
            bool actualResult = userProfileService.CanIncludeDimNameInUserProfile(existingIds, dimName);
            // Assert
            Assert.False(actualResult);
        }

        [Fact(DisplayName = "Check that DimName cannot be included in user profile, when registered data source name is 'metax'")]
        public void canIncludeDimNameInUserProfile_02()
        {
            // Arrange
            UserProfileService userProfileService = new();
            DimRegisteredDataSource dimRegisteredDataSourceVirta = new() { Name = "metax" };
            DimName dimName = new() {
                Id = 2,
                DimRegisteredDataSource = dimRegisteredDataSourceVirta
            };
            List<long> existingIds = new() { };
            // Act
            bool actualResult = userProfileService.CanIncludeDimNameInUserProfile(existingIds, dimName);
            // Assert
            Assert.False(actualResult);
        }

        [Fact(DisplayName = "Check that DimName cannot be included in user profile, when registered data source name is 'sftp_funding'")]
        public void canIncludeDimNameInUserProfile_03()
        {
            // Arrange
            UserProfileService userProfileService = new();
            DimRegisteredDataSource dimRegisteredDataSourceVirta = new() { Name = "sftp_funding" };
            DimName dimName = new() {
                Id = 3,
                DimRegisteredDataSource = dimRegisteredDataSourceVirta
            };
            List<long> existingIds = new() { };
            // Act
            bool actualResult = userProfileService.CanIncludeDimNameInUserProfile(existingIds, dimName);
            // Assert
            Assert.False(actualResult);
        }

        [Fact(DisplayName = "Check that DimName can be included in user profile, when registered data source is not 'virta', 'metax' or 'sftp_funding'")]
        public void canIncludeDimNameInUserProfile_04()
        {
            // Arrange
            UserProfileService userProfileService = new();
            DimRegisteredDataSource dimRegisteredDataSourceVirta = new() { Name = "yliopisto A" };
            DimName dimName = new()
            {
                Id = 4,
                DimRegisteredDataSource = dimRegisteredDataSourceVirta
            };
            List<long> existingIds = new() { };
            // Act
            bool actualResult = userProfileService.CanIncludeDimNameInUserProfile(existingIds, dimName);
            // Assert
            Assert.True(actualResult);
        }

        [Fact(DisplayName = "Check that DimName cannot be included in user profile, when DimName.Id is in the list of already included IDs")]
        public void canIncludeDimNameInUserProfile_05()
        {
            // Arrange
            UserProfileService userProfileService = new();
            DimRegisteredDataSource dimRegisteredDataSourceVirta = new() { Name = "yliopisto B" };
            DimName dimName = new()
            {
                Id = 5,
                DimRegisteredDataSource = dimRegisteredDataSourceVirta
            };
            List<long> existingIds = new() { 2,3,4,5,6 };
            // Act
            bool actualResult = userProfileService.CanIncludeDimNameInUserProfile(existingIds, dimName);
            // Assert
            Assert.False(actualResult);
        }

        [Fact(DisplayName = "Check that DimName can be included in user profile, when DimName.Id is not in the list of already included IDs")]
        public void canIncludeDimNameInUserProfile_06()
        {
            // Arrange
            UserProfileService userProfileService = new();
            DimRegisteredDataSource dimRegisteredDataSourceVirta = new() { Name = "yliopisto C" };
            DimName dimName = new()
            {
                Id = 101010,
                DimRegisteredDataSource = dimRegisteredDataSourceVirta
            };
            List<long> existingIds = new() { 3, 4, 5, 6, 7 };
            // Act
            bool actualResult = userProfileService.CanIncludeDimNameInUserProfile(existingIds, dimName);
            // Assert
            Assert.True(actualResult);
        }

        [Fact(DisplayName = "Check that property TemporaryUniqueId is correctly set in ProfileEditorItemMeta constructor")]
        public void ProfileEditorItemMeta_01()
        {
            // Act
            ProfileEditorItemMeta actualResult = new ProfileEditorItemMeta(
                id: 123456789,
                type: 9999,
                show: false,
                primaryValue: true
            );
            // Assert
            Assert.Equal<ulong>(9999123456789, actualResult.TemporaryUniqueId);
        }

        [Fact(DisplayName = "Memory cache key - user profile")]
        public void MemoryCacheKey_UserProfile()
        {
            // Arrange
            UserProfileService userProfileService = new();
            string orcidId = "1234-5678-9098-7654";
            string expectedMemoryCacheKey = $"userprofile-{orcidId}";
            // Act
            string actualMemoryCacheKey = userProfileService.GetCMemoryCacheKey_UserProfile(orcidId: orcidId);
            // Assert
            Assert.Equal(expectedMemoryCacheKey, actualMemoryCacheKey);
        }

        [Fact(DisplayName = "Memory cache key - profile settings")]
        public void MemoryCacheKey_ProfileSettings()
        {
            // Arrange
            UserProfileService userProfileService = new();
            string orcidId = "1234-5678-9098-7654";
            string expectedMemoryCacheKey = $"profilesettings-{orcidId}";
            // Act
            string actualMemoryCacheKey = userProfileService.GetCMemoryCacheKey_ProfileSettings(orcidId: orcidId);
            // Assert
            Assert.Equal(expectedMemoryCacheKey, actualMemoryCacheKey);
        }

        [Fact(DisplayName = "Memory cache key - user choices")]
        public void MemoryCacheKey_UserChoices()
        {
            // Arrange
            UserProfileService userProfileService = new();
            string orcidId = "1234-5678-9098-7654";
            string expectedMemoryCacheKey = $"userchoices-{orcidId}";
            // Act
            string actualMemoryCacheKey = userProfileService.GetCMemoryCacheKey_UserChoices(orcidId: orcidId);
            // Assert
            Assert.Equal(expectedMemoryCacheKey, actualMemoryCacheKey);
        }

        [Fact(DisplayName = "Memory cache key - given permissions")]
        public void MemoryCacheKey_GivenPermissions()
        {
            // Arrange
            UserProfileService userProfileService = new();
            string orcidId = "1234-5678-9098-7654";
            string expectedMemoryCacheKey = $"given_permissions-{orcidId}";
            // Act
            string actualMemoryCacheKey = userProfileService.GetCMemoryCacheKey_GivenPermissions(orcidId: orcidId);
            // Assert
            Assert.Equal(expectedMemoryCacheKey, actualMemoryCacheKey);
        }

        [Fact(DisplayName = "Research activity deduplication")]
        public void ResearchActivityDeduplication()
        {
            UserProfileService userProfileService = new();

            // Duplicate
            Assert.True(
                userProfileService.IsResearchActivityDuplicate(
                    aYear: 2006,
                    bYear: 2006,
                    aNameFi: "test name FI",
                    bNameFi: "test name FI",
                    aNameEn: "test name EN",
                    bNameEn: "test name EN",
                    aNameSv: "test name SV",
                    bNameSv: "test name SV"
                ),
                "Research activities are duplicates"
            );
            // Year differs
            Assert.False(
                userProfileService.IsResearchActivityDuplicate(
                    aYear: 2006,
                    bYear: 2007,
                    aNameFi: "test name FI",
                    bNameFi: "test name FI",
                    aNameEn: "test name EN",
                    bNameEn: "test name EN",
                    aNameSv: "test name SV",
                    bNameSv: "test name SV"
                ),
                "Research activities are not duplicates, year differs"
            );
            // Fi name differs
            Assert.False(
                userProfileService.IsResearchActivityDuplicate(
                    aYear: 2006,
                    bYear: 2006,
                    aNameFi: "test name FI",
                    bNameFi: "test name FIx",
                    aNameEn: "test name EN",
                    bNameEn: "test name EN",
                    aNameSv: "test name SV",
                    bNameSv: "test name SV"
                ),
                "Research activities are not duplicates, Fi name differs"
            );
            // En name differs
            Assert.False(
                userProfileService.IsResearchActivityDuplicate(
                    aYear: 2006,
                    bYear: 2006,
                    aNameFi: "test name FI",
                    bNameFi: "test name FI",
                    aNameEn: "test name EN",
                    bNameEn: "test name ENx",
                    aNameSv: "test name SV",
                    bNameSv: "test name SV"
                ),
                "Research activities are not duplicates, En name differs"
            );
            // Sv name differs
            Assert.False(
                userProfileService.IsResearchActivityDuplicate(
                    aYear: 2006,
                    bYear: 2006,
                    aNameFi: "test name FI",
                    bNameFi: "test name FI",
                    aNameEn: "test name EN",
                    bNameEn: "test name EN",
                    aNameSv: "test name SV",
                    bNameSv: "test name SVx"
                ),
                "Research activities are not duplicates, Sv name differs"
            );
        }

        [Fact(DisplayName = "Get profile editor source")]
        public void GetProfileEditorSource()
        {
            // Arrange
            LanguageService languageService = new LanguageService();
            UserProfileService userProfileService = new(languageService: languageService);
            ProfileDataFromSql p = new ()
            {
                DimRegisteredDataSource_Id = 1234,
                DimRegisteredDataSource_Name = "TestRegisteredDataSourceName",
                DimRegisteredDataSource_DimOrganization_NameFi = "TestOrganizationNameFi",
                DimRegisteredDataSource_DimOrganization_NameEn = "TestOrganizationNameEn",
                DimRegisteredDataSource_DimOrganization_NameSv = "TestOrganizationNameSv",
                DimRegisteredDataSource_DimOrganization_DimSector_SectorId = "TestSectorId"
            };
            ProfileEditorSource expectedProfileEditorSource = new ()
            {
                Id = 1234,
                RegisteredDataSource = "TestRegisteredDataSourceName",
                Organization = new Organization()
                {
                    NameFi = "TestOrganizationNameFi",
                    NameEn = "TestOrganizationNameEn",
                    NameSv = "TestOrganizationNameSv",
                    SectorId = "TestSectorId"
                }
            };
            // Act
            ProfileEditorSource actualProfileEditorSource = userProfileService.GetProfileEditorSource(p);
            // Assert
            Assert.Equal(expectedProfileEditorSource.Id, actualProfileEditorSource.Id);
            Assert.Equal(expectedProfileEditorSource.RegisteredDataSource, actualProfileEditorSource.RegisteredDataSource);
            Assert.Equal(expectedProfileEditorSource.Organization.NameFi, actualProfileEditorSource.Organization.NameFi);
            Assert.Equal(expectedProfileEditorSource.Organization.NameEn, actualProfileEditorSource.Organization.NameEn);
            Assert.Equal(expectedProfileEditorSource.Organization.NameSv, actualProfileEditorSource.Organization.NameSv);
            Assert.Equal(expectedProfileEditorSource.Organization.SectorId, actualProfileEditorSource.Organization.SectorId);
        }

        [Fact(DisplayName = "ProfileSettings from DimUserProfile")]
        public void GetProfileSettings()
        {
            // Arrange
            UserProfileService userProfileService = new();
            DimUserProfile dimUserProfile = new()
            {
                Hidden = true,
                PublishNewOrcidData = true
            };
            ProfileSettings expectedProfileSettings = new ProfileSettings() {
                Hidden = true,
                PublishNewData = true
            }; 
            // Act
            ProfileSettings actualProfileSettings = userProfileService.GetProfileSettings(dimUserProfile);
            // Assert
            Assert.Equal(expectedProfileSettings.Hidden, actualProfileSettings.Hidden);
            Assert.Equal(expectedProfileSettings.PublishNewData, actualProfileSettings.PublishNewData);
        }

        [Fact(DisplayName = "ProfileSettings from DimUserProfile")]
        public void GetFullNameFromLastNameAndFistNames_01()
        {
            // Arrange
            UserProfileService userProfileService = new();
            string expectedFullname = "Smith John";
            // Act
            string actualFullname = userProfileService.GetFullname("Smith", "John");
            // Assert
            Assert.Equal(expectedFullname, actualFullname);
        }

        [Fact(DisplayName = "ProfileSettings from DimUserProfile - last name is empty")]
        public void GetFullNameFromLastNameAndFistNames_02()
        {
            // Arrange
            UserProfileService userProfileService = new();
            string expectedFullname = "John";
            // Act
            string actualFullname = userProfileService.GetFullname("", "John");
            // Assert
            Assert.Equal(expectedFullname, actualFullname);
        }

        [Fact(DisplayName = "ProfileSettings from DimUserProfile - fist name is empty")]
        public void GetFullNameFromLastNameAndFistNames_03()
        {
            // Arrange
            UserProfileService userProfileService = new();
            string expectedFullname = "Smith";
            // Act
            string actualFullname = userProfileService.GetFullname("Smith", "");
            // Assert
            Assert.Equal(expectedFullname, actualFullname);
        }

        [Fact(DisplayName = "ProfileSettings from DimUserProfile - trim whitespaces")]
        public void GetFullNameFromLastNameAndFistNames_04()
        {
            // Arrange
            UserProfileService userProfileService = new();
            string expectedFullname = "Smith John";
            // Act
            string actualFullname = userProfileService.GetFullname("  Smith  ", "  John  ");
            // Assert
            Assert.Equal(expectedFullname, actualFullname);
        }

        [Fact(DisplayName = "SetFactFieldValuesShow - return false when DimUserProfile.PublishNewOrcidData is false")]
        public void SetFactFieldValuesShow_01()
        {
            // Arrange
            UtilityService utilityService = new UtilityService();
            UserProfileService userProfileService = new UserProfileService(utilityService: utilityService);
            LogUserIdentification logUserIdentification = new LogUserIdentification(keycloakId: "testKeycloakId", orcid: "testOrcidId", ip: "123.456.789.1");
            DimUserProfile dimUserProfile = new DimUserProfile() {
                PublishNewOrcidData = false
            };
            // Act
            bool actualShow = userProfileService.SetFactFieldValuesShow(dimUserProfile, Constants.FieldIdentifiers.ACTIVITY_PUBLICATION, logUserIdentification);
            // Assert
            Assert.False(actualShow);
        }

        [Fact(DisplayName = "SetFactFieldValuesShow - return false when DimUserProfile is null")]
        public void SetFactFieldValuesShow_02()
        {
            // Arrange
            UtilityService utilityService = new UtilityService();
            UserProfileService userProfileService = new UserProfileService(utilityService: utilityService);
            LogUserIdentification logUserIdentification = new LogUserIdentification(keycloakId: "testKeycloakId", orcid: "testOrcidId", ip: "123.456.789.1");
            // Act
            bool actualShow = userProfileService.SetFactFieldValuesShow(null, Constants.FieldIdentifiers.ACTIVITY_PUBLICATION, logUserIdentification);
            // Assert
            Assert.False(actualShow);
        }

        [Fact(DisplayName = "SetFactFieldValuesShow - return false when fieldIdentifier is < 0")]
        public void SetFactFieldValuesShow_03()
        {
            // Arrange
            UtilityService utilityService = new UtilityService();
            UserProfileService userProfileService = new UserProfileService(utilityService: utilityService);
            LogUserIdentification logUserIdentification = new LogUserIdentification(keycloakId: "testKeycloakId", orcid: "testOrcidId", ip: "123.456.789.1");
            DimUserProfile dimUserProfile = new DimUserProfile() {
                PublishNewOrcidData = true
            };
            // Act
            bool actualShow = userProfileService.SetFactFieldValuesShow(dimUserProfile, -1, logUserIdentification);
            // Assert
            Assert.False(actualShow);
        }

        [Fact(DisplayName = "SetFactFieldValuesShow - return false when DimUserProfile.PublishNewOrcidData is true but field identifier is PERSON_NAME")]
        public void SetFactFieldValuesShow_04()
        {
            // Arrange
            UtilityService utilityService = new UtilityService();
            UserProfileService userProfileService = new UserProfileService(utilityService: utilityService);
            LogUserIdentification logUserIdentification = new LogUserIdentification(keycloakId: "testKeycloakId", orcid: "testOrcidId", ip: "123.456.789.1");
            DimUserProfile dimUserProfile = new DimUserProfile() {
                PublishNewOrcidData = true
            };
            // Act
            bool actualShow = userProfileService.SetFactFieldValuesShow(dimUserProfile, Constants.FieldIdentifiers.PERSON_NAME, logUserIdentification);
            // Assert
            Assert.False(actualShow);
        }

        [Fact(DisplayName = "SetFactFieldValuesShow - return false when DimUserProfile.PublishNewOrcidData is true but field identifier is PERSON_TELEPHONE_NUMBER")]
        public void SetFactFieldValuesShow_05()
        {
            // Arrange
            UtilityService utilityService = new UtilityService();
            UserProfileService userProfileService = new UserProfileService(utilityService: utilityService);
            LogUserIdentification logUserIdentification = new LogUserIdentification(keycloakId: "testKeycloakId", orcid: "testOrcidId", ip: "123.456.789.1");
            DimUserProfile dimUserProfile = new DimUserProfile() {
                PublishNewOrcidData = true
            };
            // Act
            bool actualShow = userProfileService.SetFactFieldValuesShow(dimUserProfile, Constants.FieldIdentifiers.PERSON_TELEPHONE_NUMBER, logUserIdentification);
            // Assert
            Assert.False(actualShow);
        }

        [Fact(DisplayName = "SetFactFieldValuesShow - return false when DimUserProfile.PublishNewOrcidData is true but field identifier is PERSON_KEYWORD")]
        public void SetFactFieldValuesShow_06()
        {
            // Arrange
            UtilityService utilityService = new UtilityService();
            UserProfileService userProfileService = new UserProfileService(utilityService: utilityService);
            LogUserIdentification logUserIdentification = new LogUserIdentification(keycloakId: "testKeycloakId", orcid: "testOrcidId", ip: "123.456.789.1");
            DimUserProfile dimUserProfile = new DimUserProfile() {
                PublishNewOrcidData = true
            };
            // Act
            bool actualShow = userProfileService.SetFactFieldValuesShow(dimUserProfile, Constants.FieldIdentifiers.PERSON_KEYWORD, logUserIdentification);
            // Assert
            Assert.False(actualShow);
        }

        [Fact(DisplayName = "SetFactFieldValuesShow - return false when DimUserProfile.PublishNewOrcidData is true but field identifier is PERSON_RESEARCHER_DESCRIPTION")]
        public void SetFactFieldValuesShow_07()
        {
            // Arrange
            UtilityService utilityService = new UtilityService();
            UserProfileService userProfileService = new UserProfileService(utilityService: utilityService);
            LogUserIdentification logUserIdentification = new LogUserIdentification(keycloakId: "testKeycloakId", orcid: "testOrcidId", ip: "123.456.789.1");
            DimUserProfile dimUserProfile = new DimUserProfile() {
                PublishNewOrcidData = true
            };
            // Act
            bool actualShow = userProfileService.SetFactFieldValuesShow(dimUserProfile, Constants.FieldIdentifiers.PERSON_RESEARCHER_DESCRIPTION, logUserIdentification);
            // Assert
            Assert.False(actualShow);
        }

        [Fact(DisplayName = "SetFactFieldValuesShow - return true when DimUserProfile.PublishNewOrcidData is true and field identifier is ACTIVITY_PUBLICATION")]
        public void SetFactFieldValuesShow_08()
        {
            // Arrange
            UtilityService utilityService = new UtilityService();
            UserProfileService userProfileService = new UserProfileService(utilityService: utilityService);
            LogUserIdentification logUserIdentification = new LogUserIdentification(keycloakId: "testKeycloakId", orcid: "testOrcidId", ip: "123.456.789.1");
            DimUserProfile dimUserProfile = new DimUserProfile() {
                PublishNewOrcidData = true
            };
            // Act
            bool actualShow = userProfileService.SetFactFieldValuesShow(dimUserProfile, Constants.FieldIdentifiers.ACTIVITY_PUBLICATION, logUserIdentification);
            // Assert
            Assert.True(actualShow);
        }
    }
}