using CateringPlanet.Domain.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace CateringPlanet.Infrastructure.Data;

/// <summary>
/// EF Core database context for the CateringPlanet domain plus ASP.NET Core Identity.
/// Identity <c>AspNet*</c> tables replace the legacy <c>[dbo].[user]</c>/<c>[dbo].[role]</c> tables.
/// </summary>
public class CateringPlanetDbContext : IdentityDbContext<ApplicationUser>
{
    public CateringPlanetDbContext(DbContextOptions<CateringPlanetDbContext> options) : base(options)
    {

    }

    public DbSet<ComboMenu> ComboMenus => Set<ComboMenu>();
    public DbSet<ComboMenuItem> ComboMenuItems => Set<ComboMenuItem>();
    public DbSet<DeliveryMethod> DeliveryMethods => Set<DeliveryMethod>();
    public DbSet<Food> Foods => Set<Food>();
    public DbSet<FoodCategory> FoodCategories => Set<FoodCategory>();
    public DbSet<Order> Orders => Set<Order>();
    public DbSet<OrderComboItem> OrderComboItems => Set<OrderComboItem>();
    public DbSet<OrderDiscount> OrderDiscounts => Set<OrderDiscount>();
    public DbSet<OrderFoodItem> OrderFoodItems => Set<OrderFoodItem>();
    public DbSet<OrderStatus> OrderStatuses => Set<OrderStatus>();
    public DbSet<PaymentMethod> PaymentMethods => Set<PaymentMethod>();
    public DbSet<PaymentStatus> PaymentStatuses => Set<PaymentStatus>();
    public DbSet<PointTransaction> PointTransactions => Set<PointTransaction>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Apply all IEntityTypeConfiguration<T> implementations from this assembly.
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(CateringPlanetDbContext).Assembly);
    }
}
