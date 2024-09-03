using NorthWindsEComm.CrudHelper;
using NorthWindsEComm.CrudHelper.Base;

namespace NorthWindsEComm.Products.Api;

/// <inheritdoc />
public interface IProductManager : ICrudManager<Product>
{
    /// <summary>
    /// Retrieves a list of products by category ID asynchronously.
    /// </summary>
    /// <param name="categoryId">The ID of the category.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A list of products filtered by the specified category ID.</returns>
    Task<List<Product>> GetProductsByCategoryIdAsync(int categoryId, CancellationToken cancellationToken);
}