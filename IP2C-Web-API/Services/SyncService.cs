namespace IP2C_Web_API.Services;

public class SyncService : BackgroundService
{
    const int LOOP_TIME = 1 * 60000; //1 minute. Change to 60 * 60000 for 1 hour.
    const int _CACHED_MINUTES = 20;
    readonly MasterContext _context;
    readonly IServiceProvider _services;
    readonly ICacheService _cacheService;
    readonly IDatabaseService _databaseService;

    public SyncService(MasterContext context, IServiceProvider services, ICacheService cacheService, IDatabaseService databaseService)
    {
        _services = services;
        _cacheService = cacheService;
        _databaseService = databaseService;
        _context = context;
    }

    protected async override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            var batchSize = 100;
            var page = 1;
            var allIps = await _context.Ipaddresses.ToListAsync();

            while (true)
            {
                var ips = allIps.Skip(batchSize * (page - 1)).Take(batchSize).ToList();

                if (ips.Count == 0)
                    break;

                foreach (var ip in ips)
                {
                    await CheckAndUpdateIpAsync(ip, _context);
                }

                page++;
            }


            await Task.Delay(LOOP_TIME, stoppingToken);
        }
    }

    async Task CheckAndUpdateIpAsync(Ipaddress ipObject, MasterContext _context)
    {
        //Fetch fresh data from API.
        var newIPDetails = await _databaseService.GetAPIDataAsync(ipObject.Ip);

        if (newIPDetails is null)
            return;

        //Update the database.
        await _databaseService.SyncDatabaseAsync(ipObject.Ip, newIPDetails);

        //TODO: Update the cache with only 30% of the data to avoid memory issues.
        _cacheService.SetData(ipObject.Ip, newIPDetails, DateTimeOffset.Now.AddMinutes(_CACHED_MINUTES));

        await _context.SaveChangesAsync();
    }
}
