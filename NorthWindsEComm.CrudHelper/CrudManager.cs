using Microsoft.Extensions.Logging;

namespace NorthWindsEComm.CrudHelper;

/// <inheritdoc />
public class CrudManager<T> : ICrudManager<T> where T : class, IIdModel
{
    private readonly ICrudDataAccess<T> _dataAccess;
    private readonly ICrudCacheAccess<T> _cacheAccess;
    private readonly ILogger<CrudManager<T>> _logger;

    public CrudManager(ICrudDataAccess<T> dataAccess, ICrudCacheAccess<T> cacheAccess, ILogger<CrudManager<T>> logger)
    {
        _dataAccess = dataAccess;
        _cacheAccess = cacheAccess;
        _logger = logger;
    }

    /// <inheritdoc />
    public virtual async Task<List<T>> GetAllAsync(CancellationToken ctx)
    {
        var results = await _cacheAccess.GetAllAsync(ctx);
        if (results.Count > 0)
        {
            _logger.LogInformation("Retrieved {results.Count} records", results.Count);
            return results;
        }

        _logger.LogInformation("No records found");
        results = await _dataAccess.GetAllAsync(ctx);
        foreach (var result in results)
        {
            await _cacheAccess.CreateAsync(result, ctx);
            _logger.LogInformation("Created cache record with id: {result.Id}", result.Id);
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
            return result;
        }

        result = await _dataAccess.GetByIdAsync(id, ctx);
        if(result is not null)
        {
            var cacheResult = await _cacheAccess.CreateAsync(result, ctx);
            if (cacheResult is not null)
                _logger.LogInformation("Created cache record with id: {id}", id);
        }

        return result;
    }

    /// <inheritdoc />
    public virtual async Task<T?> CreateAsync(T entity, CancellationToken ctx)
    {
        var result = await  _dataAccess.CreateAsync(entity, ctx);
        if (result is not null)
        {
            _logger.LogInformation("Created database record with id: {result.Id}", result.Id);
            var cacheResult = await _cacheAccess.CreateAsync(result, ctx);
            if (cacheResult is not null)
                _logger.LogInformation("Created cache record with id: {cacheResult.Id}", cacheResult.Id);
        }

        return result;
    }

    /// <inheritdoc />
    public virtual async Task<T?> UpdateAsync(int id, T entity, CancellationToken ctx)
    {
        var result = await _dataAccess.UpdateAsync(id, entity, ctx);
        if (result is not null)
        {
            _logger.LogInformation("Updated database record with id: {entity.Id}", entity.Id);
            var cacheResult = await _cacheAccess.UpdateAsync(id, result, ctx);
            if (cacheResult is not null)
                _logger.LogInformation("Updated cache record with id: {cacheResult.Id}", cacheResult.Id);
        }

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