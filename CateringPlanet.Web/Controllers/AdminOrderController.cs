using CateringPlanet.Application.Abstractions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CateringPlanet.Web.Controllers;

/// <summary>
/// Admin management of orders (Manager/Admin only).
/// </summary>
[Authorize(Roles = "Manager,Admin")]
public class AdminOrderController : Controller
{
    private readonly IOrderService _orderService;
    private readonly IAccountService _accountService;

    public AdminOrderController(IOrderService orderService, IAccountService accountService)
    {
        _orderService = orderService;
        _accountService = accountService;
    }

    /// <summary>Lists all orders with status filter, free-text search, sorting, and pagination.</summary>
    public async Task<IActionResult> Index(int? statusId, string? searchString, string? sortOrder, string? userId, int page = 1)
    {
        ViewData["OrderStatuses"] = await _orderService.GetOrderStatusesAsync();
        ViewData["SelectedStatusId"] = statusId;

        var orders = (await _orderService.GetAllOrdersAsync(statusId)).ToList();

        if (!string.IsNullOrEmpty(userId))
        {
            orders = orders.Where(o => o.UserId == userId).ToList();
            ViewData["FilterUserId"] = userId;
            ViewData["FilterUserName"] = (await _accountService.GetByIdAsync(userId))?.FullName;
        }

        if (!string.IsNullOrEmpty(searchString))
        {
            orders = orders.Where(o =>
                o.OrderNumber.Contains(searchString, StringComparison.OrdinalIgnoreCase) ||
                o.Id.ToString().Contains(searchString, StringComparison.OrdinalIgnoreCase) ||
                (o.CustomerName?.Contains(searchString, StringComparison.OrdinalIgnoreCase) ?? false) ||
                (o.CustomerEmail?.Contains(searchString, StringComparison.OrdinalIgnoreCase) ?? false)).ToList();
        }

        orders = sortOrder switch
        {
            "orderId_asc" => orders.OrderBy(o => o.Id).ToList(),
            "orderId_desc" => orders.OrderByDescending(o => o.Id).ToList(),
            "customer_asc" => orders.OrderBy(o => o.CustomerName).ToList(),
            "customer_desc" => orders.OrderByDescending(o => o.CustomerName).ToList(),
            "date_asc" => orders.OrderBy(o => o.DateCreated).ToList(),
            "date_desc" => orders.OrderByDescending(o => o.DateCreated).ToList(),
            "total_asc" => orders.OrderBy(o => o.PriceWithDiscount ?? o.Price).ToList(),
            "total_desc" => orders.OrderByDescending(o => o.PriceWithDiscount ?? o.Price).ToList(),
            _ => orders.OrderByDescending(o => o.DateCreated).ToList()
        };

        const int pageSize = 10;
        var totalItems = orders.Count;
        var totalPages = Math.Max(1, (int)Math.Ceiling(totalItems / (double)pageSize));
        page = Math.Clamp(page, 1, totalPages);

        ViewData["CurrentFilter"] = searchString;
        ViewData["CurrentSort"] = sortOrder;
        ViewData["CurrentPage"] = page;
        ViewData["PageSize"] = pageSize;
        ViewData["TotalItems"] = totalItems;
        ViewData["TotalPages"] = totalPages;

        return View(orders.Skip((page - 1) * pageSize).Take(pageSize).ToList());
    }

    /// <summary>Shows a single order, including the status list for the update form.</summary>
    public async Task<IActionResult> ViewOrder(int id)
    {
        var order = await _orderService.GetOrderByIdAsync(id);
        if (order is null)
        {
            return RedirectToAction(nameof(Index));
        }

        ViewData["OrderStatuses"] = await _orderService.GetOrderStatusesAsync();
        return View(order);
    }

    /// <summary>Applies a new status to an order.</summary>
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> UpdateStatus(int id, int statusId, string? cancellationReason)
    {
        var ok = await _orderService.UpdateOrderStatusAsync(id, statusId, cancellationReason);
        if (!ok)
        {
            TempData["ErrorMessage"] = "Could not update the order status.";
        }

        return RedirectToAction(nameof(ViewOrder), new { id });
    }
}
