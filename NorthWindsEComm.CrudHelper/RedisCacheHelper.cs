using System.Text.Json;
using StackExchange.Redis;

namespace NorthWindsEComm.CrudHelper;

public class RedisCacheHelper<T> : ICrudCacheAccess<T> where T : class, IIdModel
{
    private readonly IConnectionMultiplexer _connectionMultiplexer;
    private readonly IDatabase _database;
    private readonly string _indexPrefix = typeof(T).Name.ToLower();
    
    public RedisCacheHelper(IConnectionMultiplexer connectionMultiplexer)
    {
        _connectionMultiplexer = connectionMultiplexer;
        _database = _connectionMultiplexer.GetDatabase();
    }
    
    public async Task<List<T>> GetAllAsync(CancellationToken ctx)
    {
        var endPoints = _connectionMultiplexer.GetEndPoints();
        var server = _connectionMultiplexer.GetServer(endPoints[0]);
        List<RedisValue> response = new();
        
        foreach (var key in server.Keys(pattern: _indexPrefix + ".*"))
            response.Add(await _database.StringGetAsync(key));
        if(response.Count == 0)
            return [];
        return response.Select(r => JsonSerializer.Deserialize<T>(r)).ToList();
    }

    public async Task<T?> GetByIdAsync(int id, CancellationToken ctx)
    {
        RedisValue response = await _database.StringGetAsync($"{_indexPrefix}.{id}");
        if(response.IsNull)
            return default;
        return JsonSerializer.Deserialize<T>(response!);
    }

    public async Task<T?> CreateAsync(T entity, CancellationToken ctx)
    {
        var response = await _database.StringSetAndGetAsync($"{_indexPrefix}.{entity.Id}", JsonSerializer.Serialize(entity));
        if(response.IsNull)
            return default;
        return JsonSerializer.Deserialize<T>(response!);
    }

    public async Task<T?> UpdateAsync(T entity, CancellationToken ctx)
    {
        await DeleteAsync(entity.Id, ctx);
        return await CreateAsync(entity, ctx);
    }

    public async Task<bool> DeleteAsync(int id, CancellationToken ctx)
    {
        await _database.KeyDeleteAsync($"{_indexPrefix}.{id}");
        return true;
    }
}