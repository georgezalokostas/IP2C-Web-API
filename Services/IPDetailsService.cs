namespace IP2C_Web_API.Services;

public class IPDetailsService : IIPDetails
{
    readonly MasterContext _context;

    public IPDetailsService(MasterContext context)
    {
        _context = context;
    }

    public async Task<ServiceResponse<IPDetailsDTO>> GetIPDetails(string ip)
    {
        var serviceResponse = new ServiceResponse<IPDetailsDTO>();

        serviceResponse.Data = await GetCachedData(ip);
        return serviceResponse; //Found in cache.

    }

    async Task<IPDetailsDTO?> GetCachedData(string ip)
    {
        return await Task.Run(() =>
        {
            return _cachedIPs.TryGetValue(ip, out IPDetailsDTO? data) ? data : null;
        });
    }

}
