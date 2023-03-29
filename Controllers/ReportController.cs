namespace IP2C_Web_API.Controllers;

[Route("/api")]
[ApiController]
public class ReportController : Controller
{
    readonly IReport _reportService;

    public ReportController(IReport reportService)
    {
        _reportService = reportService;
    }

    [HttpGet]
    [Route("/api/GetReport/{codes?}")]
    [ProducesResponseType(typeof(List<ReportDTO>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ServiceResponse<List<ReportDTO>>>> GetReports(string? codes = "")
    {
        return Ok(await _reportService.GetReport(codes));
    }
}
