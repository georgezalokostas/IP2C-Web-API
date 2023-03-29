namespace IP2C_Web_API.Controllers;

[ApiController]
public class ReportController : Controller
{
    readonly IReport _reportService;

    public ReportController(IReport reportService)
    {
        _reportService = reportService;
    }

    [HttpGet("/api/GetReport/{codes?}")]
    public async Task<ActionResult<ServiceResponse<List<ReportDTO>>>> GetReports(string? codes = "")
    {
        var response = await _reportService.GetReport(codes);
        
        if (!response.Data!.Any())
        {
            response.Message = "Invalid country codes provided. Please try again with valid codes.";
            response.Success = false;
            return NotFound(response);
        }

        return Ok(response);
    }
}
