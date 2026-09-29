using System.Text;
using System.Globalization;
using CateringPlanet.Api;
using CateringPlanet.Api.Security;
using CateringPlanet.Application.Abstractions;
using CateringPlanet.Domain.Interfaces;
using CateringPlanet.Infrastructure.Data;
using CateringPlanet.Infrastructure.Persistence;
using CateringPlanet.Infrastructure.Repositories;
using CateringPlanet.Infrastructure.Identity;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

// Pin number formatting to the invariant culture so the API's string helpers
// (e.g. OrderDto.PriceString/TotalString/TaxString) always serialize with the
// dot decimal separator, matching CateringPlanet.Web. Without this, a Serbian-
// locale machine renders "1500,00" instead of "1,500.00".
var culture = CultureInfo.InvariantCulture;
CultureInfo.DefaultThreadCurrentCulture = culture;
CultureInfo.DefaultThreadCurrentUICulture = culture;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    // JWT bearer security scheme so Swagger UI shows the Authorize button.
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Paste the JWT returned by POST /api/auth/login."
    });
    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Bearer" }
            },
            Array.Empty<string>()
        }
    });

    // XML documentation comments so Swagger UI shows endpoint, parameter and DTO descriptions.
    options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, "CateringPlanet.Api.xml"));
    if (File.Exists(Path.Combine(AppContext.BaseDirectory, "CateringPlanet.Application.xml")))
    {
        options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, "CateringPlanet.Application.xml"));
    }

    // Pre-fill the login request body with the seeded admin account (development demo only).
    options.OperationFilter<SwaggerExampleOperationFilter>();
});

// EF Core DbContext (Code-First).
builder.Services.AddDbContext<CateringPlanetDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// ASP.NET Core Identity (replaces the legacy [dbo].[user]/[dbo].[role] tables).
builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
        options.SignIn.RequireConfirmedAccount = false)
    .AddEntityFrameworkStores<CateringPlanetDbContext>()
    .AddDefaultTokenProviders();

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

// Application services (the API is stateless, so no session-backed ICartService).
builder.Services.AddScoped<IMenuService, CateringPlanet.Application.Services.MenuService>();
builder.Services.AddScoped<CateringPlanet.Application.Abstractions.IOrderService, CateringPlanet.Application.Services.OrderService>();
builder.Services.AddScoped<IAccountService, AccountService>();
builder.Services.AddScoped<IAdminService, AdminService>();

// JWT token issuance for the auth endpoints.
builder.Services.AddScoped<JwtTokenService>();

// JWT bearer authentication.
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = builder.Configuration["Jwt:Issuer"],
        ValidAudience = builder.Configuration["Jwt:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]!))
    };
});
builder.Services.AddAuthorization();

// CORS for the MVC web app (thesis-demo permissive policy, restricted to the Web origins).
var allowedOrigins = builder.Configuration.GetSection("Cors:AllowedOrigins").Get<string[]>() ?? Array.Empty<string>();
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy => policy
        .WithOrigins(allowedOrigins)
        .AllowAnyHeader()
        .AllowAnyMethod());
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseRouting();
app.UseCors();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

// Seed demo data on startup (idempotent). Requires the schema to exist (run "Update-Database" first).
await DbInitializer.SeedAsync(app.Services);

app.Run();
