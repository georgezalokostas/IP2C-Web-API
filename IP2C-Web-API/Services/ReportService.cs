using Microsoft.Data.SqlClient;

namespace IP2C_Web_API.Services;

public class ReportService : IReport
{
    const string GetReportByID = @"  SELECT CO.Name CountryName, COUNT(*) AddressesCount, MAX(IP.UpdatedAt) LastAddressUpdated
                                    FROM IPAddresses IP INNER JOIN COUNTRIES CO ON IP.CountryId = CO.Id
                                    WHERE CO.TwoLetterCode IN @codes GROUP BY CO.Name";

    const string GetReportAll = @" SELECT CO.Name CountryName, COUNT(*) AddressesCount, MAX(IP.UpdatedAt) LastAddressUpdated
                                   FROM IPAddresses IP INNER JOIN COUNTRIES CO ON IP.CountryId = CO.Id GROUP BY CO.Name";                                         

    readonly MasterContext _context;
    readonly IConfigurationBuilder? _builder = null;

    public ReportService(MasterContext context)
    {
        _context = context;
    }

    public async Task<ServiceResponse<List<ReportDTO>>> GetReport(string? input)
    {
        var configuration = _builder?.Build() ?? new ConfigurationBuilder()
                                                    .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
                                                    .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true).Build();

        using var con = new SqlConnection(configuration.GetConnectionString("DefaultConnection"));
        await con.OpenAsync();

        var serviceResponse = new ServiceResponse<List<ReportDTO>>();

        if (string.IsNullOrWhiteSpace(input))
        {
            serviceResponse.Data = (await con.QueryAsync<ReportDTO>(GetReportAll, null)).ToList();
            return serviceResponse;
        }

        var codes = input.Split(',');
        serviceResponse.Data = (await con.QueryAsync<ReportDTO>(GetReportByID, new { codes })).ToList();
        return serviceResponse;
    }
}

