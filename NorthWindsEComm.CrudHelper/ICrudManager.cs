namespace NorthWindsEComm.CrudHelper;

/// <summary>
/// Represents a CRUD manager that provides basic CRUD operations for a specific type of entity.
/// </summary>
/// <typeparam name="T">The type of entity.</typeparam>
public interface ICrudManager<T> where T : class, IIdModel
{
    /// <summary>
    /// Retrieves all items asynchronously.
    /// </summary>
    /// <param name="ctx">The cancellation token.</param>
    /// <returns>A task that represents the asynchronous operation and contains a list of items of type T.</returns>
    Task<List<T>> GetAllAsync(CancellationToken ctx);

    /// <summary>
    /// Retrieves an item asynchronously by its ID.
    /// </summary>
    /// <param name="id">The ID of the item to retrieve.</param>
    /// <param name="ctx">The cancellation token.</param>
    /// <returns>
    /// A task that represents the asynchronous operation and contains the item of type T with the specified ID.
    /// If the item does not exist, the task will return null.
    /// </returns>
    Task<T?> GetByIdAsync(int id, CancellationToken ctx);

    /// <summary>
    /// Creates a new entity asynchronously.
    /// </summary>
    /// <param name="entity">The entity to be created.</param>
    /// <param name="ctx">The cancellation token.</param>
    /// <returns>A task that represents the asynchronous operation and contains the created entity of type T.</returns>
    Task<T?> CreateAsync(T entity, CancellationToken ctx);

    /// <summary>
    /// Updates an entity asynchronously.
    /// </summary>
    /// <param name="id">The ID of the entity to update.</param>
    /// <param name="entity">The updated entity.</param>
    /// <param name="ctx">The cancellation token.</param>
    /// <returns>A task that represents the asynchronous operation and contains the updated entity of type T, or null if the entity was not found.</returns>
    Task<T?> UpdateAsync(int id, T entity, CancellationToken ctx);

    /// <summary>
    /// Deletes an entity asynchronously by its ID.
    /// </summary>
    /// <param name="id">The ID of the entity to delete.</param>
    /// <param name="ctx">The cancellation token.</param>
    /// <returns>A task that represents the asynchronous operation and returns a boolean value indicating whether the deletion was successful.</returns>
    Task<bool> DeleteAsync(int id, CancellationToken ctx);
}