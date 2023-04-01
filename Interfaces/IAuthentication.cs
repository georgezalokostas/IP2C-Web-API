namespace IP2C_Web_API.Interfaces;

public interface IAuthentication
{
    Task<ServiceResponse<int>> Register(User user, string password);
    Task<ServiceResponse<string>> Login(string username, string password);
    Task<bool> UserExists(string username);
}
