using Xunit;
using System.Collections.Generic;
using api.Services;
using api.Models;
using api.Models.Common;
using api.Models.ProfileEditor;
using api.Models.ProfileEditor.Items;
using api.Models.Ttv;

namespace api.Tests
{
    [Collection("Duplicate handler service tests.")]
    public class DuplicateHandlerServiceTests_HasSameDoiButIsDifferentPublication
    {
        [Fact(DisplayName = "Virta and ORCID publication have the same DOI, they are considered as the same publications: Virta publication type code is not A3, A4, B2, B3, D2, D3 or E1")]
        public void hasSameDoiButIsDifferentPublication_010()
        {
            DuplicateHandlerService duplicateHandlerService = new();
            DimProfileOnlyPublication dimProfileOnlyPublication = new() { DoiHandle = "doi123", PublicationName = "name123" };
            ProfileEditorPublication profileEditorPublication = new() { Doi = "doi123", PublicationName = "name123", PublicationTypeCode = "code123" };
            Assert.False(duplicateHandlerService.HasSameDoiButIsDifferentPublication(
                publicationName: dimProfileOnlyPublication.PublicationName,
                ttvPublicationName: profileEditorPublication.PublicationName,
                ttvPublicationTypeCode: profileEditorPublication.PublicationTypeCode));
        }

        [Fact(DisplayName = "Virta and ORCID publication have the same DOI, they are considered as different publications: Virta publication has type code A3, names differ.")]
        public void hasSameDoiButIsDifferentPublication_020()
        {
            DuplicateHandlerService duplicateHandlerService = new();
            DimProfileOnlyPublication dimProfileOnlyPublication = new() { DoiHandle = "doi123", PublicationName = "name123" };
            ProfileEditorPublication profileEditorPublication = new() { Doi = "doi123", PublicationName = "name456", PublicationTypeCode = "A3" };
            Assert.True(duplicateHandlerService.HasSameDoiButIsDifferentPublication(
                publicationName: dimProfileOnlyPublication.PublicationName,
                ttvPublicationName: profileEditorPublication.PublicationName,
                ttvPublicationTypeCode: profileEditorPublication.PublicationTypeCode));
        }

        [Fact(DisplayName = "Virta and ORCID publication have the same DOI, they are considered as the same publications: Virta publication has type code A3, both have the same name.")]
        public void hasSameDoiButIsDifferentPublication_030()
        {
            DuplicateHandlerService duplicateHandlerService = new();
            DimProfileOnlyPublication dimProfileOnlyPublication = new() { DoiHandle = "doi123", PublicationName = "name123" };
            ProfileEditorPublication profileEditorPublication = new() { Doi = "doi123", PublicationName = "name123", PublicationTypeCode = "A3" };
            Assert.False(duplicateHandlerService.HasSameDoiButIsDifferentPublication(
                publicationName: dimProfileOnlyPublication.PublicationName,
                ttvPublicationName: profileEditorPublication.PublicationName,
                ttvPublicationTypeCode: profileEditorPublication.PublicationTypeCode));
        }

        [Fact(DisplayName = "Virta and ORCID publication have the same DOI, they are considered as different publications: Virta publication has type code A4, names differ.")]
        public void hasSameDoiButIsDifferentPublication_040()
        {
            DuplicateHandlerService duplicateHandlerService = new();
            DimProfileOnlyPublication dimProfileOnlyPublication = new() { DoiHandle = "doi123", PublicationName = "name123" };
            ProfileEditorPublication profileEditorPublication = new() { Doi = "doi123", PublicationName = "name456", PublicationTypeCode = "A4" };
            Assert.True(duplicateHandlerService.HasSameDoiButIsDifferentPublication(
                publicationName: dimProfileOnlyPublication.PublicationName,
                ttvPublicationName: profileEditorPublication.PublicationName,
                ttvPublicationTypeCode: profileEditorPublication.PublicationTypeCode));
        }

        [Fact(DisplayName = "Virta and ORCID publication have the same DOI, they are considered as the same publications: Virta publication has type code A4, both have the same name.")]
        public void hasSameDoiButIsDifferentPublication_050()
        {
            DuplicateHandlerService duplicateHandlerService = new();
            DimProfileOnlyPublication dimProfileOnlyPublication = new() { DoiHandle = "doi123", PublicationName = "name123" };
            ProfileEditorPublication profileEditorPublication = new() { Doi = "doi123", PublicationName = "name123", PublicationTypeCode = "A4" };
            Assert.False(duplicateHandlerService.HasSameDoiButIsDifferentPublication(
                publicationName: dimProfileOnlyPublication.PublicationName,
                ttvPublicationName: profileEditorPublication.PublicationName,
                ttvPublicationTypeCode: profileEditorPublication.PublicationTypeCode));
        }

        [Fact(DisplayName = "Virta and ORCID publication have the same DOI, they are considered as different publications: Virta publication has type code B2, names differ.")]
        public void hasSameDoiButIsDifferentPublication_060()
        {
            DuplicateHandlerService duplicateHandlerService = new();
            DimProfileOnlyPublication dimProfileOnlyPublication = new() { DoiHandle = "doi123", PublicationName = "name123" };
            ProfileEditorPublication profileEditorPublication = new() { Doi = "doi123", PublicationName = "name456", PublicationTypeCode = "B2" };
            Assert.True(duplicateHandlerService.HasSameDoiButIsDifferentPublication(
                publicationName: dimProfileOnlyPublication.PublicationName,
                ttvPublicationName: profileEditorPublication.PublicationName,
                ttvPublicationTypeCode: profileEditorPublication.PublicationTypeCode));
        }

        [Fact(DisplayName = "Virta and ORCID publication have the same DOI, they are considered as the same publications: Virta publication has type code B2, both have the same name.")]
        public void hasSameDoiButIsDifferentPublication_070()
        {
            DuplicateHandlerService duplicateHandlerService = new();
            DimProfileOnlyPublication dimProfileOnlyPublication = new() { DoiHandle = "doi123", PublicationName = "name123" };
            ProfileEditorPublication profileEditorPublication = new() { Doi = "doi123", PublicationName = "name123", PublicationTypeCode = "B2" };
            Assert.False(duplicateHandlerService.HasSameDoiButIsDifferentPublication(
                publicationName: dimProfileOnlyPublication.PublicationName,
                ttvPublicationName: profileEditorPublication.PublicationName,
                ttvPublicationTypeCode: profileEditorPublication.PublicationTypeCode));
        }

        [Fact(DisplayName = "Virta and ORCID publication have the same DOI, they are considered as different publications: Virta publication has type code B3, names differ.")]
        public void hasSameDoiButIsDifferentPublication_080()
        {
            DuplicateHandlerService duplicateHandlerService = new();
            DimProfileOnlyPublication dimProfileOnlyPublication = new() { DoiHandle = "doi123", PublicationName = "name123" };
            ProfileEditorPublication profileEditorPublication = new() { Doi = "doi123", PublicationName = "name456", PublicationTypeCode = "B3" };
            Assert.True(duplicateHandlerService.HasSameDoiButIsDifferentPublication(
                publicationName: dimProfileOnlyPublication.PublicationName,
                ttvPublicationName: profileEditorPublication.PublicationName,
                ttvPublicationTypeCode: profileEditorPublication.PublicationTypeCode));
        }

        [Fact(DisplayName = "Virta and ORCID publication have the same DOI, they are considered as the same publications: Virta publication has type code B3, both have the same name.")]
        public void hasSameDoiButIsDifferentPublication_090()
        {
            DuplicateHandlerService duplicateHandlerService = new();
            DimProfileOnlyPublication dimProfileOnlyPublication = new() { DoiHandle = "doi123", PublicationName = "name123" };
            ProfileEditorPublication profileEditorPublication = new() { Doi = "doi123", PublicationName = "name123", PublicationTypeCode = "B3" };
            Assert.False(duplicateHandlerService.HasSameDoiButIsDifferentPublication(
                publicationName: dimProfileOnlyPublication.PublicationName,
                ttvPublicationName: profileEditorPublication.PublicationName,
                ttvPublicationTypeCode: profileEditorPublication.PublicationTypeCode));
        }

        [Fact(DisplayName = "Virta and ORCID publication have the same DOI, they are considered as different publications: Virta publication has type code D2, names differ.")]
        public void hasSameDoiButIsDifferentPublication_100()
        {
            DuplicateHandlerService duplicateHandlerService = new();
            DimProfileOnlyPublication dimProfileOnlyPublication = new() { DoiHandle = "doi123", PublicationName = "name123" };
            ProfileEditorPublication profileEditorPublication = new() { Doi = "doi123", PublicationName = "name456", PublicationTypeCode = "D2" };
            Assert.True(duplicateHandlerService.HasSameDoiButIsDifferentPublication(
                publicationName: dimProfileOnlyPublication.PublicationName,
                ttvPublicationName: profileEditorPublication.PublicationName,
                ttvPublicationTypeCode: profileEditorPublication.PublicationTypeCode));
        }

        [Fact(DisplayName = "Virta and ORCID publication have the same DOI, they are considered as the same publications: Virta publication has type code D2, both have the same name.")]
        public void hasSameDoiButIsDifferentPublication_110()
        {
            DuplicateHandlerService duplicateHandlerService = new();
            DimProfileOnlyPublication dimProfileOnlyPublication = new() { DoiHandle = "doi123", PublicationName = "name123" };
            ProfileEditorPublication profileEditorPublication = new() { Doi = "doi123", PublicationName = "name123", PublicationTypeCode = "D2" };
            Assert.False(duplicateHandlerService.HasSameDoiButIsDifferentPublication(
                publicationName: dimProfileOnlyPublication.PublicationName,
                ttvPublicationName: profileEditorPublication.PublicationName,
                ttvPublicationTypeCode: profileEditorPublication.PublicationTypeCode));
        }

        [Fact(DisplayName = "Virta and ORCID publication have the same DOI, they are considered as different publications: Virta publication has type code D3, names differ.")]
        public void hasSameDoiButIsDifferentPublication_120()
        {
            DuplicateHandlerService duplicateHandlerService = new();
            DimProfileOnlyPublication dimProfileOnlyPublication = new() { DoiHandle = "doi123", PublicationName = "name123" };
            ProfileEditorPublication profileEditorPublication = new() { Doi = "doi123", PublicationName = "name456", PublicationTypeCode = "D3" };
            Assert.True(duplicateHandlerService.HasSameDoiButIsDifferentPublication(
                publicationName: dimProfileOnlyPublication.PublicationName,
                ttvPublicationName: profileEditorPublication.PublicationName,
                ttvPublicationTypeCode: profileEditorPublication.PublicationTypeCode));
        }

        [Fact(DisplayName = "Virta and ORCID publication have the same DOI, they are considered as the same publications: Virta publication has type code D3, both have the same name.")]
        public void hasSameDoiButIsDifferentPublication_130()
        {
            DuplicateHandlerService duplicateHandlerService = new();
            DimProfileOnlyPublication dimProfileOnlyPublication = new() { DoiHandle = "doi123", PublicationName = "name123" };
            ProfileEditorPublication profileEditorPublication = new() { Doi = "doi123", PublicationName = "name123", PublicationTypeCode = "D3" };
            Assert.False(duplicateHandlerService.HasSameDoiButIsDifferentPublication(
                publicationName: dimProfileOnlyPublication.PublicationName,
                ttvPublicationName: profileEditorPublication.PublicationName,
                ttvPublicationTypeCode: profileEditorPublication.PublicationTypeCode));
        }

        [Fact(DisplayName = "Virta and ORCID publication have the same DOI, they are considered as different publications: Virta publication has type code E1, names differ.")]
        public void hasSameDoiButIsDifferentPublication_140()
        {
            DuplicateHandlerService duplicateHandlerService = new();
            DimProfileOnlyPublication dimProfileOnlyPublication = new() { DoiHandle = "doi123", PublicationName = "name123" };
            ProfileEditorPublication profileEditorPublication = new() { Doi = "doi123", PublicationName = "name456", PublicationTypeCode = "E1" };
            Assert.True(duplicateHandlerService.HasSameDoiButIsDifferentPublication(
                publicationName: dimProfileOnlyPublication.PublicationName,
                ttvPublicationName: profileEditorPublication.PublicationName,
                ttvPublicationTypeCode: profileEditorPublication.PublicationTypeCode));
        }

        [Fact(DisplayName = "Virta and ORCID publication have the same DOI, they are considered as the same publications: Virta publication has type code E1, both have the same name.")]
        public void hasSameDoiButIsDifferentPublication_150()
        {
            DuplicateHandlerService duplicateHandlerService = new();
            DimProfileOnlyPublication dimProfileOnlyPublication = new() { DoiHandle = "doi123", PublicationName = "name123" };
            ProfileEditorPublication profileEditorPublication = new() { Doi = "doi123", PublicationName = "name123", PublicationTypeCode = "E1" };
            Assert.False(duplicateHandlerService.HasSameDoiButIsDifferentPublication(
                publicationName: dimProfileOnlyPublication.PublicationName,
                ttvPublicationName: profileEditorPublication.PublicationName,
                ttvPublicationTypeCode: profileEditorPublication.PublicationTypeCode));
        }




        [Fact(DisplayName = "AddPublicationToProfileEditorData_HandlePublicationIdDuplicates")]
        public void addPublicationToProfileEditorData_010()
        {
            DuplicateHandlerService duplicateHandlerService = new();

            // Datasources
            ProfileEditorSource profileEditorSourceA = new()
            {
                Id = 1,
                RegisteredDataSource = "Source A",
                Organization = new Organization() { NameEn = "Organization name A" }
            };
            ProfileEditorSource profileEditorSourceB = new()
            {
                Id = 2,
                RegisteredDataSource = "Source B",
                Organization = new Organization() { NameEn = "Organization name B" }
            };

            // Create ProfileDataRaw for Virta publication 1
            ProfileDataFromSql profileDataVirta1 = new()
            {
                DimPublication_PublicationId = "publicationId123",
                DimPublication_Doi = "doi123",
                DimPublication_PublicationName = "name123",
                DimPublication_PublicationTypeCode = "A1",
                DimFieldDisplaySettings_FieldIdentifier = Constants.FieldIdentifiers.ACTIVITY_PUBLICATION
            };

            // Create ProfileDataRaw for Virta publication 2
            ProfileDataFromSql profileDataVirta2 = new()
            {
                DimPublication_PublicationId = "publicationId456",
                DimPublication_Doi = "doi456",
                DimPublication_PublicationName = "name456",
                DimPublication_PublicationTypeCode = "A2",
                DimFieldDisplaySettings_FieldIdentifier = Constants.FieldIdentifiers.ACTIVITY_PUBLICATION
            };

            // Create empty list of publications
            List<ProfileEditorPublication> publications = new();

            //
            // Add publication 1st time
            //
            List<ProfileEditorPublication> publications1 = duplicateHandlerService.AddPublicationToProfileEditorData(
                dataSource: profileEditorSourceA,
                profileData: profileDataVirta1,
                publications: publications
            );
            // Check that publication list contains one publication
            Assert.Single(publications1);
            // Check publication values
            Assert.Equal("publicationId123", publications1[0].PublicationId);
            Assert.Equal("doi123", publications1[0].Doi);
            Assert.Equal("name123", publications1[0].PublicationName);
            Assert.Single(publications1[0].DataSources);
            Assert.Equal(profileEditorSourceA.RegisteredDataSource, publications1[0].DataSources[0].RegisteredDataSource);

            //
            // Add the same publication 2nd time, the same data source
            //
            List<ProfileEditorPublication> publications2 = duplicateHandlerService.AddPublicationToProfileEditorData(
                dataSource: profileEditorSourceA,
                profileData: profileDataVirta1,
                publications: publications1
            );
            // Check that publication list contains one publication
            Assert.Single(publications2);
            // Check publication values
            Assert.Equal("publicationId123", publications2[0].PublicationId);
            Assert.Equal("doi123", publications2[0].Doi);
            Assert.Equal("name123", publications2[0].PublicationName);
            Assert.Single(publications2[0].DataSources);
            Assert.Equal(profileEditorSourceA.RegisteredDataSource, publications2[0].DataSources[0].RegisteredDataSource);

            //
            // Add the same publication 3rd time, different data source (profileEditorSourceB)
            //
            List<ProfileEditorPublication> publications3 = duplicateHandlerService.AddPublicationToProfileEditorData(
                dataSource: profileEditorSourceB,
                profileData: profileDataVirta1,
                publications: publications2
            );

            // Check that publication list contains one publication
            Assert.Single(publications3);
            // Check publication values
            Assert.Equal("publicationId123", publications3[0].PublicationId);
            Assert.Equal("doi123", publications3[0].Doi);
            Assert.Equal("name123", publications3[0].PublicationName);
            Assert.Equal(2, publications3[0].DataSources.Count);
            Assert.Equal(profileEditorSourceA.RegisteredDataSource, publications3[0].DataSources[0].RegisteredDataSource);
            Assert.Equal(profileEditorSourceB.RegisteredDataSource, publications3[0].DataSources[1].RegisteredDataSource);

            //
            // Add different publication
            //
            List<ProfileEditorPublication> publications4 = duplicateHandlerService.AddPublicationToProfileEditorData(
                dataSource: profileEditorSourceA,
                profileData: profileDataVirta2,
                publications: publications3
            );
            // Check that publication list contains two publications
            Assert.Equal(2, publications4.Count);
            // Check 1st publication values
            Assert.Equal("publicationId123", publications4[0].PublicationId);
            Assert.Equal("doi123", publications4[0].Doi);
            Assert.Equal("name123", publications4[0].PublicationName);
            Assert.Equal(2, publications4[0].DataSources.Count);
            Assert.Equal(profileEditorSourceA.RegisteredDataSource, publications4[0].DataSources[0].RegisteredDataSource);
            Assert.Equal(profileEditorSourceB.RegisteredDataSource, publications4[0].DataSources[1].RegisteredDataSource);
            // Check 2nd publication values
            Assert.Equal("publicationId456", publications4[1].PublicationId);
            Assert.Equal("doi456", publications4[1].Doi);
            Assert.Equal("name456", publications4[1].PublicationName);
            Assert.Single(publications4[1].DataSources);
            Assert.Equal(profileEditorSourceA.RegisteredDataSource, publications4[0].DataSources[0].RegisteredDataSource);
        }




        [Fact(DisplayName = "AddPublicationToProfileEditorData_HandleDoiDuplicates")]
        public void addPublicationToProfileEditorData_020()
        {
            DuplicateHandlerService duplicateHandlerService = new();

            // Datasources
            ProfileEditorSource profileEditorSourceVirta = new()
            {
                Id = 1,
                RegisteredDataSource = "Virta",
                Organization = new Organization() { }
            };
            ProfileEditorSource profileEditorSourceOrcid = new()
            {
                Id = 2,
                RegisteredDataSource = "ORCID",
                Organization = new Organization() { }
            };

            // Create ProfileDataRaw for Virta publication 1
            ProfileDataFromSql profileDataVirta1 = new()
            {
                DimPublication_PublicationId = "publicationId123",
                DimPublication_Doi = "doi123",
                DimPublication_PublicationName = "name123",
                DimPublication_PublicationTypeCode = "A4",
                DimFieldDisplaySettings_FieldIdentifier = Constants.FieldIdentifiers.ACTIVITY_PUBLICATION
            };

            // Create ProfileDataRaw for ORCID publication 1. The same DOI (in uppercase letters) and name as in Virta publication.
            ProfileDataFromSql profileDataOrcid1 = new()
            {
                DimProfileOnlyPublication_PublicationId = "publicationId456",
                DimProfileOnlyPublication_Doi = "DOI123",
                DimProfileOnlyPublication_PublicationName = "name123",
                DimFieldDisplaySettings_FieldIdentifier = Constants.FieldIdentifiers.ACTIVITY_PUBLICATION_PROFILE_ONLY
            };

            // Create ProfileDataRaw for ORCID publication 2. The same DOI as in Virta publication but different name.
            ProfileDataFromSql profileDataOrcid2 = new()
            {
                DimProfileOnlyPublication_PublicationId = "publicationId789",
                DimProfileOnlyPublication_Doi = "doi123",
                DimProfileOnlyPublication_PublicationName = "name456",
                DimFieldDisplaySettings_FieldIdentifier = Constants.FieldIdentifiers.ACTIVITY_PUBLICATION_PROFILE_ONLY
            };

            // Create empty list of publications
            List<ProfileEditorPublication> publications = new();

            // Add Virta publication
            List<ProfileEditorPublication> publications1 = duplicateHandlerService.AddPublicationToProfileEditorData(
                dataSource: profileEditorSourceVirta,
                profileData: profileDataVirta1,
                publications: publications
            );
            // Add ORCID publication with the same DOI and name
            List<ProfileEditorPublication> publications2 = duplicateHandlerService.AddPublicationToProfileEditorData(
                dataSource: profileEditorSourceOrcid,
                profileData: profileDataOrcid1,
                publications: publications1
            );
            // Check that publication list contains one publication
            Assert.Single(publications2);
            // Check that publication has two data sources
            Assert.Equal(2, publications2[0].DataSources.Count);
            Assert.Equal(profileEditorSourceVirta.RegisteredDataSource, publications2[0].DataSources[0].RegisteredDataSource);
            Assert.Equal(profileEditorSourceOrcid.RegisteredDataSource, publications2[0].DataSources[1].RegisteredDataSource);

            // Add ORCID publication with the same DOI but different name
            List<ProfileEditorPublication> publications3 = duplicateHandlerService.AddPublicationToProfileEditorData(
                dataSource: profileEditorSourceOrcid,
                profileData: profileDataOrcid2,
                publications: publications2
            );
            // Check that publication list contains two publications
            Assert.Equal(2, publications3.Count);
        }

        [Fact(DisplayName = "AddPublicationToProfileEditorData_HandleDoiNull")]
        public void addPublicationToProfileEditorData_021()
        {
            DuplicateHandlerService duplicateHandlerService = new();

            // Datasources
            ProfileEditorSource profileEditorSourceVirta = new()
            {
                Id = 1,
                RegisteredDataSource = "Virta",
                Organization = new Organization() { }
            };
            ProfileEditorSource profileEditorSourceOrcid = new()
            {
                Id = 2,
                RegisteredDataSource = "ORCID",
                Organization = new Organization() { }
            };

            // Create ProfileDataRaw for Virta publication 1
            ProfileDataFromSql profileDataVirta1 = new()
            {
                DimPublication_PublicationId = "publicationId123",
                DimPublication_Doi = null, // DOI missing
                DimPublication_PublicationName = "name123",
                DimPublication_PublicationTypeCode = "A4",
                DimFieldDisplaySettings_FieldIdentifier = Constants.FieldIdentifiers.ACTIVITY_PUBLICATION
            };

            // Create ProfileDataRaw for ORCID publication 1. The same DOI (in uppercase letters) and name as in Virta publication.
            ProfileDataFromSql profileDataOrcid1 = new()
            {
                DimProfileOnlyPublication_PublicationId = "publicationId456",
                DimProfileOnlyPublication_Doi = "doi123",
                DimProfileOnlyPublication_PublicationName = "name123",
                DimFieldDisplaySettings_FieldIdentifier = Constants.FieldIdentifiers.ACTIVITY_PUBLICATION_PROFILE_ONLY
            };

            // Create empty list of publications
            List<ProfileEditorPublication> publications = new();

            // Add Virta publication
            List<ProfileEditorPublication> publications1 = duplicateHandlerService.AddPublicationToProfileEditorData(
                dataSource: profileEditorSourceVirta,
                profileData: profileDataVirta1,
                publications: publications
            );
            // Add ORCID publication
            List<ProfileEditorPublication> publications2 = duplicateHandlerService.AddPublicationToProfileEditorData(
                dataSource: profileEditorSourceOrcid,
                profileData: profileDataOrcid1,
                publications: publications1
            );

            // Check that publication list contains two publications
            Assert.Equal(2, publications2.Count);
        }

        [Fact(DisplayName = "TestPublicationYearHandling")]
        public void testPublicationYearHandling()
        {
            DuplicateHandlerService duplicateHandlerService = new();

            // Valid publication year
            Assert.Equal<int?>(2022, duplicateHandlerService.HandlePublicationYear(2022));
            // Publication year 0
            Assert.Null(duplicateHandlerService.HandlePublicationYear(0));
            // Publication year negative
            Assert.Null(duplicateHandlerService.HandlePublicationYear(-1));
            // Publication year null
            Assert.Null(duplicateHandlerService.HandlePublicationYear(null));
        }
    }
}