using CateringPlanet.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CateringPlanet.Infrastructure.Configurations;

/// <summary>EF Core configuration for the <c>food</c> table.</summary>
public class FoodConfiguration : IEntityTypeConfiguration<Food>
{
    public void Configure(EntityTypeBuilder<Food> builder)
    {
        builder.ToTable("food");

        builder.HasIndex(e => e.FoodCategoryId, "IX_food_category_id");

        builder.Property(e => e.Id).HasColumnName("id");
        builder.Property(e => e.CreatedAt)
            .HasDefaultValueSql("(getdate())")
            .HasColumnName("created_at");
        builder.Property(e => e.DiscountPercent)
            .HasColumnType("decimal(5, 2)")
            .HasColumnName("discount_percent");
        builder.Property(e => e.FoodCategoryId).HasColumnName("food_category_id");
        builder.Property(e => e.ImageFileName)
            .HasMaxLength(300)
            .HasColumnName("image_file_name");
        builder.Property(e => e.Ingredients).HasColumnName("ingredients");
        builder.Property(e => e.IsVisible).HasColumnName("is_visible");
        builder.Property(e => e.ModifiedAt)
            .HasDefaultValueSql("(getdate())")
            .HasColumnName("modified_at");
        builder.Property(e => e.Name)
            .HasMaxLength(50)
            .HasColumnName("name");
        builder.Property(e => e.Price)
            .HasColumnType("decimal(10, 2)")
            .HasColumnName("price");
        builder.Property(e => e.UnitOfMeasurement)
            .HasMaxLength(20)
            .HasColumnName("unit_of_measurement");

        builder.HasOne(d => d.FoodCategory).WithMany(p => p.Foods)
            .HasForeignKey(d => d.FoodCategoryId)
            .OnDelete(DeleteBehavior.ClientSetNull)
            .HasConstraintName("FK_food_food_category");
    }
}
