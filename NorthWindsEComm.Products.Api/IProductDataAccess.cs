using NorthWindsEComm.CrudHelper;
using NorthWindsEComm.CrudHelper.Base;

namespace NorthWindsEComm.Products.Api;

/// <inheritdoc />
public interface IProductDataAccess : ICrudDataAccess<Product>
{
    /// <summary>
    /// Retrieves a list of products by category ID asynchronously.
    /// </summary>
    /// <param name="categoryId">The category ID to filter the products.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the list of products.</returns>
    Task<List<Product>> GetProductsByCategoryIdAsync(int categoryId, CancellationToken cancellationToken);
}