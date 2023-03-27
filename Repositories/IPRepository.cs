using Microsoft.Data.SqlClient;

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

    public async Task<List<ReportDTO>> GetReport(string? id)
    {
        if (string.IsNullOrWhiteSpace(id)) return new List<ReportDTO>();

        var builder = new ConfigurationBuilder()
            .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
            .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);

        var configuration = builder.Build();

        using var con = new SqlConnection(configuration.GetConnectionString("DefaultConnection"));
        con.Open();

        //con query the data from the SQL, mapping them in an object with Dapper and return an array.
        return new List<ReportDTO>();
    }
}

