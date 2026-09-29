using CateringPlanet.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CateringPlanet.Infrastructure.Configurations;

/// <summary>EF Core configuration for the <c>payment_method</c> table.</summary>
public class PaymentMethodConfiguration : IEntityTypeConfiguration<PaymentMethod>
{
    public void Configure(EntityTypeBuilder<PaymentMethod> builder)
    {
        builder.ToTable("payment_method");

        builder.Property(e => e.Id).HasColumnName("id");
        builder.Property(e => e.Type)
            .HasMaxLength(10)
            .HasColumnName("type");

        // NOTE: representative seed values; replace with the exact production dataset when available.
        builder.HasData(
            new PaymentMethod { Id = 1, Type = "Kartica" },
            new PaymentMethod { Id = 2, Type = "Gotovina" },
            new PaymentMethod { Id = 3, Type = "Online" });
    }
}
