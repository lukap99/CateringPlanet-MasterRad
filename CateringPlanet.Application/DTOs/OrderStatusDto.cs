namespace CateringPlanet.Application.DTOs;

/// <summary>
/// Read model for an order status lookup row (e.g. "Primljena", "Završena").
/// </summary>
public class OrderStatusDto
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;
}
