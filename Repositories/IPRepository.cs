namespace IP2C_Web_API.Repositories;

public class IPRepository : IIPRepository
{
    readonly MasterContext _context;

    public IPRepository(MasterContext context)
    {
        _context = context;
    }

    public async Task<List<Country>> GetIPs()
    {
        return await _context.Countries.Select(x => x).ToListAsync();
    }
}
