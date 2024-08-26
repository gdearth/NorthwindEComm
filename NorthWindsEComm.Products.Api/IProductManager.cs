using NorthWindsEComm.CrudHelper;

namespace NorthWindsEComm.Products.Api;

public interface IProductManager : ICrudManager<Product>
{
    Task<List<Product>> GetProductsByCategoryIdAsync(int categoryId, CancellationToken cancellationToken);
}