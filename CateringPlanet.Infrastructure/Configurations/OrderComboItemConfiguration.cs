using CateringPlanet.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CateringPlanet.Infrastructure.Configurations;

/// <summary>EF Core configuration for the <c>order_combo_item</c> table.</summary>
public class OrderComboItemConfiguration : IEntityTypeConfiguration<OrderComboItem>
{
    public void Configure(EntityTypeBuilder<OrderComboItem> builder)
    {
        builder.ToTable("order_combo_item");

        builder.HasIndex(e => e.OrderId, "IX_order_combo_item_order_id");

        builder.Property(e => e.Id).HasColumnName("id");
        builder.Property(e => e.Amount)
            .HasDefaultValue(1)
            .HasColumnName("amount");
        builder.Property(e => e.BasePrice)
            .HasColumnType("decimal(10, 2)")
            .HasColumnName("base_price");
        builder.Property(e => e.ComboMenuId).HasColumnName("combo_menu_id");
        builder.Property(e => e.DiscountPercent)
            .HasColumnType("decimal(5, 2)")
            .HasColumnName("discount_percent");
        builder.Property(e => e.OrderId).HasColumnName("order_id");
        builder.Property(e => e.Price)
            .HasColumnType("decimal(10, 2)")
            .HasColumnName("price");

        builder.HasOne(d => d.ComboMenu).WithMany(p => p.OrderComboItems)
            .HasForeignKey(d => d.ComboMenuId)
            .OnDelete(DeleteBehavior.ClientSetNull)
            .HasConstraintName("FK_orderComboItem_combo");

        builder.HasOne(d => d.Order).WithMany(p => p.OrderComboItems)
            .HasForeignKey(d => d.OrderId)
            .HasConstraintName("FK_orderComboItem_order");
    }
}
