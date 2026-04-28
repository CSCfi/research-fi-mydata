using Xunit;
using api.Services;
using api.Models.Ttv;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.Abstractions;
using System.Threading.Tasks;
using System.Collections.Generic;
using api.Models.Common;

namespace api.Tests
{
    public class CooperationChoicesServiceTestData
    {
        public DimUserProfile UserProfile { get; private set; }

        public static CooperationChoicesServiceTestData Create()
        {
            var data = new CooperationChoicesServiceTestData();
            data.UserProfile = new DimUserProfile {
                Id = 1,
                SourceId = "Source1",
                PublishNewOrcidData = true,
                HighlightOpeness = true,
                Hidden = true,
                DimKnownPersonId = 5,
                DimKnownPerson = new DimKnownPerson
                {
                    Id = 5,
                    SourceId = "Source1"
                },
                AllowAllSubscriptions = false,
                DimUserChoices = new List<DimUserChoice>
                {
                    new DimUserChoice
                    {
                        Id = 1,
                        DimUserProfileId = 1,
                        DimReferencedataIdAsUserChoiceLabelNavigation = new DimReferencedatum
                        {
                            CodeScheme = Constants.ReferenceDataCodeSchemes.USER_CHOICES,
                            CodeValue = "",
                            NameFi = "Yhteistyö 1",
                            NameEn = "Cooperation 1",
                            NameSv = "Samarbete 1",
                            Order = 1,
                            SourceDescription = "Source1",
                            SourceId = "Source1"
                        },
                        UserChoiceValue = true,
                        SourceId = "Source1"
                    },
                    new DimUserChoice
                    {
                        Id = 2,
                        DimUserProfileId = 1,
                        DimReferencedataIdAsUserChoiceLabelNavigation = new DimReferencedatum
                        {
                            CodeScheme = Constants.ReferenceDataCodeSchemes.USER_CHOICES,
                            CodeValue = "",
                            NameFi = "Yhteistyö 2",
                            NameEn = "Cooperation 2",
                            NameSv = "Samarbete 2",
                            Order = 2,
                            SourceDescription = "Source1",
                            SourceId = "Source1"
                        },
                        UserChoiceValue = false,
                        SourceId = "Source1"
                    },
                    new DimUserChoice
                    {
                        Id = 3,
                        DimUserProfileId = 1,
                        DimReferencedataIdAsUserChoiceLabelNavigation = new DimReferencedatum
                        {
                            CodeScheme = Constants.ReferenceDataCodeSchemes.USER_CHOICES,
                            CodeValue = "",
                            NameFi = "Yhteistyö 3",
                            NameEn = "Cooperation 3",
                            NameSv = "Samarbete 3",
                            Order = 3,
                            SourceDescription = "Source1",
                            SourceId = "Source1"
                        },
                        UserChoiceValue = true,
                        SourceId = "Source1"
                    }
                }
            };
            return data;
        }

        public async Task SeedAsync(TtvContext context)
        {
            context.DimUserProfiles.Add(UserProfile);
            context.DimKnownPeople.Add(UserProfile.DimKnownPerson);
            await context.SaveChangesAsync();
        }
    }


    [Collection("Cooperation choices service tests")]
    public class CooperationChoicesServiceTests
    {
        private TtvContext CreateInMemoryContext(string dbName)
        {
            var options = new DbContextOptionsBuilder<TtvContext>()
                .UseInMemoryDatabase(databaseName: dbName)
                .Options;
            return new TtvContext(options);
        }

        private CooperationChoicesService CreateService(TtvContext context)
        {
            return new CooperationChoicesService(
                ttvContext: context,
                logger: new NullLogger<CooperationChoicesService>());
        }

        [Fact(DisplayName = "GetCooperationChoices retrieves correct choices for user profile when forElasticsearch is false")]
        public async Task GetCooperationChoices_RetrievesCorrectChoices_ForElasticsearchFalse()
        {
            using var context = CreateInMemoryContext(nameof(GetCooperationChoices_RetrievesCorrectChoices_ForElasticsearchFalse));
            var testData = CooperationChoicesServiceTestData.Create();
            await testData.SeedAsync(context);

            var service = CreateService(context);
            var results = await service.GetCooperationChoices(
                userprofileId: 1,
                forElasticsearch: false
            );

            Assert.NotEmpty(results);
            Assert.Equal(3, results.Count);
            Assert.Contains(results, c => c.Id == 1 && c.Selected == true && c.NameFi == "Yhteistyö 1" && c.NameSv == "Samarbete 1" && c.NameEn == "Cooperation 1" && c.Order == 1);
            Assert.Contains(results, c => c.Id == 2 && c.Selected == false && c.NameFi == "Yhteistyö 2" && c.NameSv == "Samarbete 2" && c.NameEn == "Cooperation 2" && c.Order == 2);
            Assert.Contains(results, c => c.Id == 3 && c.Selected == true && c.NameFi == "Yhteistyö 3" && c.NameSv == "Samarbete 3" && c.NameEn == "Cooperation 3" && c.Order == 3);
        }

        [Fact(DisplayName = "GetCooperationChoices retrieves correct choices for user profile when forElasticsearch is true")]
        public async Task GetCooperationChoices_RetrievesCorrectChoices_ForElasticsearchTrue()
        {
            using var context = CreateInMemoryContext(nameof(GetCooperationChoices_RetrievesCorrectChoices_ForElasticsearchTrue));
            var testData = CooperationChoicesServiceTestData.Create();
            await testData.SeedAsync(context);

            var service = CreateService(context);
            var results = await service.GetCooperationChoices(
                userprofileId: 1,
                forElasticsearch: true
            );

            Assert.NotEmpty(results);
            Assert.Equal(2, results.Count);
            Assert.Contains(results, c => c.Id == 1 && c.Selected == true && c.NameFi == "Yhteistyö 1" && c.NameSv == "Samarbete 1" && c.NameEn == "Cooperation 1" && c.Order == 1);
            Assert.Contains(results, c => c.Id == 3 && c.Selected == true && c.NameFi == "Yhteistyö 3" && c.NameSv == "Samarbete 3" && c.NameEn == "Cooperation 3" && c.Order == 3);
        }
    }
}