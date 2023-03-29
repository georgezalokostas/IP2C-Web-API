using System.Text.RegularExpressions;
using RestSharp;

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

        if (!ValidateIP(ip))
        {
            serviceResponse.Success = false;
            serviceResponse.Message = "Invalid IP value.";
            return serviceResponse; //Empty string or invalid IP.
        }

        serviceResponse.Data = await GetCachedData(ip);

        if (serviceResponse.Data is not null)
            //Found in cache.
            return serviceResponse;

        serviceResponse.Data = await GetDatabaseData(ip);

        if (serviceResponse.Data is not null)
        {
            //Found in database.
            _cachedIPs.TryAdd(ip, new IPDetailsDTO
            {
                CountryName = serviceResponse.Data.CountryName,
                TwoLetterCode = serviceResponse.Data.TwoLetterCode,
                ThreeLetterCode = serviceResponse.Data.ThreeLetterCode
            });

            return serviceResponse;
        }

        serviceResponse.Data = await GetAPIData(ip);




    }

    static bool ValidateIP(string input)
    {
        if (string.IsNullOrWhiteSpace(input)) return false;

        return new Regex(@"^((25[0-5]|(2[0-4]|1\d|[1-9]|\d)\.?\b){4})$").IsMatch(input);
    }

    async Task<IPDetailsDTO?> GetCachedData(string ip)
    {
        return await Task.Run(() =>
        {
            return _cachedIPs.TryGetValue(ip, out IPDetailsDTO? data) ? data : null;
        });
    }

    async Task<IPDetailsDTO?> GetDatabaseData(string input)
    {
        var ipAddressEntity = await _context.Ipaddresses.FirstOrDefaultAsync(x => x.Ip == input);
        if (ipAddressEntity is null)
            return null;

        var countryEntity = await _context.Countries.FindAsync(ipAddressEntity.CountryId);
        if (countryEntity is null)
            return null;

        return new IPDetailsDTO
        {
            CountryName = countryEntity.Name,
            TwoLetterCode = countryEntity.TwoLetterCode,
            ThreeLetterCode = countryEntity.ThreeLetterCode
        };
    }

    async Task<string> GetAPIData(string ip)
    {

    }

    async Task UpdateDatabaseAsync(string ip)
    {

    }

    async Task UpdateCacheAsync(string ip)
    {

    }

}
