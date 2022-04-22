using Xunit;
using System.Collections.Generic;
using api.Services;
using api.Models.Common;
using api.Models.ProfileEditor;
using api.Models.Ttv;

namespace api.Tests
{
    [Collection("Duplicate handler service tests.")]
    public class DuplicateHandlerServiceTests_HasSameDoiButIsDifferentPublication
    {
        [Fact(DisplayName = "Virta and ORCID publication have the same DOI, they are considered as the same publications: Virta publication type code is not A3, A4, B2, B3, D2, D3 or E1")]
        public void hasSameDoiButIsDifferentPublication_010()
        {
            var duplicateHandlerService = new DuplicateHandlerService();
            var dimOrcidPublication = new DimOrcidPublication() { DoiHandle = "doi123", PublicationName = "name123" };
            var profileEditorPublicationExperimental = new ProfileEditorPublicationExperimental() { Doi = "doi123", PublicationName = "name123", TypeCode = "code123" };
            Assert.False(duplicateHandlerService.HasSameDoiButIsDifferentPublication(dimOrcidPublication, profileEditorPublicationExperimental));
        }

        [Fact(DisplayName = "Virta and ORCID publication have the same DOI, they are considered as different publications: Virta publication has type code A3, names differ.")]
        public void hasSameDoiButIsDifferentPublication_020()
        {
            var duplicateHandlerService = new DuplicateHandlerService();
            var dimOrcidPublication = new DimOrcidPublication() { DoiHandle = "doi123", PublicationName = "name123" };
            var profileEditorPublicationExperimental = new ProfileEditorPublicationExperimental() { Doi = "doi123", PublicationName = "name456", TypeCode = "A3" };
            Assert.True(duplicateHandlerService.HasSameDoiButIsDifferentPublication(dimOrcidPublication, profileEditorPublicationExperimental));
        }

        [Fact(DisplayName = "Virta and ORCID publication have the same DOI, they are considered as the same publications: Virta publication has type code A3, both have the same name.")]
        public void hasSameDoiButIsDifferentPublication_030()
        {
            var duplicateHandlerService = new DuplicateHandlerService();
            var dimOrcidPublication = new DimOrcidPublication() { DoiHandle = "doi123", PublicationName = "name123" };
            var profileEditorPublicationExperimental = new ProfileEditorPublicationExperimental() { Doi = "doi123", PublicationName = "name123", TypeCode = "A3" };
            Assert.False(duplicateHandlerService.HasSameDoiButIsDifferentPublication(dimOrcidPublication, profileEditorPublicationExperimental));
        }

        [Fact(DisplayName = "Virta and ORCID publication have the same DOI, they are considered as different publications: Virta publication has type code A4, names differ.")]
        public void hasSameDoiButIsDifferentPublication_040()
        {
            var duplicateHandlerService = new DuplicateHandlerService();
            var dimOrcidPublication = new DimOrcidPublication() { DoiHandle = "doi123", PublicationName = "name123" };
            var profileEditorPublicationExperimental = new ProfileEditorPublicationExperimental() { Doi = "doi123", PublicationName = "name456", TypeCode = "A4" };
            Assert.True(duplicateHandlerService.HasSameDoiButIsDifferentPublication(dimOrcidPublication, profileEditorPublicationExperimental));
        }

        [Fact(DisplayName = "Virta and ORCID publication have the same DOI, they are considered as the same publications: Virta publication has type code A4, both have the same name.")]
        public void hasSameDoiButIsDifferentPublication_050()
        {
            var duplicateHandlerService = new DuplicateHandlerService();
            var dimOrcidPublication = new DimOrcidPublication() { DoiHandle = "doi123", PublicationName = "name123" };
            var profileEditorPublicationExperimental = new ProfileEditorPublicationExperimental() { Doi = "doi123", PublicationName = "name123", TypeCode = "A4" };
            Assert.False(duplicateHandlerService.HasSameDoiButIsDifferentPublication(dimOrcidPublication, profileEditorPublicationExperimental));
        }

        [Fact(DisplayName = "Virta and ORCID publication have the same DOI, they are considered as different publications: Virta publication has type code B2, names differ.")]
        public void hasSameDoiButIsDifferentPublication_060()
        {
            var duplicateHandlerService = new DuplicateHandlerService();
            var dimOrcidPublication = new DimOrcidPublication() { DoiHandle = "doi123", PublicationName = "name123" };
            var profileEditorPublicationExperimental = new ProfileEditorPublicationExperimental() { Doi = "doi123", PublicationName = "name456", TypeCode = "B2" };
            Assert.True(duplicateHandlerService.HasSameDoiButIsDifferentPublication(dimOrcidPublication, profileEditorPublicationExperimental));
        }

        [Fact(DisplayName = "Virta and ORCID publication have the same DOI, they are considered as the same publications: Virta publication has type code B2, both have the same name.")]
        public void hasSameDoiButIsDifferentPublication_070()
        {
            var duplicateHandlerService = new DuplicateHandlerService();
            var dimOrcidPublication = new DimOrcidPublication() { DoiHandle = "doi123", PublicationName = "name123" };
            var profileEditorPublicationExperimental = new ProfileEditorPublicationExperimental() { Doi = "doi123", PublicationName = "name123", TypeCode = "B2" };
            Assert.False(duplicateHandlerService.HasSameDoiButIsDifferentPublication(dimOrcidPublication, profileEditorPublicationExperimental));
        }

        [Fact(DisplayName = "Virta and ORCID publication have the same DOI, they are considered as different publications: Virta publication has type code B3, names differ.")]
        public void hasSameDoiButIsDifferentPublication_080()
        {
            var duplicateHandlerService = new DuplicateHandlerService();
            var dimOrcidPublication = new DimOrcidPublication() { DoiHandle = "doi123", PublicationName = "name123" };
            var profileEditorPublicationExperimental = new ProfileEditorPublicationExperimental() { Doi = "doi123", PublicationName = "name456", TypeCode = "B3" };
            Assert.True(duplicateHandlerService.HasSameDoiButIsDifferentPublication(dimOrcidPublication, profileEditorPublicationExperimental));
        }

        [Fact(DisplayName = "Virta and ORCID publication have the same DOI, they are considered as the same publications: Virta publication has type code B3, both have the same name.")]
        public void hasSameDoiButIsDifferentPublication_090()
        {
            var duplicateHandlerService = new DuplicateHandlerService();
            var dimOrcidPublication = new DimOrcidPublication() { DoiHandle = "doi123", PublicationName = "name123" };
            var profileEditorPublicationExperimental = new ProfileEditorPublicationExperimental() { Doi = "doi123", PublicationName = "name123", TypeCode = "B3" };
            Assert.False(duplicateHandlerService.HasSameDoiButIsDifferentPublication(dimOrcidPublication, profileEditorPublicationExperimental));
        }

        [Fact(DisplayName = "Virta and ORCID publication have the same DOI, they are considered as different publications: Virta publication has type code D2, names differ.")]
        public void hasSameDoiButIsDifferentPublication_100()
        {
            var duplicateHandlerService = new DuplicateHandlerService();
            var dimOrcidPublication = new DimOrcidPublication() { DoiHandle = "doi123", PublicationName = "name123" };
            var profileEditorPublicationExperimental = new ProfileEditorPublicationExperimental() { Doi = "doi123", PublicationName = "name456", TypeCode = "D2" };
            Assert.True(duplicateHandlerService.HasSameDoiButIsDifferentPublication(dimOrcidPublication, profileEditorPublicationExperimental));
        }

        [Fact(DisplayName = "Virta and ORCID publication have the same DOI, they are considered as the same publications: Virta publication has type code D2, both have the same name.")]
        public void hasSameDoiButIsDifferentPublication_110()
        {
            var duplicateHandlerService = new DuplicateHandlerService();
            var dimOrcidPublication = new DimOrcidPublication() { DoiHandle = "doi123", PublicationName = "name123" };
            var profileEditorPublicationExperimental = new ProfileEditorPublicationExperimental() { Doi = "doi123", PublicationName = "name123", TypeCode = "D2" };
            Assert.False(duplicateHandlerService.HasSameDoiButIsDifferentPublication(dimOrcidPublication, profileEditorPublicationExperimental));
        }

        [Fact(DisplayName = "Virta and ORCID publication have the same DOI, they are considered as different publications: Virta publication has type code D3, names differ.")]
        public void hasSameDoiButIsDifferentPublication_120()
        {
            var duplicateHandlerService = new DuplicateHandlerService();
            var dimOrcidPublication = new DimOrcidPublication() { DoiHandle = "doi123", PublicationName = "name123" };
            var profileEditorPublicationExperimental = new ProfileEditorPublicationExperimental() { Doi = "doi123", PublicationName = "name456", TypeCode = "D3" };
            Assert.True(duplicateHandlerService.HasSameDoiButIsDifferentPublication(dimOrcidPublication, profileEditorPublicationExperimental));
        }

        [Fact(DisplayName = "Virta and ORCID publication have the same DOI, they are considered as the same publications: Virta publication has type code D3, both have the same name.")]
        public void hasSameDoiButIsDifferentPublication_130()
        {
            var duplicateHandlerService = new DuplicateHandlerService();
            var dimOrcidPublication = new DimOrcidPublication() { DoiHandle = "doi123", PublicationName = "name123" };
            var profileEditorPublicationExperimental = new ProfileEditorPublicationExperimental() { Doi = "doi123", PublicationName = "name123", TypeCode = "D3" };
            Assert.False(duplicateHandlerService.HasSameDoiButIsDifferentPublication(dimOrcidPublication, profileEditorPublicationExperimental));
        }

        [Fact(DisplayName = "Virta and ORCID publication have the same DOI, they are considered as different publications: Virta publication has type code E1, names differ.")]
        public void hasSameDoiButIsDifferentPublication_140()
        {
            var duplicateHandlerService = new DuplicateHandlerService();
            var dimOrcidPublication = new DimOrcidPublication() { DoiHandle = "doi123", PublicationName = "name123" };
            var profileEditorPublicationExperimental = new ProfileEditorPublicationExperimental() { Doi = "doi123", PublicationName = "name456", TypeCode = "E1" };
            Assert.True(duplicateHandlerService.HasSameDoiButIsDifferentPublication(dimOrcidPublication, profileEditorPublicationExperimental));
        }

        [Fact(DisplayName = "Virta and ORCID publication have the same DOI, they are considered as the same publications: Virta publication has type code E1, both have the same name.")]
        public void hasSameDoiButIsDifferentPublication_150()
        {
            var duplicateHandlerService = new DuplicateHandlerService();
            var dimOrcidPublication = new DimOrcidPublication() { DoiHandle = "doi123", PublicationName = "name123" };
            var profileEditorPublicationExperimental = new ProfileEditorPublicationExperimental() { Doi = "doi123", PublicationName = "name123", TypeCode = "E1" };
            Assert.False(duplicateHandlerService.HasSameDoiButIsDifferentPublication(dimOrcidPublication, profileEditorPublicationExperimental));
        }




        [Fact(DisplayName = "AddPublicationToProfileEditorData_HandlePublicationIdDuplicates")]
        public void addPublicationToProfileEditorData_010()
        {
            var duplicateHandlerService = new DuplicateHandlerService();

            // Datasources
            var profileEditorSourceA = new ProfileEditorSource()
            {
                Id = 1,
                RegisteredDataSource = "Source A",
                Organization = new Organization() { NameEn = "Organization name A" }
            };
            var profileEditorSourceB = new ProfileEditorSource()
            {
                Id = 2,
                RegisteredDataSource = "Source B",
                Organization = new Organization() { NameEn = "Organization name B" }
            };

            // Create FactFieldValue for Virta publication 1
            var ffvVirta1 = new FactFieldValue()
            {
                DimPublication = new DimPublication()
                {
                    PublicationId = "publicationId123",
                    Doi = "doi123",
                    PublicationName = "name123",
                    PublicationTypeCode = "A1"
                }
            };

            // Create FactFieldValue for Virta publication 2
            var ffvVirta2 = new FactFieldValue()
            {
                DimPublication = new DimPublication()
                {
                    PublicationId = "publicationId456",
                    Doi = "doi456",
                    PublicationName = "name456",
                    PublicationTypeCode = "A2"
                }
            };

            // Create empty list of publications
            var publications = new List<ProfileEditorPublicationExperimental>() {};

            //
            // Add publication 1st time
            //
            var publications1 = duplicateHandlerService.AddPublicationToProfileEditorData(profileEditorSourceA, ffvVirta1, publications);
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
            var publications2 = duplicateHandlerService.AddPublicationToProfileEditorData(profileEditorSourceA, ffvVirta1, publications1);
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
            var publications3 = duplicateHandlerService.AddPublicationToProfileEditorData(profileEditorSourceB, ffvVirta1, publications2);
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
            var publications4 = duplicateHandlerService.AddPublicationToProfileEditorData(profileEditorSourceA, ffvVirta2, publications3);
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
            var duplicateHandlerService = new DuplicateHandlerService();

            // Datasources
            var profileEditorSourceVirta = new ProfileEditorSource()
            {
                Id = 1,
                RegisteredDataSource = "Virta",
                Organization = new Organization() {}
            };
            var profileEditorSourceOrcid = new ProfileEditorSource()
            {
                Id = 2,
                RegisteredDataSource = "ORCID",
                Organization = new Organization() {}
            };

            // Create FactFieldValue for Virta publication 1
            var ffvVirta1 = new FactFieldValue()
            {
                DimPublication = new DimPublication()
                {
                    PublicationId = "publicationId123",
                    Doi = "doi123",
                    PublicationName = "name123",
                    PublicationTypeCode = "A4"
                }
            };

            // Create FactFieldValue for ORCID publication 1. The same DOI and name as in Virta publication.
            var ffvOrcid1 = new FactFieldValue()
            {
                DimOrcidPublication = new DimOrcidPublication()
                {
                    PublicationId = "publicationId456",
                    DoiHandle = "doi123",
                    PublicationName = "name123"
                }
            };

            // Create FactFieldValue for ORCID publication 2. The same DOI as in Virta publication but different name.
            var ffvOrcid2 = new FactFieldValue()
            {
                DimOrcidPublication = new DimOrcidPublication()
                {
                    PublicationId = "publicationId789",
                    DoiHandle = "doi123",
                    PublicationName = "name456"
                }
            };

            // Create empty list of publications
            var publications = new List<ProfileEditorPublicationExperimental>() { };

            // Add Virta publication
            var publications1 = duplicateHandlerService.AddPublicationToProfileEditorData(profileEditorSourceVirta, ffvVirta1, publications);
            // Add ORCID publication with the same DOI and name
            var publications2 = duplicateHandlerService.AddPublicationToProfileEditorData(profileEditorSourceOrcid, ffvOrcid1, publications1);
            // Check that publication list contains one publication
            Assert.Single(publications2);
            // Check that publication has two data sources
            Assert.Equal(2, publications2[0].DataSources.Count);
            Assert.Equal(profileEditorSourceVirta.RegisteredDataSource, publications2[0].DataSources[0].RegisteredDataSource);
            Assert.Equal(profileEditorSourceOrcid.RegisteredDataSource, publications2[0].DataSources[1].RegisteredDataSource);

            // Add ORCID publication with the same DOI but different name
            var publications3 = duplicateHandlerService.AddPublicationToProfileEditorData(profileEditorSourceOrcid, ffvOrcid2, publications2);
            // Check that publication list contains two publications
            Assert.Equal(2, publications3.Count);
        }
    }
}