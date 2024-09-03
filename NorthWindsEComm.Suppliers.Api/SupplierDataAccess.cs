using Microsoft.EntityFrameworkCore;
using NorthWindsEComm.CrudHelper;
using NorthWindsEComm.CrudHelper.Base;

namespace NorthWindsEComm.Suppliers.Api;

/// <inheritdoc />
public class SupplierDataAccess : ICrudDataAccess<Supplier>
{
    private readonly SupplierContext _dbContext;

    public SupplierDataAccess(SupplierContext dbContext)
    {
        _dbContext = dbContext;
    }

    /// <inheritdoc />
    public async Task<List<Supplier>> GetAllAsync(CancellationToken ctx)
    {
        return await _dbContext.Suppliers.ToListAsync(ctx);
    }

    /// <inheritdoc />
    public async Task<Supplier?> GetByIdAsync(int id, CancellationToken ctx)
    {
        return await _dbContext.Suppliers.FindAsync([id], ctx);
    }

    /// <inheritdoc />
    public async Task<Supplier?> CreateAsync(Supplier entity, CancellationToken ctx)
    {
        if (entity.SupplierId >= 0)
        {
            var Supplier = await GetByIdAsync(entity.SupplierId, ctx);
            if (Supplier != null)
                return await UpdateAsync(entity.SupplierId, entity, ctx);
        }

        var response = await _dbContext.Suppliers.AddAsync(entity, ctx);
        await _dbContext.SaveChangesAsync(ctx);
        return response.Entity;
    }

    /// <inheritdoc />
    public async Task<Supplier?> UpdateAsync(int id, Supplier entity, CancellationToken ctx)
    {
        var Supplier = await GetByIdAsync(id, ctx);
        if (Supplier != null)
            _dbContext.Entry(Supplier).CurrentValues.SetValues(entity);
        else
            return await CreateAsync(entity, ctx);
        
        await _dbContext.SaveChangesAsync(ctx);
        return await GetByIdAsync(id, ctx);
    }

    /// <inheritdoc />
    public async Task<bool> DeleteAsync(int id, CancellationToken ctx)
    {
        var Supplier = await GetByIdAsync(id, ctx);
        if (Supplier == null) return false;
        
        _dbContext.Suppliers.Remove(Supplier);
        await _dbContext.SaveChangesAsync(ctx);
        return true;
    }
}