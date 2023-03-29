using System.Net;
namespace IP2C_Web_API.Services;

public class IPDetailsService : IIPDetails
{
    readonly MasterContext _context;
    readonly RestClient _client;
    readonly Object _dictLock = null!;

    public IPDetailsService(MasterContext context)
    {
        _context = context;
        _client = new RestClient("https://ip2c.org/");
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

        serviceResponse.Data = await GetCachedDataAsync(ip);

        //Found in cache.
        if (serviceResponse.Data is not null)
            return serviceResponse;

        serviceResponse.Data = await GetDatabaseDataAsync(ip);

        //Found in database.
        if (serviceResponse.Data is not null)
        {
            await UpdateCacheAsync(ip, serviceResponse.Data);
            return serviceResponse;
        }

        serviceResponse.Data = await GetAPIDataAsync(ip);

        //Found in API.
        if (serviceResponse.Data is not null)
            await Task.WhenAll(UpdateCacheAsync(ip, serviceResponse.Data),
                               AddOrUpdateDatabaseAsync(ip, serviceResponse.Data));

        return serviceResponse;
    }

    static bool ValidateIP(string input)
    {
        if (string.IsNullOrWhiteSpace(input)) return false;

        return new Regex(@"^((25[0-5]|(2[0-4]|1\d|[1-9]|\d)\.?\b){4})$").IsMatch(input);
    }

    async Task<IPDetailsDTO?> GetCachedDataAsync(string ip)
    {
        Console.WriteLine("GetCachedDataAsync called");
        return await Task.Run(() =>
        {
            return _cachedIPs.TryGetValue(ip, out IPDetailsDTO? data) ? data : null;
        });
    }

    async Task<IPDetailsDTO?> GetDatabaseDataAsync(string input)
    {
        Console.WriteLine("GetDatabaseDataAsync called");
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

    async Task<IPDetailsDTO?> GetAPIDataAsync(string ip)
    {
        Console.WriteLine("GetAPIDataAsync called");
        var request = new RestRequest($"https://ip2c.org/{ip}");
        var response = await _client.ExecuteGetAsync(request);

        if (!response.IsSuccessful)
            return null;

        var dataFromAPI = response.Content;
        var splitData = dataFromAPI!.Split(';');

        if (splitData.Length != 4) return null; //Something went wrong

        return new IPDetailsDTO
        {
            TwoLetterCode = splitData[1],
            ThreeLetterCode = splitData[2],
            CountryName = splitData[3]
        };
    }

    async Task AddOrUpdateDatabaseAsync(string ip, IPDetailsDTO data)
    {
        Console.WriteLine("AddOrUpdateDatabaseAsync called");

        var existingCountry = _context.Countries.FirstOrDefault(x => x.TwoLetterCode == data.TwoLetterCode.ToUpper());

        // If we don't find a country, insert a new record
        if (existingCountry == null)
        {
            var newCountry = new Country
            {
                TwoLetterCode = data.TwoLetterCode.ToUpper(),
                ThreeLetterCode = data.ThreeLetterCode.ToUpper(),
                Name = data.CountryName
            };

            _context.Countries.Add(newCountry);
            await _context.SaveChangesAsync();

            existingCountry = newCountry;
        }

        // Create a new IP address correct country ID
        var ipAddress = new Ipaddress
        {
            Ip = ip,
            CountryId = existingCountry.Id,
            CreatedAt = DateTime.Now,
            UpdatedAt = DateTime.Now
        };

        // If the IP address record already exists, update it
        var existingIp = await _context.Ipaddresses.FirstOrDefaultAsync(x => x.Ip == ip);
        if (existingIp != null)
        {
            existingIp.UpdatedAt = DateTime.Now;
            _context.Ipaddresses.Update(existingIp);
        }
        else
        {
            _context.Ipaddresses.Add(ipAddress);
        }

        await _context.SaveChangesAsync();
    }

    async Task UpdateCacheAsync(string ip, IPDetailsDTO data)
    {
        Console.WriteLine("UpdateCacheAsync called");

        await Task.Run(() =>
        {
            _cachedIPs.TryAdd(ip, new IPDetailsDTO
            {
                CountryName = data.CountryName,
                TwoLetterCode = data.TwoLetterCode,
                ThreeLetterCode = data.ThreeLetterCode
            });
        });
    }

}
