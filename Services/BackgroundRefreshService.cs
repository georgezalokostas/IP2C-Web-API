namespace IP2C_Web_API.Services;

public class BackgroundRefreshService : BackgroundService
{
    int _oneHourInMS = 3600000;
    int _tenSeconds = 10000;
    readonly IServiceProvider _services;

    public BackgroundRefreshService(IServiceProvider services)
    {
        _services = services;
    }

    protected async override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            Console.WriteLine("Background refresh started");

            var _context = _services.CreateScope().ServiceProvider.GetRequiredService<MasterContext>();

            var batchSize = 100;
            var page = 1;
            var allIps = await _context.Ipaddresses.ToListAsync();

            while (true)
            {
                var ips = allIps.Skip(batchSize * (page - 1)).Take(batchSize).ToList();

                if (ips.Count == 0)
                    break;

                Parallel.ForEach(ips, new ParallelOptions { CancellationToken = stoppingToken }, async ip =>
                {
                    await CheckAndUpdateIpAsync(ip, _services.CreateScope().ServiceProvider.GetRequiredService<MasterContext>());
                });

                page++;
            }

            Console.WriteLine("Background refresh completed");
            await Task.Delay(_tenSeconds, stoppingToken);
        }
    }

    async Task CheckAndUpdateIpAsync(Ipaddress ipObject, MasterContext _context)
    {
        var tasks = new Tasks(_context);

        //Fetch fresh data from API.
        var newIPDetails = await tasks.GetAPIDataAsync(ipObject.Ip);

        if (newIPDetails is null)
            return;

        //Update the database.
        await tasks.AddOrUpdateDatabaseAsync(ipObject.Ip, newIPDetails);

        //Update the cache.
        await tasks.UpdateCacheAsync(ipObject.Ip, newIPDetails);
    }
}
