using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Moq;
using Xunit;
using api.Models.Log;
using api.Models.ProfileEditor;
using api.Models.ProfileEditor.Items;
using api.Models.Ttv;
using api.Services;
using Microsoft.EntityFrameworkCore;
using ResearchFi.PersonPublicApi;

namespace api.Tests
{
    [Collection("Public API service tests")]
    public class PublicApiServiceTests
    {
        [Fact(DisplayName = "Get username from national identification number")]
        public void GetUsernameFromNationalIdentificationNumber_01()
        {
            var publicApiService = new PublicApiService(null, null, null, null);

            // Value 010170-999R is a test value for Finnish national identification number. No real person is associated with this value.
            Assert.Equal("5f0c2c8d2107f4700fb5aa1ef717ac03", publicApiService.GetUsernameFromNationalIdentificationNumber("010170-999R"));
        }

        private static TtvContext CreateInMemoryContext(string dbName)
        {
            var options = new DbContextOptionsBuilder<TtvContext>()
                .UseInMemoryDatabase(databaseName: dbName)
                .Options;
            return new TtvContext(options);
        }

        private static ProfileEditorDataResponse CreateProfileEditorData()
        {
            return new ProfileEditorDataResponse
            {
                personal = new ProfileEditorDataPersonal
                {
                    names = new List<ProfileEditorName> { new ProfileEditorName { FirstNames = "Matti", LastName = "Virtanen" } },
                    emails = new List<ProfileEditorEmail> { new ProfileEditorEmail { Value = "matti@example.com" } },
                    telephoneNumbers = new List<ProfileEditorTelephoneNumber> { new ProfileEditorTelephoneNumber { Value = "+358401234567" } },
                    webLinks = new List<ProfileEditorWebLink> { new ProfileEditorWebLink { LinkLabel = "Home", Url = "https://example.com" } },
                    researcherDescriptions = new List<ProfileEditorResearcherDescription>
                    {
                        new ProfileEditorResearcherDescription { ResearchDescriptionFi = "Kuvaus", ResearchDescriptionEn = "Description" }
                    }
                },
                activity = new ProfileEditorDataActivity
                {
                    affiliations = new List<ProfileEditorAffiliation>
                    {
                        new ProfileEditorAffiliation
                        {
                            OrganizationNameEn = "University",
                            PositionNameEn = "Professor",
                            StartDate = new ProfileEditorDate { Year = 2020, Month = 3 }
                        }
                    },
                    educations = new List<ProfileEditorEducation>
                    {
                        new ProfileEditorEducation { NameEn = "PhD", DegreeGrantingInstitutionName = "University", StartDate = new ProfileEditorDate { Year = 2010 } }
                    },
                    publications = new List<ProfileEditorPublication>
                    {
                        new ProfileEditorPublication { PublicationName = "Article", AuthorsText = "Virtanen M", PublicationYear = 2021, Doi = "10.1234/abc", JournalName = "Journal" }
                    },
                    fundingDecisions = new List<ProfileEditorFundingDecision>
                    {
                        new ProfileEditorFundingDecision { ProjectNameEn = "Project", FunderNameEn = "Funder", FundingStartYear = 2019 }
                    },
                    researchDatasets = new List<ProfileEditorResearchDataset>
                    {
                        new ProfileEditorResearchDataset
                        {
                            NameEn = "Dataset",
                            DatasetCreated = 2022,
                            Actor = new List<ProfileEditorActor> { new ProfileEditorActor { actorRole = 2, actorRoleNameEn = "Creator" } },
                            PreferredIdentifiers = new List<ProfileEditorPreferredIdentifier> { new ProfileEditorPreferredIdentifier { PidContent = "10.1234/ds", PidType = "DOI" } }
                        }
                    }
                }
            };
        }

        private static PublicApiService CreateService(TtvContext context, DimUserProfile dimUserProfile, ProfileEditorDataResponse profileData, out Mock<IUserProfileService> userProfileServiceMock)
        {
            userProfileServiceMock = new Mock<IUserProfileService>();
            userProfileServiceMock.Setup(s => s.GetUserprofileByUsername(It.IsAny<string>())).ReturnsAsync(dimUserProfile);
            userProfileServiceMock.Setup(s => s.GetProfileData(It.IsAny<int>(), It.IsAny<LogUserIdentification>(), true)).ReturnsAsync(profileData);
            return new PublicApiService(context, null, null, userProfileServiceMock.Object);
        }

        [Fact(DisplayName = "GetProfileDataForPublicApi - profile not found is not recognized")]
        public async Task GetProfileDataForPublicApi_01()
        {
            using var context = CreateInMemoryContext(nameof(GetProfileDataForPublicApi_01));
            var service = CreateService(context, null, null, out _);

            var response = await service.GetProfileDataForPublicApi("unknown");

            Assert.False(response.PersonProfileRecognized);
            Assert.Null(response.PersonProfileData);
        }

        [Fact(DisplayName = "GetProfileDataForPublicApi - empty username does not look up profile")]
        public async Task GetProfileDataForPublicApi_02()
        {
            using var context = CreateInMemoryContext(nameof(GetProfileDataForPublicApi_02));
            var service = CreateService(context, new DimUserProfile { Id = 1 }, null, out var userProfileServiceMock);

            var response = await service.GetProfileDataForPublicApi("");

            Assert.False(response.PersonProfileRecognized);
            userProfileServiceMock.Verify(s => s.GetUserprofileByUsername(It.IsAny<string>()), Times.Never);
        }

        [Fact(DisplayName = "GetProfileDataForPublicApi - hidden profile is not recognized")]
        public async Task GetProfileDataForPublicApi_03()
        {
            using var context = CreateInMemoryContext(nameof(GetProfileDataForPublicApi_03));
            var service = CreateService(context, new DimUserProfile { Id = 1, Hidden = true }, CreateProfileEditorData(), out _);

            var response = await service.GetProfileDataForPublicApi("alice");

            Assert.False(response.PersonProfileRecognized);
            Assert.Null(response.PersonProfileData);
        }

        [Fact(DisplayName = "GetProfileDataForPublicApi - maps profile data to contract")]
        public async Task GetProfileDataForPublicApi_04()
        {
            using var context = CreateInMemoryContext(nameof(GetProfileDataForPublicApi_04));
            context.DimUserChoices.AddRange(
                new DimUserChoice
                {
                    Id = 1, DimUserProfileId = 1, UserChoiceValue = true, SourceId = "s",
                    DimReferencedataIdAsUserChoiceLabelNavigation = new DimReferencedatum { CodeScheme = "Kiinnostuksen_ilmaiseminen", CodeValue = "2", NameEn = "Interest 2", Order = 2, SourceId = "s", SourceDescription = "s" }
                },
                new DimUserChoice
                {
                    Id = 2, DimUserProfileId = 1, UserChoiceValue = false, SourceId = "s",
                    DimReferencedataIdAsUserChoiceLabelNavigation = new DimReferencedatum { CodeScheme = "Kiinnostuksen_ilmaiseminen", CodeValue = "3", NameEn = "Interest 3", Order = 3, SourceId = "s", SourceDescription = "s" }
                });
            await context.SaveChangesAsync();
            var service = CreateService(context, new DimUserProfile { Id = 1, OrcidId = "0000-0000-0000-0001" }, CreateProfileEditorData(), out var userProfileServiceMock);

            var response = await service.GetProfileDataForPublicApi("alice");

            Assert.True(response.PersonProfileRecognized);
            userProfileServiceMock.Verify(s => s.GetProfileData(1, It.IsAny<LogUserIdentification>(), true), Times.Once);
            PersonProfileData data = response.PersonProfileData;
            Assert.Equal("Matti", data.IsNamedBy.FirstName);
            Assert.Equal("Virtanen", data.IsNamedBy.LastName);
            Assert.Equal("0000-0000-0000-0001", data.PersonIsIdentifiedByORCID);
            Assert.Equal("Description", data.DescriptionOfActivities.En);
            Assert.Equal("Kuvaus", data.DescriptionOfActivities.Fi);
            Assert.Null(data.DescriptionOfActivities.Sv);
            Assert.Equal("matti@example.com", Assert.Single(data.IsReachableFrom.Email));
            Assert.Equal("+358401234567", Assert.Single(data.IsReachableFrom.PhoneNumber));
            Assert.Equal("https://example.com", Assert.Single(data.IsReachableFrom.Homepage).LinkURL);

            Affiliation affiliation = Assert.Single(data.Affiliations);
            Assert.Equal("University", affiliation.AffiliationOrganization.OrganizationName.En);
            Assert.Equal("Professor", affiliation.Position.En);
            Assert.Equal(2020, affiliation.AffiliationStartsOn.Year);
            Assert.Equal(3, affiliation.AffiliationStartsOn.Month);
            Assert.Null(affiliation.AffiliationStartsOn.Day);
            Assert.Null(affiliation.AffiliationEndsOn);

            Education education = Assert.Single(data.HasEducations);
            Assert.Equal("PhD", education.EducationName.En);
            Assert.Equal(2010, education.EducationStartsOn.Year);

            GrantedFunding funding = Assert.Single(data.HasGrantedFunding);
            Assert.Equal("Project", funding.GrantedFundingName.En);
            Assert.Equal("Funder", funding.IsFundedBy.OrganizationName.En);
            Assert.Equal(2019, funding.FundingStartsOn.Year);
            Assert.Null(funding.FundingEndsOn);

            Output output = Assert.Single(data.ContributesToOutput);
            WrittenPublication publication = Assert.Single(output.ContributesToWrittenPublication);
            Assert.Equal("Article", publication.PublicationName);
            Assert.Equal("Virtanen M", publication.ListOfAuthors);
            Assert.Equal(2021, publication.PublicationYear);
            Assert.Equal("10.1234/abc", Assert.Single(publication.WrittenPublicationIsIdentifiedBy).PidContent);
            Assert.Equal("Journal", publication.PublishedOn.PublishedInJournal.JournalName);
            ResearchDataset dataset = Assert.Single(output.ContributesToDataset);
            Assert.Equal("Dataset", Assert.Single(dataset.DatasetTitle).DescriptiveContent);
            Assert.Equal(2022, dataset.DatasetCreatedDate.Year);
            Assert.Equal(2, Assert.Single(dataset.RoleInDataset).CodeValue);
            Assert.Equal("10.1234/ds", Assert.Single(dataset.DatasetIsIdentifiedBy).PidContent);

            ReferenceData interest = Assert.Single(data.CollaborationInterests);
            Assert.Equal(2, interest.CodeValue);
            Assert.Equal("Interest 2", interest.CodeLabel.En);
        }

        [Fact(DisplayName = "GetProfileDataForPublicApi - looks up profile by username derived from person key identifier")]
        public async Task GetProfileDataForPublicApi_05()
        {
            using var context = CreateInMemoryContext(nameof(GetProfileDataForPublicApi_05));
            var service = CreateService(context, null, null, out var userProfileServiceMock);

            await service.GetProfileDataForPublicApi("010170-999R");

            // Value 010170-999R is a test value for Finnish national identification number. No real person is associated with this value.
            userProfileServiceMock.Verify(s => s.GetUserprofileByUsername("5f0c2c8d2107f4700fb5aa1ef717ac03"), Times.Once);
        }

        private static PublicApiService CreateServiceWithResponse(string jsonResponse)
        {
            var handler = new FakeHttpMessageHandler(jsonResponse);
            var httpClient = new HttpClient(handler)
            {
                BaseAddress = new Uri("https://keycloak.example.com/users")
            };
            var factoryMock = new Mock<IHttpClientFactory>();
            factoryMock.Setup(f => f.CreateClient("keycloakClient")).Returns(httpClient);
            return new PublicApiService(null, factoryMock.Object, null, null);
        }

        private class FakeHttpMessageHandler : HttpMessageHandler
        {
            private readonly string _responseBody;

            public FakeHttpMessageHandler(string responseBody)
            {
                _responseBody = responseBody;
            }

            protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
            {
                return Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = new StringContent(_responseBody, Encoding.UTF8, "application/json")
                });
            }
        }
    }
}