namespace IP2C_Web_API.Services;

public class SyncService : BackgroundService
{
    const int LOOP_TIME = 1 * 60000; //1 minute. Change to 60 * 60000 for 1 hour.
    const int _CACHED_MINUTES = 20;
    readonly IServiceProvider _services;
    readonly ICacheService _cacheService;

    public SyncService(IServiceProvider services, ICacheService cacheService)
    {
        _services = services;
        _cacheService = cacheService;
    }

    protected async override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
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


            await Task.Delay(LOOP_TIME, stoppingToken);
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
        await tasks.SyncDatabaseAsync(ipObject.Ip, newIPDetails);

        //TODO: Update the cache with only 30% of the data to avoid memory issues.
        _cacheService.SetData(ipObject.Ip, newIPDetails, DateTimeOffset.Now.AddMinutes(_CACHED_MINUTES));

        await _context.SaveChangesAsync();
    }
}
