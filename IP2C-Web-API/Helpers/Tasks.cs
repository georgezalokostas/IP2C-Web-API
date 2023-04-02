namespace IP2C_Web_API.Helpers;

public class Tasks
{
    readonly MasterContext _context;
    readonly Object _dictLock = new();
    RestClient _client;

    public Tasks(MasterContext context)
    {
        _context = context;
        _client = new RestClient();
    }

    public async Task<IPDetailsDTO?> GetCachedDataAsync(string ip)
    {
        Console.WriteLine("GetCachedDataAsync called");
        return await Task.Run(() =>
        {
            return _cachedIPs.TryGetValue(ip, out IPDetailsDTO? data) ? data : null;
        });
    }

    public async Task<IPDetailsDTO?> GetDatabaseDataAsync(string input)
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

    public async Task<IPDetailsDTO?> GetAPIDataAsync(string ip)
    {
        Console.WriteLine("GetAPIDataAsync called");
        var response = await _client.ExecuteGetAsync(new RestRequest($"https://ip2c.org/{ip}"));

        if (!response.IsSuccessful)
            return null;

        var splitData = response.Content!.Split(';');

        if (splitData.Length != 4) return null; //Something went wrong

        return new IPDetailsDTO
        {
            TwoLetterCode = splitData[1],
            ThreeLetterCode = splitData[2],
            CountryName = splitData[3]
        };
    }

    public async Task AddOrUpdateDatabaseAsync(string ip, IPDetailsDTO data)
    {
        Console.WriteLine("AddOrUpdateDatabaseAsync called");

        var existingCountry = _context.Countries.FirstOrDefault(x => x.TwoLetterCode == data.TwoLetterCode);

        // If we don't find a country, insert a new record
        if (existingCountry == null)
        {
            var newCountry = new Country
            {
                TwoLetterCode = data.TwoLetterCode,
                ThreeLetterCode = data.ThreeLetterCode,
                Name = data.CountryName.Truncate(50)
            };

            _context.Countries.Add(newCountry);
            await _context.SaveChangesAsync();

            existingCountry = newCountry;
        }


        // If the IP address record already exists, update it. Else insert it
        var existingIp = await _context.Ipaddresses.FirstOrDefaultAsync(x => x.Ip == ip);

        if (existingIp is null)
        {
            _context.Ipaddresses.Add(new Ipaddress
            {
                Ip = ip,
                CountryId = existingCountry.Id,
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now
            });
        }
        else
        {
            existingIp.UpdatedAt = DateTime.Now;
            _context.Ipaddresses.Update(existingIp);
        }

    }

    public async Task UpdateCacheAsync(string ip, IPDetailsDTO data)
    {
        Console.WriteLine($"UpdateCacheAsync called. Cache data size:{_cachedIPs.Count}");

        await Task.Run(() =>
        {
            lock (_dictLock)
            {
                _cachedIPs.TryAdd(ip, new IPDetailsDTO
                {
                    CountryName = data.CountryName,
                    TwoLetterCode = data.TwoLetterCode,
                    ThreeLetterCode = data.ThreeLetterCode
                });
            }
        });
    }

    public async Task SyncDatabaseAsync(string ip, IPDetailsDTO newIPDetails)
    {
        var existingIp = await _context.Ipaddresses.FirstOrDefaultAsync(x => x.Ip == ip);
        var existingCountry = await _context.Countries.FirstOrDefaultAsync(x => x.TwoLetterCode == newIPDetails.TwoLetterCode);

        //We found a new country, we have to save it to the database.
        if (existingCountry is null)
        {
            var newCountry = new Country
            {
                TwoLetterCode = newIPDetails.TwoLetterCode,
                ThreeLetterCode = newIPDetails.ThreeLetterCode,
                Name = newIPDetails.CountryName.Truncate(50)
            };

            _context.Countries.Add(newCountry);
            await _context.SaveChangesAsync();

            existingCountry = newCountry;
        }

        //We haven't found an IP, so we can't update anything
        if (existingIp is null)
            return;

        //Update the UpdatedAt column only if CountryID changed
        if (existingIp.CountryId == existingCountry.Id)
            return;

        existingIp.UpdatedAt = DateTime.Now;
        existingIp.CountryId = existingCountry.Id;

        _context.Ipaddresses.Update(existingIp);
    }
}
