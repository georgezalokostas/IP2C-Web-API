namespace IP2C_Web_API.Controllers;

[ApiController]
[Route("[controller]")]
public class AuthController : ControllerBase
{
    readonly IAuthentication _authRepo;

    public AuthController(IAuthentication authRepo)
    {
        _authRepo = authRepo;
    }

    [HttpPost("Register")]
    public async Task<ActionResult<ServiceResponse<int>>> Register(UserRegisterDto request)
    {
        var response = await _authRepo.Register(
            new User { Username = request.Username }, request.Password
        );
        if (!response.Success)
        {
            return BadRequest(response);
        }
        return Ok(response);
    }

    [HttpPost("Login")]
    public async Task<ActionResult<ServiceResponse<int>>> Login(UserLoginDto request)
    {
        var response = await _authRepo.Login(request.Username, request.Password);
        if (!response.Success)
        {
            return BadRequest(response);
        }
        return Ok(response);
    }
}