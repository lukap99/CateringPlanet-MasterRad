namespace CateringPlanet.Application.DTOs;

/// <summary>
/// A payment option (e.g. "Kartica", "Gotovina").
/// </summary>
public class PaymentMethodDto
{
    public int Id { get; set; }

    public string Type { get; set; } = null!;
}
