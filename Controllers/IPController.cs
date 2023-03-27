namespace IP2C_Web_API.Controllers;

[Route("api/GetCountries")]
[ApiController]
public class IPController : Controller
{
    readonly IIPRepository _ipRepository;

    public IPController(IIPRepository ipRepository)
    {
        _ipRepository = ipRepository;
    }

    [HttpGet]
    public async Task<IActionResult> GetIPs()
    {
        var ips = await _ipRepository.GetIPs();

        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        return Ok(ips);
    }
}
