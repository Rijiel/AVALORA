using AVALORA.Core.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AVALORA.Infrastructure.DbConfigurations;

public class CategoriesSeedConfiguration : IEntityTypeConfiguration<Category>
{
    public void Configure(EntityTypeBuilder<Category> builder)
    {
        builder.HasData(
            new Category
            {
                Id = 1,
                Name = "Accessories"
            },
            new Category
            {
                Id = 2,
                Name = "Bags"
            },
            new Category
            {
                Id = 3,
                Name = "Shoes"
            },
            new Category
            {
                Id = 4,
                Name = "Electronics"
            },
            new Category
            {
                Id = 5,
                Name = "Furniture"
            }
        );
    }
}

