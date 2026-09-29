using CateringPlanet.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CateringPlanet.Infrastructure.Configurations;

/// <summary>EF Core configuration for the <c>order_status</c> table.</summary>
public class OrderStatusConfiguration : IEntityTypeConfiguration<OrderStatus>
{
    public void Configure(EntityTypeBuilder<OrderStatus> builder)
    {
        builder.ToTable("order_status");

        builder.Property(e => e.Id)
            .ValueGeneratedNever()
            .HasColumnName("id");
        builder.Property(e => e.Name)
            .HasMaxLength(30)
            .HasColumnName("name");

        // NOTE: representative seed values; replace with the exact production dataset when available.
        builder.HasData(
            new OrderStatus { Id = 1, Name = "Primljena" },
            new OrderStatus { Id = 2, Name = "Potvrđena" },
            new OrderStatus { Id = 3, Name = "U pripremi" },
            new OrderStatus { Id = 4, Name = "Spremna" },
            new OrderStatus { Id = 5, Name = "U dostavi" },
            new OrderStatus { Id = 6, Name = "Završena" },
            new OrderStatus { Id = 7, Name = "Otkazana" },
            new OrderStatus { Id = 8, Name = "Odbijena" });
    }
}
