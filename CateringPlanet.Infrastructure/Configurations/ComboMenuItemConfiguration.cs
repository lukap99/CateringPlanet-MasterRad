using CateringPlanet.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CateringPlanet.Infrastructure.Configurations;

/// <summary>EF Core configuration for the <c>combo_menu_item</c> table.</summary>
public class ComboMenuItemConfiguration : IEntityTypeConfiguration<ComboMenuItem>
{
    public void Configure(EntityTypeBuilder<ComboMenuItem> builder)
    {
        builder.ToTable("combo_menu_item");

        builder.HasIndex(e => new { e.ComboMenuId, e.FoodId }, "IX_combo_menu_item_food").IsUnique();

        builder.Property(e => e.Id).HasColumnName("id");
        builder.Property(e => e.Amount)
            .HasDefaultValue(1)
            .HasColumnName("amount");
        builder.Property(e => e.ComboMenuId).HasColumnName("combo_menu_id");
        builder.Property(e => e.FoodId).HasColumnName("food_id");

        builder.HasOne(d => d.ComboMenu).WithMany(p => p.ComboMenuItems)
            .HasForeignKey(d => d.ComboMenuId)
            .HasConstraintName("FK_combo_menu_item_combo_menu");

        builder.HasOne(d => d.Food).WithMany(p => p.ComboMenuItems)
            .HasForeignKey(d => d.FoodId)
            .OnDelete(DeleteBehavior.ClientSetNull)
            .HasConstraintName("FK_combo_menu_item_food");
    }
}
