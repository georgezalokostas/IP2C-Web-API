using IP2C_Web_API.Interface;

namespace IP2C_Web_API.Controllers;

[ApiController]
public class IPDetailsController : Controller
{
    readonly IUnitOfWork _unitOfWork;

    public IPDetailsController(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    [Authorize]
    [HttpGet("/api/GetIPDetails/{ip?}")]
    public async Task<ActionResult<ServiceResponse<IPDetailsDTO>>> GetIPDetails(string? ip)
    {
        var response = await _unitOfWork.IPDetails.GetIPDetails(ip);
        await _unitOfWork.SaveAsync();

        if (response.Success == true)
            return Ok(response);
            
        return NotFound(response);
    }
}
