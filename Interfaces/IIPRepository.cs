using System.Net;
namespace IP2C_Web_API.Interfaces;

public interface IIPRepository
{
  Task<List<Country>> GetIPs();
  Task<List<ReportDTO>> GetReport(string id);
}
