using CateringPlanet.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CateringPlanet.Infrastructure.Configurations;

/// <summary>EF Core configuration for the <c>payment_status</c> table.</summary>
public class PaymentStatusConfiguration : IEntityTypeConfiguration<PaymentStatus>
{
    public void Configure(EntityTypeBuilder<PaymentStatus> builder)
    {
        builder.ToTable("payment_status");

        builder.Property(e => e.Id)
            .ValueGeneratedNever()
            .HasColumnName("id");
        builder.Property(e => e.Name)
            .HasMaxLength(20)
            .HasColumnName("name");

        // NOTE: representative seed values; replace with the exact production dataset when available.
        builder.HasData(
            new PaymentStatus { Id = 1, Name = "Neplaćena" },
            new PaymentStatus { Id = 2, Name = "Plaćena" },
            new PaymentStatus { Id = 3, Name = "Refundirana" });
    }
}
