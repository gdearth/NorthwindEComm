namespace NorthWindsEComm.CrudHelper;

public interface ICrudManager<T> where T : class, IIdModel
{
    Task<List<T>> GetAllAsync(CancellationToken ctx);
    Task<T?> GetByIdAsync(int id, CancellationToken ctx);
    Task<T?> CreateAsync(T entity, CancellationToken ctx);
    Task<T?> UpdateAsync(T entity, CancellationToken ctx);
    Task<bool> DeleteAsync(int id, CancellationToken ctx);
}