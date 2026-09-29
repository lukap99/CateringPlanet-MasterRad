namespace CateringPlanet.Application.DTOs;

/// <summary>
/// A delivery option (e.g. "Dostava", "Preuzimanje") with its price.
/// </summary>
public class DeliveryMethodDto
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public decimal Price { get; set; }

    public string PriceString => Price.ToString("N2");
}
