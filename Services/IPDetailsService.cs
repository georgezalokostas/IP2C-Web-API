namespace IP2C_Web_API.Services;

public class IPDetailsService : IIPDetails
{
    readonly MasterContext _context;

    public IPDetailsService(MasterContext context)
    {
        _context = context;
    }

    public async Task<ServiceResponse<IPDetailsDTO>> GetIPDetails(string? codes)
    {
        var serviceResponse = new ServiceResponse<IPDetailsDTO>();

        serviceResponse.Data = await GetCachedData();


    }

    async Task<IPDetailsDTO> GetCachedData()
    {
        await Task.Run(() =>
        {
            
        });
    }

}
