using Xunit;
using api.CustomMapper;
using api.Models.Elasticsearch;
using api.Models.ProfileEditor.Items;
using System.Collections.Generic;
using api.Models.ProfileEditor;
using api.Models.Common;

namespace api.Tests
{
    [Collection("Elasticsearch mapper tests")]
    public class ElasticsearchMapperTests
    {
        private const string orcidId = "1234-5678-9012-3456";
        private ProfileEditorSource profileEditorSource1 = new ProfileEditorSource
        {
            Id = 1,
            RegisteredDataSource = "source-id-1",
            Organization = new Organization
            {
                NameFi = "source-name-1-fi",
                NameEn = "source-name-1-en",
                NameSv = "source-name-1-sv",
                SectorId = "source-sector-id-1"
            }
        };

        private ProfileEditorSource profileEditorSource2 = new ProfileEditorSource
        {
            Id = 2,
            RegisteredDataSource = "source-id-2",
            Organization = new Organization
            {
                NameFi = "source-name-2-fi",
                NameEn = "source-name-2-en",
                NameSv = "source-name-2-sv",
                SectorId = "source-sector-id-2"
            }
        };

        private ProfileEditorSource profileEditorSource3 = new ProfileEditorSource
        {
            Id = 3,
            RegisteredDataSource = "source-id-3",
            Organization = new Organization
            {
                NameFi = "source-name-3-fi",
                NameEn = "source-name-3-en",
                NameSv = "source-name-3-sv",
                SectorId = "source-sector-id-3"
            }
        };

        private ElasticsearchSource elasticsearchSource1 = new ElasticsearchSource
        {
            RegisteredDataSource = "source-id-1",
            Organization = new Organization
            {
                NameFi = "source-name-1-fi",
                NameEn = "source-name-1-en",
                NameSv = "source-name-1-sv",
                SectorId = "source-sector-id-1"
            }
        };

        private ElasticsearchSource elasticsearchSource2 = new ElasticsearchSource
        {
            RegisteredDataSource = "source-id-2",
            Organization = new Organization
            {
                NameFi = "source-name-2-fi",
                NameEn = "source-name-2-en",
                NameSv = "source-name-2-sv",
                SectorId = "source-sector-id-2"
            }
        };

        private ElasticsearchSource elasticsearchSource3 = new ElasticsearchSource
        {
            RegisteredDataSource = "source-id-3",
            Organization = new Organization
            {
                NameFi = "source-name-3-fi",
                NameEn = "source-name-3-en",
                NameSv = "source-name-3-sv",
                SectorId = "source-sector-id-3"
            }
        };

        private ProfileEditorDataResponse GetProfileEditorDataResponse()
        {
            return new ProfileEditorDataResponse
            {
                personal = new ProfileEditorDataPersonal()
                {
                    names = new List<ProfileEditorName>
                    {
                        new ProfileEditorName
                        {
                            FirstNames = "name - test first name 1",
                            LastName = "name - test last name 1",
                            FullName = "name - test full name 1",
                            itemMeta = new ProfileEditorItemMeta(id: 1, type: 1, show: true, primaryValue: true),
                            DataSources = new List<ProfileEditorSource> { profileEditorSource1, profileEditorSource2 }
                        },
                        new ProfileEditorName
                        {
                            FirstNames = "name - test first name 2",
                            LastName = "name - test last name 2",
                            FullName = "name - test full name 2",
                            itemMeta = new ProfileEditorItemMeta(id: 1, type: 1, show: false, primaryValue: false), // This should not be included in the Elasticsearch mapping (show == false)
                            DataSources = new List<ProfileEditorSource> { profileEditorSource2 }
                        },
                        new ProfileEditorName
                        {
                            FirstNames = "name - test first name 3",
                            LastName = "name - test last name 3",
                            FullName = "name - test full name 3",
                            itemMeta = new ProfileEditorItemMeta(id: 1, type: 1, show: true, primaryValue: false),
                            DataSources = new List<ProfileEditorSource> { profileEditorSource3 }
                        }
                    },
                    otherNames = new List<ProfileEditorName>
                    {
                        new ProfileEditorName
                        {
                            FirstNames = "other name - test first name 1",
                            LastName = "other name - test last name 1",
                            FullName = "other name - test other name 1",
                            itemMeta = new ProfileEditorItemMeta(id: 1, type: 1, show: true, primaryValue: true),
                            DataSources = new List<ProfileEditorSource> { profileEditorSource1 }
                        },
                        new ProfileEditorName
                        {
                            FirstNames = "other name - test first name 2",
                            LastName = "other name - test last name 2",
                            FullName = "other name - test full name 2",
                            itemMeta = new ProfileEditorItemMeta(id: 1, type: 1, show: false, primaryValue: false), // This should not be included in the Elasticsearch mapping (show == false)
                            DataSources = new List<ProfileEditorSource> { profileEditorSource2 }
                        },
                        new ProfileEditorName
                        {
                            FirstNames = "other name - test first name 3",
                            LastName = "other name - test last name 3",
                            FullName = "other name - test full name 3",
                            itemMeta = new ProfileEditorItemMeta(id: 1, type: 1, show: true, primaryValue: false),
                            DataSources = new List<ProfileEditorSource> { profileEditorSource3 }
                        }
                    },
                    emails = new List<ProfileEditorEmail>
                    {
                        new ProfileEditorEmail {
                            Value = "email1@example.org",
                            itemMeta = new ProfileEditorItemMeta(id: 1, type: 1, show: true, primaryValue: true),
                            DataSources = new List<ProfileEditorSource> { profileEditorSource1 }
                        },
                        new ProfileEditorEmail {
                            Value = "email2@example.org",
                            itemMeta = new ProfileEditorItemMeta(id: 2, type: 1, show: false, primaryValue: false), // This should not be included in the Elasticsearch mapping (show == false)
                            DataSources = new List<ProfileEditorSource> { profileEditorSource2 }
                        },
                        new ProfileEditorEmail {
                            Value = "email3@example.org",
                            itemMeta = new ProfileEditorItemMeta(id: 3, type: 1, show: true, primaryValue: false),
                            DataSources = new List<ProfileEditorSource> { profileEditorSource3 }
                        }
                    },
                    telephoneNumbers = new List<ProfileEditorTelephoneNumber>
                    {
                        new ProfileEditorTelephoneNumber {
                            Value = "123456789",
                            itemMeta = new ProfileEditorItemMeta(id: 1, type: 1, show: true, primaryValue: true),
                            DataSources = new List<ProfileEditorSource> { profileEditorSource1 }
                        },
                        new ProfileEditorTelephoneNumber {
                            Value = "987654321",
                            itemMeta = new ProfileEditorItemMeta(id: 2, type: 1, show: false, primaryValue: false), // This should not be included in the Elasticsearch mapping (show == false)
                            DataSources = new List<ProfileEditorSource> { profileEditorSource2 }
                        },
                        new ProfileEditorTelephoneNumber {
                            Value = "456789012",
                            itemMeta = new ProfileEditorItemMeta(id: 3, type: 1, show: true, primaryValue: false),
                            DataSources = new List<ProfileEditorSource> { profileEditorSource3 }
                        }
                    },
                    webLinks = new List<ProfileEditorWebLink>
                    {
                        new ProfileEditorWebLink {
                            Url = "https://example1.org",
                            LinkLabel = "Example 1",
                            itemMeta = new ProfileEditorItemMeta(id: 1, type: 1, show: true, primaryValue: true),
                            DataSources = new List<ProfileEditorSource> { profileEditorSource1 }
                        },
                        new ProfileEditorWebLink {
                            Url = "https://example2.org",
                            LinkLabel = "Example 2",
                            itemMeta = new ProfileEditorItemMeta(id: 2, type: 1, show: false, primaryValue: false), // This should not be included in the Elasticsearch mapping (show == false)
                            DataSources = new List<ProfileEditorSource> { profileEditorSource2 }
                        },
                            new ProfileEditorWebLink {
                            Url = "https://example3.org",
                            LinkLabel = "Example 3",
                            itemMeta = new ProfileEditorItemMeta(id: 3, type: 1, show: true, primaryValue: false),
                            DataSources = new List<ProfileEditorSource> { profileEditorSource3 }
                        }
                    },
                    keywords = new List<ProfileEditorKeyword>
                    {
                        new ProfileEditorKeyword {
                            Value = "keyword1",
                            itemMeta = new ProfileEditorItemMeta(id: 1, type: 1, show: true, primaryValue: true),
                            DataSources = new List<ProfileEditorSource> { profileEditorSource1 }
                        },
                        new ProfileEditorKeyword {
                            Value = "keyword2",
                            itemMeta = new ProfileEditorItemMeta(id: 2, type: 1, show: false, primaryValue: false), // This should not be included in the Elasticsearch mapping (show == false)
                            DataSources = new List<ProfileEditorSource> { profileEditorSource2 }
                        },
                        new ProfileEditorKeyword {
                            Value = "keyword3",
                            itemMeta = new ProfileEditorItemMeta(id: 3, type: 1, show: true, primaryValue: false),
                            DataSources = new List<ProfileEditorSource> { profileEditorSource3 }
                        }
                    },
                    fieldOfSciences = new List<ProfileEditorFieldOfScience>
                    {
                        new ProfileEditorFieldOfScience {
                            NameFi = "fi field of science 1",
                            NameEn = "en field of science 1",
                            NameSv = "sv field of science 1",
                            itemMeta = new ProfileEditorItemMeta(id: 1, type: 1, show: true, primaryValue: true),
                            DataSources = new List<ProfileEditorSource> { profileEditorSource1 }
                        },
                        new ProfileEditorFieldOfScience {
                            NameFi = "fi field of science 2",
                            NameEn = "en field of science 2",
                            NameSv = "sv field of science 2",
                            itemMeta = new ProfileEditorItemMeta(id: 2, type: 1, show: false, primaryValue: false), // This should not be included in the Elasticsearch mapping (show == false)
                            DataSources = new List<ProfileEditorSource> { profileEditorSource2 }
                        },
                        new ProfileEditorFieldOfScience {
                            NameFi = "fi field of science 3",
                            NameEn = "en field of science 3",
                            NameSv = "sv field of science 3",
                            itemMeta = new ProfileEditorItemMeta(id: 3, type: 1, show: true, primaryValue: false),
                            DataSources = new List<ProfileEditorSource> { profileEditorSource3 }
                        }
                    },
                    researcherDescriptions = new List<ProfileEditorResearcherDescription>
                    {
                        new ProfileEditorResearcherDescription
                        {
                            ResearchDescriptionFi = "fi description 1",
                            ResearchDescriptionEn = "en description 1",
                            ResearchDescriptionSv = "sv description 1",
                            itemMeta = new ProfileEditorItemMeta(id: 1, type: 1, show: true, primaryValue: true),
                            DataSources = new List<ProfileEditorSource> { profileEditorSource1 }
                        },
                        new ProfileEditorResearcherDescription
                        {
                            ResearchDescriptionFi = "fi description 2",
                            ResearchDescriptionEn = "en description 2",
                            ResearchDescriptionSv = "sv description 2",
                            itemMeta = new ProfileEditorItemMeta(id: 1, type: 1, show: false, primaryValue: false), // This should not be included in the Elasticsearch mapping (show == false)
                            DataSources = new List<ProfileEditorSource> { profileEditorSource2 }
                        },
                        new ProfileEditorResearcherDescription
                        {
                            ResearchDescriptionFi = "fi description 3",
                            ResearchDescriptionEn = "en description 3",
                            ResearchDescriptionSv = "sv description 3",
                            itemMeta = new ProfileEditorItemMeta(id: 1, type: 1, show: true, primaryValue: false),
                            DataSources = new List<ProfileEditorSource> { profileEditorSource3 }
                        }
                    },
                    externalIdentifiers = new List<ProfileEditorExternalIdentifier>
                    {
                        new ProfileEditorExternalIdentifier {
                            PidContent = "external-id-1",
                            PidType = "type-1",
                            itemMeta = new ProfileEditorItemMeta(id: 1, type: 1, show: true, primaryValue: true),
                            DataSources = new List<ProfileEditorSource> { profileEditorSource1 }
                        },
                        new ProfileEditorExternalIdentifier {
                            PidContent = "external-id-2",
                            PidType = "type-2",
                            itemMeta = new ProfileEditorItemMeta(id: 2, type: 1, show: false, primaryValue: false), // This should not be included in the Elasticsearch mapping (show == false)
                            DataSources = new List<ProfileEditorSource> { profileEditorSource2 }
                        },
                        new ProfileEditorExternalIdentifier {
                            PidContent = "external-id-3",
                            PidType = "type-3",
                            itemMeta = new ProfileEditorItemMeta(id: 3, type: 1, show: true, primaryValue: false),
                            DataSources = new List<ProfileEditorSource> { profileEditorSource3 }
                        }
                    }
                },
                activity = new ProfileEditorDataActivity()
                {
                    affiliations = new List<ProfileEditorAffiliation>
                    {
                        new ProfileEditorAffiliation
                        {
                            OrganizationNameFi = "aff1 - orgNameFi",
                            OrganizationNameSv = "aff1 - orgNameSv",
                            OrganizationNameEn = "aff1 - orgNameEn",
                            DepartmentNameFi = "aff1 - deptNameFi",
                            DepartmentNameSv = "aff1 - deptNameSv",
                            DepartmentNameEn = "aff1 - deptNameEn",
                            PositionNameFi = "aff1 - posNameFi",
                            PositionNameSv = "aff1 - posNameSv",
                            PositionNameEn = "aff1 - posNameEn",
                            AffiliationTypeFi = "aff1 - affTypeFi",
                            AffiliationTypeEn = "aff1 - affTypeEn",
                            AffiliationTypeSv = "aff1 - affTypeSv",
                            StartDate = new ProfileEditorDate() {
                                Year = 2020,
                                Month = 1,
                                Day = 1
                            },
                            EndDate = new ProfileEditorDate() {
                                Year = 2021,
                                Month = 12,
                                Day = 31
                            },
                            sector = new List<ProfileEditorSector> {
                                new ProfileEditorSector {
                                    sectorId = "aff1 - sector1 - id",
                                    nameFiSector = "aff1 - sector1 - nameFi",
                                    nameEnSector = "aff1 - sector1 - nameEn",
                                    nameSvSector = "aff1 - sector1 - nameSv",
                                    organization = new List<ProfileEditorSectorOrganization> {
                                        new ProfileEditorSectorOrganization {
                                            organizationId = "aff1 - sector1 - org1 - orgid",
                                            OrganizationNameFi = "aff1 - sector 1 - org1 - orgNameFi",
                                            OrganizationNameEn = "aff1 - sector 1 - org1 - orgNameEn",
                                            OrganizationNameSv = "aff1 - sector 1 - org1 - orgNameSv"
                                        },
                                        new ProfileEditorSectorOrganization {
                                            organizationId = "aff1 - sector1 - org2 - orgid",
                                            OrganizationNameFi = "aff1 - sector 1 - org2 - orgNameFi",
                                            OrganizationNameEn = "aff1 - sector 1 - org2 - orgNameEn",
                                            OrganizationNameSv = "aff1 - sector 1 - org2 - orgNameSv"
                                        }
                                    }
                                },
                                new ProfileEditorSector {
                                    sectorId = "aff1 - sector2 - id",
                                    nameFiSector = "aff1 - sector2 - nameFi",
                                    nameEnSector = "aff1 - sector2 - nameEn",
                                    nameSvSector = "aff1 - sector2 - nameSv",
                                    organization = new List<ProfileEditorSectorOrganization> {
                                        new ProfileEditorSectorOrganization {
                                            organizationId = "aff1 - sector2 - org1 - orgid",
                                            OrganizationNameFi = "aff1 - sector2 - org1 - orgNameFi",
                                            OrganizationNameEn = "aff1 - sector2 - org1 - orgNameEn",
                                            OrganizationNameSv = "aff1 - sector2 - org1 - orgNameSv"
                                        },
                                        new ProfileEditorSectorOrganization {
                                            organizationId = "aff1 - sector2 - org2 - orgid",
                                            OrganizationNameFi = "aff1 - sector2 - org2 - orgNameFi",
                                            OrganizationNameEn = "aff1 - sector2 - org2 - orgNameEn",
                                            OrganizationNameSv = "aff1 - sector2 - org2 - orgNameSv"
                                        }
                                    }
                                }
                            },
                            itemMeta = new ProfileEditorItemMeta(id: 1, type: 1, show: true, primaryValue: true),
                            DataSources = new List<ProfileEditorSource> { profileEditorSource1 }
                        },
                        new ProfileEditorAffiliation
                        {
                            itemMeta = new ProfileEditorItemMeta(id: 2, type: 1, show: false, primaryValue: false), // This should not be included in the Elasticsearch mapping (show == false)
                            DataSources = new List<ProfileEditorSource> { profileEditorSource2 }
                        },
                        new ProfileEditorAffiliation
                        {
                            OrganizationNameFi = "aff3 - orgNameFi",
                            OrganizationNameSv = "aff3 - orgNameSv",
                            OrganizationNameEn = "aff3 - orgNameEn",
                            DepartmentNameFi = "aff3 - deptNameFi",
                            DepartmentNameSv = "aff3 - deptNameSv",
                            DepartmentNameEn = "aff3 - deptNameEn",
                            PositionNameFi = "aff3 - posNameFi",
                            PositionNameSv = "aff3 - posNameSv",
                            PositionNameEn = "aff3 - posNameEn",
                            AffiliationTypeFi = "aff3 - affTypeFi",
                            AffiliationTypeEn = "aff3 - affTypeEn",
                            AffiliationTypeSv = "aff3 - affTypeSv",
                            StartDate = new ProfileEditorDate() {
                                Year = 2023,
                                Month = 1,
                                Day = 1
                            },
                            EndDate = new ProfileEditorDate() {
                                Year = 2024,
                                Month = 12,
                                Day = 31
                            },
                            sector = new List<ProfileEditorSector> {
                                new ProfileEditorSector {
                                    sectorId = "aff3 - sector1 - id",
                                    nameFiSector = "aff3 - sector1 - nameFi",
                                    nameEnSector = "aff3 - sector1 - nameEn",
                                    nameSvSector = "aff3 - sector1 - nameSv",
                                    organization = new List<ProfileEditorSectorOrganization> {
                                        new ProfileEditorSectorOrganization {
                                            organizationId = "aff3 - sector1 - org1 - orgid",
                                            OrganizationNameFi = "aff3 - sector1 - org1 - orgNameFi",
                                            OrganizationNameEn = "aff3 - sector1 - org1 - orgNameEn",
                                            OrganizationNameSv = "aff3 - sector1 - org1 - orgNameSv"
                                        },
                                        new ProfileEditorSectorOrganization {
                                            organizationId = "aff3 - sector1 - org2 - orgid",
                                            OrganizationNameFi = "aff3 - sector1 - org2 - orgNameFi",
                                            OrganizationNameEn = "aff3 - sector1 - org2 - orgNameEn",
                                            OrganizationNameSv = "aff3 - sector1 - org2 - orgNameSv"
                                        }
                                    }
                                },
                                new ProfileEditorSector {
                                    sectorId = "aff3 - sector2 - id",
                                    nameFiSector = "aff3 - sector2 - nameFi",
                                    nameEnSector = "aff3 - sector2 - nameEn",
                                    nameSvSector = "aff3 - sector2 - nameSv",
                                    organization = new List<ProfileEditorSectorOrganization> {
                                        new ProfileEditorSectorOrganization {
                                            organizationId = "aff3 - sector2 - org1 - orgid",
                                            OrganizationNameFi = "aff3 - sector2 - org1 - orgNameFi",
                                            OrganizationNameEn = "aff3 - sector2 - org1 - orgNameEn",
                                            OrganizationNameSv = "aff3 - sector2 - org1 - orgNameSv"
                                        },
                                        new ProfileEditorSectorOrganization {
                                            organizationId = "aff3 - sector2 - org2 - orgid",
                                            OrganizationNameFi = "aff3 - sector2 - org2 - orgNameFi",
                                            OrganizationNameEn = "aff3 - sector2 - org2 - orgNameEn",
                                            OrganizationNameSv = "aff3 - sector2 - org2 - orgNameSv"
                                        }
                                    }
                                }
                            },
                            itemMeta = new ProfileEditorItemMeta(id: 1, type: 1, show: true, primaryValue: false),
                            DataSources = new List<ProfileEditorSource> { profileEditorSource3 }
                        }
                    },
                    educations = new List<ProfileEditorEducation>
                    {
                        new ProfileEditorEducation
                        {
                            NameFi = "ed1 - nameFi",
                            NameSv = "ed1 - nameSv",
                            NameEn = "ed1 - nameEn",
                            DegreeGrantingInstitutionName = "ed1 - degreeGrantingInstitutionName",
                            StartDate = new ProfileEditorDate { Year = 2015, Month = 1, Day = 1 },
                            EndDate = new ProfileEditorDate { Year = 2019, Month = 12, Day = 31 },
                            itemMeta = new ProfileEditorItemMeta(id: 1, type: 1, show: true, primaryValue: true),
                            DataSources = new List<ProfileEditorSource> { profileEditorSource1 }
                        },
                        new ProfileEditorEducation
                        {
                            itemMeta = new ProfileEditorItemMeta(id: 1, type: 1, show: false, primaryValue: false), // This should not be included in the Elasticsearch mapping (show == false)
                            DataSources = new List<ProfileEditorSource> { profileEditorSource3 }
                        },
                        new ProfileEditorEducation
                        {
                            NameFi = "ed3 - nameFi",
                            NameSv = "ed3 - nameSv",
                            NameEn = "ed3 - nameEn",
                            DegreeGrantingInstitutionName = "ed3 - degreeGrantingInstitutionName",
                            StartDate = new ProfileEditorDate { Year = 2016, Month = 2, Day = 2 },
                            EndDate = new ProfileEditorDate { Year = 2020, Month = 11, Day = 30 },
                            itemMeta = new ProfileEditorItemMeta(id: 1, type: 1, show: true, primaryValue: false),
                            DataSources = new List<ProfileEditorSource> { profileEditorSource3 }
                        }
                    },
                    publications = new List<ProfileEditorPublication>
                    {
                        new ProfileEditorPublication
                        {
                            AuthorsText = "pub1 - authors text",
                            Doi = "pub1 - doi",
                            ConferenceName = "pub1 - conference name",
                            JournalName = "pub1 - journal name",
                            OpenAccess = 1,
                            ParentPublicationName = "pub1 - parent publication name",
                            PeerReviewed = new List<ProfileEditorPublicationPeerReviewed>
                            {
                                new ProfileEditorPublicationPeerReviewed
                                {
                                    Id = "pub1 - peerreview1 - id",
                                    NameFiPeerReviewed = "pub1 - peerreview1 - name fi",
                                    NameSvPeerReviewed = "pub1 - peerreview1 - name en",
                                    NameEnPeerReviewed = "pub1 - peerreview1 - name sv"
                                },
                                new ProfileEditorPublicationPeerReviewed
                                {
                                    Id = "pub1 - peerreview2 - id",
                                    NameFiPeerReviewed = "pub1 - peerreview2 - name fi",
                                    NameSvPeerReviewed = "pub1 - peerreview2 - name en",
                                    NameEnPeerReviewed = "pub1 - peerreview2 - name sv"
                                }
                            },
                            PublicationId = "pub1 - publication id",
                            PublicationName = "pub1 - publication name",
                            PublicationTypeCode = "pub1 - publication type code",
                            PublicationYear = 2021,
                            SelfArchivedAddress = "pub1 - self archived address",
                            SelfArchivedCode = "pub1 - self archived code",
                            itemMeta = new ProfileEditorItemMeta(id: 1, type: 1, show: true, primaryValue: true),
                            DataSources = new List<ProfileEditorSource> { profileEditorSource1 }
                        },
                        new ProfileEditorPublication
                        {
                            itemMeta = new ProfileEditorItemMeta(id: 2, type: 1, show: false, primaryValue: false), // This should not be included in the Elasticsearch mapping (show == false)
                            DataSources = new List<ProfileEditorSource> { profileEditorSource3 }
                        },
                        new ProfileEditorPublication
                        {
                            AuthorsText = "pub3 - authors text",
                            Doi = "pub3 - doi",
                            ConferenceName = "pub3 - conference name",
                            JournalName = "pub3 - journal name",
                            OpenAccess = 0,
                            ParentPublicationName = "pub3 - parent publication name",
                            PeerReviewed = new List<ProfileEditorPublicationPeerReviewed>
                            {
                                new ProfileEditorPublicationPeerReviewed
                                {
                                    Id = "pub3 - peerreview1 - id",
                                    NameFiPeerReviewed = "pub3 - peerreview1 - name fi",
                                    NameSvPeerReviewed = "pub3 - peerreview1 - name en",
                                    NameEnPeerReviewed = "pub3 - peerreview1 - name sv"
                                },
                            },
                            PublicationId = "pub3 - publication id",
                            PublicationName = "pub3 - publication name",
                            PublicationTypeCode = "pub3 - publication type code",
                            PublicationYear = 2022,
                            SelfArchivedAddress = "pub3 - self archived address",
                            SelfArchivedCode = "pub3 - self archived code",
                            itemMeta = new ProfileEditorItemMeta(id: 3, type: 1, show: true, primaryValue: false),
                            DataSources = new List<ProfileEditorSource> { profileEditorSource3 }
                        }
                    },
                    fundingDecisions = new List<ProfileEditorFundingDecision>
                    {
                        new ProfileEditorFundingDecision
                        {
                            ProjectId = 123,
                            ProjectAcronym = "funding1 - project acronym",
                            ProjectNameFi = "funding1 - project name fi",
                            ProjectNameSv = "funding1 - project name sv",
                            ProjectNameEn = "funding1 - project name en",
                            ProjectDescriptionFi = "funding1 - project description fi",
                            ProjectDescriptionSv = "funding1 - project description sv",
                            ProjectDescriptionEn = "funding1 - project description en",
                            FunderNameFi = "funding1 - funder name fi",
                            FunderNameSv = "funding1 - funder name sv",
                            FunderNameEn = "funding1 - funder name en",
                            FunderProjectNumber = "funding1 - funder project number",
                            TypeOfFundingNameFi = "funding1 - type of funding name fi",
                            TypeOfFundingNameSv = "funding1 - type of funding name sv",
                            TypeOfFundingNameEn = "funding1 - type of funding name en",
                            CallProgrammeNameFi = "funding1 - call programme name fi",
                            CallProgrammeNameSv = "funding1 - call programme name sv",
                            CallProgrammeNameEn = "funding1 - call programme name en",
                            FundingStartYear = 2021,
                            FundingEndYear = 2023,
                            AmountInEur = 100000,
                            AmountInFundingDecisionCurrency = 90000,
                            FundingDecisionCurrencyAbbreviation = "EUR",
                            Url = "https://example.org/funding1",
                            itemMeta = new ProfileEditorItemMeta(id: 1, type: 2, show: true, primaryValue: false),
                            DataSources = new List<ProfileEditorSource> { profileEditorSource1 }
                        },
                        new ProfileEditorFundingDecision
                        {
                            itemMeta = new ProfileEditorItemMeta(id: 2, type: 2, show: false, primaryValue: false), // This should not be included in the Elasticsearch mapping (show == false)
                            DataSources = new List<ProfileEditorSource> { profileEditorSource2 }
                        },
                        new ProfileEditorFundingDecision
                        {
                            ProjectId = 456,
                            ProjectAcronym = "funding3 - project acronym",
                            ProjectNameFi = "funding3 - project name fi",
                            ProjectNameSv = "funding3 - project name sv",
                            ProjectNameEn = "funding3 - project name en",
                            ProjectDescriptionFi = "funding3 - project description fi",
                            ProjectDescriptionSv = "funding3 - project description sv",
                            ProjectDescriptionEn = "funding3 - project description en",
                            FunderNameFi = "funding3 - funder name fi",
                            FunderNameSv = "funding3 - funder name sv",
                            FunderNameEn = "funding3 - funder name en",
                            FunderProjectNumber = "funding3 - funder project number",
                            TypeOfFundingNameFi = "funding3 - type of funding name fi",
                            TypeOfFundingNameSv = "funding3 - type of funding name sv",
                            TypeOfFundingNameEn = "funding3 - type of funding name en",
                            CallProgrammeNameFi = "funding3 - call programme name fi",
                            CallProgrammeNameSv = "funding3 - call programme name sv",
                            CallProgrammeNameEn = "funding3 - call programme name en",
                            FundingStartYear = 2022,
                            FundingEndYear = 2024,
                            AmountInEur = 200000,
                            AmountInFundingDecisionCurrency = 440000,
                            FundingDecisionCurrencyAbbreviation = "USD",
                            Url = "https://example.org/funding3",
                            itemMeta = new ProfileEditorItemMeta(id: 3, type: 2, show: true, primaryValue: false),
                            DataSources = new List<ProfileEditorSource> { profileEditorSource3 }
                        }
                    },
                    researchDatasets = new List<ProfileEditorResearchDataset>
                    {
                        new ProfileEditorResearchDataset
                        {
                            AccessType = "dataset-access-type-1",
                            Actor = new List<ProfileEditorActor>
                            {
                                new ProfileEditorActor
                                {
                                    actorRole = 1,
                                    actorRoleNameFi = "Rooli 1",
                                    actorRoleNameSv = "Roll 1",
                                    actorRoleNameEn = "Role 1",
                                },
                                new ProfileEditorActor
                                {
                                    actorRole = 2,
                                    actorRoleNameFi = "Rooli 2",
                                    actorRoleNameSv = "Roll 2",
                                    actorRoleNameEn = "Role 2",
                                }
                            },
                            FairdataUrl = "https://fairdata.example.org/dataset1",
                            Identifier = "dataset-identifier-1",
                            NameFi = "dataset 1 - nameFi",
                            NameSv = "dataset 1 - nameSv",
                            NameEn = "dataset 1 - nameEn",
                            DescriptionFi = "dataset 1 - descriptionFi",
                            DescriptionSv = "dataset 1 - descriptionSv",
                            DescriptionEn = "dataset 1 - descriptionEn",
                            Url = "https://example.org/dataset1",
                            DatasetCreated = 2021,
                            PreferredIdentifiers = new List<ProfileEditorPreferredIdentifier>
                            {
                                new ProfileEditorPreferredIdentifier
                                {
                                    PidContent = "preferred-id-1",
                                    PidType = "preferred-type-1"
                                },
                                new ProfileEditorPreferredIdentifier
                                {
                                    PidContent = "preferred-id-2",
                                    PidType = "preferred-type-2"
                                }
                            },
                            itemMeta = new ProfileEditorItemMeta(id: 1, type: 3, show: true, primaryValue: true),
                            DataSources = new List<ProfileEditorSource> { profileEditorSource1 }
                        },
                        new ProfileEditorResearchDataset
                        {
                            itemMeta = new ProfileEditorItemMeta(id: 2, type: 3, show: false, primaryValue: false), // This should not be included in the Elasticsearch mapping (show == false)
                            DataSources = new List<ProfileEditorSource> { profileEditorSource2 }
                        },
                        new ProfileEditorResearchDataset
                        {
                            AccessType = "dataset-access-type-3",
                            Actor = new List<ProfileEditorActor>
                            {
                                new ProfileEditorActor
                                {
                                    actorRole = 3,
                                    actorRoleNameFi = "Rooli 3",
                                    actorRoleNameSv = "Roll 3",
                                    actorRoleNameEn = "Role 3",
                                },
                                new ProfileEditorActor
                                {
                                    actorRole = 4,
                                    actorRoleNameFi = "Rooli 4",
                                    actorRoleNameSv = "Roll 4",
                                    actorRoleNameEn = "Role 4",
                                }
                            },
                            FairdataUrl = "https://fairdata.example.org/dataset3",
                            Identifier = "dataset-identifier-3",
                            NameFi = "dataset 3 - nameFi",
                            NameSv = "dataset 3 - nameSv",
                            NameEn = "dataset 3 - nameEn",
                            DescriptionFi = "dataset 3 - descriptionFi",
                            DescriptionSv = "dataset 3 - descriptionSv",
                            DescriptionEn = "dataset 3 - descriptionEn",
                            Url = "https://example.org/dataset3",
                            DatasetCreated = 2022,
                            PreferredIdentifiers = new List<ProfileEditorPreferredIdentifier>
                            {
                                new ProfileEditorPreferredIdentifier
                                {
                                    PidContent = "preferred-id-3",
                                    PidType = "preferred-type-3"
                                },
                                new ProfileEditorPreferredIdentifier
                                {
                                    PidContent = "preferred-id-4",
                                    PidType = "preferred-type-4"
                                }
                            },
                            itemMeta = new ProfileEditorItemMeta(id: 3, type: 3, show: true, primaryValue: false),
                            DataSources = new List<ProfileEditorSource> { profileEditorSource3 }
                        }
                    },
                    activitiesAndRewards = new List<ProfileEditorActivityAndReward>
                    {
                        new ProfileEditorActivityAndReward
                        {
                            NameFi = "activityAndReward 1 - nameFi",
                            NameEn = "activityAndReward 1 - nameEn",
                            NameSv = "activityAndReward 1 - nameSv",
                            DescriptionFi = "activityAndReward 1 - descriptionFi",
                            DescriptionEn = "activityAndReward 1 - descriptionEn",
                            DescriptionSv = "activityAndReward 1 - descriptionSv",
                            InternationalCollaboration = null,
                            StartDate = new ProfileEditorDate
                            {
                                Year = 2020,
                                Month = 1,
                                Day = 1
                            },
                            EndDate = new ProfileEditorDate
                            {
                                Year = 2021,
                                Month = 12,
                                Day = 31
                            },
                            ActivityTypeCode = "activityAndReward 1 - type-code",
                            ActivityTypeNameFi = "activityAndReward 1 - type-name-fi",
                            ActivityTypeNameEn = "activityAndReward 1 - type-name-en",
                            ActivityTypeNameSv = "activityAndReward 1 - type-name-sv",
                            RoleCode = "activityAndReward 1 - role-code",
                            RoleNameFi = "activityAndReward 1 - role-name-fi",
                            RoleNameEn = "activityAndReward 1 - role-name-en",
                            RoleNameSv = "activityAndReward 1 - role-name-sv",
                            OrganizationNameFi = "activityAndReward 1 - organization-name-fi",
                            OrganizationNameSv = "activityAndReward 1 - organization-name-sv",
                            OrganizationNameEn = "activityAndReward 1 - organization-name-en",
                            DepartmentNameFi = "activityAndReward 1 - department-name-fi",
                            DepartmentNameSv = "activityAndReward 1 - department-name-sv",
                            DepartmentNameEn = "activityAndReward 1 - department-name-en",
                            Url = "activityAndReward 1 - url",
                            sector = new List<ProfileEditorSector> {
                                new ProfileEditorSector
                                {
                                    sectorId = "activityAndReward 1 - sector1 - id",
                                    nameFiSector = "activityAndReward 1 - sector1 - nameFi",
                                    nameEnSector = "activityAndReward 1 - sector1 - nameEn",
                                    nameSvSector = "activityAndReward 1 - sector1 - nameSv",
                                    organization = new List<ProfileEditorSectorOrganization> {
                                        new ProfileEditorSectorOrganization {
                                            organizationId = "activityAndReward 1 - sector1 - org1 - orgid",
                                            OrganizationNameFi = "activityAndReward 1 - sector 1 - org1 - orgNameFi",
                                            OrganizationNameEn = "activityAndReward 1 - sector 1 - org1 - orgNameEn",
                                            OrganizationNameSv = "activityAndReward 1 - sector 1 - org1 - orgNameSv"
                                        }
                                    }
                                }
                            },
                            itemMeta = new ProfileEditorItemMeta(id: 1, type: 4, show: true, primaryValue: true),
                            DataSources = new List<ProfileEditorSource> { profileEditorSource1 }
                        },
                        new ProfileEditorActivityAndReward
                        {
                            itemMeta = new ProfileEditorItemMeta(id: 2, type: 4, show: false, primaryValue: false), // This should not be included in the Elasticsearch mapping (show == false)
                            DataSources = new List<ProfileEditorSource> { profileEditorSource1 }
                        },
                        new ProfileEditorActivityAndReward
                        {
                            NameFi = "activityAndReward 3 - nameFi",
                            NameEn = "activityAndReward 3 - nameEn",
                            NameSv = "activityAndReward 3 - nameSv",
                            DescriptionFi = "activityAndReward 3 - descriptionFi",
                            DescriptionEn = "activityAndReward 3 - descriptionEn",
                            DescriptionSv = "activityAndReward 3 - descriptionSv",
                            InternationalCollaboration = null,
                            StartDate = new ProfileEditorDate
                            {
                                Year = 2022,
                                Month = 1,
                                Day = 1
                            },
                            EndDate = new ProfileEditorDate
                            {
                                Year = 2022,
                                Month = 12,
                                Day = 31
                            },
                            ActivityTypeCode = "activityAndReward 3 - type-code",
                            ActivityTypeNameFi = "activityAndReward 3 - type-name-fi",
                            ActivityTypeNameEn = "activityAndReward 3 - type-name-en",
                            ActivityTypeNameSv = "activityAndReward 3 - type-name-sv",
                            RoleCode = "activityAndReward 3 - role-code",
                            RoleNameFi = "activityAndReward 3 - role-name-fi",
                            RoleNameEn = "activityAndReward 3 - role-name-en",
                            RoleNameSv = "activityAndReward 3 - role-name-sv",
                            OrganizationNameFi = "activityAndReward 3 - organization-name-fi",
                            OrganizationNameSv = "activityAndReward 3 - organization-name-sv",
                            OrganizationNameEn = "activityAndReward 3 - organization-name-en",
                            DepartmentNameFi = "activityAndReward 3 - department-name-fi",
                            DepartmentNameSv = "activityAndReward 3 - department-name-sv",
                            DepartmentNameEn = "activityAndReward 3 - department-name-en",
                            Url = "activityAndReward 3 - url",
                            sector = new List<ProfileEditorSector> {
                                new ProfileEditorSector
                                {
                                    sectorId = "activityAndReward 3 - sector1 - id",
                                    nameFiSector = "activityAndReward 3 - sector1 - nameFi",
                                    nameEnSector = "activityAndReward 3 - sector1 - nameEn",
                                    nameSvSector = "activityAndReward 3 - sector1 - nameSv",
                                    organization = new List<ProfileEditorSectorOrganization> {
                                        new ProfileEditorSectorOrganization {
                                            organizationId = "activityAndReward 3 - sector1 - org1 - orgid",
                                            OrganizationNameFi = "activityAndReward 3 - sector 1 - org1 - orgNameFi",
                                            OrganizationNameEn = "activityAndReward 3 - sector 1 - org1 - orgNameEn",
                                            OrganizationNameSv = "activityAndReward 3 - sector 1 - org1 - orgNameSv"
                                        }
                                    }
                                }
                            },
                            itemMeta = new ProfileEditorItemMeta(id: 3, type: 4, show: true, primaryValue: false),
                            DataSources = new List<ProfileEditorSource> { profileEditorSource3 }
                        }
                    }
                },
                settings = new ProfileSettings
                {
                    Hidden = true,
                    PublishNewData = true,
                    HighlightOpeness = true
                },
                cooperation = new List<ProfileEditorCooperationItem>
                {
                    new ProfileEditorCooperationItem
                    {
                        Id = 1,
                        NameFi = "cooperation 1 - nameFi",
                        NameEn = "cooperation 1 - nameEn",
                        NameSv = "cooperation 1 - nameSv",
                        Selected = true,
                        Order = 1
                    },
                    new ProfileEditorCooperationItem
                    {
                        Id = 2,
                        NameFi = "cooperation 2 - nameFi",
                        NameEn = "cooperation 2 - nameEn",
                        NameSv = "cooperation 2 - nameSv",
                        Selected = false,
                        Order = 2
                    },
                    new ProfileEditorCooperationItem
                    {
                        Id = 3,
                        NameFi = "cooperation 3 - nameFi",
                        NameEn = "cooperation 3 - nameEn",
                        NameSv = "cooperation 3 - nameSv",
                        Selected = true,
                        Order = 3
                    }
                },
                uniqueDataSources = new List<ProfileEditorSource>
                {
                    new ProfileEditorSource
                    {
                        Id = 1,
                        RegisteredDataSource = "source-id-1",
                        Organization = new Organization()
                        {
                            NameFi = "source-name-1-fi",
                            NameEn = "source-name-1-en",
                            NameSv = "source-name-1-sv",
                            SectorId = "source-sector-id-1"
                        }
                    },
                    new ProfileEditorSource
                    {
                        Id = 2,
                        RegisteredDataSource = "source-id-2",
                        Organization = new Organization()
                        {
                            NameFi = "source-name-2-fi",
                            NameEn = "source-name-2-en",
                            NameSv = "source-name-2-sv",
                            SectorId = "source-sector-id-2"
                        }
                    }
                }
            };
        }

        private ElasticsearchPerson GetElasticsearchPerson()
        {
            return new ElasticsearchPerson
            {
                id = orcidId,
                personal = new ElasticsearchPersonal
                {
                    names = new List<ElasticsearchName>
                    {
                        new ElasticsearchName
                        {
                            FirstNames = "name - test first name 1",
                            LastName = "name - test last name 1",
                            FullName = "name - test full name 1",
                            itemMeta = new ElasticsearchItemMeta { PrimaryValue = true },
                            DataSources = new List<ElasticsearchSource> { elasticsearchSource1, elasticsearchSource2 }
                        },
                        new ElasticsearchName
                        {
                            FirstNames = "name - test first name 3",
                            LastName = "name - test last name 3",
                            FullName = "name - test full name 3",
                            itemMeta = new ElasticsearchItemMeta { PrimaryValue = false },
                            DataSources = new List<ElasticsearchSource> { elasticsearchSource3 }
                        }
                    },
                    otherNames = new List<ElasticsearchName>
                    {
                        new ElasticsearchName
                        {
                            FirstNames = "other name - test first name 1",
                            LastName = "other name - test last name 1",
                            FullName = "other name - test other name 1",
                            itemMeta = new ElasticsearchItemMeta { PrimaryValue = true },
                            DataSources = new List<ElasticsearchSource> { elasticsearchSource1 }
                        },
                        new ElasticsearchName
                        {
                            FirstNames = "other name - test first name 3",
                            LastName = "other name - test last name 3",
                            FullName = "other name - test full name 3",
                            itemMeta = new ElasticsearchItemMeta { PrimaryValue = false },
                            DataSources = new List<ElasticsearchSource> { elasticsearchSource3 }
                        }
                    },
                    emails = new List<ElasticsearchEmail>
                    {
                        new ElasticsearchEmail {
                            Value = "email1@example.org",
                            itemMeta = new ElasticsearchItemMeta { PrimaryValue = true },
                            DataSources = new List<ElasticsearchSource> { elasticsearchSource1 }
                        },
                        new ElasticsearchEmail {
                            Value = "email3@example.org",
                            itemMeta = new ElasticsearchItemMeta { PrimaryValue = false },
                            DataSources = new List<ElasticsearchSource> { elasticsearchSource3 }
                        }
                    },
                    telephoneNumbers = new List<ElasticsearchTelephoneNumber>
                    {
                        new ElasticsearchTelephoneNumber {
                            Value = "123456789",
                            itemMeta = new ElasticsearchItemMeta { PrimaryValue = true },
                            DataSources = new List<ElasticsearchSource> { elasticsearchSource1 }
                        },
                        new ElasticsearchTelephoneNumber {
                            Value = "456789012",
                            itemMeta = new ElasticsearchItemMeta { PrimaryValue = false },
                            DataSources = new List<ElasticsearchSource> { elasticsearchSource3 }
                        }
                    },
                    webLinks = new List<ElasticsearchWebLink>
                    {
                        new ElasticsearchWebLink {
                            Url = "https://example1.org",
                            LinkLabel = "Example 1",
                            itemMeta = new ElasticsearchItemMeta { PrimaryValue = true },
                            DataSources = new List<ElasticsearchSource> { elasticsearchSource1 }
                        },
                        new ElasticsearchWebLink {
                            Url = "https://example3.org",
                            LinkLabel = "Example 3",
                            itemMeta = new ElasticsearchItemMeta { PrimaryValue = false },
                            DataSources = new List<ElasticsearchSource> { elasticsearchSource3 }
                        }
                    },
                    keywords = new List<ElasticsearchKeyword>
                    {
                        new ElasticsearchKeyword {
                            Value = "keyword1",
                            itemMeta = new ElasticsearchItemMeta { PrimaryValue = true },
                            DataSources = new List<ElasticsearchSource> { elasticsearchSource1 }
                        },
                        new ElasticsearchKeyword {
                            Value = "keyword3",
                            itemMeta = new ElasticsearchItemMeta { PrimaryValue = false },
                            DataSources = new List<ElasticsearchSource> { elasticsearchSource3 }
                        }
                    },
                    fieldOfSciences = new List<ElasticsearchFieldOfScience>
                    {
                        new ElasticsearchFieldOfScience {
                            NameFi = "fi field of science 1",
                            NameEn = "en field of science 1",
                            NameSv = "sv field of science 1",
                            itemMeta = new ElasticsearchItemMeta { PrimaryValue = true },
                            DataSources = new List<ElasticsearchSource> { elasticsearchSource1 }
                        },
                        new ElasticsearchFieldOfScience {
                            NameFi = "fi field of science 3",
                            NameEn = "en field of science 3",
                            NameSv = "sv field of science 3",
                            itemMeta = new ElasticsearchItemMeta { PrimaryValue = false },
                            DataSources = new List<ElasticsearchSource> { elasticsearchSource3 }
                        },
                    },
                    researcherDescriptions = new List<ElasticsearchResearcherDescription>
                    {
                        new ElasticsearchResearcherDescription
                        {
                            ResearchDescriptionFi = "fi description 1",
                            ResearchDescriptionEn = "en description 1",
                            ResearchDescriptionSv = "sv description 1",
                            itemMeta = new ElasticsearchItemMeta { PrimaryValue = true },
                            DataSources = new List<ElasticsearchSource> { elasticsearchSource1 }
                        },
                        new ElasticsearchResearcherDescription
                        {
                            ResearchDescriptionFi = "fi description 3",
                            ResearchDescriptionEn = "en description 3",
                            ResearchDescriptionSv = "sv description 3",
                            itemMeta = new ElasticsearchItemMeta { PrimaryValue = false },
                            DataSources = new List<ElasticsearchSource> { elasticsearchSource3 }
                        }
                    },
                    externalIdentifiers = new List<ElasticsearchExternalIdentifier>
                    {
                        new ElasticsearchExternalIdentifier {
                            PidContent = "external-id-1",
                            PidType = "type-1",
                            itemMeta = new ElasticsearchItemMeta { PrimaryValue = true },
                            DataSources = new List<ElasticsearchSource> { elasticsearchSource1 }
                        },
                        new ElasticsearchExternalIdentifier {
                            PidContent = "external-id-3",
                            PidType = "type-3",
                            itemMeta = new ElasticsearchItemMeta { PrimaryValue = false },
                            DataSources = new List<ElasticsearchSource> { elasticsearchSource3 }
                        }
                    }
                },
                activity = new ElasticsearchActivity
                {
                    affiliations = new List<ElasticsearchAffiliation>
                    {
                        new ElasticsearchAffiliation
                        {
                            OrganizationNameFi = "aff1 - orgNameFi",
                            OrganizationNameSv = "aff1 - orgNameSv",
                            OrganizationNameEn = "aff1 - orgNameEn",
                            DepartmentNameFi = "aff1 - deptNameFi",
                            DepartmentNameSv = "aff1 - deptNameSv",
                            DepartmentNameEn = "aff1 - deptNameEn",
                            PositionNameFi = "aff1 - posNameFi",
                            PositionNameSv = "aff1 - posNameSv",
                            PositionNameEn = "aff1 - posNameEn",
                            AffiliationTypeFi = "aff1 - affTypeFi",
                            AffiliationTypeEn = "aff1 - affTypeEn",
                            AffiliationTypeSv = "aff1 - affTypeSv",
                            StartDate = new ElasticsearchDate() {
                                Year = 2020,
                                Month = 1,
                                Day = 1
                            },
                            EndDate = new ElasticsearchDate() {
                                Year = 2021,
                                Month = 12,
                                Day = 31
                            },
                            sector = new List<ElasticsearchSector> {
                                new ElasticsearchSector {
                                    sectorId = "aff1 - sector1 - id",
                                    nameFiSector = "aff1 - sector1 - nameFi",
                                    nameEnSector = "aff1 - sector1 - nameEn",
                                    nameSvSector = "aff1 - sector1 - nameSv",
                                    organization = new List<ElasticsearchSectorOrganization> {
                                        new ElasticsearchSectorOrganization {
                                            organizationId = "aff1 - sector1 - org1 - orgid",
                                            OrganizationNameFi = "aff1 - sector 1 - org1 - orgNameFi",
                                            OrganizationNameEn = "aff1 - sector 1 - org1 - orgNameEn",
                                            OrganizationNameSv = "aff1 - sector 1 - org1 - orgNameSv"
                                        },
                                        new ElasticsearchSectorOrganization {
                                            organizationId = "aff1 - sector1 - org2 - orgid",
                                            OrganizationNameFi = "aff1 - sector 1 - org2 - orgNameFi",
                                            OrganizationNameEn = "aff1 - sector 1 - org2 - orgNameEn",
                                            OrganizationNameSv = "aff1 - sector 1 - org2 - orgNameSv"
                                        }
                                    }
                                },
                                new ElasticsearchSector {
                                    sectorId = "aff1 - sector2 - id",
                                    nameFiSector = "aff1 - sector2 - nameFi",
                                    nameEnSector = "aff1 - sector2 - nameEn",
                                    nameSvSector = "aff1 - sector2 - nameSv",
                                    organization = new List<ElasticsearchSectorOrganization> {
                                        new ElasticsearchSectorOrganization {
                                            organizationId = "aff1 - sector2 - org1 - orgid",
                                            OrganizationNameFi = "aff1 - sector2 - org1 - orgNameFi",
                                            OrganizationNameEn = "aff1 - sector2 - org1 - orgNameEn",
                                            OrganizationNameSv = "aff1 - sector2 - org1 - orgNameSv"
                                        },
                                        new ElasticsearchSectorOrganization {
                                            organizationId = "aff1 - sector2 - org2 - orgid",
                                            OrganizationNameFi = "aff1 - sector2 - org2 - orgNameFi",
                                            OrganizationNameEn = "aff1 - sector2 - org2 - orgNameEn",
                                            OrganizationNameSv = "aff1 - sector2 - org2 - orgNameSv"
                                        }
                                    }
                                }
                            },
                            itemMeta = new ElasticsearchItemMeta { PrimaryValue = true },
                            DataSources = new List<ElasticsearchSource> { elasticsearchSource1 }
                        },
                        new ElasticsearchAffiliation
                        {
                            OrganizationNameFi = "aff3 - orgNameFi",
                            OrganizationNameSv = "aff3 - orgNameSv",
                            OrganizationNameEn = "aff3 - orgNameEn",
                            DepartmentNameFi = "aff3 - deptNameFi",
                            DepartmentNameSv = "aff3 - deptNameSv",
                            DepartmentNameEn = "aff3 - deptNameEn",
                            PositionNameFi = "aff3 - posNameFi",
                            PositionNameSv = "aff3 - posNameSv",
                            PositionNameEn = "aff3 - posNameEn",
                            AffiliationTypeFi = "aff3 - affTypeFi",
                            AffiliationTypeEn = "aff3 - affTypeEn",
                            AffiliationTypeSv = "aff3 - affTypeSv",
                            StartDate = new ElasticsearchDate() {
                                Year = 2023,
                                Month = 1,
                                Day = 1
                            },
                            EndDate = new ElasticsearchDate() {
                                Year = 2024,
                                Month = 12,
                                Day = 31
                            },
                            sector = new List<ElasticsearchSector> {
                                new ElasticsearchSector {
                                    sectorId = "aff3 - sector1 - id",
                                    nameFiSector = "aff3 - sector1 - nameFi",
                                    nameEnSector = "aff3 - sector1 - nameEn",
                                    nameSvSector = "aff3 - sector1 - nameSv",
                                    organization = new List<ElasticsearchSectorOrganization> {
                                        new ElasticsearchSectorOrganization {
                                            organizationId = "aff3 - sector1 - org1 - orgid",
                                            OrganizationNameFi = "aff3 - sector1 - org1 - orgNameFi",
                                            OrganizationNameEn = "aff3 - sector1 - org1 - orgNameEn",
                                            OrganizationNameSv = "aff3 - sector1 - org1 - orgNameSv"
                                        },
                                        new ElasticsearchSectorOrganization {
                                            organizationId = "aff3 - sector1 - org2 - orgid",
                                            OrganizationNameFi = "aff3 - sector1 - org2 - orgNameFi",
                                            OrganizationNameEn = "aff3 - sector1 - org2 - orgNameEn",
                                            OrganizationNameSv = "aff3 - sector1 - org2 - orgNameSv"
                                        }
                                    }
                                },
                                new ElasticsearchSector {
                                    sectorId = "aff3 - sector2 - id",
                                    nameFiSector = "aff3 - sector2 - nameFi",
                                    nameEnSector = "aff3 - sector2 - nameEn",
                                    nameSvSector = "aff3 - sector2 - nameSv",
                                    organization = new List<ElasticsearchSectorOrganization> {
                                        new ElasticsearchSectorOrganization {
                                            organizationId = "aff3 - sector2 - org1 - orgid",
                                            OrganizationNameFi = "aff3 - sector2 - org1 - orgNameFi",
                                            OrganizationNameEn = "aff3 - sector2 - org1 - orgNameEn",
                                            OrganizationNameSv = "aff3 - sector2 - org1 - orgNameSv"
                                        },
                                        new ElasticsearchSectorOrganization {
                                            organizationId = "aff3 - sector2 - org2 - orgid",
                                            OrganizationNameFi = "aff3 - sector2 - org2 - orgNameFi",
                                            OrganizationNameEn = "aff3 - sector2 - org2 - orgNameEn",
                                            OrganizationNameSv = "aff3 - sector2 - org2 - orgNameSv"
                                        }
                                    }
                                }
                            },
                            itemMeta = new ElasticsearchItemMeta { PrimaryValue = false },
                            DataSources = new List<ElasticsearchSource> { elasticsearchSource3 }
                        }
                    },
                    educations = new List<ElasticsearchEducation>
                    {
                        new ElasticsearchEducation
                        {
                            NameFi = "ed1 - nameFi",
                            NameSv = "ed1 - nameSv",
                            NameEn = "ed1 - nameEn",
                            DegreeGrantingInstitutionName = "ed1 - degreeGrantingInstitutionName",
                            StartDate = new ElasticsearchDate { Year = 2015, Month = 1, Day = 1 },
                            EndDate = new ElasticsearchDate { Year = 2019, Month = 12, Day = 31 },
                            itemMeta = new ElasticsearchItemMeta { PrimaryValue = true },
                            DataSources = new List<ElasticsearchSource> { elasticsearchSource1 }
                        },
                        new ElasticsearchEducation
                        {
                            NameFi = "ed3 - nameFi",
                            NameSv = "ed3 - nameSv",
                            NameEn = "ed3 - nameEn",
                            DegreeGrantingInstitutionName = "ed3 - degreeGrantingInstitutionName",
                            StartDate = new ElasticsearchDate { Year = 2016, Month = 2, Day = 2 },
                            EndDate = new ElasticsearchDate { Year = 2020, Month = 11, Day = 30 },
                            itemMeta = new ElasticsearchItemMeta { PrimaryValue = false },
                            DataSources = new List<ElasticsearchSource> { elasticsearchSource3 }
                        }
                    },
                    publications = new List<ElasticsearchPublication>()
                    {
                        new ElasticsearchPublication
                        {
                            AuthorsText = "pub1 - authors text",
                            Doi = "pub1 - doi",
                            ConferenceName = "pub1 - conference name",
                            JournalName = "pub1 - journal name",
                            OpenAccess = 1,
                            ParentPublicationName = "pub1 - parent publication name",
                            PeerReviewed = new List<ElasticsearchPublicationPeerReviewed>
                            {
                                new ElasticsearchPublicationPeerReviewed
                                {
                                    Id = "pub1 - peerreview1 - id",
                                    NameFiPeerReviewed = "pub1 - peerreview1 - name fi",
                                    NameSvPeerReviewed = "pub1 - peerreview1 - name en",
                                    NameEnPeerReviewed = "pub1 - peerreview1 - name sv"
                                },
                                new ElasticsearchPublicationPeerReviewed
                                {
                                    Id = "pub1 - peerreview2 - id",
                                    NameFiPeerReviewed = "pub1 - peerreview2 - name fi",
                                    NameSvPeerReviewed = "pub1 - peerreview2 - name en",
                                    NameEnPeerReviewed = "pub1 - peerreview2 - name sv"
                                }
                            },
                            PublicationId = "pub1 - publication id",
                            PublicationName = "pub1 - publication name",
                            PublicationTypeCode = "pub1 - publication type code",
                            PublicationYear = 2021,
                            SelfArchivedAddress = "pub1 - self archived address",
                            SelfArchivedCode = "pub1 - self archived code",
                            itemMeta = new ElasticsearchItemMeta { PrimaryValue = true },
                            DataSources = new List<ElasticsearchSource> { elasticsearchSource1 }
                        },
                        new ElasticsearchPublication
                        {
                            AuthorsText = "pub3 - authors text",
                            Doi = "pub3 - doi",
                            ConferenceName = "pub3 - conference name",
                            JournalName = "pub3 - journal name",
                            OpenAccess = 0,
                            ParentPublicationName = "pub3 - parent publication name",
                            PeerReviewed = new List<ElasticsearchPublicationPeerReviewed>
                            {
                                new ElasticsearchPublicationPeerReviewed
                                {
                                    Id = "pub3 - peerreview1 - id",
                                    NameFiPeerReviewed = "pub3 - peerreview1 - name fi",
                                    NameSvPeerReviewed = "pub3 - peerreview1 - name en",
                                    NameEnPeerReviewed = "pub3 - peerreview1 - name sv"
                                },
                            },
                            PublicationId = "pub3 - publication id",
                            PublicationName = "pub3 - publication name",
                            PublicationTypeCode = "pub3 - publication type code",
                            PublicationYear = 2022,
                            SelfArchivedAddress = "pub3 - self archived address",
                            SelfArchivedCode = "pub3 - self archived code",
                            itemMeta = new ElasticsearchItemMeta { PrimaryValue = false },
                            DataSources = new List<ElasticsearchSource> { elasticsearchSource3 }
                        }
                    },
                    fundingDecisions = new List<ElasticsearchFundingDecision>
                    {
                        new ElasticsearchFundingDecision
                        {
                            ProjectId = 123,
                            ProjectAcronym = "funding1 - project acronym",
                            ProjectNameFi = "funding1 - project name fi",
                            ProjectNameSv = "funding1 - project name sv",
                            ProjectNameEn = "funding1 - project name en",
                            ProjectDescriptionFi = "funding1 - project description fi",
                            ProjectDescriptionSv = "funding1 - project description sv",
                            ProjectDescriptionEn = "funding1 - project description en",
                            FunderNameFi = "funding1 - funder name fi",
                            FunderNameSv = "funding1 - funder name sv",
                            FunderNameEn = "funding1 - funder name en",
                            FunderProjectNumber = "funding1 - funder project number",
                            TypeOfFundingNameFi = "funding1 - type of funding name fi",
                            TypeOfFundingNameSv = "funding1 - type of funding name sv",
                            TypeOfFundingNameEn = "funding1 - type of funding name en",
                            CallProgrammeNameFi = "funding1 - call programme name fi",
                            CallProgrammeNameSv = "funding1 - call programme name sv",
                            CallProgrammeNameEn = "funding1 - call programme name en",
                            FundingStartYear = 2021,
                            FundingEndYear = 2023,
                            AmountInEur = 100000,
                            AmountInFundingDecisionCurrency = 90000,
                            FundingDecisionCurrencyAbbreviation = "EUR",
                            Url = "https://example.org/funding1",
                            itemMeta = new ElasticsearchItemMeta { PrimaryValue = true },
                            DataSources = new List<ElasticsearchSource> { elasticsearchSource1 }
                        },
                        new ElasticsearchFundingDecision
                        {
                            ProjectId = 456,
                            ProjectAcronym = "funding3 - project acronym",
                            ProjectNameFi = "funding3 - project name fi",
                            ProjectNameSv = "funding3 - project name sv",
                            ProjectNameEn = "funding3 - project name en",
                            ProjectDescriptionFi = "funding3 - project description fi",
                            ProjectDescriptionSv = "funding3 - project description sv",
                            ProjectDescriptionEn = "funding3 - project description en",
                            FunderNameFi = "funding3 - funder name fi",
                            FunderNameSv = "funding3 - funder name sv",
                            FunderNameEn = "funding3 - funder name en",
                            FunderProjectNumber = "funding3 - funder project number",
                            TypeOfFundingNameFi = "funding3 - type of funding name fi",
                            TypeOfFundingNameSv = "funding3 - type of funding name sv",
                            TypeOfFundingNameEn = "funding3 - type of funding name en",
                            CallProgrammeNameFi = "funding3 - call programme name fi",
                            CallProgrammeNameSv = "funding3 - call programme name sv",
                            CallProgrammeNameEn = "funding3 - call programme name en",
                            FundingStartYear = 2022,
                            FundingEndYear = 2024,
                            AmountInEur = 200000,
                            AmountInFundingDecisionCurrency = 440000,
                            FundingDecisionCurrencyAbbreviation = "USD",
                            Url = "https://example.org/funding3",
                            itemMeta = new ElasticsearchItemMeta { PrimaryValue = true },
                            DataSources = new List<ElasticsearchSource> { elasticsearchSource3 }
                        }
                    },
                    researchDatasets = new List<ElasticsearchResearchDataset>
                    {
                        new ElasticsearchResearchDataset
                        {
                            AccessType = "dataset-access-type-1",
                            Actor = new List<ElasticsearchActor>
                            {
                                new ElasticsearchActor
                                {
                                    actorRole = 1,
                                    actorRoleNameFi = "Rooli 1",
                                    actorRoleNameSv = "Roll 1",
                                    actorRoleNameEn = "Role 1",
                                },
                                new ElasticsearchActor
                                {
                                    actorRole = 2,
                                    actorRoleNameFi = "Rooli 2",
                                    actorRoleNameSv = "Roll 2",
                                    actorRoleNameEn = "Role 2",
                                }
                            },
                            FairdataUrl = "https://fairdata.example.org/dataset1",
                            Identifier = "dataset-identifier-1",
                            NameFi = "dataset 1 - nameFi",
                            NameSv = "dataset 1 - nameSv",
                            NameEn = "dataset 1 - nameEn",
                            DescriptionFi = "dataset 1 - descriptionFi",
                            DescriptionSv = "dataset 1 - descriptionSv",
                            DescriptionEn = "dataset 1 - descriptionEn",
                            Url = "https://example.org/dataset1",
                            DatasetCreated = 2021,
                            PreferredIdentifiers = new List<ElasticsearchPreferredIdentifier>
                            {
                                new ElasticsearchPreferredIdentifier
                                {
                                    PidContent = "preferred-id-1",
                                    PidType = "preferred-type-1"
                                },
                                new ElasticsearchPreferredIdentifier
                                {
                                    PidContent = "preferred-id-2",
                                    PidType = "preferred-type-2"
                                }
                            },
                            itemMeta = new ElasticsearchItemMeta { PrimaryValue = true },
                            DataSources = new List<ElasticsearchSource> { elasticsearchSource1 }
                        },
                        new ElasticsearchResearchDataset
                        {
                            AccessType = "dataset-access-type-3",
                            Actor = new List<ElasticsearchActor>
                            {
                                new ElasticsearchActor
                                {
                                    actorRole = 3,
                                    actorRoleNameFi = "Rooli 3",
                                    actorRoleNameSv = "Roll 3",
                                    actorRoleNameEn = "Role 3",
                                },
                                new ElasticsearchActor
                                {
                                    actorRole = 4,
                                    actorRoleNameFi = "Rooli 4",
                                    actorRoleNameSv = "Roll 4",
                                    actorRoleNameEn = "Role 4",
                                }
                            },
                            FairdataUrl = "https://fairdata.example.org/dataset3",
                            Identifier = "dataset-identifier-3",
                            NameFi = "dataset 3 - nameFi",
                            NameSv = "dataset 3 - nameSv",
                            NameEn = "dataset 3 - nameEn",
                            DescriptionFi = "dataset 3 - descriptionFi",
                            DescriptionSv = "dataset 3 - descriptionSv",
                            DescriptionEn = "dataset 3 - descriptionEn",
                            Url = "https://example.org/dataset3",
                            DatasetCreated = 2022,
                            PreferredIdentifiers = new List<ElasticsearchPreferredIdentifier>
                            {
                                new ElasticsearchPreferredIdentifier
                                {
                                    PidContent = "preferred-id-3",
                                    PidType = "preferred-type-3"
                                },
                                new ElasticsearchPreferredIdentifier
                                {
                                    PidContent = "preferred-id-4",
                                    PidType = "preferred-type-4"
                                }
                            },
                            itemMeta = new ElasticsearchItemMeta { PrimaryValue = false },
                            DataSources = new List<ElasticsearchSource> { elasticsearchSource3 }
                        }
                    },
                    activitiesAndRewards = new List<ElasticsearchActivityAndReward>
                    {
                        new ElasticsearchActivityAndReward
                        {
                            NameFi = "activityAndReward 1 - nameFi",
                            NameEn = "activityAndReward 1 - nameEn",
                            NameSv = "activityAndReward 1 - nameSv",
                            DescriptionFi = "activityAndReward 1 - descriptionFi",
                            DescriptionEn = "activityAndReward 1 - descriptionEn",
                            DescriptionSv = "activityAndReward 1 - descriptionSv",
                            InternationalCollaboration = null,
                            StartDate = new ElasticsearchDate
                            {
                                Year = 2020,
                                Month = 1,
                                Day = 1
                            },
                            EndDate = new ElasticsearchDate
                            {
                                Year = 2021,
                                Month = 12,
                                Day = 31
                            },
                            ActivityTypeCode = "activityAndReward 1 - type-code",
                            ActivityTypeNameFi = "activityAndReward 1 - type-name-fi",
                            ActivityTypeNameEn = "activityAndReward 1 - type-name-en",
                            ActivityTypeNameSv = "activityAndReward 1 - type-name-sv",
                            RoleCode = "activityAndReward 1 - role-code",
                            RoleNameFi = "activityAndReward 1 - role-name-fi",
                            RoleNameEn = "activityAndReward 1 - role-name-en",
                            RoleNameSv = "activityAndReward 1 - role-name-sv",
                            OrganizationNameFi = "activityAndReward 1 - organization-name-fi",
                            OrganizationNameSv = "activityAndReward 1 - organization-name-sv",
                            OrganizationNameEn = "activityAndReward 1 - organization-name-en",
                            DepartmentNameFi = "activityAndReward 1 - department-name-fi",
                            DepartmentNameSv = "activityAndReward 1 - department-name-sv",
                            DepartmentNameEn = "activityAndReward 1 - department-name-en",
                            Url = "activityAndReward 1 - url",
                            sector = new List<ElasticsearchSector> {
                                new ElasticsearchSector
                                {
                                    sectorId = "activityAndReward 1 - sector1 - id",
                                    nameFiSector = "activityAndReward 1 - sector1 - nameFi",
                                    nameEnSector = "activityAndReward 1 - sector1 - nameEn",
                                    nameSvSector = "activityAndReward 1 - sector1 - nameSv",
                                    organization = new List<ElasticsearchSectorOrganization> {
                                        new ElasticsearchSectorOrganization {
                                            organizationId = "activityAndReward 1 - sector1 - org1 - orgid",
                                            OrganizationNameFi = "activityAndReward 1 - sector 1 - org1 - orgNameFi",
                                            OrganizationNameEn = "activityAndReward 1 - sector 1 - org1 - orgNameEn",
                                            OrganizationNameSv = "activityAndReward 1 - sector 1 - org1 - orgNameSv"
                                        }
                                    }
                                }
                            },
                            itemMeta = new ElasticsearchItemMeta { PrimaryValue = true },
                            DataSources = new List<ElasticsearchSource> { elasticsearchSource1 }
                        },
                        new ElasticsearchActivityAndReward
                        {
                            NameFi = "activityAndReward 3 - nameFi",
                            NameEn = "activityAndReward 3 - nameEn",
                            NameSv = "activityAndReward 3 - nameSv",
                            DescriptionFi = "activityAndReward 3 - descriptionFi",
                            DescriptionEn = "activityAndReward 3 - descriptionEn",
                            DescriptionSv = "activityAndReward 3 - descriptionSv",
                            InternationalCollaboration = null,
                            StartDate = new ElasticsearchDate
                            {
                                Year = 2022,
                                Month = 1,
                                Day = 1
                            },
                            EndDate = new ElasticsearchDate
                            {
                                Year = 2022,
                                Month = 12,
                                Day = 31
                            },
                            ActivityTypeCode = "activityAndReward 3 - type-code",
                            ActivityTypeNameFi = "activityAndReward 3 - type-name-fi",
                            ActivityTypeNameEn = "activityAndReward 3 - type-name-en",
                            ActivityTypeNameSv = "activityAndReward 3 - type-name-sv",
                            RoleCode = "activityAndReward 3 - role-code",
                            RoleNameFi = "activityAndReward 3 - role-name-fi",
                            RoleNameEn = "activityAndReward 3 - role-name-en",
                            RoleNameSv = "activityAndReward 3 - role-name-sv",
                            OrganizationNameFi = "activityAndReward 3 - organization-name-fi",
                            OrganizationNameSv = "activityAndReward 3 - organization-name-sv",
                            OrganizationNameEn = "activityAndReward 3 - organization-name-en",
                            DepartmentNameFi = "activityAndReward 3 - department-name-fi",
                            DepartmentNameSv = "activityAndReward 3 - department-name-sv",
                            DepartmentNameEn = "activityAndReward 3 - department-name-en",
                            Url = "activityAndReward 3 - url",
                            sector = new List<ElasticsearchSector> {
                                new ElasticsearchSector
                                {
                                    sectorId = "activityAndReward 3 - sector1 - id",
                                    nameFiSector = "activityAndReward 3 - sector1 - nameFi",
                                    nameEnSector = "activityAndReward 3 - sector1 - nameEn",
                                    nameSvSector = "activityAndReward 3 - sector1 - nameSv",
                                    organization = new List<ElasticsearchSectorOrganization> {
                                        new ElasticsearchSectorOrganization {
                                            organizationId = "activityAndReward 3 - sector1 - org1 - orgid",
                                            OrganizationNameFi = "activityAndReward 3 - sector 1 - org1 - orgNameFi",
                                            OrganizationNameEn = "activityAndReward 3 - sector 1 - org1 - orgNameEn",
                                            OrganizationNameSv = "activityAndReward 3 - sector 1 - org1 - orgNameSv"
                                        }
                                    }
                                }
                            },
                            itemMeta = new ElasticsearchItemMeta { PrimaryValue = false },
                            DataSources = new List<ElasticsearchSource> { elasticsearchSource3 }
                        }
                    }
                },
                settings = new ElasticsearchProfileSettings
                {
                    HighlightOpeness = true
                },
                cooperation = new List<ElasticsearchCooperation>
                {
                    new ElasticsearchCooperation
                    {
                        Id = 1,
                        NameFi = "cooperation 1 - nameFi",
                        NameEn = "cooperation 1 - nameEn",
                        NameSv = "cooperation 1 - nameSv",
                        Order = 1
                    },
                    // Id 2 should not be present, only those with Selected=true are mapped
                    new ElasticsearchCooperation
                    {
                        Id = 3,
                        NameFi = "cooperation 3 - nameFi",
                        NameEn = "cooperation 3 - nameEn",
                        NameSv = "cooperation 3 - nameSv",
                        Order = 3
                    }
                },
                uniqueDataSources = new List<ElasticsearchSource>
                {
                    new ElasticsearchSource
                    {
                        RegisteredDataSource = "source-id-1",
                        Organization = new Organization
                        {
                            NameFi = "source-name-1-fi",
                            NameEn = "source-name-1-en",
                            NameSv = "source-name-1-sv",
                            SectorId = "source-sector-id-1"
                        }
                    },
                    new ElasticsearchSource
                    {
                        RegisteredDataSource = "source-id-2",
                        Organization = new Organization
                        {
                            NameFi = "source-name-2-fi",
                            NameEn = "source-name-2-en",
                            NameSv = "source-name-2-sv",
                            SectorId = "source-sector-id-2"
                        }
                    }
                },
                updated = null
            };
        }

        [Fact(DisplayName = "ProfileEditorDataResponse is correctly mapped to ElasticsearchPerson")]
        public void getCorrectDataSourceOrganizationName_ORCID()
        {
            // Arrange
            ProfileEditorDataResponse sourceObject = GetProfileEditorDataResponse();
            ElasticsearchPerson expectedObject = GetElasticsearchPerson();

            // Act
            ElasticsearchPerson actualObject = ElasticsearchMapper.MapToElasticsearchPerson(src: sourceObject, orcidId: orcidId);

            // Assert
            Assert.NotNull(actualObject);
            // Assert id
            Assert.Equal(orcidId, actualObject.id);
            // Assert personal.names
            Assert.Equal(expectedObject.personal.names[0].FirstNames, actualObject.personal.names[0].FirstNames);
            Assert.Equal(expectedObject.personal.names[0].LastName, actualObject.personal.names[0].LastName);
            Assert.Equal(expectedObject.personal.names[0].FullName, actualObject.personal.names[0].FullName);
            Assert.Equal(expectedObject.personal.names[0].itemMeta.PrimaryValue, actualObject.personal.names[0].itemMeta.PrimaryValue);
            Assert.Equal(expectedObject.personal.names[0].DataSources[0].RegisteredDataSource, actualObject.personal.names[0].DataSources[0].RegisteredDataSource);
            Assert.Equal(expectedObject.personal.names[0].DataSources[0].Organization.NameEn, actualObject.personal.names[0].DataSources[0].Organization.NameEn);
            Assert.Equal(expectedObject.personal.names[0].DataSources[0].Organization.NameFi, actualObject.personal.names[0].DataSources[0].Organization.NameFi);
            Assert.Equal(expectedObject.personal.names[0].DataSources[0].Organization.NameSv, actualObject.personal.names[0].DataSources[0].Organization.NameSv);
            Assert.Equal(expectedObject.personal.names[0].DataSources[0].Organization.SectorId, actualObject.personal.names[0].DataSources[0].Organization.SectorId);
            Assert.Equal(expectedObject.personal.names[0].DataSources[1].RegisteredDataSource, actualObject.personal.names[0].DataSources[1].RegisteredDataSource);
            Assert.Equal(expectedObject.personal.names[0].DataSources[1].Organization.NameEn, actualObject.personal.names[0].DataSources[1].Organization.NameEn);
            Assert.Equal(expectedObject.personal.names[0].DataSources[1].Organization.NameFi, actualObject.personal.names[0].DataSources[1].Organization.NameFi);
            Assert.Equal(expectedObject.personal.names[0].DataSources[1].Organization.NameSv, actualObject.personal.names[0].DataSources[1].Organization.NameSv);
            Assert.Equal(expectedObject.personal.names[0].DataSources[1].Organization.SectorId, actualObject.personal.names[0].DataSources[1].Organization.SectorId);
            Assert.Equal(expectedObject.personal.names[1].LastName, actualObject.personal.names[1].LastName);
            Assert.Equal(expectedObject.personal.names[1].FullName, actualObject.personal.names[1].FullName);
            Assert.Equal(expectedObject.personal.names[1].FirstNames, actualObject.personal.names[1].FirstNames);
            Assert.Equal(expectedObject.personal.names[1].itemMeta.PrimaryValue, actualObject.personal.names[1].itemMeta.PrimaryValue);
            Assert.Equal(expectedObject.personal.names[1].DataSources[0].RegisteredDataSource, actualObject.personal.names[1].DataSources[0].RegisteredDataSource);
            Assert.Equal(expectedObject.personal.names[1].DataSources[0].Organization.NameEn, actualObject.personal.names[1].DataSources[0].Organization.NameEn);
            Assert.Equal(expectedObject.personal.names[1].DataSources[0].Organization.NameFi, actualObject.personal.names[1].DataSources[0].Organization.NameFi);
            Assert.Equal(expectedObject.personal.names[1].DataSources[0].Organization.NameSv, actualObject.personal.names[1].DataSources[0].Organization.NameSv);
            Assert.Equal(expectedObject.personal.names[1].DataSources[0].Organization.SectorId, actualObject.personal.names[1].DataSources[0].Organization.SectorId);
            // Assert personal.otherNames
            Assert.Equal(expectedObject.personal.otherNames[0].FirstNames, actualObject.personal.otherNames[0].FirstNames);
            Assert.Equal(expectedObject.personal.otherNames[0].LastName, actualObject.personal.otherNames[0].LastName);
            Assert.Equal(expectedObject.personal.otherNames[0].FullName, actualObject.personal.otherNames[0].FullName);
            Assert.Equal(expectedObject.personal.otherNames[0].itemMeta.PrimaryValue, actualObject.personal.otherNames[0].itemMeta.PrimaryValue);
            Assert.Equal(expectedObject.personal.otherNames[0].DataSources[0].RegisteredDataSource, actualObject.personal.otherNames[0].DataSources[0].RegisteredDataSource);
            Assert.Equal(expectedObject.personal.otherNames[0].DataSources[0].Organization.NameEn, actualObject.personal.otherNames[0].DataSources[0].Organization.NameEn);
            Assert.Equal(expectedObject.personal.otherNames[0].DataSources[0].Organization.NameFi, actualObject.personal.otherNames[0].DataSources[0].Organization.NameFi);
            Assert.Equal(expectedObject.personal.otherNames[0].DataSources[0].Organization.NameSv, actualObject.personal.otherNames[0].DataSources[0].Organization.NameSv);
            Assert.Equal(expectedObject.personal.otherNames[0].DataSources[0].Organization.SectorId, actualObject.personal.otherNames[0].DataSources[0].Organization.SectorId);
            Assert.Equal(expectedObject.personal.otherNames[1].LastName, actualObject.personal.otherNames[1].LastName);
            Assert.Equal(expectedObject.personal.otherNames[1].FullName, actualObject.personal.otherNames[1].FullName);
            Assert.Equal(expectedObject.personal.otherNames[1].FirstNames, actualObject.personal.otherNames[1].FirstNames);
            Assert.Equal(expectedObject.personal.otherNames[1].itemMeta.PrimaryValue, actualObject.personal.otherNames[1].itemMeta.PrimaryValue);
            Assert.Equal(expectedObject.personal.otherNames[1].DataSources[0].RegisteredDataSource, actualObject.personal.otherNames[1].DataSources[0].RegisteredDataSource);
            Assert.Equal(expectedObject.personal.otherNames[1].DataSources[0].Organization.NameEn, actualObject.personal.otherNames[1].DataSources[0].Organization.NameEn);
            Assert.Equal(expectedObject.personal.otherNames[1].DataSources[0].Organization.NameFi, actualObject.personal.otherNames[1].DataSources[0].Organization.NameFi);
            Assert.Equal(expectedObject.personal.otherNames[1].DataSources[0].Organization.NameSv, actualObject.personal.otherNames[1].DataSources[0].Organization.NameSv);
            Assert.Equal(expectedObject.personal.otherNames[1].DataSources[0].Organization.SectorId, actualObject.personal.otherNames[1].DataSources[0].Organization.SectorId);
            // Assert personal.emails
            Assert.Equal(expectedObject.personal.emails[0].Value, actualObject.personal.emails[0].Value);
            Assert.Equal(expectedObject.personal.emails[0].itemMeta.PrimaryValue, actualObject.personal.emails[0].itemMeta.PrimaryValue);
            Assert.Equal(expectedObject.personal.emails[0].DataSources[0].RegisteredDataSource, actualObject.personal.emails[0].DataSources[0].RegisteredDataSource);
            Assert.Equal(expectedObject.personal.emails[0].DataSources[0].Organization.NameEn, actualObject.personal.emails[0].DataSources[0].Organization.NameEn);
            Assert.Equal(expectedObject.personal.emails[0].DataSources[0].Organization.NameFi, actualObject.personal.emails[0].DataSources[0].Organization.NameFi);
            Assert.Equal(expectedObject.personal.emails[0].DataSources[0].Organization.NameSv, actualObject.personal.emails[0].DataSources[0].Organization.NameSv);
            Assert.Equal(expectedObject.personal.emails[0].DataSources[0].Organization.SectorId, actualObject.personal.emails[0].DataSources[0].Organization.SectorId);
            Assert.Equal(expectedObject.personal.emails[1].Value, actualObject.personal.emails[1].Value);
            Assert.Equal(expectedObject.personal.emails[1].itemMeta.PrimaryValue, actualObject.personal.emails[1].itemMeta.PrimaryValue);
            Assert.Equal(expectedObject.personal.emails[1].DataSources[0].RegisteredDataSource, actualObject.personal.emails[1].DataSources[0].RegisteredDataSource);
            Assert.Equal(expectedObject.personal.emails[1].DataSources[0].Organization.NameEn, actualObject.personal.emails[1].DataSources[0].Organization.NameEn);
            Assert.Equal(expectedObject.personal.emails[1].DataSources[0].Organization.NameFi, actualObject.personal.emails[1].DataSources[0].Organization.NameFi);
            Assert.Equal(expectedObject.personal.emails[1].DataSources[0].Organization.NameSv, actualObject.personal.emails[1].DataSources[0].Organization.NameSv);
            Assert.Equal(expectedObject.personal.emails[1].DataSources[0].Organization.SectorId, actualObject.personal.emails[1].DataSources[0].Organization.SectorId);
            // Assert personal.telephoneNumbers
            Assert.Equal(expectedObject.personal.telephoneNumbers[0].Value, actualObject.personal.telephoneNumbers[0].Value);
            Assert.Equal(expectedObject.personal.telephoneNumbers[0].itemMeta.PrimaryValue, actualObject.personal.telephoneNumbers[0].itemMeta.PrimaryValue);
            Assert.Equal(expectedObject.personal.telephoneNumbers[0].DataSources[0].RegisteredDataSource, actualObject.personal.telephoneNumbers[0].DataSources[0].RegisteredDataSource);
            Assert.Equal(expectedObject.personal.telephoneNumbers[0].DataSources[0].Organization.NameEn, actualObject.personal.telephoneNumbers[0].DataSources[0].Organization.NameEn);
            Assert.Equal(expectedObject.personal.telephoneNumbers[0].DataSources[0].Organization.NameFi, actualObject.personal.telephoneNumbers[0].DataSources[0].Organization.NameFi);
            Assert.Equal(expectedObject.personal.telephoneNumbers[0].DataSources[0].Organization.NameSv, actualObject.personal.telephoneNumbers[0].DataSources[0].Organization.NameSv);
            Assert.Equal(expectedObject.personal.telephoneNumbers[0].DataSources[0].Organization.SectorId, actualObject.personal.telephoneNumbers[0].DataSources[0].Organization.SectorId);
            Assert.Equal(expectedObject.personal.telephoneNumbers[1].Value, actualObject.personal.telephoneNumbers[1].Value);
            Assert.Equal(expectedObject.personal.telephoneNumbers[1].itemMeta.PrimaryValue, actualObject.personal.telephoneNumbers[1].itemMeta.PrimaryValue);
            Assert.Equal(expectedObject.personal.telephoneNumbers[1].DataSources[0].RegisteredDataSource, actualObject.personal.telephoneNumbers[1].DataSources[0].RegisteredDataSource);
            Assert.Equal(expectedObject.personal.telephoneNumbers[1].DataSources[0].Organization.NameEn, actualObject.personal.telephoneNumbers[1].DataSources[0].Organization.NameEn);
            Assert.Equal(expectedObject.personal.telephoneNumbers[1].DataSources[0].Organization.NameFi, actualObject.personal.telephoneNumbers[1].DataSources[0].Organization.NameFi);
            Assert.Equal(expectedObject.personal.telephoneNumbers[1].DataSources[0].Organization.NameSv, actualObject.personal.telephoneNumbers[1].DataSources[0].Organization.NameSv);
            Assert.Equal(expectedObject.personal.telephoneNumbers[1].DataSources[0].Organization.SectorId, actualObject.personal.telephoneNumbers[1].DataSources[0].Organization.SectorId);
            // Assert personal.webLinks
            Assert.Equal(expectedObject.personal.webLinks[0].Url, actualObject.personal.webLinks[0].Url);
            Assert.Equal(expectedObject.personal.webLinks[0].LinkLabel, actualObject.personal.webLinks[0].LinkLabel);
            Assert.Equal(expectedObject.personal.webLinks[0].itemMeta.PrimaryValue, actualObject.personal.webLinks[0].itemMeta.PrimaryValue);
            Assert.Equal(expectedObject.personal.webLinks[0].DataSources[0].RegisteredDataSource, actualObject.personal.webLinks[0].DataSources[0].RegisteredDataSource);
            Assert.Equal(expectedObject.personal.webLinks[0].DataSources[0].Organization.NameEn, actualObject.personal.webLinks[0].DataSources[0].Organization.NameEn);
            Assert.Equal(expectedObject.personal.webLinks[0].DataSources[0].Organization.NameFi, actualObject.personal.webLinks[0].DataSources[0].Organization.NameFi);
            Assert.Equal(expectedObject.personal.webLinks[0].DataSources[0].Organization.NameSv, actualObject.personal.webLinks[0].DataSources[0].Organization.NameSv);
            Assert.Equal(expectedObject.personal.webLinks[0].DataSources[0].Organization.SectorId, actualObject.personal.webLinks[0].DataSources[0].Organization.SectorId);
            Assert.Equal(expectedObject.personal.webLinks[1].Url, actualObject.personal.webLinks[1].Url);
            Assert.Equal(expectedObject.personal.webLinks[1].LinkLabel, actualObject.personal.webLinks[1].LinkLabel);
            Assert.Equal(expectedObject.personal.webLinks[1].itemMeta.PrimaryValue, actualObject.personal.webLinks[1].itemMeta.PrimaryValue);
            Assert.Equal(expectedObject.personal.webLinks[1].DataSources[0].RegisteredDataSource, actualObject.personal.webLinks[1].DataSources[0].RegisteredDataSource);
            Assert.Equal(expectedObject.personal.webLinks[1].DataSources[0].Organization.NameEn, actualObject.personal.webLinks[1].DataSources[0].Organization.NameEn);
            Assert.Equal(expectedObject.personal.webLinks[1].DataSources[0].Organization.NameFi, actualObject.personal.webLinks[1].DataSources[0].Organization.NameFi);
            Assert.Equal(expectedObject.personal.webLinks[1].DataSources[0].Organization.NameSv, actualObject.personal.webLinks[1].DataSources[0].Organization.NameSv);
            Assert.Equal(expectedObject.personal.webLinks[1].DataSources[0].Organization.SectorId, actualObject.personal.webLinks[1].DataSources[0].Organization.SectorId);
            // Assert personal.keywords
            Assert.Equal(expectedObject.personal.keywords[0].Value, actualObject.personal.keywords[0].Value);
            Assert.Equal(expectedObject.personal.keywords[0].itemMeta.PrimaryValue, actualObject.personal.keywords[0].itemMeta.PrimaryValue);
            Assert.Equal(expectedObject.personal.keywords[0].DataSources[0].RegisteredDataSource, actualObject.personal.keywords[0].DataSources[0].RegisteredDataSource);
            Assert.Equal(expectedObject.personal.keywords[0].DataSources[0].Organization.NameEn, actualObject.personal.keywords[0].DataSources[0].Organization.NameEn);
            Assert.Equal(expectedObject.personal.keywords[0].DataSources[0].Organization.NameFi, actualObject.personal.keywords[0].DataSources[0].Organization.NameFi);
            Assert.Equal(expectedObject.personal.keywords[0].DataSources[0].Organization.NameSv, actualObject.personal.keywords[0].DataSources[0].Organization.NameSv);
            Assert.Equal(expectedObject.personal.keywords[0].DataSources[0].Organization.SectorId, actualObject.personal.keywords[0].DataSources[0].Organization.SectorId);
            Assert.Equal(expectedObject.personal.keywords[1].Value, actualObject.personal.keywords[1].Value);
            Assert.Equal(expectedObject.personal.keywords[1].itemMeta.PrimaryValue, actualObject.personal.keywords[1].itemMeta.PrimaryValue);
            Assert.Equal(expectedObject.personal.keywords[1].DataSources[0].RegisteredDataSource, actualObject.personal.keywords[1].DataSources[0].RegisteredDataSource);
            Assert.Equal(expectedObject.personal.keywords[1].DataSources[0].Organization.NameEn, actualObject.personal.keywords[1].DataSources[0].Organization.NameEn);
            Assert.Equal(expectedObject.personal.keywords[1].DataSources[0].Organization.NameFi, actualObject.personal.keywords[1].DataSources[0].Organization.NameFi);
            Assert.Equal(expectedObject.personal.keywords[1].DataSources[0].Organization.NameSv, actualObject.personal.keywords[1].DataSources[0].Organization.NameSv);
            Assert.Equal(expectedObject.personal.keywords[1].DataSources[0].Organization.SectorId, actualObject.personal.keywords[1].DataSources[0].Organization.SectorId);
            // Assert personal.fieldOfSciences
            Assert.Equal(expectedObject.personal.fieldOfSciences[0].NameFi, actualObject.personal.fieldOfSciences[0].NameFi);
            Assert.Equal(expectedObject.personal.fieldOfSciences[0].NameEn, actualObject.personal.fieldOfSciences[0].NameEn);
            Assert.Equal(expectedObject.personal.fieldOfSciences[0].NameSv, actualObject.personal.fieldOfSciences[0].NameSv);
            Assert.Equal(expectedObject.personal.fieldOfSciences[0].itemMeta.PrimaryValue, actualObject.personal.fieldOfSciences[0].itemMeta.PrimaryValue);
            Assert.Equal(expectedObject.personal.fieldOfSciences[0].DataSources[0].RegisteredDataSource, actualObject.personal.fieldOfSciences[0].DataSources[0].RegisteredDataSource);
            Assert.Equal(expectedObject.personal.fieldOfSciences[0].DataSources[0].Organization.NameEn, actualObject.personal.fieldOfSciences[0].DataSources[0].Organization.NameEn);
            Assert.Equal(expectedObject.personal.fieldOfSciences[0].DataSources[0].Organization.NameFi, actualObject.personal.fieldOfSciences[0].DataSources[0].Organization.NameFi);
            Assert.Equal(expectedObject.personal.fieldOfSciences[0].DataSources[0].Organization.NameSv, actualObject.personal.fieldOfSciences[0].DataSources[0].Organization.NameSv);
            Assert.Equal(expectedObject.personal.fieldOfSciences[0].DataSources[0].Organization.SectorId, actualObject.personal.fieldOfSciences[0].DataSources[0].Organization.SectorId);
            Assert.Equal(expectedObject.personal.fieldOfSciences[1].NameFi, actualObject.personal.fieldOfSciences[1].NameFi);
            Assert.Equal(expectedObject.personal.fieldOfSciences[1].NameEn, actualObject.personal.fieldOfSciences[1].NameEn);
            Assert.Equal(expectedObject.personal.fieldOfSciences[1].NameSv, actualObject.personal.fieldOfSciences[1].NameSv);
            Assert.Equal(expectedObject.personal.fieldOfSciences[1].itemMeta.PrimaryValue, actualObject.personal.fieldOfSciences[1].itemMeta.PrimaryValue);
            Assert.Equal(expectedObject.personal.fieldOfSciences[1].DataSources[0].RegisteredDataSource, actualObject.personal.fieldOfSciences[1].DataSources[0].RegisteredDataSource);
            Assert.Equal(expectedObject.personal.fieldOfSciences[1].DataSources[0].Organization.NameEn, actualObject.personal.fieldOfSciences[1].DataSources[0].Organization.NameEn);
            Assert.Equal(expectedObject.personal.fieldOfSciences[1].DataSources[0].Organization.NameFi, actualObject.personal.fieldOfSciences[1].DataSources[0].Organization.NameFi);
            Assert.Equal(expectedObject.personal.fieldOfSciences[1].DataSources[0].Organization.NameSv, actualObject.personal.fieldOfSciences[1].DataSources[0].Organization.NameSv);
            Assert.Equal(expectedObject.personal.fieldOfSciences[1].DataSources[0].Organization.SectorId, actualObject.personal.fieldOfSciences[1].DataSources[0].Organization.SectorId);
            // Assert personal.researcherDescriptions
            Assert.Equal(expectedObject.personal.researcherDescriptions[0].ResearchDescriptionFi, actualObject.personal.researcherDescriptions[0].ResearchDescriptionFi);
            Assert.Equal(expectedObject.personal.researcherDescriptions[0].ResearchDescriptionEn, actualObject.personal.researcherDescriptions[0].ResearchDescriptionEn);
            Assert.Equal(expectedObject.personal.researcherDescriptions[0].ResearchDescriptionSv, actualObject.personal.researcherDescriptions[0].ResearchDescriptionSv);
            Assert.Equal(expectedObject.personal.researcherDescriptions[0].itemMeta.PrimaryValue, actualObject.personal.researcherDescriptions[0].itemMeta.PrimaryValue);
            Assert.Equal(expectedObject.personal.researcherDescriptions[0].DataSources[0].RegisteredDataSource, actualObject.personal.researcherDescriptions[0].DataSources[0].RegisteredDataSource);
            Assert.Equal(expectedObject.personal.researcherDescriptions[0].DataSources[0].Organization.NameEn, actualObject.personal.researcherDescriptions[0].DataSources[0].Organization.NameEn);
            Assert.Equal(expectedObject.personal.researcherDescriptions[0].DataSources[0].Organization.NameFi, actualObject.personal.researcherDescriptions[0].DataSources[0].Organization.NameFi);
            Assert.Equal(expectedObject.personal.researcherDescriptions[0].DataSources[0].Organization.NameSv, actualObject.personal.researcherDescriptions[0].DataSources[0].Organization.NameSv);
            Assert.Equal(expectedObject.personal.researcherDescriptions[0].DataSources[0].Organization.SectorId, actualObject.personal.researcherDescriptions[0].DataSources[0].Organization.SectorId);
            Assert.Equal(expectedObject.personal.researcherDescriptions[1].ResearchDescriptionFi, actualObject.personal.researcherDescriptions[1].ResearchDescriptionFi);
            Assert.Equal(expectedObject.personal.researcherDescriptions[1].ResearchDescriptionEn, actualObject.personal.researcherDescriptions[1].ResearchDescriptionEn);
            Assert.Equal(expectedObject.personal.researcherDescriptions[1].ResearchDescriptionSv, actualObject.personal.researcherDescriptions[1].ResearchDescriptionSv);
            Assert.Equal(expectedObject.personal.researcherDescriptions[1].itemMeta.PrimaryValue, actualObject.personal.researcherDescriptions[1].itemMeta.PrimaryValue);
            Assert.Equal(expectedObject.personal.researcherDescriptions[1].DataSources[0].RegisteredDataSource, actualObject.personal.researcherDescriptions[1].DataSources[0].RegisteredDataSource);
            Assert.Equal(expectedObject.personal.researcherDescriptions[1].DataSources[0].Organization.NameEn, actualObject.personal.researcherDescriptions[1].DataSources[0].Organization.NameEn);
            Assert.Equal(expectedObject.personal.researcherDescriptions[1].DataSources[0].Organization.NameFi, actualObject.personal.researcherDescriptions[1].DataSources[0].Organization.NameFi);
            Assert.Equal(expectedObject.personal.researcherDescriptions[1].DataSources[0].Organization.NameSv, actualObject.personal.researcherDescriptions[1].DataSources[0].Organization.NameSv);
            Assert.Equal(expectedObject.personal.researcherDescriptions[1].DataSources[0].Organization.SectorId, actualObject.personal.researcherDescriptions[1].DataSources[0].Organization.SectorId);
            // Assert personal.externalIdentifiers
            Assert.Equal(expectedObject.personal.externalIdentifiers[0].PidContent, actualObject.personal.externalIdentifiers[0].PidContent);
            Assert.Equal(expectedObject.personal.externalIdentifiers[0].PidType, actualObject.personal.externalIdentifiers[0].PidType);
            Assert.Equal(expectedObject.personal.externalIdentifiers[0].itemMeta.PrimaryValue, actualObject.personal.externalIdentifiers[0].itemMeta.PrimaryValue);
            Assert.Equal(expectedObject.personal.externalIdentifiers[0].DataSources[0].RegisteredDataSource, actualObject.personal.externalIdentifiers[0].DataSources[0].RegisteredDataSource);
            Assert.Equal(expectedObject.personal.externalIdentifiers[0].DataSources[0].Organization.NameEn, actualObject.personal.externalIdentifiers[0].DataSources[0].Organization.NameEn);
            Assert.Equal(expectedObject.personal.externalIdentifiers[0].DataSources[0].Organization.NameFi, actualObject.personal.externalIdentifiers[0].DataSources[0].Organization.NameFi);
            Assert.Equal(expectedObject.personal.externalIdentifiers[0].DataSources[0].Organization.NameSv, actualObject.personal.externalIdentifiers[0].DataSources[0].Organization.NameSv);
            Assert.Equal(expectedObject.personal.externalIdentifiers[0].DataSources[0].Organization.SectorId, actualObject.personal.externalIdentifiers[0].DataSources[0].Organization.SectorId);
            Assert.Equal(expectedObject.personal.externalIdentifiers[1].PidContent, actualObject.personal.externalIdentifiers[1].PidContent);
            Assert.Equal(expectedObject.personal.externalIdentifiers[1].PidType, actualObject.personal.externalIdentifiers[1].PidType);
            Assert.Equal(expectedObject.personal.externalIdentifiers[1].itemMeta.PrimaryValue, actualObject.personal.externalIdentifiers[1].itemMeta.PrimaryValue);
            Assert.Equal(expectedObject.personal.externalIdentifiers[1].DataSources[0].RegisteredDataSource, actualObject.personal.externalIdentifiers[1].DataSources[0].RegisteredDataSource);
            Assert.Equal(expectedObject.personal.externalIdentifiers[1].DataSources[0].Organization.NameEn, actualObject.personal.externalIdentifiers[1].DataSources[0].Organization.NameEn);
            Assert.Equal(expectedObject.personal.externalIdentifiers[1].DataSources[0].Organization.NameFi, actualObject.personal.externalIdentifiers[1].DataSources[0].Organization.NameFi);
            Assert.Equal(expectedObject.personal.externalIdentifiers[1].DataSources[0].Organization.NameSv, actualObject.personal.externalIdentifiers[1].DataSources[0].Organization.NameSv);
            Assert.Equal(expectedObject.personal.externalIdentifiers[1].DataSources[0].Organization.SectorId, actualObject.personal.externalIdentifiers[1].DataSources[0].Organization.SectorId);
            // Assert activity.affiliations - affiliation 1
            Assert.Equal(expectedObject.activity.affiliations[0].OrganizationNameFi, actualObject.activity.affiliations[0].OrganizationNameFi);
            Assert.Equal(expectedObject.activity.affiliations[0].OrganizationNameSv, actualObject.activity.affiliations[0].OrganizationNameSv);
            Assert.Equal(expectedObject.activity.affiliations[0].OrganizationNameEn, actualObject.activity.affiliations[0].OrganizationNameEn);
            Assert.Equal(expectedObject.activity.affiliations[0].DepartmentNameFi, actualObject.activity.affiliations[0].DepartmentNameFi);
            Assert.Equal(expectedObject.activity.affiliations[0].DepartmentNameSv, actualObject.activity.affiliations[0].DepartmentNameSv);
            Assert.Equal(expectedObject.activity.affiliations[0].DepartmentNameEn, actualObject.activity.affiliations[0].DepartmentNameEn);
            Assert.Equal(expectedObject.activity.affiliations[0].PositionNameFi, actualObject.activity.affiliations[0].PositionNameFi);
            Assert.Equal(expectedObject.activity.affiliations[0].PositionNameSv, actualObject.activity.affiliations[0].PositionNameSv);
            Assert.Equal(expectedObject.activity.affiliations[0].PositionNameEn, actualObject.activity.affiliations[0].PositionNameEn);
            Assert.Equal(expectedObject.activity.affiliations[0].AffiliationTypeFi, actualObject.activity.affiliations[0].AffiliationTypeFi);
            Assert.Equal(expectedObject.activity.affiliations[0].AffiliationTypeEn, actualObject.activity.affiliations[0].AffiliationTypeEn);
            Assert.Equal(expectedObject.activity.affiliations[0].AffiliationTypeSv, actualObject.activity.affiliations[0].AffiliationTypeSv);
            Assert.Equal(expectedObject.activity.affiliations[0].StartDate.Year, actualObject.activity.affiliations[0].StartDate.Year);
            Assert.Equal(expectedObject.activity.affiliations[0].StartDate.Month, actualObject.activity.affiliations[0].StartDate.Month);
            Assert.Equal(expectedObject.activity.affiliations[0].StartDate.Day, actualObject.activity.affiliations[0].StartDate.Day);
            Assert.Equal(expectedObject.activity.affiliations[0].EndDate.Year, actualObject.activity.affiliations[0].EndDate.Year);
            Assert.Equal(expectedObject.activity.affiliations[0].EndDate.Month, actualObject.activity.affiliations[0].EndDate.Month);
            Assert.Equal(expectedObject.activity.affiliations[0].EndDate.Day, actualObject.activity.affiliations[0].EndDate.Day);
            Assert.Equal(expectedObject.activity.affiliations[0].sector.Count, actualObject.activity.affiliations[0].sector.Count);
            Assert.Equal(expectedObject.activity.affiliations[0].sector[0].sectorId, actualObject.activity.affiliations[0].sector[0].sectorId);
            Assert.Equal(expectedObject.activity.affiliations[0].sector[0].nameFiSector, actualObject.activity.affiliations[0].sector[0].nameFiSector);
            Assert.Equal(expectedObject.activity.affiliations[0].sector[0].nameEnSector, actualObject.activity.affiliations[0].sector[0].nameEnSector);
            Assert.Equal(expectedObject.activity.affiliations[0].sector[0].nameSvSector, actualObject.activity.affiliations[0].sector[0].nameSvSector);
            Assert.Equal(expectedObject.activity.affiliations[0].sector[0].organization.Count, actualObject.activity.affiliations[0].sector[0].organization.Count);
            Assert.Equal(expectedObject.activity.affiliations[0].sector[0].organization[0].organizationId, actualObject.activity.affiliations[0].sector[0].organization[0].organizationId);
            Assert.Equal(expectedObject.activity.affiliations[0].sector[0].organization[0].OrganizationNameFi, actualObject.activity.affiliations[0].sector[0].organization[0].OrganizationNameFi);
            Assert.Equal(expectedObject.activity.affiliations[0].sector[0].organization[0].OrganizationNameEn, actualObject.activity.affiliations[0].sector[0].organization[0].OrganizationNameEn);
            Assert.Equal(expectedObject.activity.affiliations[0].sector[0].organization[0].OrganizationNameSv, actualObject.activity.affiliations[0].sector[0].organization[0].OrganizationNameSv);
            Assert.Equal(expectedObject.activity.affiliations[0].sector[0].organization[1].organizationId, actualObject.activity.affiliations[0].sector[0].organization[1].organizationId);
            Assert.Equal(expectedObject.activity.affiliations[0].sector[0].organization[1].OrganizationNameFi, actualObject.activity.affiliations[0].sector[0].organization[1].OrganizationNameFi);
            Assert.Equal(expectedObject.activity.affiliations[0].sector[0].organization[1].OrganizationNameEn, actualObject.activity.affiliations[0].sector[0].organization[1].OrganizationNameEn);
            Assert.Equal(expectedObject.activity.affiliations[0].sector[0].organization[1].OrganizationNameSv, actualObject.activity.affiliations[0].sector[0].organization[1].OrganizationNameSv);
            Assert.Equal(expectedObject.activity.affiliations[0].sector[1].sectorId, actualObject.activity.affiliations[0].sector[1].sectorId);
            Assert.Equal(expectedObject.activity.affiliations[0].sector[1].nameFiSector, actualObject.activity.affiliations[0].sector[1].nameFiSector);
            Assert.Equal(expectedObject.activity.affiliations[0].sector[1].nameEnSector, actualObject.activity.affiliations[0].sector[1].nameEnSector);
            Assert.Equal(expectedObject.activity.affiliations[0].sector[1].nameSvSector, actualObject.activity.affiliations[0].sector[1].nameSvSector);
            Assert.Equal(expectedObject.activity.affiliations[0].sector[1].organization.Count, actualObject.activity.affiliations[0].sector[1].organization.Count);
            Assert.Equal(expectedObject.activity.affiliations[0].sector[1].organization[0].organizationId, actualObject.activity.affiliations[0].sector[1].organization[0].organizationId);
            Assert.Equal(expectedObject.activity.affiliations[0].sector[1].organization[0].OrganizationNameFi, actualObject.activity.affiliations[0].sector[1].organization[0].OrganizationNameFi);
            Assert.Equal(expectedObject.activity.affiliations[0].sector[1].organization[0].OrganizationNameEn, actualObject.activity.affiliations[0].sector[1].organization[0].OrganizationNameEn);
            Assert.Equal(expectedObject.activity.affiliations[0].sector[1].organization[0].OrganizationNameSv, actualObject.activity.affiliations[0].sector[1].organization[0].OrganizationNameSv);
            Assert.Equal(expectedObject.activity.affiliations[0].sector[1].organization[1].organizationId, actualObject.activity.affiliations[0].sector[1].organization[1].organizationId);
            Assert.Equal(expectedObject.activity.affiliations[0].sector[1].organization[1].OrganizationNameFi, actualObject.activity.affiliations[0].sector[1].organization[1].OrganizationNameFi);
            Assert.Equal(expectedObject.activity.affiliations[0].sector[1].organization[1].OrganizationNameEn, actualObject.activity.affiliations[0].sector[1].organization[1].OrganizationNameEn);
            Assert.Equal(expectedObject.activity.affiliations[0].sector[1].organization[1].OrganizationNameSv, actualObject.activity.affiliations[0].sector[1].organization[1].OrganizationNameSv);
            Assert.Equal(expectedObject.activity.affiliations[0].itemMeta.PrimaryValue, actualObject.activity.affiliations[0].itemMeta.PrimaryValue);
            Assert.Equal(expectedObject.activity.affiliations[0].DataSources[0].RegisteredDataSource, actualObject.activity.affiliations[0].DataSources[0].RegisteredDataSource);
            Assert.Equal(expectedObject.activity.affiliations[0].DataSources[0].Organization.NameEn, actualObject.activity.affiliations[0].DataSources[0].Organization.NameEn);
            Assert.Equal(expectedObject.activity.affiliations[0].DataSources[0].Organization.NameFi, actualObject.activity.affiliations[0].DataSources[0].Organization.NameFi);
            Assert.Equal(expectedObject.activity.affiliations[0].DataSources[0].Organization.NameSv, actualObject.activity.affiliations[0].DataSources[0].Organization.NameSv);
            Assert.Equal(expectedObject.activity.affiliations[0].DataSources[0].Organization.SectorId, actualObject.activity.affiliations[0].DataSources[0].Organization.SectorId);
            // Assert activity.affiliations - affiliation 2
            Assert.Equal(expectedObject.activity.affiliations[1].OrganizationNameFi, actualObject.activity.affiliations[1].OrganizationNameFi);
            Assert.Equal(expectedObject.activity.affiliations[1].OrganizationNameSv, actualObject.activity.affiliations[1].OrganizationNameSv);
            Assert.Equal(expectedObject.activity.affiliations[1].OrganizationNameEn, actualObject.activity.affiliations[1].OrganizationNameEn);
            Assert.Equal(expectedObject.activity.affiliations[1].DepartmentNameFi, actualObject.activity.affiliations[1].DepartmentNameFi);
            Assert.Equal(expectedObject.activity.affiliations[1].DepartmentNameSv, actualObject.activity.affiliations[1].DepartmentNameSv);
            Assert.Equal(expectedObject.activity.affiliations[1].DepartmentNameEn, actualObject.activity.affiliations[1].DepartmentNameEn);
            Assert.Equal(expectedObject.activity.affiliations[1].PositionNameFi, actualObject.activity.affiliations[1].PositionNameFi);
            Assert.Equal(expectedObject.activity.affiliations[1].PositionNameSv, actualObject.activity.affiliations[1].PositionNameSv);
            Assert.Equal(expectedObject.activity.affiliations[1].PositionNameEn, actualObject.activity.affiliations[1].PositionNameEn);
            Assert.Equal(expectedObject.activity.affiliations[1].AffiliationTypeFi, actualObject.activity.affiliations[1].AffiliationTypeFi);
            Assert.Equal(expectedObject.activity.affiliations[1].AffiliationTypeEn, actualObject.activity.affiliations[1].AffiliationTypeEn);
            Assert.Equal(expectedObject.activity.affiliations[1].AffiliationTypeSv, actualObject.activity.affiliations[1].AffiliationTypeSv);
            Assert.Equal(expectedObject.activity.affiliations[1].StartDate.Year, actualObject.activity.affiliations[1].StartDate.Year);
            Assert.Equal(expectedObject.activity.affiliations[1].StartDate.Month, actualObject.activity.affiliations[1].StartDate.Month);
            Assert.Equal(expectedObject.activity.affiliations[1].StartDate.Day, actualObject.activity.affiliations[1].StartDate.Day);
            Assert.Equal(expectedObject.activity.affiliations[1].EndDate.Year, actualObject.activity.affiliations[1].EndDate.Year);
            Assert.Equal(expectedObject.activity.affiliations[1].EndDate.Month, actualObject.activity.affiliations[1].EndDate.Month);
            Assert.Equal(expectedObject.activity.affiliations[1].EndDate.Day, actualObject.activity.affiliations[1].EndDate.Day);
            Assert.Equal(expectedObject.activity.affiliations[1].sector.Count, actualObject.activity.affiliations[1].sector.Count);
            Assert.Equal(expectedObject.activity.affiliations[1].sector[0].sectorId, actualObject.activity.affiliations[1].sector[0].sectorId);
            Assert.Equal(expectedObject.activity.affiliations[1].sector[0].nameFiSector, actualObject.activity.affiliations[1].sector[0].nameFiSector);
            Assert.Equal(expectedObject.activity.affiliations[1].sector[0].nameEnSector, actualObject.activity.affiliations[1].sector[0].nameEnSector);
            Assert.Equal(expectedObject.activity.affiliations[1].sector[0].nameSvSector, actualObject.activity.affiliations[1].sector[0].nameSvSector);
            Assert.Equal(expectedObject.activity.affiliations[1].sector[0].organization.Count, actualObject.activity.affiliations[1].sector[0].organization.Count);
            Assert.Equal(expectedObject.activity.affiliations[1].sector[0].organization[0].organizationId, actualObject.activity.affiliations[1].sector[0].organization[0].organizationId);
            Assert.Equal(expectedObject.activity.affiliations[1].sector[0].organization[0].OrganizationNameFi, actualObject.activity.affiliations[1].sector[0].organization[0].OrganizationNameFi);
            Assert.Equal(expectedObject.activity.affiliations[1].sector[0].organization[0].OrganizationNameEn, actualObject.activity.affiliations[1].sector[0].organization[0].OrganizationNameEn);
            Assert.Equal(expectedObject.activity.affiliations[1].sector[0].organization[0].OrganizationNameSv, actualObject.activity.affiliations[1].sector[0].organization[0].OrganizationNameSv);
            Assert.Equal(expectedObject.activity.affiliations[1].sector[0].organization[1].organizationId, actualObject.activity.affiliations[1].sector[0].organization[1].organizationId);
            Assert.Equal(expectedObject.activity.affiliations[1].sector[0].organization[1].OrganizationNameFi, actualObject.activity.affiliations[1].sector[0].organization[1].OrganizationNameFi);
            Assert.Equal(expectedObject.activity.affiliations[1].sector[0].organization[1].OrganizationNameEn, actualObject.activity.affiliations[1].sector[0].organization[1].OrganizationNameEn);
            Assert.Equal(expectedObject.activity.affiliations[1].sector[0].organization[1].OrganizationNameSv, actualObject.activity.affiliations[1].sector[0].organization[1].OrganizationNameSv);
            Assert.Equal(expectedObject.activity.affiliations[1].sector[1].sectorId, actualObject.activity.affiliations[1].sector[1].sectorId);
            Assert.Equal(expectedObject.activity.affiliations[1].sector[1].nameFiSector, actualObject.activity.affiliations[1].sector[1].nameFiSector);
            Assert.Equal(expectedObject.activity.affiliations[1].sector[1].nameEnSector, actualObject.activity.affiliations[1].sector[1].nameEnSector);
            Assert.Equal(expectedObject.activity.affiliations[1].sector[1].nameSvSector, actualObject.activity.affiliations[1].sector[1].nameSvSector);
            Assert.Equal(expectedObject.activity.affiliations[1].sector[1].organization.Count, actualObject.activity.affiliations[1].sector[1].organization.Count);
            Assert.Equal(expectedObject.activity.affiliations[1].sector[1].organization[0].organizationId, actualObject.activity.affiliations[1].sector[1].organization[0].organizationId);
            Assert.Equal(expectedObject.activity.affiliations[1].sector[1].organization[0].OrganizationNameFi, actualObject.activity.affiliations[1].sector[1].organization[0].OrganizationNameFi);
            Assert.Equal(expectedObject.activity.affiliations[1].sector[1].organization[0].OrganizationNameEn, actualObject.activity.affiliations[1].sector[1].organization[0].OrganizationNameEn);
            Assert.Equal(expectedObject.activity.affiliations[1].sector[1].organization[0].OrganizationNameSv, actualObject.activity.affiliations[1].sector[1].organization[0].OrganizationNameSv);
            Assert.Equal(expectedObject.activity.affiliations[1].sector[1].organization[1].organizationId, actualObject.activity.affiliations[1].sector[1].organization[1].organizationId);
            Assert.Equal(expectedObject.activity.affiliations[1].sector[1].organization[1].OrganizationNameFi, actualObject.activity.affiliations[1].sector[1].organization[1].OrganizationNameFi);
            Assert.Equal(expectedObject.activity.affiliations[1].sector[1].organization[1].OrganizationNameEn, actualObject.activity.affiliations[1].sector[1].organization[1].OrganizationNameEn);
            Assert.Equal(expectedObject.activity.affiliations[1].sector[1].organization[1].OrganizationNameSv, actualObject.activity.affiliations[1].sector[1].organization[1].OrganizationNameSv);
            Assert.Equal(expectedObject.activity.affiliations[1].itemMeta.PrimaryValue, actualObject.activity.affiliations[1].itemMeta.PrimaryValue);
            Assert.Equal(expectedObject.activity.affiliations[1].DataSources[0].RegisteredDataSource, actualObject.activity.affiliations[1].DataSources[0].RegisteredDataSource);
            Assert.Equal(expectedObject.activity.affiliations[1].DataSources[0].Organization.NameEn, actualObject.activity.affiliations[1].DataSources[0].Organization.NameEn);
            Assert.Equal(expectedObject.activity.affiliations[1].DataSources[0].Organization.NameFi, actualObject.activity.affiliations[1].DataSources[0].Organization.NameFi);
            Assert.Equal(expectedObject.activity.affiliations[1].DataSources[0].Organization.NameSv, actualObject.activity.affiliations[1].DataSources[0].Organization.NameSv);
            Assert.Equal(expectedObject.activity.affiliations[1].DataSources[0].Organization.SectorId, actualObject.activity.affiliations[1].DataSources[0].Organization.SectorId);
            // Assert activity.educations - education 1
            Assert.Equal(expectedObject.activity.educations[0].NameFi, actualObject.activity.educations[0].NameFi);
            Assert.Equal(expectedObject.activity.educations[0].NameSv, actualObject.activity.educations[0].NameSv);
            Assert.Equal(expectedObject.activity.educations[0].NameEn, actualObject.activity.educations[0].NameEn);
            Assert.Equal(expectedObject.activity.educations[0].DegreeGrantingInstitutionName, actualObject.activity.educations[0].DegreeGrantingInstitutionName);
            Assert.Equal(expectedObject.activity.educations[0].StartDate.Year, actualObject.activity.educations[0].StartDate.Year);
            Assert.Equal(expectedObject.activity.educations[0].StartDate.Month, actualObject.activity.educations[0].StartDate.Month);
            Assert.Equal(expectedObject.activity.educations[0].StartDate.Day, actualObject.activity.educations[0].StartDate.Day);
            Assert.Equal(expectedObject.activity.educations[0].EndDate.Year, actualObject.activity.educations[0].EndDate.Year);
            Assert.Equal(expectedObject.activity.educations[0].EndDate.Month, actualObject.activity.educations[0].EndDate.Month);
            Assert.Equal(expectedObject.activity.educations[0].EndDate.Day, actualObject.activity.educations[0].EndDate.Day);
            Assert.Equal(expectedObject.activity.educations[0].itemMeta.PrimaryValue, actualObject.activity.educations[0].itemMeta.PrimaryValue);
            Assert.Equal(expectedObject.activity.educations[0].DataSources[0].RegisteredDataSource, actualObject.activity.educations[0].DataSources[0].RegisteredDataSource);
            Assert.Equal(expectedObject.activity.educations[0].DataSources[0].Organization.NameEn, actualObject.activity.educations[0].DataSources[0].Organization.NameEn);
            Assert.Equal(expectedObject.activity.educations[0].DataSources[0].Organization.NameFi, actualObject.activity.educations[0].DataSources[0].Organization.NameFi);
            Assert.Equal(expectedObject.activity.educations[0].DataSources[0].Organization.NameSv, actualObject.activity.educations[0].DataSources[0].Organization.NameSv);
            Assert.Equal(expectedObject.activity.educations[0].DataSources[0].Organization.SectorId, actualObject.activity.educations[0].DataSources[0].Organization.SectorId);
            // Assert activity.educations - education 2
            Assert.Equal(expectedObject.activity.educations[1].NameFi, actualObject.activity.educations[1].NameFi);
            Assert.Equal(expectedObject.activity.educations[1].NameSv, actualObject.activity.educations[1].NameSv);
            Assert.Equal(expectedObject.activity.educations[1].NameEn, actualObject.activity.educations[1].NameEn);
            Assert.Equal(expectedObject.activity.educations[1].DegreeGrantingInstitutionName, actualObject.activity.educations[1].DegreeGrantingInstitutionName);
            Assert.Equal(expectedObject.activity.educations[1].StartDate.Year, actualObject.activity.educations[1].StartDate.Year);
            Assert.Equal(expectedObject.activity.educations[1].StartDate.Month, actualObject.activity.educations[1].StartDate.Month);
            Assert.Equal(expectedObject.activity.educations[1].StartDate.Day, actualObject.activity.educations[1].StartDate.Day);
            Assert.Equal(expectedObject.activity.educations[1].EndDate.Year, actualObject.activity.educations[1].EndDate.Year);
            Assert.Equal(expectedObject.activity.educations[1].EndDate.Month, actualObject.activity.educations[1].EndDate.Month);
            Assert.Equal(expectedObject.activity.educations[1].EndDate.Day, actualObject.activity.educations[1].EndDate.Day);
            Assert.Equal(expectedObject.activity.educations[1].itemMeta.PrimaryValue, actualObject.activity.educations[1].itemMeta.PrimaryValue);
            Assert.Equal(expectedObject.activity.educations[1].DataSources[0].RegisteredDataSource, actualObject.activity.educations[1].DataSources[0].RegisteredDataSource);
            Assert.Equal(expectedObject.activity.educations[1].DataSources[0].Organization.NameEn, actualObject.activity.educations[1].DataSources[0].Organization.NameEn);
            Assert.Equal(expectedObject.activity.educations[1].DataSources[0].Organization.NameFi, actualObject.activity.educations[1].DataSources[0].Organization.NameFi);
            Assert.Equal(expectedObject.activity.educations[1].DataSources[0].Organization.NameSv, actualObject.activity.educations[1].DataSources[0].Organization.NameSv);
            Assert.Equal(expectedObject.activity.educations[1].DataSources[0].Organization.SectorId, actualObject.activity.educations[1].DataSources[0].Organization.SectorId);
            // Assert activity.publications - publication 1
            Assert.Equal(expectedObject.activity.publications[0].AuthorsText, actualObject.activity.publications[0].AuthorsText);
            Assert.Equal(expectedObject.activity.publications[0].Doi, actualObject.activity.publications[0].Doi);
            Assert.Equal(expectedObject.activity.publications[0].ConferenceName, actualObject.activity.publications[0].ConferenceName);
            Assert.Equal(expectedObject.activity.publications[0].JournalName, actualObject.activity.publications[0].JournalName);
            Assert.Equal(expectedObject.activity.publications[0].OpenAccess, actualObject.activity.publications[0].OpenAccess);
            Assert.Equal(expectedObject.activity.publications[0].ParentPublicationName, actualObject.activity.publications[0].ParentPublicationName);
            Assert.NotEmpty(actualObject.activity.publications[0].PeerReviewed);
            Assert.Equal(expectedObject.activity.publications[0].PeerReviewed[0].Id, actualObject.activity.publications[0].PeerReviewed[0].Id);
            Assert.Equal(expectedObject.activity.publications[0].PeerReviewed[0].NameFiPeerReviewed, actualObject.activity.publications[0].PeerReviewed[0].NameFiPeerReviewed);
            Assert.Equal(expectedObject.activity.publications[0].PeerReviewed[0].NameSvPeerReviewed, actualObject.activity.publications[0].PeerReviewed[0].NameSvPeerReviewed);
            Assert.Equal(expectedObject.activity.publications[0].PeerReviewed[0].NameEnPeerReviewed, actualObject.activity.publications[0].PeerReviewed[0].NameEnPeerReviewed);
            Assert.Equal(expectedObject.activity.publications[0].PeerReviewed[1].Id, actualObject.activity.publications[0].PeerReviewed[1].Id);
            Assert.Equal(expectedObject.activity.publications[0].PeerReviewed[1].NameFiPeerReviewed, actualObject.activity.publications[0].PeerReviewed[1].NameFiPeerReviewed);
            Assert.Equal(expectedObject.activity.publications[0].PeerReviewed[1].NameSvPeerReviewed, actualObject.activity.publications[0].PeerReviewed[1].NameSvPeerReviewed);
            Assert.Equal(expectedObject.activity.publications[0].PeerReviewed[1].NameEnPeerReviewed, actualObject.activity.publications[0].PeerReviewed[1].NameEnPeerReviewed);
            Assert.Equal(expectedObject.activity.publications[0].PublicationId, actualObject.activity.publications[0].PublicationId);
            Assert.Equal(expectedObject.activity.publications[0].PublicationName, actualObject.activity.publications[0].PublicationName);
            Assert.Equal(expectedObject.activity.publications[0].PublicationTypeCode, actualObject.activity.publications[0].PublicationTypeCode);
            Assert.Equal(expectedObject.activity.publications[0].PublicationYear, actualObject.activity.publications[0].PublicationYear);
            Assert.Equal(expectedObject.activity.publications[0].SelfArchivedAddress, actualObject.activity.publications[0].SelfArchivedAddress);
            Assert.Equal(expectedObject.activity.publications[0].SelfArchivedCode, actualObject.activity.publications[0].SelfArchivedCode);
            Assert.Equal(expectedObject.activity.publications[0].itemMeta.PrimaryValue, actualObject.activity.publications[0].itemMeta.PrimaryValue);
            Assert.Equal(expectedObject.activity.publications[0].DataSources[0].RegisteredDataSource, actualObject.activity.publications[0].DataSources[0].RegisteredDataSource);
            Assert.Equal(expectedObject.activity.publications[0].DataSources[0].Organization.NameEn, actualObject.activity.publications[0].DataSources[0].Organization.NameEn);
            Assert.Equal(expectedObject.activity.publications[0].DataSources[0].Organization.NameFi, actualObject.activity.publications[0].DataSources[0].Organization.NameFi);
            Assert.Equal(expectedObject.activity.publications[0].DataSources[0].Organization.NameSv, actualObject.activity.publications[0].DataSources[0].Organization.NameSv);
            Assert.Equal(expectedObject.activity.publications[0].DataSources[0].Organization.SectorId, actualObject.activity.publications[0].DataSources[0].Organization.SectorId);
            // Assert activity.publications - publication 2
            Assert.Equal(expectedObject.activity.publications[1].AuthorsText, actualObject.activity.publications[1].AuthorsText);
            Assert.Equal(expectedObject.activity.publications[1].Doi, actualObject.activity.publications[1].Doi);
            Assert.Equal(expectedObject.activity.publications[1].ConferenceName, actualObject.activity.publications[1].ConferenceName);
            Assert.Equal(expectedObject.activity.publications[1].JournalName, actualObject.activity.publications[1].JournalName);
            Assert.Equal(expectedObject.activity.publications[1].OpenAccess, actualObject.activity.publications[1].OpenAccess);
            Assert.Equal(expectedObject.activity.publications[1].ParentPublicationName, actualObject.activity.publications[1].ParentPublicationName);
            Assert.NotEmpty(actualObject.activity.publications[1].PeerReviewed);
            Assert.Equal(expectedObject.activity.publications[1].PeerReviewed[0].Id, actualObject.activity.publications[1].PeerReviewed[0].Id);
            Assert.Equal(expectedObject.activity.publications[1].PeerReviewed[0].NameFiPeerReviewed, actualObject.activity.publications[1].PeerReviewed[0].NameFiPeerReviewed);
            Assert.Equal(expectedObject.activity.publications[1].PeerReviewed[0].NameSvPeerReviewed, actualObject.activity.publications[1].PeerReviewed[0].NameSvPeerReviewed);
            Assert.Equal(expectedObject.activity.publications[1].PeerReviewed[0].NameEnPeerReviewed, actualObject.activity.publications[1].PeerReviewed[0].NameEnPeerReviewed);
            Assert.Equal(expectedObject.activity.publications[1].PublicationId, actualObject.activity.publications[1].PublicationId);
            Assert.Equal(expectedObject.activity.publications[1].PublicationName, actualObject.activity.publications[1].PublicationName);
            Assert.Equal(expectedObject.activity.publications[1].PublicationTypeCode, actualObject.activity.publications[1].PublicationTypeCode);
            Assert.Equal(expectedObject.activity.publications[1].PublicationYear, actualObject.activity.publications[1].PublicationYear);
            Assert.Equal(expectedObject.activity.publications[1].SelfArchivedAddress, actualObject.activity.publications[1].SelfArchivedAddress);
            Assert.Equal(expectedObject.activity.publications[1].SelfArchivedCode, actualObject.activity.publications[1].SelfArchivedCode);
            Assert.Equal(expectedObject.activity.publications[1].DataSources[0].RegisteredDataSource, actualObject.activity.publications[1].DataSources[0].RegisteredDataSource);
            Assert.Equal(expectedObject.activity.publications[1].DataSources[0].Organization.NameEn, actualObject.activity.publications[1].DataSources[0].Organization.NameEn);
            Assert.Equal(expectedObject.activity.publications[1].DataSources[0].Organization.NameFi, actualObject.activity.publications[1].DataSources[0].Organization.NameFi);
            Assert.Equal(expectedObject.activity.publications[1].DataSources[0].Organization.NameSv, actualObject.activity.publications[1].DataSources[0].Organization.NameSv);
            Assert.Equal(expectedObject.activity.publications[1].DataSources[0].Organization.SectorId, actualObject.activity.publications[1].DataSources[0].Organization.SectorId);
            // Assert activity.fundingDecisions - funding decision 1
            Assert.Equal(expectedObject.activity.fundingDecisions[0].ProjectId, actualObject.activity.fundingDecisions[0].ProjectId);
            Assert.Equal(expectedObject.activity.fundingDecisions[0].ProjectAcronym, actualObject.activity.fundingDecisions[0].ProjectAcronym);
            Assert.Equal(expectedObject.activity.fundingDecisions[0].ProjectNameFi, actualObject.activity.fundingDecisions[0].ProjectNameFi);
            Assert.Equal(expectedObject.activity.fundingDecisions[0].ProjectNameEn, actualObject.activity.fundingDecisions[0].ProjectNameEn);
            Assert.Equal(expectedObject.activity.fundingDecisions[0].ProjectNameSv, actualObject.activity.fundingDecisions[0].ProjectNameSv);
            Assert.Equal(expectedObject.activity.fundingDecisions[0].ProjectDescriptionFi, actualObject.activity.fundingDecisions[0].ProjectDescriptionFi);
            Assert.Equal(expectedObject.activity.fundingDecisions[0].ProjectDescriptionEn, actualObject.activity.fundingDecisions[0].ProjectDescriptionEn);
            Assert.Equal(expectedObject.activity.fundingDecisions[0].ProjectDescriptionSv, actualObject.activity.fundingDecisions[0].ProjectDescriptionSv);
            Assert.Equal(expectedObject.activity.fundingDecisions[0].FunderNameFi, actualObject.activity.fundingDecisions[0].FunderNameFi);
            Assert.Equal(expectedObject.activity.fundingDecisions[0].FunderNameEn, actualObject.activity.fundingDecisions[0].FunderNameEn);
            Assert.Equal(expectedObject.activity.fundingDecisions[0].FunderNameSv, actualObject.activity.fundingDecisions[0].FunderNameSv);
            Assert.Equal(expectedObject.activity.fundingDecisions[0].FunderProjectNumber, actualObject.activity.fundingDecisions[0].FunderProjectNumber);
            Assert.Equal(expectedObject.activity.fundingDecisions[0].TypeOfFundingNameFi, actualObject.activity.fundingDecisions[0].TypeOfFundingNameFi);
            Assert.Equal(expectedObject.activity.fundingDecisions[0].TypeOfFundingNameEn, actualObject.activity.fundingDecisions[0].TypeOfFundingNameEn);
            Assert.Equal(expectedObject.activity.fundingDecisions[0].TypeOfFundingNameSv, actualObject.activity.fundingDecisions[0].TypeOfFundingNameSv);
            Assert.Equal(expectedObject.activity.fundingDecisions[0].CallProgrammeNameFi, actualObject.activity.fundingDecisions[0].CallProgrammeNameFi);
            Assert.Equal(expectedObject.activity.fundingDecisions[0].CallProgrammeNameEn, actualObject.activity.fundingDecisions[0].CallProgrammeNameEn);
            Assert.Equal(expectedObject.activity.fundingDecisions[0].CallProgrammeNameSv, actualObject.activity.fundingDecisions[0].CallProgrammeNameSv);
            Assert.Equal(expectedObject.activity.fundingDecisions[0].FundingStartYear, actualObject.activity.fundingDecisions[0].FundingStartYear);
            Assert.Equal(expectedObject.activity.fundingDecisions[0].FundingEndYear, actualObject.activity.fundingDecisions[0].FundingEndYear);
            Assert.Equal(expectedObject.activity.fundingDecisions[0].AmountInEur, actualObject.activity.fundingDecisions[0].AmountInEur);
            Assert.Equal(expectedObject.activity.fundingDecisions[0].AmountInFundingDecisionCurrency, actualObject.activity.fundingDecisions[0].AmountInFundingDecisionCurrency);
            Assert.Equal(expectedObject.activity.fundingDecisions[0].FundingDecisionCurrencyAbbreviation, actualObject.activity.fundingDecisions[0].FundingDecisionCurrencyAbbreviation);
            Assert.Equal(expectedObject.activity.fundingDecisions[0].Url, actualObject.activity.fundingDecisions[0].Url);
            Assert.Equal(expectedObject.activity.fundingDecisions[0].DataSources[0].RegisteredDataSource, actualObject.activity.fundingDecisions[0].DataSources[0].RegisteredDataSource);
            Assert.Equal(expectedObject.activity.fundingDecisions[0].DataSources[0].Organization.NameEn, actualObject.activity.fundingDecisions[0].DataSources[0].Organization.NameEn);
            Assert.Equal(expectedObject.activity.fundingDecisions[0].DataSources[0].Organization.NameFi, actualObject.activity.fundingDecisions[0].DataSources[0].Organization.NameFi);
            Assert.Equal(expectedObject.activity.fundingDecisions[0].DataSources[0].Organization.NameSv, actualObject.activity.fundingDecisions[0].DataSources[0].Organization.NameSv);
            Assert.Equal(expectedObject.activity.fundingDecisions[0].DataSources[0].Organization.SectorId, actualObject.activity.fundingDecisions[0].DataSources[0].Organization.SectorId);
            // Assert activity.fundingDecisions - funding decision 2
            Assert.Equal(expectedObject.activity.fundingDecisions[1].ProjectId, actualObject.activity.fundingDecisions[1].ProjectId);
            Assert.Equal(expectedObject.activity.fundingDecisions[1].ProjectAcronym, actualObject.activity.fundingDecisions[1].ProjectAcronym);
            Assert.Equal(expectedObject.activity.fundingDecisions[1].ProjectNameFi, actualObject.activity.fundingDecisions[1].ProjectNameFi);
            Assert.Equal(expectedObject.activity.fundingDecisions[1].ProjectNameEn, actualObject.activity.fundingDecisions[1].ProjectNameEn);
            Assert.Equal(expectedObject.activity.fundingDecisions[1].ProjectNameSv, actualObject.activity.fundingDecisions[1].ProjectNameSv);
            Assert.Equal(expectedObject.activity.fundingDecisions[1].ProjectDescriptionFi, actualObject.activity.fundingDecisions[1].ProjectDescriptionFi);
            Assert.Equal(expectedObject.activity.fundingDecisions[1].ProjectDescriptionEn, actualObject.activity.fundingDecisions[1].ProjectDescriptionEn);
            Assert.Equal(expectedObject.activity.fundingDecisions[1].ProjectDescriptionSv, actualObject.activity.fundingDecisions[1].ProjectDescriptionSv);
            Assert.Equal(expectedObject.activity.fundingDecisions[1].FunderNameFi, actualObject.activity.fundingDecisions[1].FunderNameFi);
            Assert.Equal(expectedObject.activity.fundingDecisions[1].FunderNameEn, actualObject.activity.fundingDecisions[1].FunderNameEn);
            Assert.Equal(expectedObject.activity.fundingDecisions[1].FunderNameSv, actualObject.activity.fundingDecisions[1].FunderNameSv);
            Assert.Equal(expectedObject.activity.fundingDecisions[1].FunderProjectNumber, actualObject.activity.fundingDecisions[1].FunderProjectNumber);
            Assert.Equal(expectedObject.activity.fundingDecisions[1].TypeOfFundingNameFi, actualObject.activity.fundingDecisions[1].TypeOfFundingNameFi);
            Assert.Equal(expectedObject.activity.fundingDecisions[1].TypeOfFundingNameEn, actualObject.activity.fundingDecisions[1].TypeOfFundingNameEn);
            Assert.Equal(expectedObject.activity.fundingDecisions[1].TypeOfFundingNameSv, actualObject.activity.fundingDecisions[1].TypeOfFundingNameSv);
            Assert.Equal(expectedObject.activity.fundingDecisions[1].CallProgrammeNameFi, actualObject.activity.fundingDecisions[1].CallProgrammeNameFi);
            Assert.Equal(expectedObject.activity.fundingDecisions[1].CallProgrammeNameEn, actualObject.activity.fundingDecisions[1].CallProgrammeNameEn);
            Assert.Equal(expectedObject.activity.fundingDecisions[1].CallProgrammeNameSv, actualObject.activity.fundingDecisions[1].CallProgrammeNameSv);
            Assert.Equal(expectedObject.activity.fundingDecisions[1].FundingStartYear, actualObject.activity.fundingDecisions[1].FundingStartYear);
            Assert.Equal(expectedObject.activity.fundingDecisions[1].FundingEndYear, actualObject.activity.fundingDecisions[1].FundingEndYear);
            Assert.Equal(expectedObject.activity.fundingDecisions[1].AmountInEur, actualObject.activity.fundingDecisions[1].AmountInEur);
            Assert.Equal(expectedObject.activity.fundingDecisions[1].AmountInFundingDecisionCurrency, actualObject.activity.fundingDecisions[1].AmountInFundingDecisionCurrency);
            Assert.Equal(expectedObject.activity.fundingDecisions[1].FundingDecisionCurrencyAbbreviation, actualObject.activity.fundingDecisions[1].FundingDecisionCurrencyAbbreviation);
            Assert.Equal(expectedObject.activity.fundingDecisions[1].Url, actualObject.activity.fundingDecisions[1].Url);
            Assert.Equal(expectedObject.activity.fundingDecisions[1].DataSources[0].RegisteredDataSource, actualObject.activity.fundingDecisions[1].DataSources[0].RegisteredDataSource);
            Assert.Equal(expectedObject.activity.fundingDecisions[1].DataSources[0].Organization.NameEn, actualObject.activity.fundingDecisions[1].DataSources[0].Organization.NameEn);
            Assert.Equal(expectedObject.activity.fundingDecisions[1].DataSources[0].Organization.NameFi, actualObject.activity.fundingDecisions[1].DataSources[0].Organization.NameFi);
            Assert.Equal(expectedObject.activity.fundingDecisions[1].DataSources[0].Organization.NameSv, actualObject.activity.fundingDecisions[1].DataSources[0].Organization.NameSv);
            Assert.Equal(expectedObject.activity.fundingDecisions[1].DataSources[0].Organization.SectorId, actualObject.activity.fundingDecisions[1].DataSources[0].Organization.SectorId);
            // Assert activity.researchDatasets - dataset 1
            Assert.Equal(expectedObject.activity.researchDatasets[0].AccessType, actualObject.activity.researchDatasets[0].AccessType);
            Assert.Equal(expectedObject.activity.researchDatasets[0].Actor[0].actorRole, actualObject.activity.researchDatasets[0].Actor[0].actorRole);
            Assert.Equal(expectedObject.activity.researchDatasets[0].Actor[0].actorRoleNameFi, actualObject.activity.researchDatasets[0].Actor[0].actorRoleNameFi);
            Assert.Equal(expectedObject.activity.researchDatasets[0].Actor[0].actorRoleNameSv, actualObject.activity.researchDatasets[0].Actor[0].actorRoleNameSv);
            Assert.Equal(expectedObject.activity.researchDatasets[0].Actor[0].actorRoleNameEn, actualObject.activity.researchDatasets[0].Actor[0].actorRoleNameEn);
            Assert.Equal(expectedObject.activity.researchDatasets[0].Actor[1].actorRole, actualObject.activity.researchDatasets[0].Actor[1].actorRole);
            Assert.Equal(expectedObject.activity.researchDatasets[0].Actor[1].actorRoleNameFi, actualObject.activity.researchDatasets[0].Actor[1].actorRoleNameFi);
            Assert.Equal(expectedObject.activity.researchDatasets[0].Actor[1].actorRoleNameSv, actualObject.activity.researchDatasets[0].Actor[1].actorRoleNameSv);
            Assert.Equal(expectedObject.activity.researchDatasets[0].Actor[1].actorRoleNameEn, actualObject.activity.researchDatasets[0].Actor[1].actorRoleNameEn);
            Assert.Equal(expectedObject.activity.researchDatasets[0].FairdataUrl, actualObject.activity.researchDatasets[0].FairdataUrl);
            Assert.Equal(expectedObject.activity.researchDatasets[0].Identifier, actualObject.activity.researchDatasets[0].Identifier);
            Assert.Equal(expectedObject.activity.researchDatasets[0].NameFi, actualObject.activity.researchDatasets[0].NameFi);
            Assert.Equal(expectedObject.activity.researchDatasets[0].NameEn, actualObject.activity.researchDatasets[0].NameEn);
            Assert.Equal(expectedObject.activity.researchDatasets[0].NameSv, actualObject.activity.researchDatasets[0].NameSv);
            Assert.Equal(expectedObject.activity.researchDatasets[0].DescriptionFi, actualObject.activity.researchDatasets[0].DescriptionFi);
            Assert.Equal(expectedObject.activity.researchDatasets[0].DescriptionEn, actualObject.activity.researchDatasets[0].DescriptionEn);
            Assert.Equal(expectedObject.activity.researchDatasets[0].DescriptionSv, actualObject.activity.researchDatasets[0].DescriptionSv);
            Assert.Equal(expectedObject.activity.researchDatasets[0].Url, actualObject.activity.researchDatasets[0].Url);
            Assert.Equal(expectedObject.activity.researchDatasets[0].DatasetCreated, actualObject.activity.researchDatasets[0].DatasetCreated);
            Assert.Equal(expectedObject.activity.researchDatasets[0].PreferredIdentifiers[0].PidContent, actualObject.activity.researchDatasets[0].PreferredIdentifiers[0].PidContent);
            Assert.Equal(expectedObject.activity.researchDatasets[0].PreferredIdentifiers[0].PidType, actualObject.activity.researchDatasets[0].PreferredIdentifiers[0].PidType);
            Assert.Equal(expectedObject.activity.researchDatasets[0].PreferredIdentifiers[1].PidContent, actualObject.activity.researchDatasets[0].PreferredIdentifiers[1].PidContent);
            Assert.Equal(expectedObject.activity.researchDatasets[0].PreferredIdentifiers[1].PidType, actualObject.activity.researchDatasets[0].PreferredIdentifiers[1].PidType);
            Assert.Equal(expectedObject.activity.researchDatasets[0].DataSources[0].RegisteredDataSource, actualObject.activity.researchDatasets[0].DataSources[0].RegisteredDataSource);
            Assert.Equal(expectedObject.activity.researchDatasets[0].DataSources[0].Organization.NameEn, actualObject.activity.researchDatasets[0].DataSources[0].Organization.NameEn);
            Assert.Equal(expectedObject.activity.researchDatasets[0].DataSources[0].Organization.NameFi, actualObject.activity.researchDatasets[0].DataSources[0].Organization.NameFi);
            Assert.Equal(expectedObject.activity.researchDatasets[0].DataSources[0].Organization.NameSv, actualObject.activity.researchDatasets[0].DataSources[0].Organization.NameSv);
            Assert.Equal(expectedObject.activity.researchDatasets[0].DataSources[0].Organization.SectorId, actualObject.activity.researchDatasets[0].DataSources[0].Organization.SectorId);
            // Assert activity.researchDatasets - dataset 2
            Assert.Equal(expectedObject.activity.researchDatasets[1].AccessType, actualObject.activity.researchDatasets[1].AccessType);
            Assert.Equal(expectedObject.activity.researchDatasets[1].Actor[0].actorRole, actualObject.activity.researchDatasets[1].Actor[0].actorRole);
            Assert.Equal(expectedObject.activity.researchDatasets[1].Actor[0].actorRoleNameFi, actualObject.activity.researchDatasets[1].Actor[0].actorRoleNameFi);
            Assert.Equal(expectedObject.activity.researchDatasets[1].Actor[0].actorRoleNameSv, actualObject.activity.researchDatasets[1].Actor[0].actorRoleNameSv);
            Assert.Equal(expectedObject.activity.researchDatasets[1].Actor[0].actorRoleNameEn, actualObject.activity.researchDatasets[1].Actor[0].actorRoleNameEn);
            Assert.Equal(expectedObject.activity.researchDatasets[1].Actor[1].actorRole, actualObject.activity.researchDatasets[1].Actor[1].actorRole);
            Assert.Equal(expectedObject.activity.researchDatasets[1].Actor[1].actorRoleNameFi, actualObject.activity.researchDatasets[1].Actor[1].actorRoleNameFi);
            Assert.Equal(expectedObject.activity.researchDatasets[1].Actor[1].actorRoleNameSv, actualObject.activity.researchDatasets[1].Actor[1].actorRoleNameSv);
            Assert.Equal(expectedObject.activity.researchDatasets[1].Actor[1].actorRoleNameEn, actualObject.activity.researchDatasets[1].Actor[1].actorRoleNameEn);
            Assert.Equal(expectedObject.activity.researchDatasets[1].FairdataUrl, actualObject.activity.researchDatasets[1].FairdataUrl);
            Assert.Equal(expectedObject.activity.researchDatasets[1].Identifier, actualObject.activity.researchDatasets[1].Identifier);
            Assert.Equal(expectedObject.activity.researchDatasets[1].NameFi, actualObject.activity.researchDatasets[1].NameFi);
            Assert.Equal(expectedObject.activity.researchDatasets[1].NameEn, actualObject.activity.researchDatasets[1].NameEn);
            Assert.Equal(expectedObject.activity.researchDatasets[1].NameSv, actualObject.activity.researchDatasets[1].NameSv);
            Assert.Equal(expectedObject.activity.researchDatasets[1].DescriptionFi, actualObject.activity.researchDatasets[1].DescriptionFi);
            Assert.Equal(expectedObject.activity.researchDatasets[1].DescriptionEn, actualObject.activity.researchDatasets[1].DescriptionEn);
            Assert.Equal(expectedObject.activity.researchDatasets[1].DescriptionSv, actualObject.activity.researchDatasets[1].DescriptionSv);
            Assert.Equal(expectedObject.activity.researchDatasets[1].Url, actualObject.activity.researchDatasets[1].Url);
            Assert.Equal(expectedObject.activity.researchDatasets[1].DatasetCreated, actualObject.activity.researchDatasets[1].DatasetCreated);
            Assert.Equal(expectedObject.activity.researchDatasets[1].PreferredIdentifiers[0].PidContent, actualObject.activity.researchDatasets[1].PreferredIdentifiers[0].PidContent);
            Assert.Equal(expectedObject.activity.researchDatasets[1].PreferredIdentifiers[0].PidType, actualObject.activity.researchDatasets[1].PreferredIdentifiers[0].PidType);
            Assert.Equal(expectedObject.activity.researchDatasets[1].PreferredIdentifiers[1].PidContent, actualObject.activity.researchDatasets[1].PreferredIdentifiers[1].PidContent);
            Assert.Equal(expectedObject.activity.researchDatasets[1].PreferredIdentifiers[1].PidType, actualObject.activity.researchDatasets[1].PreferredIdentifiers[1].PidType);
            Assert.Equal(expectedObject.activity.researchDatasets[1].DataSources[0].RegisteredDataSource, actualObject.activity.researchDatasets[1].DataSources[0].RegisteredDataSource);
            Assert.Equal(expectedObject.activity.researchDatasets[1].DataSources[0].Organization.NameEn, actualObject.activity.researchDatasets[1].DataSources[0].Organization.NameEn);
            Assert.Equal(expectedObject.activity.researchDatasets[1].DataSources[0].Organization.NameFi, actualObject.activity.researchDatasets[1].DataSources[0].Organization.NameFi);
            Assert.Equal(expectedObject.activity.researchDatasets[1].DataSources[0].Organization.NameSv, actualObject.activity.researchDatasets[1].DataSources[0].Organization.NameSv);
            Assert.Equal(expectedObject.activity.researchDatasets[1].DataSources[0].Organization.SectorId, actualObject.activity.researchDatasets[1].DataSources[0].Organization.SectorId);
            // Assert activity.activitiesAndRewards - activity and reward 1
            Assert.Equal(expectedObject.activity.activitiesAndRewards[0].NameFi, actualObject.activity.activitiesAndRewards[0].NameFi);
            Assert.Equal(expectedObject.activity.activitiesAndRewards[0].NameEn, actualObject.activity.activitiesAndRewards[0].NameEn);
            Assert.Equal(expectedObject.activity.activitiesAndRewards[0].NameSv, actualObject.activity.activitiesAndRewards[0].NameSv);
            Assert.Equal(expectedObject.activity.activitiesAndRewards[0].DescriptionFi, actualObject.activity.activitiesAndRewards[0].DescriptionFi);
            Assert.Equal(expectedObject.activity.activitiesAndRewards[0].DescriptionEn, actualObject.activity.activitiesAndRewards[0].DescriptionEn);
            Assert.Equal(expectedObject.activity.activitiesAndRewards[0].DescriptionSv, actualObject.activity.activitiesAndRewards[0].DescriptionSv);
            Assert.Equal(expectedObject.activity.activitiesAndRewards[0].InternationalCollaboration, actualObject.activity.activitiesAndRewards[0].InternationalCollaboration);
            Assert.Equal(expectedObject.activity.activitiesAndRewards[0].StartDate.Year, actualObject.activity.activitiesAndRewards[0].StartDate.Year);
            Assert.Equal(expectedObject.activity.activitiesAndRewards[0].StartDate.Month, actualObject.activity.activitiesAndRewards[0].StartDate.Month);
            Assert.Equal(expectedObject.activity.activitiesAndRewards[0].StartDate.Day, actualObject.activity.activitiesAndRewards[0].StartDate.Day);
            Assert.Equal(expectedObject.activity.activitiesAndRewards[0].EndDate.Year, actualObject.activity.activitiesAndRewards[0].EndDate.Year);
            Assert.Equal(expectedObject.activity.activitiesAndRewards[0].EndDate.Month, actualObject.activity.activitiesAndRewards[0].EndDate.Month);
            Assert.Equal(expectedObject.activity.activitiesAndRewards[0].EndDate.Day, actualObject.activity.activitiesAndRewards[0].EndDate.Day);
            Assert.Equal(expectedObject.activity.activitiesAndRewards[0].ActivityTypeCode, actualObject.activity.activitiesAndRewards[0].ActivityTypeCode);
            Assert.Equal(expectedObject.activity.activitiesAndRewards[0].ActivityTypeNameFi, actualObject.activity.activitiesAndRewards[0].ActivityTypeNameFi);
            Assert.Equal(expectedObject.activity.activitiesAndRewards[0].ActivityTypeNameEn, actualObject.activity.activitiesAndRewards[0].ActivityTypeNameEn);
            Assert.Equal(expectedObject.activity.activitiesAndRewards[0].ActivityTypeNameSv, actualObject.activity.activitiesAndRewards[0].ActivityTypeNameSv);
            Assert.Equal(expectedObject.activity.activitiesAndRewards[0].RoleCode, actualObject.activity.activitiesAndRewards[0].RoleCode);
            Assert.Equal(expectedObject.activity.activitiesAndRewards[0].RoleNameFi, actualObject.activity.activitiesAndRewards[0].RoleNameFi);
            Assert.Equal(expectedObject.activity.activitiesAndRewards[0].RoleNameEn, actualObject.activity.activitiesAndRewards[0].RoleNameEn);
            Assert.Equal(expectedObject.activity.activitiesAndRewards[0].RoleNameSv, actualObject.activity.activitiesAndRewards[0].RoleNameSv);
            Assert.Equal(expectedObject.activity.activitiesAndRewards[0].OrganizationNameFi, actualObject.activity.activitiesAndRewards[0].OrganizationNameFi);
            Assert.Equal(expectedObject.activity.activitiesAndRewards[0].OrganizationNameSv, actualObject.activity.activitiesAndRewards[0].OrganizationNameSv);
            Assert.Equal(expectedObject.activity.activitiesAndRewards[0].OrganizationNameEn, actualObject.activity.activitiesAndRewards[0].OrganizationNameEn);
            Assert.Equal(expectedObject.activity.activitiesAndRewards[0].DepartmentNameFi, actualObject.activity.activitiesAndRewards[0].DepartmentNameFi);
            Assert.Equal(expectedObject.activity.activitiesAndRewards[0].DepartmentNameSv, actualObject.activity.activitiesAndRewards[0].DepartmentNameSv);
            Assert.Equal(expectedObject.activity.activitiesAndRewards[0].DepartmentNameEn, actualObject.activity.activitiesAndRewards[0].DepartmentNameEn);
            Assert.Equal(expectedObject.activity.activitiesAndRewards[0].Url, actualObject.activity.activitiesAndRewards[0].Url);
            Assert.Equal(expectedObject.activity.activitiesAndRewards[0].sector.Count, actualObject.activity.activitiesAndRewards[0].sector.Count);
            Assert.Equal(expectedObject.activity.activitiesAndRewards[0].sector[0].sectorId, actualObject.activity.activitiesAndRewards[0].sector[0].sectorId);
            Assert.Equal(expectedObject.activity.activitiesAndRewards[0].sector[0].nameFiSector, actualObject.activity.activitiesAndRewards[0].sector[0].nameFiSector);
            Assert.Equal(expectedObject.activity.activitiesAndRewards[0].sector[0].nameEnSector, actualObject.activity.activitiesAndRewards[0].sector[0].nameEnSector);
            Assert.Equal(expectedObject.activity.activitiesAndRewards[0].sector[0].nameSvSector, actualObject.activity.activitiesAndRewards[0].sector[0].nameSvSector);
            Assert.Equal(expectedObject.activity.activitiesAndRewards[0].sector[0].organization.Count, actualObject.activity.activitiesAndRewards[0].sector[0].organization.Count);
            Assert.Equal(expectedObject.activity.activitiesAndRewards[0].sector[0].organization[0].organizationId, actualObject.activity.activitiesAndRewards[0].sector[0].organization[0].organizationId);
            Assert.Equal(expectedObject.activity.activitiesAndRewards[0].sector[0].organization[0].OrganizationNameFi, actualObject.activity.activitiesAndRewards[0].sector[0].organization[0].OrganizationNameFi);
            Assert.Equal(expectedObject.activity.activitiesAndRewards[0].sector[0].organization[0].OrganizationNameEn, actualObject.activity.activitiesAndRewards[0].sector[0].organization[0].OrganizationNameEn);
            Assert.Equal(expectedObject.activity.activitiesAndRewards[0].sector[0].organization[0].OrganizationNameSv, actualObject.activity.activitiesAndRewards[0].sector[0].organization[0].OrganizationNameSv);
            Assert.Equal(expectedObject.activity.activitiesAndRewards[0].DataSources[0].RegisteredDataSource, actualObject.activity.activitiesAndRewards[0].DataSources[0].RegisteredDataSource);
            Assert.Equal(expectedObject.activity.activitiesAndRewards[0].DataSources[0].Organization.NameEn, actualObject.activity.activitiesAndRewards[0].DataSources[0].Organization.NameEn);
            Assert.Equal(expectedObject.activity.activitiesAndRewards[0].DataSources[0].Organization.NameFi, actualObject.activity.activitiesAndRewards[0].DataSources[0].Organization.NameFi);
            Assert.Equal(expectedObject.activity.activitiesAndRewards[0].DataSources[0].Organization.NameSv, actualObject.activity.activitiesAndRewards[0].DataSources[0].Organization.NameSv);
            Assert.Equal(expectedObject.activity.activitiesAndRewards[0].DataSources[0].Organization.SectorId, actualObject.activity.activitiesAndRewards[0].DataSources[0].Organization.SectorId);
            // Assert activity.activitiesAndRewards - activity and reward 2
            Assert.Equal(expectedObject.activity.activitiesAndRewards[1].NameFi, actualObject.activity.activitiesAndRewards[1].NameFi);
            Assert.Equal(expectedObject.activity.activitiesAndRewards[1].NameEn, actualObject.activity.activitiesAndRewards[1].NameEn);
            Assert.Equal(expectedObject.activity.activitiesAndRewards[1].NameSv, actualObject.activity.activitiesAndRewards[1].NameSv);
            Assert.Equal(expectedObject.activity.activitiesAndRewards[1].DescriptionFi, actualObject.activity.activitiesAndRewards[1].DescriptionFi);
            Assert.Equal(expectedObject.activity.activitiesAndRewards[1].DescriptionEn, actualObject.activity.activitiesAndRewards[1].DescriptionEn);
            Assert.Equal(expectedObject.activity.activitiesAndRewards[1].DescriptionSv, actualObject.activity.activitiesAndRewards[1].DescriptionSv);
            Assert.Equal(expectedObject.activity.activitiesAndRewards[1].InternationalCollaboration, actualObject.activity.activitiesAndRewards[1].InternationalCollaboration);
            Assert.Equal(expectedObject.activity.activitiesAndRewards[1].StartDate.Year, actualObject.activity.activitiesAndRewards[1].StartDate.Year);
            Assert.Equal(expectedObject.activity.activitiesAndRewards[1].StartDate.Month, actualObject.activity.activitiesAndRewards[1].StartDate.Month);
            Assert.Equal(expectedObject.activity.activitiesAndRewards[1].StartDate.Day, actualObject.activity.activitiesAndRewards[1].StartDate.Day);
            Assert.Equal(expectedObject.activity.activitiesAndRewards[1].EndDate.Year, actualObject.activity.activitiesAndRewards[1].EndDate.Year);
            Assert.Equal(expectedObject.activity.activitiesAndRewards[1].EndDate.Month, actualObject.activity.activitiesAndRewards[1].EndDate.Month);
            Assert.Equal(expectedObject.activity.activitiesAndRewards[1].EndDate.Day, actualObject.activity.activitiesAndRewards[1].EndDate.Day);
            Assert.Equal(expectedObject.activity.activitiesAndRewards[1].ActivityTypeCode, actualObject.activity.activitiesAndRewards[1].ActivityTypeCode);
            Assert.Equal(expectedObject.activity.activitiesAndRewards[1].ActivityTypeNameFi, actualObject.activity.activitiesAndRewards[1].ActivityTypeNameFi);
            Assert.Equal(expectedObject.activity.activitiesAndRewards[1].ActivityTypeNameEn, actualObject.activity.activitiesAndRewards[1].ActivityTypeNameEn);
            Assert.Equal(expectedObject.activity.activitiesAndRewards[1].ActivityTypeNameSv, actualObject.activity.activitiesAndRewards[1].ActivityTypeNameSv);
            Assert.Equal(expectedObject.activity.activitiesAndRewards[1].RoleCode, actualObject.activity.activitiesAndRewards[1].RoleCode);
            Assert.Equal(expectedObject.activity.activitiesAndRewards[1].RoleNameFi, actualObject.activity.activitiesAndRewards[1].RoleNameFi);
            Assert.Equal(expectedObject.activity.activitiesAndRewards[1].RoleNameEn, actualObject.activity.activitiesAndRewards[1].RoleNameEn);
            Assert.Equal(expectedObject.activity.activitiesAndRewards[1].RoleNameSv, actualObject.activity.activitiesAndRewards[1].RoleNameSv);
            Assert.Equal(expectedObject.activity.activitiesAndRewards[1].OrganizationNameFi, actualObject.activity.activitiesAndRewards[1].OrganizationNameFi);
            Assert.Equal(expectedObject.activity.activitiesAndRewards[1].OrganizationNameSv, actualObject.activity.activitiesAndRewards[1].OrganizationNameSv);
            Assert.Equal(expectedObject.activity.activitiesAndRewards[1].OrganizationNameEn, actualObject.activity.activitiesAndRewards[1].OrganizationNameEn);
            Assert.Equal(expectedObject.activity.activitiesAndRewards[1].DepartmentNameFi, actualObject.activity.activitiesAndRewards[1].DepartmentNameFi);
            Assert.Equal(expectedObject.activity.activitiesAndRewards[1].DepartmentNameSv, actualObject.activity.activitiesAndRewards[1].DepartmentNameSv);
            Assert.Equal(expectedObject.activity.activitiesAndRewards[1].DepartmentNameEn, actualObject.activity.activitiesAndRewards[1].DepartmentNameEn);
            Assert.Equal(expectedObject.activity.activitiesAndRewards[1].Url, actualObject.activity.activitiesAndRewards[1].Url);
            Assert.Equal(expectedObject.activity.activitiesAndRewards[1].sector.Count, actualObject.activity.activitiesAndRewards[1].sector.Count);
            Assert.Equal(expectedObject.activity.activitiesAndRewards[1].sector[0].sectorId, actualObject.activity.activitiesAndRewards[1].sector[0].sectorId);
            Assert.Equal(expectedObject.activity.activitiesAndRewards[1].sector[0].nameFiSector, actualObject.activity.activitiesAndRewards[1].sector[0].nameFiSector);
            Assert.Equal(expectedObject.activity.activitiesAndRewards[1].sector[0].nameEnSector, actualObject.activity.activitiesAndRewards[1].sector[0].nameEnSector);
            Assert.Equal(expectedObject.activity.activitiesAndRewards[1].sector[0].nameSvSector, actualObject.activity.activitiesAndRewards[1].sector[0].nameSvSector);
            Assert.Equal(expectedObject.activity.activitiesAndRewards[1].sector[0].organization.Count, actualObject.activity.activitiesAndRewards[1].sector[0].organization.Count);
            Assert.Equal(expectedObject.activity.activitiesAndRewards[1].sector[0].organization[0].organizationId, actualObject.activity.activitiesAndRewards[1].sector[0].organization[0].organizationId);
            Assert.Equal(expectedObject.activity.activitiesAndRewards[1].sector[0].organization[0].OrganizationNameFi, actualObject.activity.activitiesAndRewards[1].sector[0].organization[0].OrganizationNameFi);
            Assert.Equal(expectedObject.activity.activitiesAndRewards[1].sector[0].organization[0].OrganizationNameEn, actualObject.activity.activitiesAndRewards[1].sector[0].organization[0].OrganizationNameEn);
            Assert.Equal(expectedObject.activity.activitiesAndRewards[1].sector[0].organization[0].OrganizationNameSv, actualObject.activity.activitiesAndRewards[1].sector[0].organization[0].OrganizationNameSv);
            Assert.Equal(expectedObject.activity.activitiesAndRewards[1].DataSources[0].RegisteredDataSource, actualObject.activity.activitiesAndRewards[1].DataSources[0].RegisteredDataSource);
            Assert.Equal(expectedObject.activity.activitiesAndRewards[1].DataSources[0].Organization.NameEn, actualObject.activity.activitiesAndRewards[1].DataSources[0].Organization.NameEn);
            Assert.Equal(expectedObject.activity.activitiesAndRewards[1].DataSources[0].Organization.NameFi, actualObject.activity.activitiesAndRewards[1].DataSources[0].Organization.NameFi);
            Assert.Equal(expectedObject.activity.activitiesAndRewards[1].DataSources[0].Organization.NameSv, actualObject.activity.activitiesAndRewards[1].DataSources[0].Organization.NameSv);
            Assert.Equal(expectedObject.activity.activitiesAndRewards[1].DataSources[0].Organization.SectorId, actualObject.activity.activitiesAndRewards[1].DataSources[0].Organization.SectorId);
            // Assert settings
            Assert.Equal(expectedObject.settings.HighlightOpeness, actualObject.settings.HighlightOpeness);
            Assert.Null(actualObject.settings.GetType().GetProperty("Hidden")); // Hidden is not mapped
            Assert.Null(actualObject.settings.GetType().GetProperty("PublishNewData")); // PublishNewData is not mapped
            // Assert cooperation
            Assert.Equal(expectedObject.cooperation.Count, actualObject.cooperation.Count);
            Assert.Equal(expectedObject.cooperation[0].Id, actualObject.cooperation[0].Id);
            Assert.Equal(expectedObject.cooperation[0].NameFi, actualObject.cooperation[0].NameFi);
            Assert.Equal(expectedObject.cooperation[0].NameEn, actualObject.cooperation[0].NameEn);
            Assert.Equal(expectedObject.cooperation[0].NameSv, actualObject.cooperation[0].NameSv);
            Assert.Equal(expectedObject.cooperation[0].Order, actualObject.cooperation[0].Order);
            Assert.Equal(expectedObject.cooperation[1].Id, actualObject.cooperation[1].Id);
            Assert.Equal(expectedObject.cooperation[1].NameFi, actualObject.cooperation[1].NameFi);
            Assert.Equal(expectedObject.cooperation[1].NameEn, actualObject.cooperation[1].NameEn);
            Assert.Equal(expectedObject.cooperation[1].NameSv, actualObject.cooperation[1].NameSv);
            Assert.Equal(expectedObject.cooperation[1].Order, actualObject.cooperation[1].Order);
            // Assert uniqueDataSources
            Assert.Equal(expectedObject.uniqueDataSources.Count, actualObject.uniqueDataSources.Count);
            Assert.Equal(expectedObject.uniqueDataSources[0].RegisteredDataSource, actualObject.uniqueDataSources[0].RegisteredDataSource);
            Assert.Equal(expectedObject.uniqueDataSources[0].Organization.NameFi, actualObject.uniqueDataSources[0].Organization.NameFi);
            Assert.Equal(expectedObject.uniqueDataSources[0].Organization.NameEn, actualObject.uniqueDataSources[0].Organization.NameEn);
            Assert.Equal(expectedObject.uniqueDataSources[0].Organization.NameSv, actualObject.uniqueDataSources[0].Organization.NameSv);
            Assert.Equal(expectedObject.uniqueDataSources[0].Organization.SectorId, actualObject.uniqueDataSources[0].Organization.SectorId);
            Assert.Equal(expectedObject.uniqueDataSources[1].RegisteredDataSource, actualObject.uniqueDataSources[1].RegisteredDataSource);
            Assert.Equal(expectedObject.uniqueDataSources[1].Organization.NameFi, actualObject.uniqueDataSources[1].Organization.NameFi);
            Assert.Equal(expectedObject.uniqueDataSources[1].Organization.NameEn, actualObject.uniqueDataSources[1].Organization.NameEn);
            Assert.Equal(expectedObject.uniqueDataSources[1].Organization.NameSv, actualObject.uniqueDataSources[1].Organization.NameSv);
            Assert.Equal(expectedObject.uniqueDataSources[1].Organization.SectorId, actualObject.uniqueDataSources[1].Organization.SectorId);
        }
    }
}