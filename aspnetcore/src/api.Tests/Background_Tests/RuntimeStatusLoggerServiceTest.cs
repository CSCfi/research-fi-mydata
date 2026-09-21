using System;
using Xunit;

namespace api.Tests.Background_Tests
{
    public class RuntimeStatusLoggerServiceTest
    {
        [Fact]
        public void CalculateCpuUsagePercent_ReturnsExpectedValue()
        {
            // 1 core quota fully busy for the whole wall-clock interval => 100%.
            int? result = RuntimeStatusLoggerService.CalculateCpuUsagePercent(
                cpuTimeDelta: TimeSpan.FromSeconds(5),
                wallTimeDelta: TimeSpan.FromSeconds(5),
                cpuQuotaCores: 1);

            Assert.Equal(100, result);
        }

        [Fact]
        public void CalculateCpuUsagePercent_DividesAcrossCpuQuotaCores()
        {
            // 1 second of CPU time out of a 2-core quota over 1 second of wall time => 50%.
            int? result = RuntimeStatusLoggerService.CalculateCpuUsagePercent(
                cpuTimeDelta: TimeSpan.FromSeconds(1),
                wallTimeDelta: TimeSpan.FromSeconds(1),
                cpuQuotaCores: 2);

            Assert.Equal(50, result);
        }

        [Fact]
        public void CalculateCpuUsagePercent_SupportsFractionalCpuQuotaCores()
        {
            // OpenShift 500m limit (0.5 core) fully consumed for the whole wall-clock interval => 100%.
            int? result = RuntimeStatusLoggerService.CalculateCpuUsagePercent(
                cpuTimeDelta: TimeSpan.FromMilliseconds(500),
                wallTimeDelta: TimeSpan.FromSeconds(1),
                cpuQuotaCores: 0.5);

            Assert.Equal(100, result);
        }

        [Fact]
        public void CalculateCpuUsagePercent_RoundsToNearestPercent()
        {
            int? result = RuntimeStatusLoggerService.CalculateCpuUsagePercent(
                cpuTimeDelta: TimeSpan.FromMilliseconds(456),
                wallTimeDelta: TimeSpan.FromSeconds(1),
                cpuQuotaCores: 1);

            Assert.Equal(46, result);
        }

        [Fact]
        public void CalculateCpuUsagePercent_ReturnsNull_WhenWallTimeDeltaIsZero()
        {
            int? result = RuntimeStatusLoggerService.CalculateCpuUsagePercent(
                cpuTimeDelta: TimeSpan.FromSeconds(1),
                wallTimeDelta: TimeSpan.Zero,
                cpuQuotaCores: 1);

            Assert.Null(result);
        }

        [Fact]
        public void CalculateCpuUsagePercent_ReturnsNull_WhenCpuQuotaCoresIsZero()
        {
            int? result = RuntimeStatusLoggerService.CalculateCpuUsagePercent(
                cpuTimeDelta: TimeSpan.FromSeconds(1),
                wallTimeDelta: TimeSpan.FromSeconds(1),
                cpuQuotaCores: 0);

            Assert.Null(result);
        }

        [Fact]
        public void GetCpuQuotaCores_ReturnsPositiveValue()
        {
            // No assertion on the exact source (cgroup files vs. Environment.ProcessorCount
            // fallback), since that depends on the environment the test runs in - just that a
            // usable, positive quota is always returned.
            double result = RuntimeStatusLoggerService.GetCpuQuotaCores();

            Assert.True(result > 0);
        }
    }
}
