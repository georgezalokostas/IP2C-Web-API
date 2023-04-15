namespace IP2C_Web_API.Services;

public class IPDetailsService : IIPDetails
{
    readonly MasterContext _context;
    readonly ICacheService _cacheService;
    readonly IDatabaseService _databaseService;
    const int _CACHED_MINUTES = 20;

    public IPDetailsService(MasterContext context, ICacheService cacheService, IDatabaseService databaseService)
    {
        _context = context;
        _cacheService = cacheService;
        _databaseService = databaseService;
    }

    public async Task<ServiceResponse<IPDetailsDTO>> GetIPDetails(string? ip)
    {
        var serviceResponse = new ServiceResponse<IPDetailsDTO>();

        if (!ValidateIP(ip))
        {
            serviceResponse.Success = false;
            serviceResponse.Message = "Invalid IP value.";
            return serviceResponse; //Empty string or invalid IP.
        }

        serviceResponse.Data = _cacheService.GetData<IPDetailsDTO>(ip!);

        //Found in cache.
        if (serviceResponse.Data is not null)
            return serviceResponse;

        serviceResponse.Data = await _databaseService.GetDatabaseDataAsync(ip!);

        //Found in database.
        if (serviceResponse.Data is not null)
        {
            _cacheService.SetData(ip!, serviceResponse.Data, DateTimeOffset.Now.AddMinutes(_CACHED_MINUTES));
            return serviceResponse;
        }

        serviceResponse.Data = await _databaseService.GetAPIDataAsync(ip!);

        //Found in API.
        if (serviceResponse.Data is not null)
        {
            _cacheService.SetData(ip!, serviceResponse.Data, DateTimeOffset.Now.AddMinutes(_CACHED_MINUTES));
            await _databaseService.AddOrUpdateDatabaseAsync(ip!, serviceResponse.Data);

            return serviceResponse;
        }

        serviceResponse.Success = false;
        serviceResponse.Message = "An error occured while accessing the API.";
        return serviceResponse;
    }

    static bool ValidateIP(string? input)
    {
        if (string.IsNullOrWhiteSpace(input))
            return false;

        return new Regex(@"^([0-9]|[1-9][0-9]|1[0-9]{2}|2[0-4][0-9]|25[0-5])(\.([0-9]|[1-9][0-9]|1[0-9]{2}|2[0-4][0-9]|25[0-5])){3}$").IsMatch(input);
    }
}
