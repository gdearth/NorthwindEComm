using NorthWindsEComm.CrudHelper;

namespace NorthWindsEComm.Products.Api;

public class ProductManager : CrudManager<Product>, IProductManager
{
    private readonly IProductDataAccess _dataAccess;

    public ProductManager(IProductDataAccess dataAccess, ICrudCacheAccess<Product> cacheAccess, ILogger<CrudManager<Product>> logger) : base(dataAccess, cacheAccess, logger)
    {
        _dataAccess = dataAccess;
    }

    public async Task<List<Product>> GetProductsByCategoryIdAsync(int categoryId, CancellationToken cancellationToken) 
        => await _dataAccess.GetProductsByCategoryIdAsync(categoryId, cancellationToken);
}