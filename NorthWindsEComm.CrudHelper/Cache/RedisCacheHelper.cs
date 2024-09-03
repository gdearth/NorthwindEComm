using System.Text.Json;
using NorthWindsEComm.CrudHelper.Base;
using StackExchange.Redis;

namespace NorthWindsEComm.CrudHelper.Cache;

/// <inheritdoc />
public class RedisCacheHelper<T> : ICrudCacheAccess<T> where T : class, IIdModel
{
    private readonly IConnectionMultiplexer _connectionMultiplexer;
    private readonly IDatabase _database;
    private readonly string _indexPrefix = typeof(T).Name.ToLower();
    private readonly string _idProperty = nameof(IIdModel.Id).ToLower();
    
    public RedisCacheHelper(IConnectionMultiplexer connectionMultiplexer)
    {
        _connectionMultiplexer = connectionMultiplexer;
        _database = _connectionMultiplexer.GetDatabase();
    }
    
    /// <inheritdoc />
    public virtual async Task<List<T>> GetAllAsync(CancellationToken ctx)
    {
        var endPoints = _connectionMultiplexer.GetEndPoints();
        var server = _connectionMultiplexer.GetServer(endPoints[0]);
        List<RedisValue> response = [];
        
        foreach (var key in server.Keys(pattern: $"{_indexPrefix}:{_idProperty}:*"))
            response.Add(await _database.StringGetAsync(key));
        return response.Count == 0 ? [] : response.Select(r => JsonSerializer.Deserialize<T>(r)).ToList();
    }

    /// <inheritdoc />
    public virtual async Task<T?> GetByIdAsync(int id, CancellationToken ctx)
    {
        RedisValue response = await _database.StringGetAsync($"{_indexPrefix}:{_idProperty}:{id}");
        if(response.IsNull)
            return default;
        return JsonSerializer.Deserialize<T>(response!);
    }

    /// <inheritdoc />
    public virtual async Task<T?> CreateAsync(T entity, CancellationToken ctx)
    {
        var response = await _database.StringSetAndGetAsync($"{_indexPrefix}:{_idProperty}:{entity.Id}", JsonSerializer.Serialize(entity));
        if(response.IsNull)
            return default;
        return JsonSerializer.Deserialize<T>(response!);
    }

    /// <inheritdoc />
    public virtual async Task<T?> UpdateAsync(int id, T entity, CancellationToken ctx)
    {
        await DeleteAsync(id, ctx);
        return await CreateAsync(entity, ctx);
    }

    /// <inheritdoc />
    public virtual async Task<bool> DeleteAsync(int id, CancellationToken ctx)
    {
        await _database.KeyDeleteAsync($"{_indexPrefix}:{_idProperty}:{id}");
        return true;
    }
}