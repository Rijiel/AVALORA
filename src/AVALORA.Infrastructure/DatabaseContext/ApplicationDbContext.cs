using AVALORA.Core.Domain.Models;
using AVALORA.Infrastructure.DbConfigurations;
using Microsoft.EntityFrameworkCore;

namespace AVALORA.Infrastructure.DatabaseContext;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {        
    }

    public DbSet<Category> Categories { get; set; }
    public DbSet<Product> Products { get; set; }
    public DbSet<ProductImage> ProductImages { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        #region Fluent API
        modelBuilder.Entity<Category>().HasIndex(x => x.Name).IsUnique();
        #endregion

        modelBuilder.ApplyConfiguration(new CategoriesSeedConfiguration());
        modelBuilder.ApplyConfiguration(new ProductsSeedConfiguration());
    }
}

