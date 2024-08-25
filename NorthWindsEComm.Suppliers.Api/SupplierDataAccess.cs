using Microsoft.EntityFrameworkCore;
using NorthWindsEComm.CrudHelper;

namespace NorthWindsEComm.Suppliers.Api;

public class SupplierDataAccess : ICrudDataAccess<Supplier>
{
    private readonly SupplierContext _dbContext;

    public SupplierDataAccess(SupplierContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<List<Supplier>> GetAllAsync(CancellationToken ctx)
    {
        return await _dbContext.Suppliers.ToListAsync(ctx);
    }

    public async Task<Supplier?> GetByIdAsync(int id, CancellationToken ctx)
    {
        return await _dbContext.Suppliers.FindAsync([id], ctx);
    }

    public async Task<Supplier?> CreateAsync(Supplier entity, CancellationToken ctx)
    {
        var Supplier = await GetByIdAsync(entity.SupplierId, ctx);
        if (Supplier != null)
            return await UpdateAsync(entity, ctx);
        
        var response = await _dbContext.Suppliers.AddAsync(entity, ctx);
        return response.Entity;
    }

    public async Task<Supplier?> UpdateAsync(Supplier entity, CancellationToken ctx)
    {
        var Supplier = await GetByIdAsync(entity.SupplierId, ctx);
        if (Supplier != null)
            _dbContext.Entry(Supplier).CurrentValues.SetValues(entity);
        else
            return await CreateAsync(entity, ctx);
        
        return await GetByIdAsync(entity.SupplierId, ctx);
    }

    public async Task<bool> DeleteAsync(int id, CancellationToken ctx)
    {
        var Supplier = await GetByIdAsync(id, ctx);
        if (Supplier == null) return false;
        
        _dbContext.Suppliers.Remove(Supplier);
        return true;

    }
}