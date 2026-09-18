/*
 * Periodically logs runtime environment status (memory, CPU, background queue length,
 * GC/thread pool stats) to aid operational visibility when running in an OpenShift pod.
 */

using System;
using System.Diagnostics;
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

        using PeriodicTimer timer = new(_interval);

        while (await timer.WaitForNextTickAsync(stoppingToken))
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
                    processorCount: Environment.ProcessorCount);
            }
            previousCpuTime = currentCpuTime;
            previousSampleTimeUtc = nowUtc;

            ThreadPool.GetAvailableThreads(out int availableWorkerThreads, out int availableIoThreads);

            LogRuntimeStatus logRuntimeStatus = new(
                memoryWorkingSetBytes: Environment.WorkingSet,
                gcHeapBytes: GC.GetTotalMemory(false),
                gcTotalAvailableMemoryBytes: GC.GetGCMemoryInfo().TotalAvailableMemoryBytes,
                cpuUsagePercent: cpuUsagePercent,
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

    // Percentage of available CPU capacity (across all processor count) consumed during wallTimeDelta.
    public static int? CalculateCpuUsagePercent(TimeSpan cpuTimeDelta, TimeSpan wallTimeDelta, int processorCount)
    {
        if (wallTimeDelta <= TimeSpan.Zero || processorCount <= 0)
        {
            return null;
        }

        double percent = 100.0 * cpuTimeDelta.TotalMilliseconds / (wallTimeDelta.TotalMilliseconds * processorCount);
        return (int)Math.Round(percent, MidpointRounding.AwayFromZero);
    }
}
