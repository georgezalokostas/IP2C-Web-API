namespace IP2C_Web_API.Services;

public class IPDetailsService : IIPDetails
{
    readonly MasterContext _context;

    public IPDetailsService(MasterContext context)
    {
        _context = context;
    }

    public async Task<ServiceResponse<IPDetailsDTO>> GetIPDetails(string? ip)
    {
        var serviceResponse = new ServiceResponse<IPDetailsDTO>();
        var tasks = new Tasks(_context);

        if (!ValidateIP(ip))
        {
            serviceResponse.Success = false;
            serviceResponse.Message = "Invalid IP value.";
            return serviceResponse; //Empty string or invalid IP.
        }

        serviceResponse.Data = await tasks.GetCachedDataAsync(ip!);

        //Found in cache.
        if (serviceResponse.Data is not null)
            return serviceResponse;

        serviceResponse.Data = await tasks.GetDatabaseDataAsync(ip!);

        //Found in database.
        if (serviceResponse.Data is not null)
        {
            await tasks.UpdateCacheAsync(ip!, serviceResponse.Data);
            return serviceResponse;
        }

        serviceResponse.Data = await tasks.GetAPIDataAsync(ip!);

        //Found in API.
        if (serviceResponse.Data is not null)
            await Task.WhenAll(tasks.UpdateCacheAsync(ip!, serviceResponse.Data),
                               tasks.AddOrUpdateDatabaseAsync(ip!, serviceResponse.Data));

        return serviceResponse;
    }

    static bool ValidateIP(string? input)
    {
        if (string.IsNullOrWhiteSpace(input))
            return false;

        return new Regex(@"^([0-9]|[1-9][0-9]|1[0-9]{2}|2[0-4][0-9]|25[0-5])(\.([0-9]|[1-9][0-9]|1[0-9]{2}|2[0-4][0-9]|25[0-5])){3}$").IsMatch(input);
    }
}
