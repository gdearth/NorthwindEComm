using Microsoft.EntityFrameworkCore;

namespace NorthWindsEComm.Suppliers.Api;

/// <inheritdoc />
public class SupplierContext(DbContextOptions options) : DbContext(options)
{
    /// <summary>
    /// 
    /// </summary>
    public DbSet<Supplier> Suppliers { get; set; }
}