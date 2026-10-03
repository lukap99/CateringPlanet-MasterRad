using CateringPlanet.Application.DTOs;

namespace CateringPlanet.Application.Abstractions;

/// <summary>
/// Shopping-cart abstraction. Implemented session-based in the web layer for now,
/// so the storage can later be swapped to a database without touching controllers.
/// </summary>
public interface ICartService
{
    /// <summary>Gets the current cart (creating an empty one if needed).</summary>
    CartDto GetCart();

    /// <summary>Adds a food item, or updates its amount if already present.</summary>
    void AddFood(FoodDto food, int amount);

    /// <summary>Adds a combo menu, or updates its amount if already present.</summary>
    void AddCombo(ComboMenuDto combo, int amount);

    /// <summary>Updates the amount of an existing line (food or combo).</summary>
    void UpdateAmount(int itemId, bool isCombo, int amount);

    /// <summary>Removes a line (food or combo) from the cart.</summary>
    void RemoveItem(int itemId, bool isCombo);

    /// <summary>Empties the cart.</summary>
    void Clear();

    /// <summary>Sets the selected delivery method and recalculates the total.</summary>
    void SetDeliveryMethod(DeliveryMethodDto delivery);

    /// <summary>Sets the selected payment method.</summary>
    void SetPaymentMethod(PaymentMethodDto payment);

    /// <summary>Sets (or clears) the redeemed order discount and recalculates the total.</summary>
    void SetOrderDiscount(OrderDiscountDto? discount);

    /// <summary>Stores contact/delivery details captured at checkout.</summary>
    void SetContact(string name, string phone, string location);

    /// <summary>Stores the requested delivery date and time (null = as soon as possible).</summary>
    void SetRequestedDatetime(DateTime? requestedDatetime);
}
