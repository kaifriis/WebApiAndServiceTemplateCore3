using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Service.api
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;


        public Worker(ILogger<Worker> logger)
        {
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation($"Worker: In executing async loop. Using ILogger");

            while (!stoppingToken.IsCancellationRequested)
            {
                _logger.LogInformation($"Worker: The service is servicing.....");
                await Task.Delay(5000, stoppingToken);
            }
        }
        public override void Dispose()
        {
            _logger.LogInformation("Worker: Service has been stopped!");
            base.Dispose();
        }
    }
}
