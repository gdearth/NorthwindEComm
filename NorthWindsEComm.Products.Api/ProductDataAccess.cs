using Microsoft.EntityFrameworkCore;
using NorthWindsEComm.CrudHelper;

namespace NorthWindsEComm.Products.Api;

public class ProductDataAccess : ICrudDataAccess<Product>
{
    private readonly NorthWindsDbContext _dbContext;

    public ProductDataAccess(NorthWindsDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<List<Product>> GetAllAsync(CancellationToken ctx)
    {
        return await _dbContext.Products.ToListAsync(ctx);
    }

    public async Task<Product?> GetByIdAsync(int id, CancellationToken ctx)
    {
        return await _dbContext.Products.FindAsync([id], ctx);
    }

    public async Task<Product?> CreateAsync(Product entity, CancellationToken ctx)
    {
        var product = await GetByIdAsync(entity.ProductId, ctx);
        if (product != null)
            return await UpdateAsync(entity, ctx);
        
        var response = await _dbContext.Products.AddAsync(entity, ctx);
        return response.Entity;
    }

    public async Task<Product?> UpdateAsync(Product entity, CancellationToken ctx)
    {
        var product = await GetByIdAsync(entity.ProductId, ctx);
        if (product != null)
            _dbContext.Entry(product).CurrentValues.SetValues(entity);
        else
            return await CreateAsync(entity, ctx);
        
        return await GetByIdAsync(entity.ProductId, ctx);
    }

    public async Task<bool> DeleteAsync(int id, CancellationToken ctx)
    {
        var product = await GetByIdAsync(id, ctx);
        if (product == null) return false;
        
        _dbContext.Products.Remove(product);
        return true;
    }
}