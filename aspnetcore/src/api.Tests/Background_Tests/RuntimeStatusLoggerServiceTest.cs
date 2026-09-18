using System;
using Xunit;

namespace api.Tests.Background_Tests
{
    public class RuntimeStatusLoggerServiceTest
    {
        [Fact]
        public void CalculateCpuUsagePercent_ReturnsExpectedValue()
        {
            // 1 processor fully busy for the whole wall-clock interval => 100%.
            int? result = RuntimeStatusLoggerService.CalculateCpuUsagePercent(
                cpuTimeDelta: TimeSpan.FromSeconds(5),
                wallTimeDelta: TimeSpan.FromSeconds(5),
                processorCount: 1);

            Assert.Equal(100, result);
        }

        [Fact]
        public void CalculateCpuUsagePercent_DividesAcrossProcessorCount()
        {
            // 1 second of CPU time out of 2 processors over 1 second of wall time => 50%.
            int? result = RuntimeStatusLoggerService.CalculateCpuUsagePercent(
                cpuTimeDelta: TimeSpan.FromSeconds(1),
                wallTimeDelta: TimeSpan.FromSeconds(1),
                processorCount: 2);

            Assert.Equal(50, result);
        }

        [Fact]
        public void CalculateCpuUsagePercent_RoundsToNearestPercent()
        {
            int? result = RuntimeStatusLoggerService.CalculateCpuUsagePercent(
                cpuTimeDelta: TimeSpan.FromMilliseconds(456),
                wallTimeDelta: TimeSpan.FromSeconds(1),
                processorCount: 1);

            Assert.Equal(46, result);
        }

        [Fact]
        public void CalculateCpuUsagePercent_ReturnsNull_WhenWallTimeDeltaIsZero()
        {
            int? result = RuntimeStatusLoggerService.CalculateCpuUsagePercent(
                cpuTimeDelta: TimeSpan.FromSeconds(1),
                wallTimeDelta: TimeSpan.Zero,
                processorCount: 1);

            Assert.Null(result);
        }

        [Fact]
        public void CalculateCpuUsagePercent_ReturnsNull_WhenProcessorCountIsZero()
        {
            int? result = RuntimeStatusLoggerService.CalculateCpuUsagePercent(
                cpuTimeDelta: TimeSpan.FromSeconds(1),
                wallTimeDelta: TimeSpan.FromSeconds(1),
                processorCount: 0);

            Assert.Null(result);
        }
    }
}
