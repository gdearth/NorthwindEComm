namespace NorthWindsEComm.CrudHelper.Base;

/// <summary>
/// Represents an interface for CRUD (Create, Read, Update, Delete) operations for a specific entity.
/// </summary>
/// <typeparam name="T">The type of entity.</typeparam>
public interface ICrudDataAccess<T> where T : class, IIdModel
{
    /// <summary>
    /// Gets all entities of type T asynchronously.
    /// </summary>
    /// <param name="
    Task<List<T>> GetAllAsync(CancellationToken ctx);

    /// <summary>
    /// Retrieves an entity of type <typeparamref name="T"/> asynchronously by its ID.
    /// </summary>
    /// <param name="id">The ID of the entity to retrieve.</param>
    /// <param name="ctx">The cancellation token.</param>
    /// <returns>The retrieved entity, or <see langword="null"/> if not found.</returns>
    Task<T?> GetByIdAsync(int id, CancellationToken ctx);

    /// <summary>
    /// Creates a new entity of type T asynchronously.
    /// If an entity with the same Id already exists, it will be updated instead of creating a new one.
    /// </summary>
    /// <param name="entity">The entity to create or update.</param>
    /// <param name="ctx">The cancellation token.</param>
    /// <returns>The created or updated entity.</returns>
    Task<T?> CreateAsync(T entity, CancellationToken ctx);

    /// <summary>
    /// Updates an entity of type T asynchronously with the specified ID.
    /// If the entity with the given ID exists, the current values of the entity are updated with the provided entity object.
    /// If the entity with the given ID does not exist, a new entity is created using the provided entity object.
    /// Returns the updated or created entity as a result of the operation.
    /// </summary>
    /// <param name="id">The ID of the entity to be updated.</param>
    /// <param name="entity">The entity object containing the updated values.</param>
    /// <param name="ctx">The cancellation token to cancel the operation if needed.</param>
    /// <returns>The updated or created entity.</returns>
    Task<T?> UpdateAsync(int id, T entity, CancellationToken ctx);

    /// <summary>
    /// Deletes an entity of type T asynchronously based on its id.
    /// </summary>
    /// <param name="id">The id of the entity to delete.</param>
    /// <param name="ctx">The cancellation token.</param>
    /// <returns>True if the entity is deleted successfully, false otherwise.</returns>
    Task<bool> DeleteAsync(int id, CancellationToken ctx);
}