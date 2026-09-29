using CateringPlanet.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CateringPlanet.Infrastructure.Configurations;

/// <summary>EF Core configuration for the <c>combo_menu</c> table.</summary>
public class ComboMenuConfiguration : IEntityTypeConfiguration<ComboMenu>
{
    public void Configure(EntityTypeBuilder<ComboMenu> builder)
    {
        builder.ToTable("combo_menu");

        builder.Property(e => e.Id).HasColumnName("id");
        builder.Property(e => e.CreatedAt)
            .HasDefaultValueSql("(getdate())")
            .HasColumnName("created_at");
        builder.Property(e => e.DiscountPercent)
            .HasColumnType("decimal(5, 2)")
            .HasColumnName("discount_percent");
        builder.Property(e => e.ImageFileName)
            .HasMaxLength(300)
            .HasColumnName("image_file_name");
        builder.Property(e => e.IsVisible).HasColumnName("is_visible");
        builder.Property(e => e.ModifiedAt)
            .HasDefaultValueSql("(getdate())")
            .HasColumnName("modified_at");
        builder.Property(e => e.Name)
            .HasMaxLength(30)
            .HasColumnName("name");
        builder.Property(e => e.Price)
            .HasColumnType("decimal(10, 2)")
            .HasColumnName("price");
    }
}
