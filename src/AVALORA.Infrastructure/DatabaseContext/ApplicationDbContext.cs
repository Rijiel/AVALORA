using AVALORA.Core.Domain.Models;
using AVALORA.Infrastructure.DbConfigurations;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace AVALORA.Infrastructure.DatabaseContext;

public class ApplicationDbContext : IdentityDbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {        
    }

    public DbSet<Category> Categories { get; set; }
    public DbSet<Product> Products { get; set; }
    public DbSet<ProductImage> ProductImages { get; set; }
    public DbSet<ApplicationUser> ApplicationUsers { get; set; }
    public DbSet<CartItem> CartItems { get; set; }
    public DbSet<OrderHeader> OrderHeaders { get; set; }
    public DbSet<OrderSummaryItem> OrderSummaryItems { get; set; }
    public DbSet<OrderSummary> OrderSummaries { get; set; }
    public DbSet<ProductReview> ProductReviews { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        #region Fluent API
        modelBuilder.Entity<Category>().HasIndex(c => c.Name).IsUnique();
        modelBuilder.Entity<OrderHeader>().HasIndex(o => o.TrackingNumber).IsUnique();
        modelBuilder.Entity<CartItem>().Navigation(e => e.Product).AutoInclude();
        #endregion

        modelBuilder.ApplyConfiguration(new CategoriesSeedConfiguration());
        modelBuilder.ApplyConfiguration(new ProductsSeedConfiguration());
    }
}

