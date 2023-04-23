using IP2C_Web_API.Interface;

namespace IP2C_Web_API.Controllers;

[ApiController]
public class IPDetailsController : Controller
{
    readonly IUnitOfWork _unitOfWork;
    readonly IMessageProducer _messageProducer;

    public IPDetailsController(IUnitOfWork unitOfWork, IMessageProducer messageProducer)
    {
        _unitOfWork = unitOfWork;
        _messageProducer = messageProducer;
    }

    [Authorize]
    [HttpGet("/api/GetIPDetails/{ip?}")]
    public async Task<ActionResult<ServiceResponse<IPDetailsDTO>>> GetIPDetails(string? ip)
    {
        var response = await _unitOfWork.IPDetails.GetIPDetails(ip);
        await _unitOfWork.SaveAsync();

        _messageProducer.SendingMessage<IPDetailsDTO>(response.Data!);

        return response.Success ? Ok(response) : NotFound(response);
    }
}
