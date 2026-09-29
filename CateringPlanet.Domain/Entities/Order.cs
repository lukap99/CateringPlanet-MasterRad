using System;
using System.Collections.Generic;

namespace CateringPlanet.Domain.Entities;

/// <summary>
/// A customer order containing food and/or combo items.
/// </summary>
public class Order
{
    public int Id { get; set; }

    public string OrderNumber { get; set; } = null!;

    /// <summary>
    /// Identity user id (GUID string) — references the <c>AspNetUsers</c> table.
    /// </summary>
    public string UserId { get; set; } = null!;

    public DateTime DateCreated { get; set; }

    public DateTime? RequestedDatetime { get; set; }

    public DateTime? DateCompleted { get; set; }

    public string? DeliveryLocation { get; set; }

    public string? ContactPhone { get; set; }

    public string? ContactName { get; set; }

    public int StatusId { get; set; }

    public int PaymentStatusId { get; set; }

    public int PaymentMethodId { get; set; }

    public int DiscountId { get; set; }

    public int DeliveryId { get; set; }

    public decimal Price { get; set; }

    public decimal? PriceWithDiscount { get; set; }

    public decimal? TaxAmount { get; set; }

    public decimal TaxRate { get; set; }

    public string? CancellationReason { get; set; }

    public string Source { get; set; } = null!;

    public DateTime UpdatedAt { get; set; }

    public virtual DeliveryMethod Delivery { get; set; } = null!;

    public virtual OrderDiscount Discount { get; set; } = null!;

    public virtual PaymentMethod PaymentMethod { get; set; } = null!;

    public virtual PaymentStatus PaymentStatus { get; set; } = null!;

    public virtual OrderStatus Status { get; set; } = null!;

    public virtual ICollection<OrderComboItem> OrderComboItems { get; set; } = new List<OrderComboItem>();

    public virtual ICollection<OrderFoodItem> OrderFoodItems { get; set; } = new List<OrderFoodItem>();

    public virtual ICollection<PointTransaction> PointTransactions { get; set; } = new List<PointTransaction>();
}
