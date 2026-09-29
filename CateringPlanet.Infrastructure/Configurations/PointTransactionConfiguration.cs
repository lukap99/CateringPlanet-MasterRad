using CateringPlanet.Domain.Entities;
using CateringPlanet.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CateringPlanet.Infrastructure.Configurations;

/// <summary>EF Core configuration for the <c>point_transaction</c> table.</summary>
public class PointTransactionConfiguration : IEntityTypeConfiguration<PointTransaction>
{
    public void Configure(EntityTypeBuilder<PointTransaction> builder)
    {
        builder.ToTable("point_transaction");

        builder.Property(e => e.Id).HasColumnName("id");
        builder.Property(e => e.Amount).HasColumnName("amount");
        builder.Property(e => e.CreatedAt)
            .HasDefaultValueSql("(getdate())")
            .HasColumnName("created_at");
        builder.Property(e => e.OrderId).HasColumnName("order_id");
        builder.Property(e => e.Reason)
            .HasMaxLength(50)
            .HasColumnName("reason");
        builder.Property(e => e.UserId).HasColumnName("user_id");

        builder.HasOne(d => d.Order).WithMany(p => p.PointTransactions)
            .HasForeignKey(d => d.OrderId)
            .HasConstraintName("FK_point_transaction_order");

        // Identity user replaces the legacy [dbo].[user] table.
        builder.HasOne<ApplicationUser>().WithMany(u => u.PointTransactions)
            .HasForeignKey(d => d.UserId)
            .OnDelete(DeleteBehavior.ClientSetNull)
            .HasConstraintName("FK_point_transaction_user");
    }
}
