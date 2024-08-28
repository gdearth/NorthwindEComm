using Microsoft.EntityFrameworkCore;

namespace NorthWindsEComm.Suppliers.Api;

public class SupplierContext(DbContextOptions options) : DbContext(options)
{
    public DbSet<Supplier> Suppliers { get; set; }
}