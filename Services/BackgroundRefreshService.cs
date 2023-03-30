using System.Runtime.Serialization;
namespace IP2C_Web_API.Services;

public class BackgroundRefreshService : BackgroundService
{
    int _oneHourInMS = 3600000;
    int _tenSeconds = 10000;
    readonly MasterContext _context;

    public BackgroundRefreshService(MasterContext context)
    {
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

                await Parallel.ForEachAsync(ips, async (ip, ct) => { await CheckAndUpdateIpAsync(ip); });

                page++;
            }

            await Task.Delay(_oneHourInMS, stoppingToken);
        }
    }

    async Task CheckAndUpdateIpAsync(Ipaddress ipObject)
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
