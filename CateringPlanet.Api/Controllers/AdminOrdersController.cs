using CateringPlanet.Api.DTOs;
using CateringPlanet.Application.Abstractions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CateringPlanet.Api.Controllers;

/// <summary>
/// Admin order-management endpoints. Requires the Manager or Admin role.
/// </summary>
[Route("api/admin/orders")]
[ApiController]
[Authorize(Roles = "Manager,Admin")]
public class AdminOrdersController : ControllerBase
{
    private readonly IOrderService _orderService;

    public AdminOrdersController(IOrderService orderService)
    {
        _orderService = orderService;
    }

    /// <summary>Gets all orders, optionally filtered by status id.</summary>
    /// <param name="statusId">Optional order status ID to filter by.</param>
    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] int? statusId)
        => Ok(await _orderService.GetAllOrdersAsync(statusId));

    /// <summary>Applies a new status to an order (any transition is allowed).</summary>
    /// <param name="id">Numeric ID of the order.</param>
    /// <param name="request">New status ID and optional cancellation reason.</param>
    [HttpPatch("{id:int}/status")]
    public async Task<IActionResult> UpdateStatus(int id, [FromBody] UpdateOrderStatusRequestDto request)
    {
        var ok = await _orderService.UpdateOrderStatusAsync(id, request.StatusId, request.CancellationReason);
        if (!ok)
        {
            return NotFound(new { message = "Order or status not found." });
        }

        return NoContent();
    }
}
