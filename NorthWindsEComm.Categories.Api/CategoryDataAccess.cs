using Microsoft.EntityFrameworkCore;
using NorthWindsEComm.CrudHelper;
using NorthWindsEComm.CrudHelper.Base;

namespace NorthWindsEComm.Categories.Api;

/// <inheritdoc />
public class CategoryDataAccess : ICrudDataAccess<Category>
{
    private readonly NorthWindsDbContext _dbContext;

    public CategoryDataAccess(NorthWindsDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    /// <inheritdoc />
    public async Task<List<Category>> GetAllAsync(CancellationToken ctx) => await _dbContext.Categories.ToListAsync(ctx);

    /// <inheritdoc />
    public async Task<Category?> GetByIdAsync(int id, CancellationToken ctx) => await _dbContext.Categories.FindAsync([id], ctx);

    /// <inheritdoc />
    public async Task<Category?> CreateAsync(Category entity, CancellationToken ctx)
    {
        if (entity.CategoryId > 0)
        {
            var category = await GetByIdAsync(entity.CategoryId, ctx);
            if (category != null)
                return await UpdateAsync(entity.CategoryId, entity, ctx);
        }

        var response = await _dbContext.Categories.AddAsync(entity, ctx);
        await _dbContext.SaveChangesAsync(ctx);
        return response.Entity;
    }

    /// <inheritdoc />
    public async Task<Category?> UpdateAsync(int id, Category entity, CancellationToken ctx)
    {
        var category = await GetByIdAsync(id, ctx);
        if (category != null)
            _dbContext.Entry(category).CurrentValues.SetValues(entity);
        else
            return await CreateAsync(entity, ctx);
        
        await _dbContext.SaveChangesAsync(ctx);
        return await GetByIdAsync(id, ctx);
    }

    /// <inheritdoc />
    public async Task<bool> DeleteAsync(int id, CancellationToken ctx)
    {
        var product = await GetByIdAsync(id, ctx);
        if (product == null) return false;
        
        _dbContext.Categories.Remove(product);
        await _dbContext.SaveChangesAsync(ctx);
        return true;
    }
}