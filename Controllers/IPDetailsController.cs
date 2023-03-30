namespace IP2C_Web_API.Controllers;

[ApiController]
public class IPDetailsController : Controller
{
    readonly IIPDetails _IipDetailsService;

    public IPDetailsController(IIPDetails ipDetailsService)
    {
        _IipDetailsService = ipDetailsService;
    }

    [HttpGet("/api/GetIPDetails/{ip?}")]
    public async Task<ActionResult<ServiceResponse<IPDetailsDTO>>> GetIPDetails(string? ip)
    {
        var response = await _IipDetailsService.GetIPDetails(ip);
        return response.Success == true ? Ok(response) : NotFound(response);
    }
}
