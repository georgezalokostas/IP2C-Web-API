namespace IP2C_Web_API.Interfaces;

public interface IIPDetails
{
    Task<ServiceResponse<IPDetailsDTO>> GetIPDetails(string? codes);
}
