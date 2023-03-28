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

    public async Task<List<ReportDTO>> GetReport(string? input)
    {
        var builder = new ConfigurationBuilder()
        .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
        .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);

        var configuration = builder.Build();

        using var con = new SqlConnection(configuration.GetConnectionString("DefaultConnection"));
        await con.OpenAsync();

        if (string.IsNullOrWhiteSpace(input))    
            return (await con.QueryAsync<ReportDTO>(Queries.GetReportAll, null)).ToList();        

        var codes = input.Split(',');
        return (await con.QueryAsync<ReportDTO>(Queries.GetReportById, new { codes })).ToList();
    }
}

