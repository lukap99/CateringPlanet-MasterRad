using System.ComponentModel.DataAnnotations;

namespace CateringPlanet.Api.DTOs;

/// <summary>
/// Payload for placing a new order through the API (stateless cart).
/// </summary>
public class CreateOrderRequestDto
{
    /// <summary>Order lines; each entry is a food or a combo menu with an amount.</summary>
    [Required]
    public List<OrderLineRequestDto> Lines { get; set; } = new();

    /// <summary>ID of the delivery method to use.</summary>
    [Required]
    public int DeliveryMethodId { get; set; }

    /// <summary>ID of the payment method to use.</summary>
    [Required]
    public int PaymentMethodId { get; set; }

    /// <summary>Redeemed order discount tier id; null means no discount.</summary>
    public int? DiscountId { get; set; }

    /// <summary>Contact person's name for the delivery.</summary>
    public string? ContactName { get; set; }

    /// <summary>Contact phone number for the delivery.</summary>
    public string? ContactPhone { get; set; }

    /// <summary>Delivery address or location description.</summary>
    public string? DeliveryLocation { get; set; }
}

/// <summary>
/// A single requested order line (a food or a combo menu).
/// </summary>
public class OrderLineRequestDto
{
    /// <summary>Food id or combo menu id, depending on <see cref="IsCombo"/>.</summary>
    [Required]
    public int ItemId { get; set; }

    /// <summary>True when the item is a combo menu; false when it is a food.</summary>
    public bool IsCombo { get; set; }

    /// <summary>Number of portions; at least 1.</summary>
    [Range(1, int.MaxValue)]
    public int Amount { get; set; } = 1;
}
