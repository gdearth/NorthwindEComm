using Microsoft.EntityFrameworkCore;
using NorthWindsEComm.CrudHelper;

namespace NorthWindsEComm.Products.Api;

/// <inheritdoc />
public class ProductDataAccess : IProductDataAccess
{
    private readonly NorthWindsDbContext _dbContext;

    public ProductDataAccess(NorthWindsDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    /// <inheritdoc />
    public async Task<List<Product>> GetAllAsync(CancellationToken ctx)
    {
        return await _dbContext.Products.ToListAsync(ctx);
    }

    /// <inheritdoc />
    public async Task<Product?> GetByIdAsync(int id, CancellationToken ctx)
    {
        return await _dbContext.Products.FindAsync([id], ctx);
    }

    /// <inheritdoc />
    public async Task<Product?> CreateAsync(Product entity, CancellationToken ctx)
    {
        if (entity.ProductId >= 0)
        {
            var product = await GetByIdAsync(entity.ProductId, ctx);
            if (product != null)
                return await UpdateAsync(entity.ProductId, entity, ctx);
        }

        var response = await _dbContext.Products.AddAsync(entity, ctx);
        await _dbContext.SaveChangesAsync(ctx);
        return response.Entity;
    }

    /// <inheritdoc />
    public async Task<Product?> UpdateAsync(int id, Product entity, CancellationToken ctx)
    {
        var product = await GetByIdAsync(id, ctx);
        if (product != null)
        {
            _dbContext.Entry(product).CurrentValues.SetValues(entity);
            await _dbContext.SaveChangesAsync(ctx);
        }
        else
        {
            return await CreateAsync(entity, ctx);
        }

        return await GetByIdAsync(id, ctx);
    }

    public async Task<bool> DeleteAsync(int id, CancellationToken ctx)
    {
        var product = await GetByIdAsync(id, ctx);
        if (product == null) return false;
        
        _dbContext.Products.Remove(product);
        await _dbContext.SaveChangesAsync(ctx);
        return true;
    }

    public async Task<List<Product>> GetProductsByCategoryIdAsync(int categoryId, CancellationToken cancellationToken)
    {
        return await _dbContext.Products.Where(p => p.CategoryId == categoryId).ToListAsync(cancellationToken);
    }
}