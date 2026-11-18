using System;
using System.Data.Common;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using api.Models.Ttv;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace api.Background;

public sealed class ConsentExpiryService(
    IServiceScopeFactory scopeFactory,
    ILogger<ConsentExpiryService> logger) : BackgroundService
{
    private static readonly TimeZoneInfo ScheduleTimeZone = TimeZoneInfo.FindSystemTimeZoneById("Europe/Helsinki");

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            DateTime utcNow = DateTime.UtcNow;
            await Task.Delay(GetNextRunUtc(utcNow) - utcNow, stoppingToken);

            try
            {
                int count = await DeactivateExpiredConsentsAsync(DateTime.UtcNow, stoppingToken);
                logger.LogInformation("Nightly consent expiry deactivated {ConsentCount} consents.", count);
            }
            catch (Exception exception) when (exception is DbException or RetryLimitExceededException)
            {
                logger.LogError(exception, "Nightly consent expiry failed. The job will run again at the next scheduled midnight.");
            }
        }
    }

    public static DateTime GetNextRunUtc(DateTime utcNow)
    {
        DateTime nextMidnight = TimeZoneInfo.ConvertTimeFromUtc(utcNow, ScheduleTimeZone).Date.AddDays(1);
        return TimeZoneInfo.ConvertTimeToUtc(nextMidnight, ScheduleTimeZone);
    }

    public async Task<int> DeactivateExpiredConsentsAsync(DateTime utcNow, CancellationToken cancellationToken)
    {
        await using var scope = scopeFactory.CreateAsyncScope();
        var context = scope.ServiceProvider.GetRequiredService<TtvContext>();
        return await context.FactProfileDataSharingConsents
            .Where(consent => consent.ConsentActive && consent.DateConsentExpire < utcNow)
            .ExecuteUpdateAsync(setters => setters.SetProperty(consent => consent.ConsentActive, false), cancellationToken);
    }
}
