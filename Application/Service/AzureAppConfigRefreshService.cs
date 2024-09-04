using Microsoft.Extensions.Configuration.AzureAppConfiguration;


namespace Personal.BlobStorage.Application
{
    public class AzureAppConfigRefreshService : BackgroundService
    {
        private readonly IConfigurationRefresher _refresher;

        public AzureAppConfigRefreshService(IConfigurationRefresher refresher)
        {
            _refresher = refresher;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                await _refresher.TryRefreshAsync();
                await Task.Delay(TimeSpan.FromSeconds(10), stoppingToken);
            }
        }
    }
}



