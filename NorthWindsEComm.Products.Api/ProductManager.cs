using NorthWindsEComm.CrudHelper;
using NorthWindsEComm.CrudHelper.Base;
using NorthWindsEComm.CrudHelper.Cache;
using NorthWindsEComm.CrudHelper.Messaging;

namespace NorthWindsEComm.Products.Api;

/// <inheritdoc cref="NorthWindsEComm.Products.Api.IProductManager" />
public class ProductManager : CrudManager<Product>, IProductManager
{
    private readonly IProductDataAccess _productDataAccess;

    public ProductManager(IProductDataAccess dataAccess, ICrudCacheAccess<Product> cacheAccess, 
        ILogger<CrudManager<Product>> logger, IKafkaHelper<Product> kafkaHelper, CacheHitMetrics metrics) 
        : base(dataAccess, cacheAccess, logger, metrics, kafkaHelper)
    {
        _productDataAccess = dataAccess;
    }

    /// <summary>
    /// Retrieves a list of products by category ID asynchronously.
    /// </summary>
    /// <param name="categoryId">The ID of the category to filter products by.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task representing the asynchronous operation. The task result contains a list of products.</returns>
    public async Task<List<Product>> GetProductsByCategoryIdAsync(int categoryId, CancellationToken cancellationToken)
        => await _productDataAccess.GetProductsByCategoryIdAsync(categoryId, cancellationToken);
}