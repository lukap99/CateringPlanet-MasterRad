namespace CateringPlanet.Application.DTOs;

/// <summary>
/// Read model for a customer order (used on "My orders" and "View order" screens).
/// </summary>
public class OrderDto
{
    /// <summary>Order id.</summary>
    public int Id { get; set; }

    /// <summary>Human-readable order number (e.g. "ORD-20240101120000...").</summary>
    public string OrderNumber { get; set; } = null!;

    /// <summary>Identity user id (GUID string).</summary>
    public string UserId { get; set; } = null!;

    /// <summary>Customer display name (populated in admin order views).</summary>
    public string? CustomerName { get; set; }

    /// <summary>Customer email (populated in admin order views).</summary>
    public string? CustomerEmail { get; set; }

    /// <summary>Date and time the order was placed.</summary>
    public DateTime DateCreated { get; set; }

    /// <summary>Requested delivery date and time, if provided.</summary>
    public DateTime? RequestedDatetime { get; set; }

    /// <summary>Date and time the order was completed, if completed.</summary>
    public DateTime? DateCompleted { get; set; }

    /// <summary>Delivery address or location description.</summary>
    public string? DeliveryLocation { get; set; }

    /// <summary>Contact phone number for the delivery.</summary>
    public string? ContactPhone { get; set; }

    /// <summary>Contact person's name for the delivery.</summary>
    public string? ContactName { get; set; }

    /// <summary>Display name of the current order status.</summary>
    public string StatusName { get; set; } = null!;

    /// <summary>Current status id (see the order_status lookup).</summary>
    public int StatusId { get; set; }

    /// <summary>Display name of the current payment status.</summary>
    public string PaymentStatusName { get; set; } = null!;

    /// <summary>Current payment status id (see the payment_status lookup).</summary>
    public int PaymentStatusId { get; set; }

    /// <summary>Name of the chosen payment method.</summary>
    public string PaymentMethodName { get; set; } = null!;

    /// <summary>Name of the chosen delivery method.</summary>
    public string DeliveryMethodName { get; set; } = null!;

    /// <summary>Delivery fee charged for this order (snapshot of the delivery method price).</summary>
    public decimal DeliveryPrice { get; set; }

    /// <summary>Order discount tier name (e.g. "Srebrni"), if a discount was applied.</summary>
    public string? DiscountName { get; set; }

    /// <summary>Loyalty points earned (positive) or spent (negative) on this order.</summary>
    public int? PointsChange { get; set; }

    /// <summary>Reason recorded when the order was cancelled or rejected.</summary>
    public string? CancellationReason { get; set; }

    /// <summary>Subtotal of all order lines plus delivery, before the order discount.</summary>
    public decimal Price { get; set; }

    /// <summary>Total charged after the order discount, if one was applied.</summary>
    public decimal? PriceWithDiscount { get; set; }

    /// <summary>VAT amount calculated on the order.</summary>
    public decimal? TaxAmount { get; set; }

    /// <summary>Order lines (foods and combo menus with amounts and prices).</summary>
    public ICollection<OrderLineDto> Lines { get; set; } = new List<OrderLineDto>();

    public int ItemCount => Lines.Count;

    public string PriceString => Price.ToString("N2");

    public string DeliveryPriceString => DeliveryPrice.ToString("N2");

    /// <summary>Monetary amount reduced by the order discount (subtotal + delivery - charged total).</summary>
    public decimal DiscountReduction => Price + DeliveryPrice - (PriceWithDiscount ?? Price);

    /// <summary>VAT amount formatted as currency.</summary>
    public string TaxString => (TaxAmount ?? 0m).ToString("N2");

    /// <summary>Final amount actually charged (discounted total when present).</summary>
    public string TotalString => (PriceWithDiscount ?? Price).ToString("N2");
}
