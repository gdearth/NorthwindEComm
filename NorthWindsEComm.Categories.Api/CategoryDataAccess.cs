using Microsoft.EntityFrameworkCore;
using NorthWindsEComm.CrudHelper;

namespace NorthWindsEComm.Categories.Api;

public class CategoryDataAccess : ICrudDataAccess<Category>
{
    private readonly NorthWindsDbContext _dbContext;

    public CategoryDataAccess(NorthWindsDbContext dbContext)
    {
        _dbContext = dbContext;
    }
    
    public async Task<List<Category>> GetAllAsync(CancellationToken ctx)
    {
        return await _dbContext.Categories.ToListAsync(ctx);
    }

    public async Task<Category?> GetByIdAsync(int id, CancellationToken ctx)
    {
        return await _dbContext.Categories.FindAsync([id], ctx);
    }

    public async Task<Category?> CreateAsync(Category entity, CancellationToken ctx)
    {
        var category = await GetByIdAsync(entity.CategoryId, ctx);
        if (category != null)
            return await UpdateAsync(entity, ctx);
        
        var response = await _dbContext.Categories.AddAsync(entity, ctx);
        return response.Entity;
    }

    public async Task<Category?> UpdateAsync(Category entity, CancellationToken ctx)
    {
        var category = await GetByIdAsync(entity.CategoryId, ctx);
        if (category != null)
            _dbContext.Entry(category).CurrentValues.SetValues(entity);
        else
            return await CreateAsync(entity, ctx);
        
        return await GetByIdAsync(entity.CategoryId, ctx);
    }

    public async Task<bool> DeleteAsync(int id, CancellationToken ctx)
    {
        var product = await GetByIdAsync(id, ctx);
        if (product == null) return false;
        
        _dbContext.Categories.Remove(product);
        return true;
    }
}