using CateringPlanet.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CateringPlanet.Infrastructure.Configurations;

/// <summary>EF Core configuration for the <c>order_food_item</c> table.</summary>
public class OrderFoodItemConfiguration : IEntityTypeConfiguration<OrderFoodItem>
{
    public void Configure(EntityTypeBuilder<OrderFoodItem> builder)
    {
        builder.ToTable("order_food_item");

        builder.HasIndex(e => e.OrderId, "IX_order_food_item_order_id");

        builder.Property(e => e.Id).HasColumnName("id");
        builder.Property(e => e.Amount)
            .HasDefaultValue(1)
            .HasColumnName("amount");
        builder.Property(e => e.BasePrice)
            .HasColumnType("decimal(10, 2)")
            .HasColumnName("base_price");
        builder.Property(e => e.DiscountPercent)
            .HasColumnType("decimal(5, 2)")
            .HasColumnName("discount_percent");
        builder.Property(e => e.FoodId).HasColumnName("food_id");
        builder.Property(e => e.OrderId).HasColumnName("order_id");
        builder.Property(e => e.Price)
            .HasColumnType("decimal(10, 2)")
            .HasColumnName("price");

        builder.HasOne(d => d.Food).WithMany(p => p.OrderFoodItems)
            .HasForeignKey(d => d.FoodId)
            .OnDelete(DeleteBehavior.ClientSetNull)
            .HasConstraintName("FK_orderFoodItem_food");

        builder.HasOne(d => d.Order).WithMany(p => p.OrderFoodItems)
            .HasForeignKey(d => d.OrderId)
            .HasConstraintName("FK_orderFoodItem_order");
    }
}
