using CateringPlanet.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CateringPlanet.Infrastructure.Configurations;

/// <summary>EF Core configuration for the <c>food_category</c> table.</summary>
public class FoodCategoryConfiguration : IEntityTypeConfiguration<FoodCategory>
{
    public void Configure(EntityTypeBuilder<FoodCategory> builder)
    {
        builder.ToTable("food_category");

        builder.Property(e => e.Id).HasColumnName("id");
        builder.Property(e => e.CreatedAt)
            .HasDefaultValueSql("(getdate())")
            .HasColumnName("created_at");
        builder.Property(e => e.DiscountPercent)
            .HasColumnType("decimal(5, 2)")
            .HasColumnName("discount_percent");
        builder.Property(e => e.ModifiedAt)
            .HasDefaultValueSql("(getdate())")
            .HasColumnName("modified_at");
        builder.Property(e => e.Name)
            .HasMaxLength(30)
            .HasColumnName("name");
        builder.Property(e => e.Slug)
            .HasMaxLength(50)
            .HasColumnName("slug");

        // NOTE: representative seed values; replace with the exact production dataset when available.
        var seed = new DateTime(2024, 1, 1);
        builder.HasData(
            new FoodCategory { Id = 1, Name = "Supe i čorbe", Slug = "supe-i-corbe", DiscountPercent = 0m, CreatedAt = seed, ModifiedAt = seed },
            new FoodCategory { Id = 2, Name = "Hleb i peciva", Slug = "hleb-i-peciva", DiscountPercent = 0m, CreatedAt = seed, ModifiedAt = seed },
            new FoodCategory { Id = 3, Name = "Torte i kolači", Slug = "torte-i-kolaci", DiscountPercent = 0m, CreatedAt = seed, ModifiedAt = seed },
            new FoodCategory { Id = 4, Name = "Salate", Slug = "salate", DiscountPercent = 0m, CreatedAt = seed, ModifiedAt = seed },
            new FoodCategory { Id = 5, Name = "Kuvana jela", Slug = "kuvana-jela", DiscountPercent = 0m, CreatedAt = seed, ModifiedAt = seed },
            new FoodCategory { Id = 6, Name = "Roštilj", Slug = "rostilj", DiscountPercent = 0m, CreatedAt = seed, ModifiedAt = seed },
            new FoodCategory { Id = 7, Name = "Kanapei", Slug = "kanapei", DiscountPercent = 0m, CreatedAt = seed, ModifiedAt = seed },
            new FoodCategory { Id = 8, Name = "Pića", Slug = "pica", DiscountPercent = 0m, CreatedAt = seed, ModifiedAt = seed },
            new FoodCategory { Id = 9, Name = "Vegan", Slug = "vegan", DiscountPercent = 0m, CreatedAt = seed, ModifiedAt = seed },
            new FoodCategory { Id = 10, Name = "Pizza", Slug = "pizza", DiscountPercent = 0m, CreatedAt = seed, ModifiedAt = seed });
    }
}
