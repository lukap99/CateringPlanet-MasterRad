using System.Text.RegularExpressions;
using CateringPlanet.Application.DTOs;
using CateringPlanet.Application.Services;
using CateringPlanet.Domain.Entities;
using CateringPlanet.Tests.Fakes;

namespace CateringPlanet.Tests;

/// <summary>
/// Unit tests for <see cref="OrderService"/> — order placement, status changes,
/// loyalty-points movement and ownership checks, run against in-memory fakes.
/// </summary>
public class OrderServiceTests
{
    /// <summary>Assembles the service with all seven fakes and seeded lookup data.</summary>
    private sealed class Fixture
    {
        public FakeOrderRepository Orders { get; } = new();

        public FakePointTransactionRepository PointTransactions { get; } = new();

        public FakeAccountService Account { get; } = new();

        public FakeDeliveryMethodRepository DeliveryMethods { get; } = new();

        public FakePaymentMethodRepository PaymentMethods { get; } = new();

        public FakeOrderDiscountRepository Discounts { get; } = new();

        public FakeOrderStatusRepository Statuses { get; } = new();

        public OrderService Service { get; }

        public Fixture()
        {
            Statuses.Statuses.AddRange(Enumerable.Range(1, 8).Select(i => new OrderStatus { Id = i, Name = $"Status{i}" }));
            PaymentMethods.Methods.Add(new PaymentMethod { Id = 1, Type = "Gotovina" });
            DeliveryMethods.Methods.Add(new DeliveryMethod { Id = 1, Name = "Dostava", Price = 300m });
            DeliveryMethods.Methods.Add(new DeliveryMethod { Id = 2, Name = "Preuzimanje", Price = 0m });
            Discounts.Discounts.Add(new OrderDiscount { Id = 1, Name = "Osnovni", DiscountPercent = 0m, PointCost = 0 });
            Discounts.Discounts.Add(new OrderDiscount { Id = 2, Name = "Srebrni", DiscountPercent = 10m, PointCost = 300 });

            Service = new OrderService(
                Orders,
                PointTransactions,
                Account,
                DeliveryMethods,
                PaymentMethods,
                Discounts,
                Statuses);
        }
    }

    private static CartDto StandardCart(OrderDiscountDto? discount = null, DeliveryMethodDto? delivery = null, PaymentMethodDto? payment = null) => new()
    {
        Items =
        {
            new CartItemDto { ItemId = 5, Name = "Juneća supa", IsCombo = false, Amount = 2, UnitPrice = 855m },
            new CartItemDto { ItemId = 3, Name = "Ručak", IsCombo = true, Amount = 1, UnitPrice = 720m }
        },
        ContactName = "Luka Popović",
        ContactPhone = "064 123-4567",
        DeliveryLocation = "Cara Dušana 34, Beograd",
        OrderDiscount = discount,
        DeliveryMethod = delivery,
        PaymentMethod = payment
    };

    private static Order StoredOrder(int id, string userId, int statusId = 1, int paymentStatusId = 1) => new()
    {
        Id = id,
        OrderNumber = $"ORD-{id:D16}",
        UserId = userId,
        DateCreated = new DateTime(2026, 1, 1),
        RequestedDatetime = new DateTime(2026, 1, 1),
        StatusId = statusId,
        PaymentStatusId = paymentStatusId,
        PaymentMethodId = 1,
        DiscountId = 1,
        DeliveryId = 1,
        Price = 100m,
        PriceWithDiscount = 100m,
        TaxRate = 20m,
        Source = "Web",
        UpdatedAt = new DateTime(2026, 1, 1),
        Status = new OrderStatus { Id = statusId, Name = $"Status{statusId}" },
        PaymentStatus = new PaymentStatus { Id = paymentStatusId, Name = $"PaymentStatus{paymentStatusId}" },
        PaymentMethod = new PaymentMethod { Id = 1, Type = "Gotovina" },
        Delivery = new DeliveryMethod { Id = 1, Name = "Dostava", Price = 300m },
        Discount = new OrderDiscount { Id = 1, Name = "Osnovni", DiscountPercent = 0m, PointCost = 0 }
    };

    [Fact]
    public async Task PlaceOrder_NoDiscount_ComputesTotalsStoresOrderAndEarnsPoints()
    {
        var fx = new Fixture();
        var cart = StandardCart(
            delivery: new DeliveryMethodDto { Id = 1, Name = "Dostava", Price = 300m },
            payment: new PaymentMethodDto { Id = 1, Type = "Gotovina" });

        var id = await fx.Service.PlaceOrderAsync("userA", cart);
        var order = Assert.Single(fx.Orders.Orders);

        Assert.Equal(id, order.Id);
        Assert.Equal(2430m, order.Price);
        Assert.Equal(2730m, order.PriceWithDiscount!.Value);
        Assert.Equal(546m, order.TaxAmount!.Value);
        Assert.Equal(20m, order.TaxRate);
        Assert.Equal(1, order.StatusId);
        Assert.Equal(1, order.PaymentStatusId);
        Assert.Equal(1, order.PaymentMethodId);
        Assert.Equal(1, order.DeliveryId);
        Assert.Equal(1, order.DiscountId);
        Assert.Equal("Web", order.Source);
        Assert.Matches("^ORD-\\d{16}$", order.OrderNumber);

        var foodLine = Assert.Single(order.OrderFoodItems);
        Assert.Equal(5, foodLine.FoodId);
        Assert.Equal(2, foodLine.Amount);
        Assert.Equal(1710m, foodLine.Price);

        var comboLine = Assert.Single(order.OrderComboItems);
        Assert.Equal(3, comboLine.ComboMenuId);
        Assert.Equal(1, comboLine.Amount);
        Assert.Equal(720m, comboLine.Price);

        Assert.Equal(5, fx.Account.Points["userA"]);
        var tx = Assert.Single(fx.PointTransactions.Transactions);
        Assert.Equal(5, tx.Amount);
        Assert.Equal("Poeni za porudžbinu", tx.Reason);
        Assert.Equal("userA", tx.UserId);
        Assert.Equal((int?)order.Id, tx.OrderId);
    }

    [Fact]
    public async Task PlaceOrder_WithDiscount_AppliesOrderDiscountAndSpendsPoints()
    {
        var fx = new Fixture();
        var discount = new OrderDiscountDto { Id = 2, Name = "Srebrni", DiscountPercent = 10m, PointCost = 300 };
        var cart = StandardCart(discount: discount);

        await fx.Service.PlaceOrderAsync("userA", cart);
        var order = Assert.Single(fx.Orders.Orders);

        Assert.Equal(2430m, order.Price);
        Assert.Equal(2187m, order.PriceWithDiscount!.Value);
        Assert.Equal(2, order.DiscountId);
        Assert.Equal(2, order.DeliveryId);
        Assert.Equal(1, order.PaymentMethodId);

        Assert.Equal(-300, fx.Account.Points["userA"]);
        var tx = Assert.Single(fx.PointTransactions.Transactions);
        Assert.Equal(-300, tx.Amount);
        Assert.Equal("Iskorišćen popust", tx.Reason);
    }

    [Fact]
    public async Task PlaceOrder_SmallOrderSum_RecordsNoPointsMovement()
    {
        var fx = new Fixture();
        var cart = new CartDto
        {
            Items = { new CartItemDto { ItemId = 5, Name = "Kifla", IsCombo = false, Amount = 1, UnitPrice = 100m } }
        };

        await fx.Service.PlaceOrderAsync("userA", cart);

        Assert.Empty(fx.PointTransactions.Transactions);
        Assert.Empty(fx.Account.PointsCalls);
    }

    [Fact]
    public async Task UpdateOrderStatus_Completed_MarksPaidAndRecordsDateCompleted()
    {
        var fx = new Fixture();
        fx.Orders.Orders.Add(StoredOrder(1, "userA"));

        var result = await fx.Service.UpdateOrderStatusAsync(1, 6);
        var order = fx.Orders.Orders.Single();

        Assert.True(result);
        Assert.Equal(6, order.StatusId);
        Assert.Equal(2, order.PaymentStatusId);
        Assert.NotNull(order.DateCompleted);
    }

    [Fact]
    public async Task UpdateOrderStatus_Cancelled_StoresCancellationReason()
    {
        var fx = new Fixture();
        fx.Orders.Orders.Add(StoredOrder(1, "userA"));

        var result = await fx.Service.UpdateOrderStatusAsync(1, 7, "Cancelled by customer");
        var order = fx.Orders.Orders.Single();

        Assert.True(result);
        Assert.Equal(7, order.StatusId);
        Assert.Equal("Cancelled by customer", order.CancellationReason);
    }

    [Fact]
    public async Task UpdateOrderStatus_Rejected_StoresCancellationReason()
    {
        var fx = new Fixture();
        fx.Orders.Orders.Add(StoredOrder(1, "userA"));

        var result = await fx.Service.UpdateOrderStatusAsync(1, 8, "Odbijena od strane servisa");
        var order = fx.Orders.Orders.Single();

        Assert.True(result);
        Assert.Equal(8, order.StatusId);
        Assert.Equal("Odbijena od strane servisa", order.CancellationReason);
    }

    [Fact]
    public async Task UpdateOrderStatus_UnknownStatus_ReturnsFalse()
    {
        var fx = new Fixture();
        fx.Orders.Orders.Add(StoredOrder(1, "userA"));

        Assert.False(await fx.Service.UpdateOrderStatusAsync(1, 99));
    }

    [Fact]
    public async Task UpdateOrderStatus_UnknownOrder_ReturnsFalse()
    {
        var fx = new Fixture();

        Assert.False(await fx.Service.UpdateOrderStatusAsync(999, 2));
    }

    [Fact]
    public async Task UpdateOrderStatus_RegularStatus_ChangesOnlyStatusId()
    {
        var fx = new Fixture();
        fx.Orders.Orders.Add(StoredOrder(1, "userA"));

        var result = await fx.Service.UpdateOrderStatusAsync(1, 2);
        var order = fx.Orders.Orders.Single();

        Assert.True(result);
        Assert.Equal(2, order.StatusId);
        Assert.Equal(1, order.PaymentStatusId);
        Assert.Null(order.DateCompleted);
    }

    [Fact]
    public async Task GetOrderAsync_OtherUser_ReturnsNull()
    {
        var fx = new Fixture();
        fx.Orders.Orders.Add(StoredOrder(1, "userA"));

        Assert.Null(await fx.Service.GetOrderAsync(1, "userB"));
    }

    [Fact]
    public async Task GetOrderAsync_Owner_ReturnsDtoWithPointsChange()
    {
        var fx = new Fixture();
        fx.Orders.Orders.Add(StoredOrder(1, "userA"));
        fx.PointTransactions.Transactions.Add(new PointTransaction
        {
            Id = 1,
            UserId = "userA",
            OrderId = 1,
            Amount = 5,
            Reason = "Poeni za porudžbinu",
            CreatedAt = new DateTime(2026, 1, 1)
        });

        var dto = await fx.Service.GetOrderAsync(1, "userA");

        Assert.NotNull(dto);
        Assert.Matches("^ORD-\\d{16}$", dto.OrderNumber);
        Assert.Equal(5, dto.PointsChange!.Value);
    }

    [Fact]
    public async Task GetAllOrdersAsync_FiltersByStatusAndEnrichesCustomer()
    {
        var fx = new Fixture();
        fx.Orders.Orders.Add(StoredOrder(1, "userA", statusId: 1));
        fx.Orders.Orders.Add(StoredOrder(2, "userB", statusId: 2));
        fx.Account.Users["userA"] = new UserDto { Id = "userA", FirstName = "Luka", LastName = "Popović", Email = "luka@example.com" };
        fx.Account.Users["userB"] = new UserDto { Id = "userB", FirstName = "Marko", LastName = "Jovanović", Email = "marko@example.com" };

        var result = (await fx.Service.GetAllOrdersAsync(2)).ToList();
        var dto = Assert.Single(result);

        Assert.Equal(2, dto.StatusId);
        Assert.Equal("Marko Jovanović", dto.CustomerName);
        Assert.Equal("marko@example.com", dto.CustomerEmail);
    }

    [Fact]
    public async Task GetOrdersAsync_PopulatesPointsChangeFromTransactions()
    {
        var fx = new Fixture();
        fx.Orders.Orders.Add(StoredOrder(1, "userA"));
        fx.PointTransactions.Transactions.Add(new PointTransaction
        {
            Id = 1,
            UserId = "userA",
            OrderId = 1,
            Amount = 5,
            Reason = "Poeni za porudžbinu",
            CreatedAt = new DateTime(2026, 1, 1)
        });

        var result = (await fx.Service.GetOrdersAsync("userA")).ToList();
        var dto = Assert.Single(result);

        Assert.Equal(5, dto.PointsChange!.Value);
    }
}
