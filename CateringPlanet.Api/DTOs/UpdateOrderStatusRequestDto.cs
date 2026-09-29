using System.ComponentModel.DataAnnotations;

namespace CateringPlanet.Api.DTOs;

/// <summary>
/// Payload for changing an order's status (admin API).
/// </summary>
public class UpdateOrderStatusRequestDto
{
    /// <summary>New order status ID (1-8).</summary>
    [Required]
    public int StatusId { get; set; }

    /// <summary>Reason, expected when the status is Otkazana (7) or Odbijena (8).</summary>
    public string? CancellationReason { get; set; }
}
