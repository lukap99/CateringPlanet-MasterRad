using CateringPlanet.Application.Abstractions;
using CateringPlanet.Application.DTOs;
using CateringPlanet.Application.Helpers;
using CateringPlanet.Domain.Entities;
using CateringPlanet.Domain.Interfaces;

namespace CateringPlanet.Application.Services;

/// <summary>
/// Creates and reads customer orders, mapping entities to DTOs.
/// </summary>
public class OrderService : IOrderService
{
    private readonly IOrderRepository _orderRepository;
    private readonly IPointTransactionRepository _pointTransactionRepository;
    private readonly IAccountService _accountService;
    private readonly IDeliveryMethodRepository _deliveryMethodRepository;
    private readonly IPaymentMethodRepository _paymentMethodRepository;
    private readonly IOrderDiscountRepository _orderDiscountRepository;
    private readonly IOrderStatusRepository _orderStatusRepository;

    public OrderService(
        IOrderRepository orderRepository,
        IPointTransactionRepository pointTransactionRepository,
        IAccountService accountService,
        IDeliveryMethodRepository deliveryMethodRepository,
        IPaymentMethodRepository paymentMethodRepository,
        IOrderDiscountRepository orderDiscountRepository,
        IOrderStatusRepository orderStatusRepository)
    {
        _orderRepository = orderRepository;
        _pointTransactionRepository = pointTransactionRepository;
        _accountService = accountService;
        _deliveryMethodRepository = deliveryMethodRepository;
        _paymentMethodRepository = paymentMethodRepository;
        _orderDiscountRepository = orderDiscountRepository;
        _orderStatusRepository = orderStatusRepository;
    }

    /// <inheritdoc />
    public async Task<IEnumerable<OrderDto>> GetOrdersAsync(string userId)
    {
        var dtos = (await _orderRepository.GetByUserAsync(userId)).Select(MapOrder).ToList();
        foreach (var dto in dtos)
        {
            await EnrichPointsAsync(dto);
        }
        return dtos;
    }

    /// <inheritdoc />
    public async Task<OrderDto?> GetOrderAsync(int id, string userId)
    {
        var order = await _orderRepository.GetByIdAsync(id);
        if (order is null || order.UserId != userId)
        {
            return null;
        }

        var dto = MapOrder(order);
        await EnrichPointsAsync(dto);
        return dto;
    }

    /// <inheritdoc />
    public async Task<IEnumerable<OrderDto>> GetAllOrdersAsync(int? statusId = null)
    {
        var orders = await _orderRepository.GetAllAsync();
        if (statusId.HasValue)
        {
            orders = orders.Where(o => o.StatusId == statusId.Value);
        }

        var dtos = orders.Select(MapOrder).ToList();
        await EnrichCustomersAsync(dtos);
        foreach (var dto in dtos)
        {
            await EnrichPointsAsync(dto);
        }
        return dtos;
    }

    /// <inheritdoc />
    public async Task<OrderDto?> GetOrderByIdAsync(int id)
    {
        var order = await _orderRepository.GetByIdAsync(id);
        if (order is null)
        {
            return null;
        }

        var dto = MapOrder(order);
        await EnrichCustomerAsync(dto);
        await EnrichPointsAsync(dto);
        return dto;
    }

    /// <inheritdoc />
    public async Task<IEnumerable<OrderStatusDto>> GetOrderStatusesAsync()
        => (await _orderStatusRepository.GetAllAsync())
            .Select(s => new OrderStatusDto { Id = s.Id, Name = s.Name })
            .ToList();

    /// <inheritdoc />
    public async Task<bool> UpdateOrderStatusAsync(int orderId, int statusId, string? cancellationReason = null)
    {
        // Guard against unknown status ids before touching the order.
        var status = await _orderStatusRepository.GetByIdAsync(statusId);
        if (status is null)
        {
            return false;
        }

        var order = await _orderRepository.GetByIdAsync(orderId);
        if (order is null)
        {
            return false;
        }

        // Any status change is allowed (D1), so no transition validation here.
        order.StatusId = statusId;
        order.UpdatedAt = DateTime.Now;

        if (statusId == 6) // Završena
        {
            order.PaymentStatusId = 2; // Plaćena
            order.DateCompleted ??= DateTime.Now;
        }

        if (statusId is 7 or 8) // Otkazana / Odbijena
        {
            order.CancellationReason = cancellationReason;
        }

        await _orderRepository.UpdateAsync(order);
        return true;
    }

    /// <summary>Populates customer name/email on a single order DTO.</summary>
    private async Task EnrichCustomerAsync(OrderDto dto)
    {
        var user = await _accountService.GetByIdAsync(dto.UserId);
        dto.CustomerName = user?.FullName;
        dto.CustomerEmail = user?.Email;
    }

    /// <summary>Populates customer name/email on a batch of order DTOs (single user query).</summary>
    private async Task EnrichCustomersAsync(IEnumerable<OrderDto> dtos)
    {
        var users = await _accountService.GetAllAsync();
        var map = users.ToDictionary(u => u.Id);
        foreach (var dto in dtos)
        {
            if (map.TryGetValue(dto.UserId, out var user))
            {
                dto.CustomerName = user.FullName;
                dto.CustomerEmail = user.Email;
            }
        }
    }

    /// <summary>Populates the loyalty-points movement recorded for an order.</summary>
    private async Task EnrichPointsAsync(OrderDto dto)
    {
        var transaction = await _pointTransactionRepository.GetByOrderAsync(dto.Id);
        dto.PointsChange = transaction?.Amount;
    }

    /// <inheritdoc />
    public async Task<int> PlaceOrderAsync(string userId, CartDto cart)
    {
        var now = DateTime.Now;
        var itemsSum = cart.Items.Sum(i => i.LineTotal);
        var discountPercent = cart.OrderDiscount?.DiscountPercent ?? 0m;
        var deliveryPrice = cart.DeliveryMethod?.Price ?? 0m;
        var buyingPrice = PricingService.OrderTotal(itemsSum, discountPercent, deliveryPrice);

        var order = new Order
        {
            // "ORD-" (4) + yyyyMMddHHmmssff (16) = 20 chars
            // DB column: nvarchar(20)
            OrderNumber = $"ORD-{now:yyyyMMddHHmmssff}",
            UserId = userId,
            DateCreated = now,
            RequestedDatetime = now,
            DeliveryLocation = cart.DeliveryLocation,
            ContactPhone = cart.ContactPhone,
            ContactName = cart.ContactName,
            StatusId = 1,        // Primljena
            PaymentStatusId = 1, // Neplaćena
            PaymentMethodId = cart.PaymentMethod?.Id ?? 1,
            DiscountId = cart.OrderDiscount?.Id ?? 1,
            DeliveryId = cart.DeliveryMethod?.Id ?? 2,
            Price = itemsSum,
            PriceWithDiscount = buyingPrice,
            TaxRate = 20m,
            Source = "Web",
            UpdatedAt = now
        };
        order.TaxAmount = decimal.Round(buyingPrice * order.TaxRate / 100m, 2);

        foreach (var item in cart.Items)
        {
            if (item.IsCombo)
            {
                order.OrderComboItems.Add(new OrderComboItem
                {
                    ComboMenuId = item.ItemId,
                    BasePrice = item.UnitPrice,
                    DiscountPercent = 0m,
                    Price = item.LineTotal,
                    Amount = item.Amount
                });
            }
            else
            {
                order.OrderFoodItems.Add(new OrderFoodItem
                {
                    FoodId = item.ItemId,
                    BasePrice = item.UnitPrice,
                    DiscountPercent = 0m,
                    Price = item.LineTotal,
                    Amount = item.Amount
                });
            }
        }

        await _orderRepository.AddAsync(order);

        // Record the loyalty-points movement (earn or spend) and update the user balance.
        var pointsDifference = discountPercent > 0m
            ? -(cart.OrderDiscount?.PointCost ?? 0)
            : PricingService.PointsEarned(itemsSum);

        if (pointsDifference != 0)
        {
            await _pointTransactionRepository.AddAsync(new PointTransaction
            {
                UserId = userId,
                OrderId = order.Id,
                Amount = pointsDifference,
                Reason = pointsDifference > 0 ? "Poeni za porudžbinu" : "Iskorišćen popust",
                CreatedAt = now
            });

            await _accountService.AddPointsAsync(userId, pointsDifference);
        }

        return order.Id;
    }

    /// <inheritdoc />
    public async Task<IEnumerable<DeliveryMethodDto>> GetDeliveryMethodsAsync()
        => (await _deliveryMethodRepository.GetAllAsync()).Select(MapDelivery).ToList();

    /// <inheritdoc />
    public async Task<DeliveryMethodDto?> GetDeliveryMethodAsync(int id)
    {
        var delivery = await _deliveryMethodRepository.GetByIdAsync(id);
        return delivery is null ? null : MapDelivery(delivery);
    }

    /// <inheritdoc />
    public async Task<IEnumerable<PaymentMethodDto>> GetPaymentMethodsAsync()
        => (await _paymentMethodRepository.GetAllAsync()).Select(MapPayment).ToList();

    /// <inheritdoc />
    public async Task<PaymentMethodDto?> GetPaymentMethodAsync(int id)
    {
        var payment = await _paymentMethodRepository.GetByIdAsync(id);
        return payment is null ? null : MapPayment(payment);
    }

    /// <inheritdoc />
    public async Task<IEnumerable<OrderDiscountDto>> GetOrderDiscountsAsync()
        => (await _orderDiscountRepository.GetAllAsync()).Select(MapDiscount).ToList();

    /// <inheritdoc />
    public async Task<OrderDiscountDto?> GetOrderDiscountAsync(int id)
    {
        var discount = await _orderDiscountRepository.GetByIdAsync(id);
        return discount is null ? null : MapDiscount(discount);
    }

    private static DeliveryMethodDto MapDelivery(DeliveryMethod d) => new()
    {
        Id = d.Id,
        Name = d.Name,
        Price = d.Price
    };

    private static PaymentMethodDto MapPayment(PaymentMethod p) => new()
    {
        Id = p.Id,
        Type = p.Type
    };

    private static OrderDiscountDto MapDiscount(OrderDiscount d) => new()
    {
        Id = d.Id,
        Name = d.Name,
        DiscountPercent = d.DiscountPercent,
        PointCost = d.PointCost
    };

    private static OrderDto MapOrder(Order o) => new()
    {
        Id = o.Id,
        OrderNumber = o.OrderNumber,
        UserId = o.UserId,
        DateCreated = o.DateCreated,
        RequestedDatetime = o.RequestedDatetime,
        DateCompleted = o.DateCompleted,
        DeliveryLocation = o.DeliveryLocation,
        ContactPhone = o.ContactPhone,
        ContactName = o.ContactName,
        StatusName = o.Status.Name,
        StatusId = o.StatusId,
        PaymentStatusName = o.PaymentStatus.Name,
        PaymentStatusId = o.PaymentStatusId,
        PaymentMethodName = o.PaymentMethod.Type,
        DeliveryMethodName = o.Delivery.Name,
        DeliveryPrice = o.Delivery?.Price ?? 0m,
        DiscountName = o.Discount?.Name,
        CancellationReason = o.CancellationReason,
        Price = o.Price,
        PriceWithDiscount = o.PriceWithDiscount,
        TaxAmount = o.TaxAmount,
        Lines = o.OrderFoodItems.Select(f => new OrderLineDto
        {
            Name = f.Food.Name,
            IsCombo = false,
            Amount = f.Amount,
            BasePrice = f.BasePrice,
            DiscountPercent = f.DiscountPercent,
            Price = f.Price
        })
        .Concat(o.OrderComboItems.Select(c => new OrderLineDto
        {
            Name = c.ComboMenu.Name,
            IsCombo = true,
            Amount = c.Amount,
            BasePrice = c.BasePrice,
            DiscountPercent = c.DiscountPercent,
            Price = c.Price
        }))
        .ToList()
    };
}
