namespace IP2C_Web_API.Controllers;

[Route("/api")]
[ApiController]
public class ReportController : Controller
{
    readonly IReport _reportRepository;

    public ReportController(IReport reportRepository)
    {
        _reportRepository = reportRepository;
    }

    [HttpGet]
    [Route("/api/GetReport/{codes?}")]
    [ProducesResponseType(typeof(List<ReportDTO>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<List<ReportDTO>>> GetReports(string? codes = "")
    {
        var ips = await _reportRepository.GetReport(codes);

        return ips.Any() ? Ok(ips) : BadRequest(ModelState);
    }
}
