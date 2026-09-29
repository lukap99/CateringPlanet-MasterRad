using System.Text.Json;
using CateringPlanet.Application.DTOs;
using CateringPlanet.Application.Helpers;
using CateringPlanet.Application.Abstractions;
using Microsoft.AspNetCore.Http;

namespace CateringPlanet.Web.Adapters;

/// <summary>
/// Session-backed implementation of <see cref="ICartService"/>. The cart is serialized
/// to JSON in the session, keeping the storage swappable to a database later.
/// </summary>
public class SessionCartService : ICartService
{
    private const string CartKey = "Cart";

    private readonly IHttpContextAccessor _httpContextAccessor;

    public SessionCartService(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    private ISession Session => _httpContextAccessor.HttpContext?.Session
        ?? throw new InvalidOperationException("Session is not available.");

    /// <inheritdoc />
    public CartDto GetCart()
    {
        var json = Session.GetString(CartKey);
        if (string.IsNullOrEmpty(json))
        {
            return new CartDto();
        }

        return JsonSerializer.Deserialize<CartDto>(json) ?? new CartDto();
    }

    /// <inheritdoc />
    public void AddFood(FoodDto food, int amount)
    {
        var cart = GetCart();
        var existing = cart.Items.FirstOrDefault(i => !i.IsCombo && i.ItemId == food.Id);
        if (existing is not null)
        {
            existing.Amount = amount;
        }
        else
        {
            cart.Items.Add(new CartItemDto
            {
                ItemId = food.Id,
                Name = food.Name,
                IsCombo = false,
                Amount = amount,
                UnitPrice = food.SellingPrice,
                ImageFileName = food.ImageFileName,
                ImageUrl = food.ImageUrl
            });
        }
        Recalculate(cart);
        Save(cart);
    }

    /// <inheritdoc />
    public void AddCombo(ComboMenuDto combo, int amount)
    {
        var cart = GetCart();
        var existing = cart.Items.FirstOrDefault(i => i.IsCombo && i.ItemId == combo.Id);
        if (existing is not null)
        {
            existing.Amount = amount;
        }
        else
        {
            cart.Items.Add(new CartItemDto
            {
                ItemId = combo.Id,
                Name = combo.Name,
                IsCombo = true,
                Amount = amount,
                UnitPrice = combo.SellingPrice,
                ImageFileName = combo.ImageFileName,
                ImageUrl = combo.ImageUrl
            });
        }
        Recalculate(cart);
        Save(cart);
    }

    /// <inheritdoc />
    public void UpdateAmount(int itemId, bool isCombo, int amount)
    {
        var cart = GetCart();
        var item = cart.Items.FirstOrDefault(i => i.ItemId == itemId && i.IsCombo == isCombo);
        if (item is not null)
        {
            item.Amount = amount;
            Recalculate(cart);
            Save(cart);
        }
    }

    /// <inheritdoc />
    public void RemoveItem(int itemId, bool isCombo)
    {
        var cart = GetCart();
        cart.Items.RemoveAll(i => i.ItemId == itemId && i.IsCombo == isCombo);
        Recalculate(cart);
        Save(cart);
    }

    /// <inheritdoc />
    public void Clear() => Session.Remove(CartKey);

    /// <inheritdoc />
    public void SetDeliveryMethod(DeliveryMethodDto delivery)
    {
        var cart = GetCart();
        cart.DeliveryMethod = delivery;
        Recalculate(cart);
        Save(cart);
    }

    /// <inheritdoc />
    public void SetPaymentMethod(PaymentMethodDto payment)
    {
        var cart = GetCart();
        cart.PaymentMethod = payment;
        Save(cart);
    }

    /// <inheritdoc />
    public void SetOrderDiscount(OrderDiscountDto? discount)
    {
        var cart = GetCart();
        cart.OrderDiscount = discount;
        Recalculate(cart);
        Save(cart);
    }

    /// <inheritdoc />
    public void SetContact(string name, string phone, string location)
    {
        var cart = GetCart();
        cart.ContactName = name;
        cart.ContactPhone = phone;
        cart.DeliveryLocation = location;
        Save(cart);
    }

    private void Save(CartDto cart) => Session.SetString(CartKey, JsonSerializer.Serialize(cart));

    private static void Recalculate(CartDto cart)
    {
        var itemsSum = cart.BasePrice; // sum of line totals
        var discountPercent = cart.OrderDiscount?.DiscountPercent ?? 0m;
        var deliveryPrice = cart.DeliveryMethod?.Price ?? 0m;

        cart.BuyingPrice = PricingService.OrderTotal(itemsSum, discountPercent, deliveryPrice);

        cart.PointsDifference = discountPercent > 0m
            ? -(cart.OrderDiscount?.PointCost ?? 0)
            : PricingService.PointsEarned(itemsSum);
    }
}
