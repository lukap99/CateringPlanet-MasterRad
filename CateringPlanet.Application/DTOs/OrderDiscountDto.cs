namespace CateringPlanet.Application.DTOs;

/// <summary>
/// A discount tier that can be redeemed with loyalty points.
/// </summary>
public class OrderDiscountDto
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public decimal DiscountPercent { get; set; }

    public int PointCost { get; set; }
}
