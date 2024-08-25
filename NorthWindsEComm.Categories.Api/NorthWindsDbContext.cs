using Microsoft.EntityFrameworkCore;

namespace NorthWindsEComm.Categories.Api;

public class NorthWindsDbContext(DbContextOptions options) : DbContext(options)
{
    public DbSet<Category> Categories { get; set; }
}