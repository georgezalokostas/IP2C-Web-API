using System.Text.Json;
using StackExchange.Redis;

namespace IP2C_Web_API.Services;

public class CacheService : ICacheService
{
    IDatabase _cacheDb;

    public CacheService()
    {
        var redis = ConnectionMultiplexer.Connect("localhost:6379");
        _cacheDb = redis.GetDatabase();
    }

    public T GetData<T>(string key)
    {
        var value = _cacheDb.StringGet(key);
        if (!string.IsNullOrWhiteSpace(value))
            return JsonSerializer.Deserialize<T>(value!)!;

        return default!;
    }

    public object RemoveData(string key)
    {
        var exists = _cacheDb.KeyExists(key);

        if (exists)
            return _cacheDb.KeyDelete(key);

        return false;
    }

    public bool SetData<T>(string key, T value, DateTimeOffset expirationTime)
    {
        var expiryTime = expirationTime.DateTime.Subtract(DateTime.Now);
        return _cacheDb.StringSet(key, JsonSerializer.Serialize(value), expiryTime);
    }
}
