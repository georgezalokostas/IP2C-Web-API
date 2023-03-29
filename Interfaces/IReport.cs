namespace IP2C_Web_API.Interfaces;

public interface IReport
{
  Task<ServiceResponse<List<ReportDTO>>> GetReport(string? codes);
}
