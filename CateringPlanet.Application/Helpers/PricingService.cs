namespace CateringPlanet.Application.Helpers;

/// <summary>
/// Centralized price and loyalty-points calculations (decimal-based).
/// Reimplements the business rules that lived in the legacy <c>*Bo</c> classes.
/// </summary>
public static class PricingService
{
    /// <summary>
    /// Applies the food item discount and the category discount multiplicatively.
    /// </summary>
    /// <param name="basePrice">Base price before discounts.</param>
    /// <param name="foodDiscountPercent">Item-level discount (0..100).</param>
    /// <param name="categoryDiscountPercent">Category-level discount (0..100).</param>
    public static decimal FoodSellingPrice(decimal basePrice, decimal foodDiscountPercent, decimal categoryDiscountPercent)
        => basePrice * (1m - foodDiscountPercent / 100m) * (1m - categoryDiscountPercent / 100m);

    /// <summary>
    /// Applies a combo menu discount.
    /// </summary>
    /// <param name="basePrice">Combo base price before discount.</param>
    /// <param name="discountPercent">Combo discount (0..100).</param>
    public static decimal ComboSellingPrice(decimal basePrice, decimal discountPercent)
        => basePrice * (1m - discountPercent / 100m);

    /// <summary>
    /// Computes the final order total: item sum, reduced by the order discount, plus delivery.
    /// </summary>
    /// <param name="itemsSum">Sum of all line selling prices.</param>
    /// <param name="discountPercent">Order-level discount (0..100).</param>
    /// <param name="deliveryPrice">Delivery method price.</param>
    public static decimal OrderTotal(decimal itemsSum, decimal discountPercent, decimal deliveryPrice)
        => itemsSum * (1m - discountPercent / 100m) + deliveryPrice;

    /// <summary>
    /// Loyalty points earned from a base price: one point per 500 currency units (rounded).
    /// </summary>
    public static int PointsEarned(decimal basePrice)
        => (int)Math.Round(basePrice / 500m);
}
