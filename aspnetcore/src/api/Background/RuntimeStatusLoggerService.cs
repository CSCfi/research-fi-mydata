/*
 * Periodically logs runtime environment status (memory, CPU, background queue length,
 * GC/thread pool stats) to aid operational visibility when running in an OpenShift pod.
 */

using System;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using api.Models.Log;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

public class RuntimeStatusLoggerService : BackgroundService
{
    private readonly IBackgroundTaskQueue _taskQueue;
    private readonly ILogger<RuntimeStatusLoggerService> _logger;
    private readonly TimeSpan _interval;

    public RuntimeStatusLoggerService(
        IBackgroundTaskQueue taskQueue,
        ILogger<RuntimeStatusLoggerService> logger,
        IConfiguration configuration)
    {
        _taskQueue = taskQueue;
        _logger = logger;

        int intervalSeconds = configuration.GetValue("RuntimeStatusLogging:IntervalSeconds", 300);
        // IntervalSeconds <= 0 disables periodic logging.
        _interval = intervalSeconds > 0 ? TimeSpan.FromSeconds(intervalSeconds) : TimeSpan.Zero;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        if (_interval <= TimeSpan.Zero)
        {
            return;
        }

        Process process = Process.GetCurrentProcess();
        DateTime startTimeUtc = DateTime.UtcNow;
        TimeSpan? previousCpuTime = null;
        DateTime? previousSampleTimeUtc = null;

        // Read once: the cgroup CPU quota is fixed for the container's lifetime.
        double cpuQuotaCores = GetCpuQuotaCores();

        // Log immediately on startup so status is visible without waiting for the first interval.
        LogStatus();

        using PeriodicTimer timer = new(_interval);

        while (await timer.WaitForNextTickAsync(stoppingToken))
        {
            LogStatus();
        }

        void LogStatus()
        {
            process.Refresh();

            DateTime nowUtc = DateTime.UtcNow;
            TimeSpan currentCpuTime = process.TotalProcessorTime;

            int? cpuUsagePercent = null;
            if (previousCpuTime.HasValue && previousSampleTimeUtc.HasValue)
            {
                cpuUsagePercent = CalculateCpuUsagePercent(
                    cpuTimeDelta: currentCpuTime - previousCpuTime.Value,
                    wallTimeDelta: nowUtc - previousSampleTimeUtc.Value,
                    cpuQuotaCores: cpuQuotaCores);
            }
            previousCpuTime = currentCpuTime;
            previousSampleTimeUtc = nowUtc;

            ThreadPool.GetAvailableThreads(out int availableWorkerThreads, out int availableIoThreads);

            LogRuntimeStatus logRuntimeStatus = new(
                memoryWorkingSetBytes: Environment.WorkingSet,
                gcHeapBytes: GC.GetTotalMemory(false),
                gcTotalAvailableMemoryBytes: GC.GetGCMemoryInfo().TotalAvailableMemoryBytes,
                cpuUsagePercent: cpuUsagePercent,
                cpuQuotaCores: cpuQuotaCores,
                processorCount: Environment.ProcessorCount,
                backgroundQueueLength: _taskQueue.Count,
                gen0Collections: GC.CollectionCount(0),
                gen1Collections: GC.CollectionCount(1),
                gen2Collections: GC.CollectionCount(2),
                threadPoolAvailableWorkerThreads: availableWorkerThreads,
                threadPoolAvailableIoThreads: availableIoThreads,
                uptimeSeconds: (long)(nowUtc - startTimeUtc).TotalSeconds);

            _logger.LogInformation(LogContent.MESSAGE_TEMPLATE_RUNTIME_STATUS, logRuntimeStatus);
        }
    }

    // Percentage of the container's CPU quota (fractional cores, e.g. 0.5 for a 500m OpenShift limit) consumed during wallTimeDelta.
    public static int? CalculateCpuUsagePercent(TimeSpan cpuTimeDelta, TimeSpan wallTimeDelta, double cpuQuotaCores)
    {
        if (wallTimeDelta <= TimeSpan.Zero || cpuQuotaCores <= 0)
        {
            return null;
        }

        double percent = 100.0 * cpuTimeDelta.TotalMilliseconds / (wallTimeDelta.TotalMilliseconds * cpuQuotaCores);
        return (int)Math.Round(percent, MidpointRounding.AwayFromZero);
    }

    // Fractional CPU quota in cores (e.g. 0.5 for an OpenShift "500m" limit). Environment.ProcessorCount
    // rounds this up to the nearest whole core (minimum 1), which understates CPU% for sub-1-core limits,
    // so the cgroup quota files are read directly when present, falling back to Environment.ProcessorCount
    // (e.g. local/non-container dev, or no CPU limit set).
    public static double GetCpuQuotaCores()
    {
        try
        {
            const string cgroupV2Path = "/sys/fs/cgroup/cpu.max";
            if (File.Exists(cgroupV2Path))
            {
                string[] parts = File.ReadAllText(cgroupV2Path).Split(' ', StringSplitOptions.RemoveEmptyEntries);
                if (parts.Length == 2 && parts[0] != "max"
                    && long.TryParse(parts[0], out long quota)
                    && long.TryParse(parts[1], out long period)
                    && period > 0)
                {
                    return (double)quota / period;
                }
            }
            else
            {
                const string cgroupV1QuotaPath = "/sys/fs/cgroup/cpu/cpu.cfs_quota_us";
                const string cgroupV1PeriodPath = "/sys/fs/cgroup/cpu/cpu.cfs_period_us";
                if (File.Exists(cgroupV1QuotaPath) && File.Exists(cgroupV1PeriodPath)
                    && long.TryParse(File.ReadAllText(cgroupV1QuotaPath).Trim(), out long quota)
                    && long.TryParse(File.ReadAllText(cgroupV1PeriodPath).Trim(), out long period)
                    && quota > 0 && period > 0)
                {
                    return (double)quota / period;
                }
            }
        }
        catch (IOException)
        {
            // Fall back below.
        }
        catch (UnauthorizedAccessException)
        {
            // Fall back below.
        }

        return Environment.ProcessorCount;
    }
}
