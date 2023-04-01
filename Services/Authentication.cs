namespace IP2C_Web_API.Services;

public class Authentication : IAuthentication
{
    public Task<ServiceResponse<string>> Login(string username, string password)
    {
        throw new NotImplementedException();
    }

    public Task<ServiceResponse<int>> Register(User user, string password)
    {
        throw new NotImplementedException();
    }

    public Task<bool> UserExists(string username)
    {
        throw new NotImplementedException();
    }
}
