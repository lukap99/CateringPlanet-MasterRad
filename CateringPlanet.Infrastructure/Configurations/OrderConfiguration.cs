using CateringPlanet.Domain.Entities;
using CateringPlanet.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CateringPlanet.Infrastructure.Configurations;

/// <summary>EF Core configuration for the <c>order</c> table.</summary>
public class OrderConfiguration : IEntityTypeConfiguration<Order>
{
    public void Configure(EntityTypeBuilder<Order> builder)
    {
        builder.ToTable("order");

        builder.HasIndex(e => e.DateCreated, "IX_order_date_created");
        builder.HasIndex(e => e.OrderNumber, "IX_order_order_number").IsUnique();
        builder.HasIndex(e => e.UserId, "IX_order_user_id");

        builder.Property(e => e.Id).HasColumnName("id");
        builder.Property(e => e.CancellationReason)
            .HasMaxLength(200)
            .HasColumnName("cancellation_reason");
        builder.Property(e => e.ContactName)
            .HasMaxLength(50)
            .HasColumnName("contact_name");
        builder.Property(e => e.ContactPhone)
            .HasMaxLength(20)
            .HasColumnName("contact_phone");
        builder.Property(e => e.DateCompleted).HasColumnName("date_completed");
        builder.Property(e => e.DateCreated)
            .HasDefaultValueSql("(getdate())")
            .HasColumnName("date_created");
        builder.Property(e => e.DeliveryId)
            .HasDefaultValue(1)
            .HasColumnName("delivery_id");
        builder.Property(e => e.DeliveryLocation)
            .HasMaxLength(100)
            .HasDefaultValue("Takeout")
            .HasColumnName("delivery_location");
        builder.Property(e => e.DiscountId)
            .HasDefaultValue(1)
            .HasColumnName("discount_id");
        builder.Property(e => e.OrderNumber)
            .HasMaxLength(20)
            .HasColumnName("order_number");
        builder.Property(e => e.PaymentMethodId).HasColumnName("payment_method_id");
        builder.Property(e => e.PaymentStatusId)
            .HasDefaultValue(1)
            .HasColumnName("payment_status_id");
        builder.Property(e => e.Price)
            .HasColumnType("decimal(10, 2)")
            .HasColumnName("price");
        builder.Property(e => e.PriceWithDiscount)
            .HasColumnType("decimal(10, 2)")
            .HasColumnName("price_with_discount");
        builder.Property(e => e.RequestedDatetime).HasColumnName("requested_datetime");
        builder.Property(e => e.Source)
            .HasMaxLength(20)
            .HasDefaultValue("Web")
            .HasColumnName("source");
        builder.Property(e => e.StatusId)
            .HasDefaultValue(1)
            .HasColumnName("status_id");
        builder.Property(e => e.TaxAmount)
            .HasColumnType("decimal(10, 2)")
            .HasColumnName("tax_amount");
        builder.Property(e => e.TaxRate)
            // Reference declared decimal(5,2) with a 2000m default, which overflows (max 999.99);
            // widened to decimal(10,2) so the documented 2000.00 default is representable.
            .HasColumnType("decimal(10, 2)")
            .HasDefaultValue(2000m)
            .HasColumnName("tax_rate");
        builder.Property(e => e.UpdatedAt)
            .HasDefaultValueSql("(getdate())")
            .HasColumnName("updated_at");
        builder.Property(e => e.UserId).HasColumnName("user_id");

        builder.HasOne(d => d.Delivery).WithMany(p => p.Orders)
            .HasForeignKey(d => d.DeliveryId)
            .OnDelete(DeleteBehavior.ClientSetNull)
            .HasConstraintName("FK_order_delivery_method");

        builder.HasOne(d => d.Discount).WithMany(p => p.Orders)
            .HasForeignKey(d => d.DiscountId)
            .OnDelete(DeleteBehavior.ClientSetNull)
            .HasConstraintName("FK_order_discount");

        builder.HasOne(d => d.PaymentMethod).WithMany(p => p.Orders)
            .HasForeignKey(d => d.PaymentMethodId)
            .OnDelete(DeleteBehavior.ClientSetNull)
            .HasConstraintName("FK_order_payment_method");

        builder.HasOne(d => d.PaymentStatus).WithMany(p => p.Orders)
            .HasForeignKey(d => d.PaymentStatusId)
            .OnDelete(DeleteBehavior.ClientSetNull)
            .HasConstraintName("FK_order_payment_status");

        builder.HasOne(d => d.Status).WithMany(p => p.Orders)
            .HasForeignKey(d => d.StatusId)
            .OnDelete(DeleteBehavior.ClientSetNull)
            .HasConstraintName("FK_order_status");

        // Identity user replaces the legacy [dbo].[user] table.
        builder.HasOne<ApplicationUser>().WithMany(u => u.Orders)
            .HasForeignKey(d => d.UserId)
            .OnDelete(DeleteBehavior.ClientSetNull)
            .HasConstraintName("FK_order_user");
    }
}
