using CateringPlanet.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CateringPlanet.Infrastructure.Configurations;

/// <summary>EF Core configuration for the <c>order_discount</c> table.</summary>
public class OrderDiscountConfiguration : IEntityTypeConfiguration<OrderDiscount>
{
    public void Configure(EntityTypeBuilder<OrderDiscount> builder)
    {
        builder.ToTable("order_discount");

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
            .HasMaxLength(40)
            .HasColumnName("name");
        builder.Property(e => e.PointCost).HasColumnName("point_cost");

        // NOTE: representative seed values; replace with the exact production dataset when available.
        var seed = new DateTime(2024, 1, 1);
        builder.HasData(
            new OrderDiscount { Id = 1, Name = "Bez popusta", DiscountPercent = 0m, PointCost = 0, CreatedAt = seed, ModifiedAt = seed },
            new OrderDiscount { Id = 2, Name = "Popust 5%", DiscountPercent = 5m, PointCost = 100, CreatedAt = seed, ModifiedAt = seed },
            new OrderDiscount { Id = 3, Name = "Popust 10%", DiscountPercent = 10m, PointCost = 200, CreatedAt = seed, ModifiedAt = seed },
            new OrderDiscount { Id = 4, Name = "Popust 15%", DiscountPercent = 15m, PointCost = 300, CreatedAt = seed, ModifiedAt = seed });
    }
}
