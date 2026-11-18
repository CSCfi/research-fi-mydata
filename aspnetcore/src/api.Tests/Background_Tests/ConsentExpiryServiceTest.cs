using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using api.Background;
using api.Models.Common;
using api.Models.Ttv;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging.Abstractions;
using Xunit;

namespace api.Tests;

public class ConsentExpiryServiceTest
{
    [Theory]
    [InlineData(3, 29, 21)] // First evening after the switch to summer time.
    [InlineData(10, 25, 22)] // First evening after the switch to standard time.
    public void GetNextRunUtc_ReturnsNextHelsinkiMidnight(int month, int day, int expectedUtcHour)
    {
        var utcNow = new DateTime(2026, month, day, 0, 0, 0, DateTimeKind.Utc);

        Assert.Equal(new DateTime(2026, month, day, expectedUtcHour, 0, 0, DateTimeKind.Utc),
            ConsentExpiryService.GetNextRunUtc(utcNow));
    }

    [Fact]
    public async Task DeactivateExpiredConsentsAsync_OnlyDeactivatesActiveConsentsPastTheirExpiry()
    {
        var now = new DateTime(2026, 11, 18, 22, 0, 0, DateTimeKind.Utc);
        var withdrawn = now.AddDays(-1);
        await using var connection = new SqliteConnection("Data Source=:memory:;Foreign Keys=False");
        await connection.OpenAsync();
        await using var services = new ServiceCollection()
            .AddDbContext<TtvContext>(options => options.UseSqlite(connection))
            .BuildServiceProvider(validateScopes: true);
        var scopeFactory = services.GetRequiredService<IServiceScopeFactory>();
        await using var testScope = scopeFactory.CreateAsyncScope();
        var context = testScope.ServiceProvider.GetRequiredService<TtvContext>();
        await context.Database.EnsureCreatedAsync();
        context.FactProfileDataSharingConsents.AddRange(
            CreateConsent(1, true, now.AddSeconds(-1)),
            CreateConsent(2, true, now),
            CreateConsent(3, true, now.AddSeconds(1)),
            CreateConsent(4, true, null),
            CreateConsent(5, false, now.AddSeconds(-1), withdrawn));
        await context.SaveChangesAsync();

        using var service = new ConsentExpiryService(scopeFactory, NullLogger<ConsentExpiryService>.Instance);
        Assert.Equal(1, await service.DeactivateExpiredConsentsAsync(now, CancellationToken.None));
        Assert.Equal(0, await service.DeactivateExpiredConsentsAsync(now, CancellationToken.None));

        var consents = await context.FactProfileDataSharingConsents.AsNoTracking().OrderBy(consent => consent.Id).ToListAsync();
        Assert.Equal(new[] { false, true, true, true, false }, consents.Select(consent => consent.ConsentActive));
        Assert.Null(consents[0].DateConsentWithdrawn);
        Assert.Equal(withdrawn, consents[4].DateConsentWithdrawn);

        FactProfileDataSharingConsent CreateConsent(int id, bool active, DateTime? expiry, DateTime? withdrawal = null) =>
            new()
            {
                Id = id,
                ConsentActive = active,
                DateConsentExpire = expiry,
                DateConsentWithdrawn = withdrawal,
                DateConsentGranted = now.AddYears(-Constants.Consent.EXPIRY_YEARS),
                Created = now.AddYears(-Constants.Consent.EXPIRY_YEARS)
            };
    }
}
