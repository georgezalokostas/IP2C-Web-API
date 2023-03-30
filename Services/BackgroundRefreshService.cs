namespace IP2C_Web_API.Services;

public class BackgroundRefreshService : BackgroundService
{
    int _oneHourInMS = 3600000;
    int _tenSeconds = 10000;
    
    protected async override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            Console.WriteLine("Hello World");
            await Task.Delay(_tenSeconds, stoppingToken);
        }
    }
}
