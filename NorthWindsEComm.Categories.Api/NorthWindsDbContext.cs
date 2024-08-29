using Microsoft.EntityFrameworkCore;

namespace NorthWindsEComm.Categories.Api;

/// <inheritdoc />
public class NorthWindsDbContext(DbContextOptions options) : DbContext(options)
{
    /// <summary>
    /// Represents a category entity in the database.
    /// </summary>
    public DbSet<Category> Categories { get; set; }
}