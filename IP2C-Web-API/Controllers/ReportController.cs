using IP2C_Web_API.Interface;

namespace IP2C_Web_API.Controllers;

[ApiController]
public class ReportController : Controller
{
    readonly IUnitOfWork _unitOfWork;

    public ReportController(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    [Authorize]
    [HttpGet("/api/GetReport/{codes?}")]
    public async Task<ActionResult<ServiceResponse<List<ReportDTO>>>> GetReports(string? codes = "")
    {
        var response = await _unitOfWork.Report.GetReport(codes);

        if (response.Data == null || response.Data?.Count == 0)
        {
            response.Message = "No data found for the provided country codes.";
            response.Success = false;
            return NotFound(response);
        }

        return Ok(response);
    }
}
