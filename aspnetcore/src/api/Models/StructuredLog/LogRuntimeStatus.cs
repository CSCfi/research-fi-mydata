namespace api.Models.Log
{
    public class LogRuntimeStatus
    {
        public LogRuntimeStatus(
            long memoryWorkingSetBytes,
            long gcHeapBytes,
            long gcTotalAvailableMemoryBytes,
            int? cpuUsagePercent,
            int processorCount,
            int backgroundQueueLength,
            int gen0Collections,
            int gen1Collections,
            int gen2Collections,
            int threadPoolAvailableWorkerThreads,
            int threadPoolAvailableIoThreads,
            long uptimeSeconds)
        {
            MemoryWorkingSetBytes = memoryWorkingSetBytes;
            GcHeapBytes = gcHeapBytes;
            GcTotalAvailableMemoryBytes = gcTotalAvailableMemoryBytes;
            CpuUsagePercent = cpuUsagePercent;
            ProcessorCount = processorCount;
            BackgroundQueueLength = backgroundQueueLength;
            Gen0Collections = gen0Collections;
            Gen1Collections = gen1Collections;
            Gen2Collections = gen2Collections;
            ThreadPoolAvailableWorkerThreads = threadPoolAvailableWorkerThreads;
            ThreadPoolAvailableIoThreads = threadPoolAvailableIoThreads;
            UptimeSeconds = uptimeSeconds;
        }

        // Process working set size in bytes (Environment.WorkingSet).
        public long MemoryWorkingSetBytes { get; set; }

        // Managed heap bytes currently in use (GC.GetTotalMemory(false)).
        public long GcHeapBytes { get; set; }

        // Cgroup-aware memory limit (GC.GetGCMemoryInfo().TotalAvailableMemoryBytes); reflects
        // the OpenShift container's memory limit, not the host's physical RAM.
        public long GcTotalAvailableMemoryBytes { get; set; }

        // Null on the first tick, before a previous CPU-time sample exists to diff against.
        public int? CpuUsagePercent { get; set; }

        // Cgroup-aware CPU quota (Environment.ProcessorCount), not the host's physical core count.
        public int ProcessorCount { get; set; }

        // Number of queued-but-not-yet-dequeued background work items (IBackgroundTaskQueue.Count).
        public int BackgroundQueueLength { get; set; }

        // Cumulative GC collection counts per generation since process start (GC.CollectionCount(n)).
        public int Gen0Collections { get; set; }
        public int Gen1Collections { get; set; }
        public int Gen2Collections { get; set; }

        // Currently idle/available thread pool threads; low values under high max can indicate pool exhaustion.
        public int ThreadPoolAvailableWorkerThreads { get; set; }
        public int ThreadPoolAvailableIoThreads { get; set; }

        // Elapsed time since this logger started sampling, not the process's own start time.
        public long UptimeSeconds { get; set; }
    }
}
