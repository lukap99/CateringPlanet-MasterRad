using System.Globalization;
using System.Security.Claims;
using CateringPlanet.Application.DTOs;
using CateringPlanet.Application.Abstractions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CateringPlanet.Web.Controllers;

/// <summary>
/// Shopping cart and checkout flow.
/// </summary>
public class CheckoutController : Controller
{
    private readonly ICartService _cartService;
    private readonly IOrderService _orderService;
    private readonly IAccountService _accountService;

    public CheckoutController(ICartService cartService, IOrderService orderService, IAccountService accountService)
    {
        _cartService = cartService;
        _orderService = orderService;
        _accountService = accountService;
    }

    /// <summary>Shows the cart contents and the checkout sidebar.</summary>
    public async Task<IActionResult> Index()
    {
        await LoadCheckoutDataAsync();

        // Prefill delivery info from the user's profile only when the cart is untouched.
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (userId is not null)
        {
            var user = await _accountService.GetByIdAsync(userId);
            var cart = _cartService.GetCart();
            if (user is not null
                && string.IsNullOrEmpty(cart.ContactName)
                && string.IsNullOrEmpty(cart.ContactPhone)
                && string.IsNullOrEmpty(cart.DeliveryLocation))
            {
                _cartService.SetContact(user.FullName, user.Phone ?? string.Empty, user.Address ?? string.Empty);
            }
        }

        if (TempData.ContainsKey("ChangeAmountMessage"))
        {
            ViewData["ChangeAmountMessage"] = TempData["ChangeAmountMessage"];
        }
        if (TempData.ContainsKey("ErrorMessage"))
        {
            ViewData["ErrorMessage"] = TempData["ErrorMessage"];
        }

        return View(_cartService.GetCart());
    }

    /// <summary>Empties the cart.</summary>
    public IActionResult EmptyCart()
    {
        _cartService.Clear();
        return RedirectToAction(nameof(Index));
    }

    /// <summary>Updates the amount of a cart line (food or combo).</summary>
    [HttpPost]
    public IActionResult UpdateCartItem(int itemId, bool isCombo, int amount)
    {
        if (amount > 0)
        {
            _cartService.UpdateAmount(itemId, isCombo, amount);
            TempData["ChangeAmountMessage"] = "Amount changed.";
        }
        else
        {
            TempData["ErrorMessage"] = "Cannot change quantity to 0!";
        }
        return RedirectToAction(nameof(Index));
    }

    /// <summary>Removes a cart line (food or combo).</summary>
    public IActionResult RemoveCartItem(int itemId, bool isCombo)
    {
        _cartService.RemoveItem(itemId, isCombo);
        return RedirectToAction(nameof(Index));
    }

    /// <summary>Captures delivery/payment/discount selections and shows the review screen.</summary>
    [HttpPost]
    [Authorize]
    public async Task<IActionResult> CheckoutReview(int deliveryMethodId, int paymentMethodId, int orderDiscountId, string deliveryLocation, string? requestedDatetime)
    {
        var cart = _cartService.GetCart();

        // Parse the datetime-local value ("yyyy-MM-ddTHH:mm", optionally with seconds).
        DateTime? parsedRequestedDatetime = null;
        if (!string.IsNullOrEmpty(requestedDatetime))
        {
            if (!DateTime.TryParseExact(requestedDatetime,
                    new[] { "yyyy-MM-ddTHH:mm", "yyyy-MM-ddTHH:mm:ss" },
                    CultureInfo.InvariantCulture, DateTimeStyles.None, out var value))
            {
                TempData["ErrorMessage"] = "Unesite ispravno vreme isporuke.";
                return RedirectToAction(nameof(Index));
            }

            parsedRequestedDatetime = value;
        }

        // Reject past values, allowing a 2-minute grace for clock skew.
        if (parsedRequestedDatetime.HasValue && parsedRequestedDatetime.Value < DateTime.Now.AddMinutes(-2))
        {
            TempData["ErrorMessage"] = "Vreme isporuke ne može biti u prošlosti.";
            return RedirectToAction(nameof(Index));
        }

        var delivery = await _orderService.GetDeliveryMethodAsync(deliveryMethodId);
        var payment = await _orderService.GetPaymentMethodAsync(paymentMethodId);
        var discount = await _orderService.GetOrderDiscountAsync(orderDiscountId);

        if (delivery is not null)
        {
            _cartService.SetDeliveryMethod(delivery);
        }
        if (payment is not null)
        {
            _cartService.SetPaymentMethod(payment);
        }
        _cartService.SetOrderDiscount(discount);
        _cartService.SetContact(cart.ContactName ?? string.Empty, cart.ContactPhone ?? string.Empty, deliveryLocation ?? string.Empty);
        _cartService.SetRequestedDatetime(parsedRequestedDatetime);

        return View(_cartService.GetCart());
    }

    /// <summary>Places the order from the cart and clears it.</summary>
    [HttpPost]
    [Authorize]
    public async Task<IActionResult> CreateOrder()
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (userId is null)
        {
            return RedirectToAction("Login", "Account");
        }

        var cart = _cartService.GetCart();
        if (cart.ItemCount == 0)
        {
            return RedirectToAction(nameof(Index));
        }

        var orderId = await _orderService.PlaceOrderAsync(userId, cart, cart.RequestedDatetime);
        _cartService.Clear();

        TempData["SuccessMessage"] = "Order has been placed.";
        return RedirectToAction("ViewOrder", "Order", new { id = orderId });
    }

    /// <summary>Loads the lookup lists and the current user's points into ViewData for the sidebar.</summary>
    private async Task LoadCheckoutDataAsync()
    {
        ViewData["DeliveryMethods"] = await _orderService.GetDeliveryMethodsAsync();
        ViewData["PaymentMethods"] = await _orderService.GetPaymentMethodsAsync();
        ViewData["Discounts"] = await _orderService.GetOrderDiscountsAsync();

        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        var points = userId is null ? 0 : await _accountService.GetPointsAsync(userId);
        ViewData["UserPoints"] = points;
    }
}
