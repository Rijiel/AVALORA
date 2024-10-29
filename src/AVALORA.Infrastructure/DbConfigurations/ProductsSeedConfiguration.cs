using AVALORA.Core.Domain.Models;
using AVALORA.Core.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AVALORA.Infrastructure.DbConfigurations;

public class ProductsSeedConfiguration : IEntityTypeConfiguration<Product>
{
	public void Configure(EntityTypeBuilder<Product> builder)
	{
		builder.HasData(
			new Product
			{
				Id = 1,
				Name = "Product 1",
				Description = "This is a test product.",
				ListPrice = 90,
				Price = 100,
				CategoryId = 1,
				Colors = [Color.Red, Color.Black]
			},
			new Product
			{
				Id = 2,
				Name = "Product 2",
				Description = "This is a test product.",
				ListPrice = 45.50,
				Price = 60,
				CategoryId = 2,
				Colors = [Color.Orange, Color.Green, Color.Blue]
			},
			new Product
			{
				Id = 3,
				Name = "Product 3",
				Description = "This is a test product.",
				ListPrice = 79.99,
				Price = 85.99,
				CategoryId = 3,
				Colors = [Color.White]
			}
		);
	}
}

