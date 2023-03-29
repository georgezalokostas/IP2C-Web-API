namespace IP2C_Web_API.Controllers;

//  
//          lock (_packsLock) //Lock in order to avoid simultaneously check of ContainsKey and InsertPack
//         {
//             if (myPacks.ContainsKey(packDesc)) return;
//         }

[ApiController]
public class IPDetailsController
{
    readonly IIPDetails _IipDetails;

    public IPDetailsController(IIPDetails ipDetails)
    {
        _IipDetails = ipDetails;
    }

    [HttpGet("/api/GetIPDetails/{ip?}")]
    public async Task<ActionResult<ServiceResponse<IPDetailsDTO>>> GetIPDetails(string? ip)
    {         




        // if (string.IsNullOrWhiteSpace(ip))
        // {
        //     response.Message = "Invalid country codes provided. Please try again with valid codes.";
        //     response.Success = false;
        //     return NotFound(response);
        // }

        // return Ok(response);
    }
}
