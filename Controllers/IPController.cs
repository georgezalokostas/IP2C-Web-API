namespace IP2C_Web_API.Controllers;

[Route("/api")]
[ApiController]
public class IPController : Controller
{
    readonly IIPRepository _ipRepository;

    public IPController(IIPRepository ipRepository)
    {
        _ipRepository = ipRepository;
    }

    [HttpGet]
    [Route("/api/GetReport/{codes?}")]
    [ProducesResponseType(typeof(List<ReportDTO>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<List<ReportDTO>>> GetReports(string? codes = "")
    {
        var ips = await _ipRepository.GetReport(codes);

        if (!ips.Any())
            return BadRequest(ModelState);

        return Ok(ips);
    }
}
