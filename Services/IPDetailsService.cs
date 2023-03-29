namespace IP2C_Web_API.Services;

public class IPDetailsService
{
    readonly MasterContext _context;
    readonly IConfigurationBuilder? _builder = null;

    public IPDetailsService(MasterContext context)
    {
        _context = context;
    }

    // public async Task<ServiceResponse<List<IPDetails>>> GetIPDetails(string? input)
    // {
       
    // }
}
