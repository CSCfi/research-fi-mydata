using System;
using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using api.Models.Api;
using api.Models.Ttv;
using api.Services;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace api.Tests
{
    public class ConsentServiceTests
    {
        private static async Task<(SqliteConnection Connection, TtvContext Context)> CreateContextAsync()
        {
            var connection = new SqliteConnection("DataSource=:memory:");
            await connection.OpenAsync();
            using (var command = connection.CreateCommand())
            {
                command.CommandText = "PRAGMA foreign_keys = OFF;";
                command.ExecuteNonQuery();
            }

            var options = new DbContextOptionsBuilder<TtvContext>()
                .UseSqlite(connection)
                .Options;
            var context = new TtvContext(options);
            await context.Database.EnsureCreatedAsync();
            return (connection, context);
        }

        private static async Task AddTargetAsync(TtvContext context, int targetId, int organizationId, int controllerId, int purposeId, string rorId)
        {
            if (!await context.DimOrganizations.AnyAsync(item => item.Id == organizationId))
            {
                context.DimOrganizations.Add(new DimOrganization
                {
                    Id = organizationId,
                    DimSectorid = 1,
                    DimRegisteredDataSourceId = 1,
                    NameFi = $"Organisaatio {organizationId}",
                    NameSv = $"Organisation {organizationId}",
                    NameEn = $"Organization {organizationId}",
                    SourceId = "test"
                });
            }

            if (!await context.FactOrganizationGenerals.AnyAsync(item => item.Id == controllerId))
            {
                context.FactOrganizationGenerals.Add(new FactOrganizationGeneral
                {
                    Id = controllerId,
                    DimOrganizationId = organizationId,
                    DimIdentifierlessOrganizationId = -1,
                    ParentOrgGeneralId = -1,
                    SourceId = "test"
                });
            }

            if (!await context.DimReferencedata.AnyAsync(item => item.Id == purposeId))
            {
                context.DimReferencedata.Add(new DimReferencedatum
                {
                    Id = purposeId,
                    CodeScheme = "profile_data_use_purpose",
                    CodeValue = $"purpose-{purposeId}",
                    NameFi = $"Tarkoitus {purposeId}",
                    NameEn = $"Purpose {purposeId}",
                    NameSv = $"Ändamål {purposeId}",
                    SourceId = "test",
                    SourceDescription = "test"
                });
            }

            if (rorId != null)
            {
                context.DimPids.Add(new DimPid
                {
                    Id = targetId + 1000,
                    DimOrganizationId = organizationId,
                    PidType = "RORID",
                    PidContent = rorId,
                    SourceId = "test"
                });
            }

            context.DimConsentTargets.Add(new DimConsentTarget
            {
                Id = targetId,
                ControllerOrganizationId = controllerId,
                PurposeCodeId = purposeId
            });
            await context.SaveChangesAsync();
        }

        private static void AddConsent(TtvContext context, int profileId, int controllerId, DateTime? expiry = null, DateTime? withdrawn = null)
        {
            var now = DateTime.UtcNow;
            context.FactProfileDataSharingConsents.Add(new FactProfileDataSharingConsent
            {
                DimUserProfileId = profileId,
                ControllerOrganizationId = controllerId,
                DateConsentGranted = now,
                DateConsentExpire = expiry,
                DateConsentWithdrawn = withdrawn,
                Created = now
            });
        }

        [Fact]
        public async Task GetTargetsAsync_ProjectsPurposesAndFiltersByNormalizedRor()
        {
            var (connection, context) = await CreateContextAsync();
            using (connection)
            using (context)
            {
                await AddTargetAsync(context, targetId: 30, organizationId: 1, controllerId: 10, purposeId: 20, rorId: "029pk6x14");
                await AddTargetAsync(context, targetId: 31, organizationId: 2, controllerId: 11, purposeId: 21, rorId: null);
                await AddTargetAsync(context, targetId: 32, organizationId: 3, controllerId: 12, purposeId: 22, rorId: "other-ror");
                await AddTargetAsync(context, targetId: 33, organizationId: 4, controllerId: 13, purposeId: 23, rorId: "\t");
                var service = new ConsentService(context);

                var allTargets = await service.GetTargetsAsync(null, null, CancellationToken.None);
                var filteredTargets = await service.GetTargetsAsync("  029PK6X14  ", null, CancellationToken.None);
                var unknownTargets = await service.GetTargetsAsync("not-a-ror", null, CancellationToken.None);

                Assert.Equal(new[] { 30, 32 }, allTargets.Select(item => item.ConsentTargetId));
                Assert.Equal("Organisaatio 1", allTargets[0].ConsentController.OrganizationName.Fi);
                Assert.Equal("029pk6x14", allTargets[0].ConsentController.Ror);
                Assert.Equal("Purpose 20", Assert.Single(allTargets[0].ConsentPurpose).CodeLabel.En);
                Assert.Equal("purpose-20", allTargets[0].ConsentPurpose[0].CodeValue);
                Assert.Null(allTargets[0].Consent.ConsentGrantedDate);
                Assert.Equal(new[] { 30 }, filteredTargets.Select(item => item.ConsentTargetId));
                Assert.Empty(unknownTargets);
            }
        }

        [Fact]
        public async Task GetTargetsAsync_IncludesOnlyRequestingProfilesActiveConsentForEachTarget()
        {
            var (connection, context) = await CreateContextAsync();
            using (connection)
            using (context)
            {
                await AddTargetAsync(context, 30, 1, 10, 20, "ror-one");
                await AddTargetAsync(context, 31, 1, 10, 21, null);
                await AddTargetAsync(context, 32, 2, 11, 22, "ror-two");
                await AddTargetAsync(context, 33, 3, 12, 23, "ror-three");
                await AddTargetAsync(context, 34, 4, 13, 24, "ror-four");
                AddConsent(context, 1, 10, expiry: DateTime.UtcNow.AddDays(1));
                AddConsent(context, 2, 11);
                AddConsent(context, 1, 12, expiry: DateTime.UtcNow.AddDays(-1));
                AddConsent(context, 1, 13, withdrawn: DateTime.UtcNow);
                await context.SaveChangesAsync();

                var service = new ConsentService(context);
                var targets = await service.GetTargetsAsync(null, 1, CancellationToken.None);
                var filtered = await service.GetTargetsAsync("ror-one", 1, CancellationToken.None);
                var noProfile = await service.GetTargetsAsync(null, null, CancellationToken.None);

                Assert.Equal(new[] { 30, 31, 32, 33, 34 }, targets.Select(item => item.ConsentTargetId));
                Assert.NotNull(targets[0].Consent.ConsentGrantedDate);
                Assert.NotNull(targets[0].Consent.ConsentExpireDate);
                Assert.Equal(targets[0].Consent, targets[1].Consent);
                Assert.All(targets.Skip(2), target => Assert.Null(target.Consent.ConsentGrantedDate));
                Assert.Equal(new[] { 30, 31 }, filtered.Select(item => item.ConsentTargetId));
                Assert.All(filtered, target => Assert.NotNull(target.Consent.ConsentGrantedDate));
                Assert.All(noProfile, target => Assert.Null(target.Consent.ConsentGrantedDate));
            }
        }

        [Fact]
        public async Task GetConsentsAsync_ReturnsOnlyActiveConsentsForRequestedProfile()
        {
            var (connection, context) = await CreateContextAsync();
            using (connection)
            using (context)
            {
                await AddTargetAsync(context, targetId: 30, organizationId: 1, controllerId: 10, purposeId: 20, rorId: "029pk6x14");
                await AddTargetAsync(context, targetId: 31, organizationId: 1, controllerId: 10, purposeId: 21, rorId: null);
                await AddTargetAsync(context, targetId: 32, organizationId: 2, controllerId: 13, purposeId: 22, rorId: null);
                await AddTargetAsync(context, targetId: 33, organizationId: 3, controllerId: 14, purposeId: 23, rorId: "\t");
                AddConsent(context, profileId: 1, controllerId: 10, expiry: DateTime.UtcNow.AddDays(1));
                AddConsent(context, profileId: 1, controllerId: 13);
                AddConsent(context, profileId: 1, controllerId: 14);
                AddConsent(context, profileId: 1, controllerId: 11, expiry: DateTime.UtcNow.AddDays(-1));
                AddConsent(context, profileId: 2, controllerId: 10, expiry: DateTime.UtcNow.AddDays(1));
                AddConsent(context, profileId: 1, controllerId: 12, withdrawn: DateTime.UtcNow);
                await context.SaveChangesAsync();

                var consents = await new ConsentService(context).GetConsentsAsync(1, CancellationToken.None);

                Assert.Equal(new[] { 30, 31 }, consents.Select(consent => consent.ConsentTargetId));
                Assert.All(consents, consent =>
                {
                    Assert.Equal("029pk6x14", consent.ConsentController.Ror);
                    Assert.Equal("Organisaatio 1", consent.ConsentController.OrganizationName.Fi);
                    Assert.NotNull(consent.Consent);
                    Assert.NotNull(consent.Consent.ConsentExpireDate);
                });
                Assert.Equal("purpose-20", Assert.Single(consents[0].ConsentPurpose).CodeValue);
                Assert.Equal("purpose-21", Assert.Single(consents[1].ConsentPurpose).CodeValue);
            }
        }

        [Fact]
        public async Task ConsentTargets_SerializeWithSchemaPropertyNamesAndOmitNullOptionalValues()
        {
            var (connection, context) = await CreateContextAsync();
            using (connection)
            using (context)
            {
                await AddTargetAsync(context, 30, 1, 10, 20, "029pk6x14");
                var organization = await context.DimOrganizations.SingleAsync();
                organization.NameSv = null;
                var purpose = await context.DimReferencedata.SingleAsync();
                purpose.NameSv = null;
                AddConsent(context, 1, 10);
                await context.SaveChangesAsync();
                var service = new ConsentService(context);

                var target = Assert.Single(await service.GetTargetsAsync(null, null, CancellationToken.None));
                var grantedTarget = Assert.Single(await service.GetTargetsAsync(null, 1, CancellationToken.None));
                var activeTarget = Assert.Single(await service.GetConsentsAsync(1, CancellationToken.None));
                using var targetsJson = JsonDocument.Parse(JsonSerializer.Serialize(
                    new ApiResponseConsentTargetsGet(true, "", new[] { target }), JsonSerializerOptions.Web));
                using var consentsJson = JsonDocument.Parse(JsonSerializer.Serialize(
                    new ApiResponseConsentGet(true, "", new[] { activeTarget }), JsonSerializerOptions.Web));

                var targetJson = targetsJson.RootElement.GetProperty("data")[0];
                Assert.Equal(new[] { "consentTargetId", "consentController", "consentPurpose", "consent" },
                    targetJson.EnumerateObject().Select(property => property.Name));
                Assert.Equal(JsonValueKind.Object, targetJson.GetProperty("consent").ValueKind);
                Assert.Empty(targetJson.GetProperty("consent").EnumerateObject());
                var controller = targetJson.GetProperty("consentController");
                Assert.Equal(new[] { "organizationName", "ROR" },
                    controller.EnumerateObject().Select(property => property.Name));
                Assert.Equal("029pk6x14", controller.GetProperty("ROR").GetString());
                Assert.Equal("Organisaatio 1", controller.GetProperty("organizationName").GetProperty("fi").GetString());
                Assert.False(controller.GetProperty("organizationName").TryGetProperty("sv", out _));
                var code = Assert.Single(targetJson.GetProperty("consentPurpose").EnumerateArray());
                Assert.Equal(new[] { "codeLabel", "codeValue" },
                    code.EnumerateObject().Select(property => property.Name));
                Assert.Equal("purpose-20", code.GetProperty("codeValue").GetString());
                Assert.Equal("Purpose 20", code.GetProperty("codeLabel").GetProperty("en").GetString());
                Assert.False(code.GetProperty("codeLabel").TryGetProperty("sv", out _));

                var consentJson = consentsJson.RootElement.GetProperty("data")[0].GetProperty("consent");
                Assert.Equal(JsonValueKind.String, consentJson.GetProperty("consentGrantedDate").ValueKind);
                Assert.False(consentJson.TryGetProperty("consentWithdrawnDate", out _));
                Assert.False(consentJson.TryGetProperty("consentExpireDate", out _));
                using var grantedJson = JsonDocument.Parse(JsonSerializer.Serialize(
                    new ApiResponseConsentTargetsGet(true, "", new[] { grantedTarget }), JsonSerializerOptions.Web));
                Assert.Equal(JsonValueKind.String, grantedJson.RootElement.GetProperty("data")[0]
                    .GetProperty("consent").GetProperty("consentGrantedDate").ValueKind);
            }
        }

        [Fact]
        public async Task GrantConsentAsync_UsesOrganizationKeyAndCreatesUtcFiveYearConsent()
        {
            var (connection, context) = await CreateContextAsync();
            using (connection)
            using (context)
            {
                await AddTargetAsync(context, targetId: 30, organizationId: 1, controllerId: 10, purposeId: 20, rorId: null);
                await AddTargetAsync(context, targetId: 31, organizationId: 1, controllerId: 10, purposeId: 21, rorId: null);
                var service = new ConsentService(context);
                DateTime beforeGrant = DateTime.UtcNow;

                ConsentGrantResult result = await service.GrantConsentAsync(1, 30, CancellationToken.None);
                DateTime afterGrant = DateTime.UtcNow;
                var consent = await context.FactProfileDataSharingConsents.SingleAsync();

                Assert.Equal(ConsentGrantResult.Created, result);
                Assert.InRange(consent.DateConsentGranted, beforeGrant, afterGrant);
                Assert.Equal(consent.DateConsentGranted.AddYears(5), consent.DateConsentExpire);
                Assert.Null(consent.DateConsentWithdrawn);
                Assert.Equal(ConsentGrantResult.ConsentAlreadyExists,
                    await service.GrantConsentAsync(1, 31, CancellationToken.None));
                Assert.Equal(ConsentGrantResult.TargetNotFound,
                    await service.GrantConsentAsync(1, 999, CancellationToken.None));
            }
        }

        [Fact]
        public async Task WithdrawConsentAsync_DeletesOnlyTheRequestingProfilesConsentAndAllowsRegrant()
        {
            var (connection, context) = await CreateContextAsync();
            using (connection)
            using (context)
            {
                await AddTargetAsync(context, targetId: 30, organizationId: 1, controllerId: 10, purposeId: 20, rorId: null);
                AddConsent(context, profileId: 1, controllerId: 10);
                AddConsent(context, profileId: 2, controllerId: 10);
                await context.SaveChangesAsync();
                var service = new ConsentService(context);

                Assert.False(await service.WithdrawConsentAsync(3, 30, CancellationToken.None));
                Assert.False(await service.WithdrawConsentAsync(1, 999, CancellationToken.None));
                Assert.True(await service.WithdrawConsentAsync(1, 30, CancellationToken.None));
                Assert.Equal(1, await context.FactProfileDataSharingConsents.CountAsync());

                Assert.Equal(ConsentGrantResult.Created, await service.GrantConsentAsync(1, 30, CancellationToken.None));
                Assert.Equal(2, await context.FactProfileDataSharingConsents.CountAsync());
            }
        }
    }
}
