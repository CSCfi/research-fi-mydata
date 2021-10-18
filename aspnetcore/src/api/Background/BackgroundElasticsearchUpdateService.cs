using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;

namespace api.Services
{
    public class BackgroundElasticsearchUpdateService : BackgroundService
    {
        private readonly BackgroundElasticsearchPersonUpdateQueue queue;

        public BackgroundElasticsearchUpdateService(BackgroundElasticsearchPersonUpdateQueue queue)
        {
            this.queue = queue;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                var workItem = await queue.DequeueAsync(stoppingToken);
                await workItem(stoppingToken);
            }
        }
    }
}