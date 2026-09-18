using System;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace api.Tests.Background_Tests
{
    public class BackgroundTaskQueueTest
    {
        [Fact]
        public async Task Count_ReflectsQueuedAndDequeuedItems()
        {
            BackgroundTaskQueue queue = new();

            Assert.Equal(0, queue.Count);

            await queue.QueueBackgroundWorkItemAsync(_ => ValueTask.CompletedTask);
            await queue.QueueBackgroundWorkItemAsync(_ => ValueTask.CompletedTask);
            Assert.Equal(2, queue.Count);

            await queue.DequeueAsync(CancellationToken.None);
            Assert.Equal(1, queue.Count);
        }
    }
}
