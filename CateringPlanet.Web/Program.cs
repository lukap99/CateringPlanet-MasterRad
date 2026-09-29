using CateringPlanet.Application.Abstractions;
using CateringPlanet.Domain.Interfaces;
using CateringPlanet.Infrastructure.Data;
using CateringPlanet.Infrastructure.Persistence;
using CateringPlanet.Infrastructure.Repositories;
using CateringPlanet.Infrastructure.Identity;
using CateringPlanet.Web.Adapters;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Globalization;

// Pin number parsing/formatting to the invariant culture so form decimals always
// use the dot separator, matching the invariant values rendered by the tag helpers
// (the "__Invariant" hidden inputs). Without this, a Serbian-locale machine parses
// "300.00" as 30000 and "15.00" as 1500, corrupting prices/discounts and overflowing
// decimal(5,2) columns on save.
var culture = CultureInfo.InvariantCulture;
CultureInfo.DefaultThreadCurrentCulture = culture;
CultureInfo.DefaultThreadCurrentUICulture = culture;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

// EF Core DbContext (Code-First).
builder.Services.AddDbContext<CateringPlanetDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// ASP.NET Core Identity (replaces the legacy [dbo].[user]/[dbo].[role] tables).
builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
        options.SignIn.RequireConfirmedAccount = false)
    .AddEntityFrameworkStores<CateringPlanetDbContext>()
    .AddDefaultTokenProviders();

// Session state (required by the session-backed cart service).
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});
builder.Services.AddHttpContextAccessor();

// Repositories (Infrastructure).
builder.Services.AddScoped<IFoodRepository, FoodRepository>();
builder.Services.AddScoped<IFoodCategoryRepository, FoodCategoryRepository>();
builder.Services.AddScoped<IComboMenuRepository, ComboMenuRepository>();
builder.Services.AddScoped<IOrderRepository, OrderRepository>();
builder.Services.AddScoped<IOrderStatusRepository, OrderStatusRepository>();
builder.Services.AddScoped<IOrderDiscountRepository, OrderDiscountRepository>();
builder.Services.AddScoped<IDeliveryMethodRepository, DeliveryMethodRepository>();
builder.Services.AddScoped<IPaymentMethodRepository, PaymentMethodRepository>();
builder.Services.AddScoped<IPointTransactionRepository, PointTransactionRepository>();

// Application services.
builder.Services.AddScoped<IMenuService, CateringPlanet.Application.Services.MenuService>();
builder.Services.AddScoped<CateringPlanet.Application.Abstractions.IOrderService, CateringPlanet.Application.Services.OrderService>();
builder.Services.AddScoped<IAccountService, AccountService>();
builder.Services.AddScoped<IAdminService, AdminService>();
builder.Services.AddScoped<ICartService, SessionCartService>();
builder.Services.AddScoped<IImageStorageService, LocalImageStorageService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseSession();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

// Seed demo data on startup (idempotent). Requires the schema to exist (run "Update-Database" first).
await DbInitializer.SeedAsync(app.Services);

app.Run();
