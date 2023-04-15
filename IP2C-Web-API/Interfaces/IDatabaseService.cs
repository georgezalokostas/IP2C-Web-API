namespace IP2C_Web_API.Interfaces;

public interface IDatabaseService
{
Task<IPDetailsDTO?> GetDatabaseDataAsync(string input);
Task<IPDetailsDTO?> GetAPIDataAsync(string ip);
Task AddOrUpdateDatabaseAsync(string ip, IPDetailsDTO data);
Task SyncDatabaseAsync(string ip, IPDetailsDTO newIPDetails);
}
