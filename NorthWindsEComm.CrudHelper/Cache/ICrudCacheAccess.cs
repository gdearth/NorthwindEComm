using NorthWindsEComm.CrudHelper.Base;

namespace NorthWindsEComm.CrudHelper.Cache;

public interface ICrudCacheAccess<T> where T : class, IIdModel
{
    Task<List<T>> GetAllAsync(CancellationToken ctx);
    Task<T?> GetByIdAsync(int id, CancellationToken ctx);
    Task<T?> CreateAsync(T entity, CancellationToken ctx);
    Task<T?> UpdateAsync(int id, T entity, CancellationToken ctx);
    Task<bool> DeleteAsync(int id, CancellationToken ctx);
}