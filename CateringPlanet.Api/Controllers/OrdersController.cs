using System.Security.Claims;
using CateringPlanet.Api.DTOs;
using CateringPlanet.Application.DTOs;
using CateringPlanet.Application.Abstractions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CateringPlanet.Api.Controllers;

/// <summary>
/// Customer order endpoints. Requires a valid JWT.
/// </summary>
[Route("api/orders")]
[ApiController]
[Authorize]
public class OrdersController : ControllerBase
{
    private readonly IOrderService _orderService;
    private readonly IMenuService _menuService;

    public OrdersController(IOrderService orderService, IMenuService menuService)
    {
        _orderService = orderService;
        _menuService = menuService;
    }

    /// <summary>Gets the current user's orders (newest first).</summary>
    [HttpGet]
    public async Task<IActionResult> GetMyOrders()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userId))
        {
            return Unauthorized();
        }

        return Ok(await _orderService.GetOrdersAsync(userId));
    }

    /// <summary>Gets one of the current user's orders by id.</summary>
    /// <param name="id">Numeric ID of the order to fetch.</param>
    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetOrder(int id)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userId))
        {
            return Unauthorized();
        }

        var order = await _orderService.GetOrderAsync(id, userId);
        return order is null ? NotFound() : Ok(order);
    }

    /// <summary>
    /// Places an order from a stateless cart payload and returns the new order id.
    /// </summary>
    /// <param name="request">Stateless cart payload with contact data, items, delivery/payment method IDs and optional discount.</param>
    [HttpPost]
    public async Task<IActionResult> PlaceOrder([FromBody] CreateOrderRequestDto request)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userId))
        {
            return Unauthorized();
        }

        var cart = await BuildCartAsync(request);
        if (cart is null)
        {
            return BadRequest(new { message = "One or more requested items, the delivery method, or the payment method do not exist." });
        }

        var orderId = await _orderService.PlaceOrderAsync(userId, cart);
        return CreatedAtAction(nameof(GetOrder), new { id = orderId }, new { orderId });
    }

    // Resolves the request lines against the menu so prices can never be forged by the client.
    private async Task<CartDto?> BuildCartAsync(CreateOrderRequestDto request)
    {
        var cart = new CartDto
        {
            ContactName = request.ContactName,
            ContactPhone = request.ContactPhone,
            DeliveryLocation = request.DeliveryLocation,
            DeliveryMethod = await _orderService.GetDeliveryMethodAsync(request.DeliveryMethodId),
            PaymentMethod = await _orderService.GetPaymentMethodAsync(request.PaymentMethodId),
            OrderDiscount = request.DiscountId.HasValue
                ? await _orderService.GetOrderDiscountAsync(request.DiscountId.Value)
                : null
        };

        if (cart.DeliveryMethod is null || cart.PaymentMethod is null)
        {
            return null;
        }

        foreach (var line in request.Lines)
        {
            if (line.IsCombo)
            {
                var combo = await _menuService.GetComboAsync(line.ItemId);
                if (combo is null)
                {
                    return null;
                }

                cart.Items.Add(new CartItemDto
                {
                    ItemId = combo.Id,
                    Name = combo.Name,
                    IsCombo = true,
                    Amount = line.Amount,
                    UnitPrice = combo.SellingPrice,
                    ImageFileName = combo.ImageFileName,
                    ImageUrl = combo.ImageUrl
                });
            }
            else
            {
                var food = await _menuService.GetFoodAsync(line.ItemId);
                if (food is null)
                {
                    return null;
                }

                cart.Items.Add(new CartItemDto
                {
                    ItemId = food.Id,
                    Name = food.Name,
                    IsCombo = false,
                    Amount = line.Amount,
                    UnitPrice = food.SellingPrice,
                    ImageFileName = food.ImageFileName,
                    ImageUrl = food.ImageUrl
                });
            }
        }

        return cart;
    }
}
