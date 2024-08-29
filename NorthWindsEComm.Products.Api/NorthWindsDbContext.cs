using Microsoft.EntityFrameworkCore;

namespace NorthWindsEComm.Products.Api;

/// <inheritdoc />
public class NorthWindsDbContext(DbContextOptions options) : DbContext(options)
{
    /// <summary>
    /// 
    /// </summary>
    public DbSet<Product> Products { get; set; }
}