using Microsoft.Data.SqlClient;

namespace IP2C_Web_API.Services;

public class ReportService : IReport
{
    readonly MasterContext _context;

    public ReportService(MasterContext context)
    {
        _context = context;
    }

    public async Task<ServiceResponse<List<ReportDTO>>> GetReport(string? input)
    {
        var serviceResponse = new ServiceResponse<List<ReportDTO>>();

        var builder = new ConfigurationBuilder()
        .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
        .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);

        var configuration = builder.Build();

        using var con = new SqlConnection(configuration.GetConnectionString("DefaultConnection"));
        await con.OpenAsync();

        if (string.IsNullOrWhiteSpace(input))
        {
            serviceResponse.Data = (await con.QueryAsync<ReportDTO>(Queries.GetReportAll, null)).ToList();
            return serviceResponse;
        }

        var codes = input.Split(',');
        serviceResponse.Data = (await con.QueryAsync<ReportDTO>(Queries.GetReportById, new { codes })).ToList();
        return serviceResponse;
    }
}

