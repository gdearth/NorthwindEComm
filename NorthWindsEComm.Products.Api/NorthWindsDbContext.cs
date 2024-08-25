using Microsoft.EntityFrameworkCore;

namespace NorthWindsEComm.Products.Api;

public class NorthWindsDbContext(DbContextOptions options) : DbContext(options)
{
    public DbSet<Product> Products { get; set; }
}