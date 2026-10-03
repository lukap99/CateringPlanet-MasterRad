using CateringPlanet.Application.DTOs;

namespace CateringPlanet.Application.Abstractions;

/// <summary>
/// Creates and reads customer orders.
/// </summary>
public interface IOrderService
{
    /// <summary>Gets all orders for a user (newest first).</summary>
    Task<IEnumerable<OrderDto>> GetOrdersAsync(string userId);

    /// <summary>Gets a single order owned by the user, or <c>null</c>.</summary>
    Task<OrderDto?> GetOrderAsync(int id, string userId);

    /// <summary>Gets a single order by id without a user-ownership check (admin).</summary>
    Task<OrderDto?> GetOrderByIdAsync(int id);

    /// <summary>Gets all orders (admin view), optionally filtered by status id.</summary>
    Task<IEnumerable<OrderDto>> GetAllOrdersAsync(int? statusId = null);

    /// <summary>Creates an order from a cart and returns the new order id.</summary>
    Task<int> PlaceOrderAsync(string userId, CartDto cart, DateTime? requestedDatetime = null);

    /// <summary>Gets all order statuses (id + display name).</summary>
    Task<IEnumerable<OrderStatusDto>> GetOrderStatusesAsync();

    /// <summary>
    /// Applies a new status to an order. Status 6 ("Završena") also marks payment as paid
    /// and records DateCompleted; statuses 7/8 record the supplied cancellation reason.
    /// Returns false when the order or the status does not exist.
    /// </summary>
    Task<bool> UpdateOrderStatusAsync(int orderId, int statusId, string? cancellationReason = null);

    /// <summary>Gets all delivery methods.</summary>
    Task<IEnumerable<DeliveryMethodDto>> GetDeliveryMethodsAsync();

    /// <summary>Gets a delivery method by id, or <c>null</c>.</summary>
    Task<DeliveryMethodDto?> GetDeliveryMethodAsync(int id);

    /// <summary>Gets all payment methods.</summary>
    Task<IEnumerable<PaymentMethodDto>> GetPaymentMethodsAsync();

    /// <summary>Gets a payment method by id, or <c>null</c>.</summary>
    Task<PaymentMethodDto?> GetPaymentMethodAsync(int id);

    /// <summary>Gets all order discount tiers.</summary>
    Task<IEnumerable<OrderDiscountDto>> GetOrderDiscountsAsync();

    /// <summary>Gets an order discount tier by id, or <c>null</c>.</summary>
    Task<OrderDiscountDto?> GetOrderDiscountAsync(int id);
}
