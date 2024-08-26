using NorthWindsEComm.CrudHelper;

namespace NorthWindsEComm.Products.Api;

public interface IProductDataAccess : ICrudDataAccess<Product>
{
    Task<List<Product>> GetProductsByCategoryIdAsync(int categoryId, CancellationToken cancellationToken);
}