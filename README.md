# CateringPlanet

A full-stack catering web application — an ASP.NET Core port of the legacy CateringAgency project.
Customers browse the menu, build a cart, and place orders; managers/admin manage foods, combos,
and orders. A separate REST API exposes the same domain with JWT authentication and Swagger.

## Tech stack

- .NET 8 (ASP.NET Core MVC + Web API)
- Entity Framework Core (Code-First) with SQL Server
- ASP.NET Core Identity (roles: `Admin`, `Manager`, `Customer`)
- Bootstrap 5 UI (Razor views)
- Swagger/OpenAPI on the API project (Development only)

## Solution structure

| Project | Responsibility |
|---|---|
| `CateringPlanet.Domain` | Entities and repository interfaces |
| `CateringPlanet.Application` | Services, DTOs, business logic |
| `CateringPlanet.Infrastructure` | EF Core `DbContext`, migrations, repositories, `DbInitializer` |
| `CateringPlanet.Web` | MVC web app (menu, cart, checkout, orders, admin UI) |
| `CateringPlanet.Api` | REST API with JWT auth and Swagger |

## Prerequisites

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- SQL Server (any edition) or LocalDB
- Visual Studio 2022 (recommended) or the `dotnet` CLI
- `dotnet-ef` tool (only needed to create migrations): `dotnet tool install --global dotnet-ef`

## Database setup

The database schema and lookup data (categories, order statuses, payment statuses, delivery
methods, payment methods, order discounts) are created by an EF Core migration — **no `.bak`
file is needed**.

1. Open `CateringPlanet.Web\appsettings.json` and `CateringPlanet.Api\appsettings.json`.
2. Set `ConnectionStrings:DefaultConnection` to your SQL Server, e.g.:

   ```json
   "DefaultConnection": "Server=.\\SQLEXPRESS;Database=CateringPlanetDb;Trusted_Connection=True;TrustServerCertificate=True"
   ```

   For LocalDB use: `Server=(localdb)\\mssqllocaldb;Database=CateringPlanetDb;Trusted_Connection=True`

3. Apply the migration (from the solution root):

   ```powershell
   dotnet ef database update --project CateringPlanet.Infrastructure --startup-project CateringPlanet.Web
   ```

   Or in Visual Studio: **Package Manager Console** → `Update-Database`
   (default project: `CateringPlanet.Infrastructure`).

## Automatic seeding

`DbInitializer.SeedAsync` runs on every startup and is **idempotent** — it only inserts data
when the database is empty:

- Roles: `Admin`, `Customer`, `Manager`
- 12 demo users (all with the password below)
- 40 demo foods across 10 categories
- 2 demo combos and 1 demo order

> The seed dataset is a **representative demo set** — the exact production dataset is not
> committed. Replace it in `CateringPlanet.Infrastructure\Persistence\DbInitializer.cs`
> when the real records are available.

## Demo accounts

| Role | Email | Password |
|---|---|---|
| Admin | `luka.popovic@cateringplanet.rs` | `Catering123!` |
| Customer | `marko.jovanovic@example.com` | `Catering123!` |
| Customer | `ana.nikolic@example.com` | `Catering123!` |

## Running the app

Both the web app and the API must run for the full experience (the admin navbar links to
Swagger).

**Option 1 — Visual Studio**
1. Right-click the solution → **Configure Startup Projects…**
2. Select **Multiple startup projects** and set both to **Start**:
   - `CateringPlanet.Web` (https profile)
   - `CateringPlanet.Api` (https profile)
3. Press **F5**.

**Option 2 — CLI** (two terminals)

```powershell
dotnet run --project CateringPlanet.Web --launch-profile https
```

```powershell
dotnet run --project CateringPlanet.Api --launch-profile https
```

| App | URL |
|---|---|
| Web (MVC) | `https://localhost:7243` (http: `http://localhost:5202`) |
| API + Swagger | `https://localhost:7025/swagger` (http: `http://localhost:5156`) |

> Swagger is only available when `ASPNETCORE_ENVIRONMENT` is `Development`.

## API authentication

The API uses JWT bearer tokens (`CateringPlanet.Api` project):

- `POST /api/auth/login` with `{ "email": "...", "password": "..." }`
- Send the returned token as `Authorization: Bearer <token>`
- JWT settings live in `CateringPlanet.Api\appsettings.json` — **the committed signing key is
  for development only; change it for production.**

## Image uploads

Admin pages accept food/combo images:

- Allowed: `.jpg`, `.jpeg`, `.png`, `.gif`, `.webp` (SVG is rejected)
- Max size: **15 MB**
- Files are stored under `CateringPlanet.Web\wwwroot\images\`
- Items without an image fall back to `no_image.png`

## Configuration notes

| Key | File | Purpose |
|---|---|---|
| `ConnectionStrings:DefaultConnection` | Web + Api `appsettings.json` | SQL Server connection |
| `Api:BaseUrl` | Web `appsettings.json` | URL the admin navbar uses for Swagger |
| `Jwt:*` | Api `appsettings.json` | JWT issuer/audience/key/expiry (dev only) |
| `Cors:AllowedOrigins` | Api `appsettings.json` | Allowed web origins |

## Notes

- Do not commit SQL Server `.mdf`/`.ldf`/`.bak` files, uploaded images, or real secrets —
  the migrations + seeder recreate everything the app needs.
- The `CateringPlanet` folder at the repo root is a leftover template and is **not** part of
  `CateringPlanet.sln`.
