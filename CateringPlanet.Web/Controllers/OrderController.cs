using System.Security.Claims;
using CateringPlanet.Application.Abstractions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CateringPlanet.Web.Controllers;

/// <summary>
/// Customer order history (My orders / view order).
/// </summary>
[Authorize]
public class OrderController : Controller
{
    private readonly IOrderService _orderService;

    public OrderController(IOrderService orderService)
    {
        _orderService = orderService;
    }

    /// <summary>Redirects the default order route to MyOrders.</summary>
    public IActionResult Index() => RedirectToAction(nameof(MyOrders));

    /// <summary>Lists the current user's orders (newest first).</summary>
    public async Task<IActionResult> MyOrders()
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (userId is null)
        {
            return RedirectToAction("Index", "Home");
        }

        var orders = await _orderService.GetOrdersAsync(userId);
        return View(orders);
    }

    /// <summary>Shows a single order owned by the current user.</summary>
    public async Task<IActionResult> ViewOrder(int id)
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (userId is null)
        {
            return RedirectToAction("Index", "Home");
        }

        var order = await _orderService.GetOrderAsync(id, userId);
        if (order is null)
        {
            return RedirectToAction(nameof(MyOrders));
        }

        return View(order);
    }

    /// <summary>Cancels the current user's order while it is still "Primljena".</summary>
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CancelOrder(int id)
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (userId is null)
        {
            return RedirectToAction("Index", "Home");
        }

        // Ownership check: GetOrderAsync returns null unless the order belongs to this user.
        var order = await _orderService.GetOrderAsync(id, userId);
        if (order is null || order.StatusId != 1)
        {
            TempData["ErrorMessage"] = "This order cannot be cancelled.";
            return RedirectToAction(nameof(MyOrders));
        }

        await _orderService.UpdateOrderStatusAsync(id, 7, "Cancelled by customer");
        TempData["SuccessMessage"] = "Order has been cancelled.";
        return RedirectToAction(nameof(MyOrders));
    }
}
