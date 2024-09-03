using Microsoft.Extensions.Logging;
using NorthWindsEComm.CrudHelper.Cache;
using NorthWindsEComm.CrudHelper.Messaging;

namespace NorthWindsEComm.CrudHelper.Base;

/// <inheritdoc />
public class CrudManager<T> : ICrudManager<T> where T : class, IIdModel
{
    protected readonly ICrudDataAccess<T> _dataAccess;
    protected readonly ICrudCacheAccess<T> _cacheAccess;
    protected readonly ILogger<CrudManager<T>> _logger;
    protected readonly CacheHitMetrics _metrics;
    protected readonly IKafkaHelper<T> _kafkaHelper;

    public CrudManager(ICrudDataAccess<T> dataAccess, ICrudCacheAccess<T> cacheAccess, 
        ILogger<CrudManager<T>> logger, CacheHitMetrics metrics, IKafkaHelper<T> kafkaHelper)
    {
        _dataAccess = dataAccess;
        _cacheAccess = cacheAccess;
        _logger = logger;
        _metrics = metrics;
        _kafkaHelper = kafkaHelper;
    }

    /// <inheritdoc />
    public virtual async Task<List<T>> GetAllAsync(CancellationToken ctx)
    {
        var results = await _cacheAccess.GetAllAsync(ctx);
        if (results.Count > 0)
        {
            _logger.LogInformation("Retrieved {ResultsCount} records", results.Count);
            _metrics.Hit(results.Count);
            return results;
        }

        _logger.LogInformation("No records found");
        results = await _dataAccess.GetAllAsync(ctx);
        
        if (results.Count <= 0) return results;
        
        _metrics.Miss(results.Count);
            
        foreach (var result in results)
        {
            await _cacheAccess.CreateAsync(result, ctx);
            _logger.LogInformation("Created cache record with id: {ResultId}", result.Id);
        }

        return results;
    }

    /// <inheritdoc />
    public virtual async Task<T?> GetByIdAsync(int id, CancellationToken ctx)
    {
        var result = await _cacheAccess.GetByIdAsync(id, ctx);
        if (result is not null)
        {
            _logger.LogInformation("Retrieved record with id: {id}", id);
            _metrics.Hit();
            return result;
        }
        
        result = await _dataAccess.GetByIdAsync(id, ctx);
        
        if (result is null) return result;
        
        _metrics.Miss();
        var cacheResult = await _cacheAccess.CreateAsync(result, ctx);
        if (cacheResult is not null)
            _logger.LogInformation("Created cache record with id: {id}", id);

        return result;
    }

    /// <inheritdoc />
    public virtual async Task<T?> CreateAsync(T entity, CancellationToken ctx)
    {
        var result = await  _dataAccess.CreateAsync(entity, ctx);
        if (result is null) return result;
        
        _logger.LogInformation("Created database record with id: {ResultId}", result.Id);
        var cacheResult = await _cacheAccess.CreateAsync(result, ctx);
        if (cacheResult is not null)
            _logger.LogInformation("Created cache record with id: {CacheResultId}", cacheResult.Id);
        _ = await _kafkaHelper.PublishAsync(EventType.Create, result, ctx);

        return result;
    }

    /// <inheritdoc />
    public virtual async Task<T?> UpdateAsync(int id, T entity, CancellationToken ctx)
    {
        var result = await _dataAccess.UpdateAsync(id, entity, ctx);
        if (result is null) return result;
        
        _logger.LogInformation("Updated database record with id: {EntityId}", entity.Id);
        var cacheResult = await _cacheAccess.UpdateAsync(id, result, ctx);
        if (cacheResult is not null)
            _logger.LogInformation("Updated cache record with id: {CacheResultId}", cacheResult.Id);
        _ = await _kafkaHelper.PublishAsync(EventType.Update, result, ctx);
        
        return result;
    }

    /// <inheritdoc />
    public virtual async Task<bool> DeleteAsync(int id, CancellationToken ctx)
    {
        var result = await _dataAccess.DeleteAsync(id, ctx);
        if (!result) return result;
        
        _logger.LogInformation("Deleted database record with id: {id}", id);
        var cacheResult = await _cacheAccess.DeleteAsync(id, ctx);
        if (cacheResult)
            _logger.LogInformation("Deleted cache record with id: {id}", id);
        
        return result;
    }
}