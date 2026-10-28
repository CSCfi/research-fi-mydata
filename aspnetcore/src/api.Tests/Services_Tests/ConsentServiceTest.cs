using System;
using System.Linq;
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
                var service = new ConsentService(context);

                var allTargets = await service.GetTargetsAsync(null, CancellationToken.None);
                var filteredTargets = await service.GetTargetsAsync("  029PK6X14  ", CancellationToken.None);
                var unknownTargets = await service.GetTargetsAsync("not-a-ror", CancellationToken.None);

                Assert.Equal(3, allTargets.Count);
                Assert.Null(allTargets.Single(item => item.ConsentTargetId == 31).RorId);
                Assert.Equal("Organisaatio 1", allTargets[0].OrganizationNameFi);
                Assert.Equal("Purpose 20", allTargets[0].PurposeNameEn);
                Assert.Equal("Ändamål 20", allTargets[0].PurposeNameSv);
                Assert.Equal(new[] { 30 }, filteredTargets.Select(item => item.ConsentTargetId));
                Assert.Empty(unknownTargets);
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
                AddConsent(context, profileId: 1, controllerId: 10, expiry: DateTime.UtcNow.AddDays(1));
                AddConsent(context, profileId: 1, controllerId: 11, expiry: DateTime.UtcNow.AddDays(-1));
                AddConsent(context, profileId: 2, controllerId: 10, expiry: DateTime.UtcNow.AddDays(1));
                AddConsent(context, profileId: 1, controllerId: 12, withdrawn: DateTime.UtcNow);
                await context.SaveChangesAsync();

                var consents = await new ConsentService(context).GetConsentsAsync(1, CancellationToken.None);

                var consent = Assert.Single(consents);
                Assert.Equal(30, consent.ConsentTargetId);
                Assert.Equal("029pk6x14", consent.RorId);
                Assert.Equal("Organisaatio 1", consent.OrganizationNameFi);
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
