using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CateringPlanet.Domain.Entities;
using CateringPlanet.Infrastructure.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;

namespace CateringPlanet.Infrastructure.Persistence;

/// <summary>
/// Seeds bulk/demo data that is not covered by the EF Core <c>HasData</c> migration
/// (foods, users, combos, orders, point transactions). Idempotent: skips food seeding
/// when the database already contains food records.
/// </summary>
/// <remarks>
/// IMPORTANT: the exact production dataset is not available on disk, so this seeder uses a
/// clearly-marked representative demo dataset that should be replaced with the real records later.
/// Run "Update-Database" first so the schema and <c>HasData</c> lookup rows exist.
/// </remarks>
public static class DbInitializer
{
    private const string DefaultPassword = "Catering123!";

    public static async Task SeedAsync(IServiceProvider serviceProvider)
    {
        using var scope = serviceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<CateringPlanetDbContext>();
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
        var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();

        await SeedRolesAsync(roleManager);
        await SeedUsersAsync(userManager);

        if (!context.Foods.Any())
        {
            var foods = SeedFoods(context);
            SeedCombos(context, foods);
            SeedOrders(context, foods);
        }

        await context.SaveChangesAsync();
    }

    private static async Task SeedRolesAsync(RoleManager<IdentityRole> roleManager)
    {
        // "Manager" is required by the role-based navbars ported from CateringAgency.
        string[] roles = { "Admin", "Customer", "Manager" };
        foreach (var role in roles)
        {
            if (!await roleManager.RoleExistsAsync(role))
            {
                await roleManager.CreateAsync(new IdentityRole(role));
            }
        }
    }

    private static async Task SeedUsersAsync(UserManager<ApplicationUser> userManager)
    {
        if (userManager.Users.Any())
        {
            return;
        }

        // NOTE: representative demo users; replace with the exact production dataset when available.
        var demoUsers = new (string First, string Last, string Email, string Role)[]
        {
            ("Luka", "Popović", "luka.popovic@cateringplanet.rs", "Admin"),
            ("Ana", "Nikolić", "ana.nikolic@example.com", "Customer"),
            ("Marko", "Jovanović", "marko.jovanovic@example.com", "Customer"),
            ("Jelena", "Petrović", "jelena.petrovic@example.com", "Customer"),
            ("Stefan", "Đorđević", "stefan.djordjevic@example.com", "Customer"),
            ("Milica", "Ilić", "milica.ilic@example.com", "Customer"),
            ("Nikola", "Stanković", "nikola.stankovic@example.com", "Customer"),
            ("Teodora", "Pavlović", "teodora.pavlovic@example.com", "Customer"),
            ("Dušan", "Milovanović", "dusan.milovanovic@example.com", "Customer"),
            ("Sara", "Kovačević", "sara.kovacevic@example.com", "Customer"),
            ("Filip", "Lukić", "filip.lukic@example.com", "Customer"),
            ("Nina", "Marjanović", "nina.marjanovic@example.com", "Customer"),
        };

        foreach (var demo in demoUsers)
        {
            var user = new ApplicationUser
            {
                UserName = demo.Email,
                Email = demo.Email,
                EmailConfirmed = true,
                FirstName = demo.First,
                LastName = demo.Last,
                Points = 0
            };

            var result = await userManager.CreateAsync(user, DefaultPassword);
            if (result.Succeeded)
            {
                await userManager.AddToRoleAsync(user, demo.Role);
            }
        }
    }

    private static List<Food> SeedFoods(CateringPlanetDbContext context)
    {
        // NOTE: representative demo dataset — replace with the exact production 94 items when available.
        var items = new (int CategoryId, string Name, decimal Price, string Unit)[]
        {
            // 1 - Supe i čorbe
            (1, "Juneća supa", 320m, "ml"),
            (1, "Pileća supa", 280m, "ml"),
            (1, "Paradajz čorba", 260m, "ml"),
            (1, "Riblja čorba", 350m, "ml"),
            // 2 - Hleb i peciva
            (2, "Beli hleb", 80m, "kom"),
            (2, "Kifla", 40m, "kom"),
            (2, "Kroasan", 120m, "kom"),
            (2, "Pogača", 200m, "kom"),
            // 3 - Torte i kolači
            (3, "Čokoladna torta", 420m, "kom"),
            (3, "Voćna torta", 400m, "kom"),
            (3, "Čizkejk", 380m, "kom"),
            (3, "Reforma torta", 450m, "kom"),
            // 4 - Salate
            (4, "Srpska salata", 240m, "g"),
            (4, "Šopska salata", 260m, "g"),
            (4, "Cezar salata", 380m, "g"),
            (4, "Grčka salata", 340m, "g"),
            // 5 - Kuvana jela
            (5, "Sarma", 420m, "g"),
            (5, "Punjena paprika", 400m, "g"),
            (5, "Musaka", 380m, "g"),
            (5, "Pasulj", 300m, "g"),
            // 6 - Roštilj
            (6, "Ćevapi", 450m, "g"),
            (6, "Pljeskavica", 420m, "kom"),
            (6, "Ražnjići", 500m, "g"),
            (6, "Kobasica", 350m, "kom"),
            // 7 - Kanapei
            (7, "Kanape sa pršutom", 180m, "kom"),
            (7, "Kanape sa sirom", 150m, "kom"),
            (7, "Kanape sa lososom", 220m, "kom"),
            // 8 - Pića
            (8, "Gazirana voda", 120m, "kom"),
            (8, "Sok od pomorandže", 180m, "kom"),
            (8, "Limunada", 200m, "kom"),
            // 9 - Vegan
            (9, "Veganski burger", 380m, "kom"),
            (9, "Tofu salata", 340m, "g"),
            (9, "Humus", 280m, "g"),
            // 10 - Pizza
            (10, "Margarita", 500m, "kom"),
            (10, "Kaprićoza", 560m, "kom"),
            (10, "Vegetariana", 520m, "kom"),
            (10, "Četiri sira", 580m, "kom"),
        };

        var foods = new List<Food>();
        foreach (var item in items)
        {
            var food = new Food
            {
                Name = item.Name,
                UnitOfMeasurement = item.Unit,
                FoodCategoryId = item.CategoryId,
                IsVisible = true,
                Price = item.Price,
                DiscountPercent = 0m,
                ImageFileName = null, // ImageUrlResolver falls back to no_image.png
                CreatedAt = DateTime.Now,
                ModifiedAt = DateTime.Now
            };
            foods.Add(food);
            context.Foods.Add(food);
        }

        return foods;
    }

    private static void SeedCombos(CateringPlanetDbContext context, List<Food> foods)
    {
        Food ByName(string name) => foods.First(f => f.Name == name);

        var grillCombo = new ComboMenu
        {
            Name = "Roštilj paket",
            DiscountPercent = 10m,
            IsVisible = true,
            Price = 1050m,
            ImageFileName = null,
            CreatedAt = DateTime.Now,
            ModifiedAt = DateTime.Now
        };
        grillCombo.ComboMenuItems.Add(new ComboMenuItem { Food = ByName("Ćevapi"), Amount = 2 });
        grillCombo.ComboMenuItems.Add(new ComboMenuItem { Food = ByName("Pljeskavica"), Amount = 1 });
        grillCombo.ComboMenuItems.Add(new ComboMenuItem { Food = ByName("Srpska salata"), Amount = 1 });

        var familyCombo = new ComboMenu
        {
            Name = "Porodični paket",
            DiscountPercent = 15m,
            IsVisible = true,
            Price = 1850m,
            ImageFileName = null,
            CreatedAt = DateTime.Now,
            ModifiedAt = DateTime.Now
        };
        familyCombo.ComboMenuItems.Add(new ComboMenuItem { Food = ByName("Ražnjići"), Amount = 2 });
        familyCombo.ComboMenuItems.Add(new ComboMenuItem { Food = ByName("Pogača"), Amount = 1 });
        familyCombo.ComboMenuItems.Add(new ComboMenuItem { Food = ByName("Grčka salata"), Amount = 1 });

        context.ComboMenus.AddRange(grillCombo, familyCombo);
    }

    private static void SeedOrders(CateringPlanetDbContext context, List<Food> foods)
    {
        var users = context.Users.ToList();
        if (users.Count == 0)
        {
            return;
        }

        Food ByName(string name) => foods.First(f => f.Name == name);

        var order = new Order
        {
            OrderNumber = "ORD-2024-001",
            UserId = users[0].Id,
            DateCreated = DateTime.Now.AddDays(-3),
            DeliveryLocation = "Bulevar kralja Aleksandra 1, Beograd",
            ContactPhone = "+381641234567",
            ContactName = $"{users[0].FirstName} {users[0].LastName}",
            StatusId = 6,        // Završena
            PaymentStatusId = 2, // Plaćena
            PaymentMethodId = 1, // Kartica
            DiscountId = 1,      // Bez popusta
            DeliveryId = 1,      // Dostava
            Source = "Web",
            TaxRate = 20m,       // 20% PDV (representative)
            UpdatedAt = DateTime.Now
        };

        var cevapi = ByName("Ćevapi");
        var salata = ByName("Srpska salata");
        order.OrderFoodItems.Add(new OrderFoodItem
        {
            Food = cevapi,
            BasePrice = cevapi.Price,
            DiscountPercent = 0m,
            Price = cevapi.Price,
            Amount = 2
        });
        order.OrderFoodItems.Add(new OrderFoodItem
        {
            Food = salata,
            BasePrice = salata.Price,
            DiscountPercent = 0m,
            Price = salata.Price,
            Amount = 1
        });

        order.Price = order.OrderFoodItems.Sum(i => i.Price);
        order.PriceWithDiscount = order.Price;
        order.TaxAmount = decimal.Round(order.Price * order.TaxRate / 100m, 2);

        context.Orders.Add(order);

        // Loyalty points earned for the completed order.
        context.PointTransactions.Add(new PointTransaction
        {
            UserId = users[0].Id,
            Order = order,
            Amount = 100,
            Reason = "Poeni za porudžbinu",
            CreatedAt = DateTime.Now
        });
    }
}
