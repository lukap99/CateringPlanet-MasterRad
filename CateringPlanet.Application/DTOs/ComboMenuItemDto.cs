namespace CateringPlanet.Application.DTOs;

/// <summary>
/// A single food entry inside a combo menu.
/// </summary>
public class ComboMenuItemDto
{
    /// <summary>Combo menu item id (the join-entity id, used for edit/delete).</summary>
    public int Id { get; set; }

    public int FoodId { get; set; }

    public string FoodName { get; set; } = null!;

    public int Amount { get; set; }
}
