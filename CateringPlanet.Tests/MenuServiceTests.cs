using CateringPlanet.Application.DTOs;
using CateringPlanet.Application.Services;
using CateringPlanet.Domain.Entities;
using CateringPlanet.Tests.Fakes;

namespace CateringPlanet.Tests;

/// <summary>
/// Unit tests for <see cref="MenuService"/> mapping and persistence flows,
/// run against in-memory repository fakes (no database).
/// </summary>
public class MenuServiceTests
{
    private static FoodCategory SupeCategory() => new()
    {
        Id = 1, Name = "Supe i čorbe", Slug = "supe-i-corbe", DiscountPercent = 5m
    };

    private static Food JunecaSupa(bool visible = true, string? image = "juneca_supa.jpg") => new()
    {
        Id = 1,
        Name = "Juneća supa",
        UnitOfMeasurement = "ml",
        FoodCategoryId = 1,
        FoodCategory = SupeCategory(),
        IsVisible = visible,
        Ingredients = "Govedina, povrće",
        Price = 1000m,
        DiscountPercent = 10m,
        ImageFileName = image
    };

    private static ComboMenu RucakCombo(bool visible = true, string? image = "combo1.jpg") => new()
    {
        Id = 3,
        Name = "Ručak",
        DiscountPercent = 10m,
        IsVisible = visible,
        Price = 800m,
        ImageFileName = image,
        ComboMenuItems = new List<ComboMenuItem>
        {
            new()
            {
                Id = 1,
                FoodId = 5,
                ComboMenuId = 3,
                Amount = 2,
                Food = new Food { Id = 5, Name = "Ćevapi" }
            }
        }
    };

    private static MenuService Service(FakeFoodRepository foods, FakeFoodCategoryRepository categories, FakeComboMenuRepository combos)
        => new(foods, categories, combos);

    [Fact]
    public async Task GetVisibleFoodsAsync_NullCategory_UsesVisibleList()
    {
        var foods = new FakeFoodRepository();
        foods.Foods.Add(JunecaSupa());
        var service = Service(foods, new FakeFoodCategoryRepository(), new FakeComboMenuRepository());

        var result = (await service.GetVisibleFoodsAsync(null)).ToList();

        Assert.Single(result);
        Assert.Equal(1, foods.VisibleCalled);
        Assert.Equal(0, foods.VisibleByCategoryCalled);
    }

    [Fact]
    public async Task GetVisibleFoodsAsync_WithCategory_UsesVisibleByCategoryList()
    {
        var foods = new FakeFoodRepository();
        foods.Foods.Add(JunecaSupa());
        var service = Service(foods, new FakeFoodCategoryRepository(), new FakeComboMenuRepository());

        var result = (await service.GetVisibleFoodsAsync(1)).ToList();

        Assert.Single(result);
        Assert.Equal(0, foods.VisibleCalled);
        Assert.Equal(1, foods.VisibleByCategoryCalled);
    }

    [Fact]
    public async Task GetVisibleFoodsAsync_MapsSellingPriceAndImageUrl()
    {
        var foods = new FakeFoodRepository();
        foods.Foods.Add(JunecaSupa());
        var service = Service(foods, new FakeFoodCategoryRepository(), new FakeComboMenuRepository());

        var dto = (await service.GetVisibleFoodsAsync(null)).Single();

        Assert.Equal(855m, dto.SellingPrice);
        Assert.Equal("/images/food/supe-i-corbe/juneca_supa.jpg", dto.ImageUrl);
        Assert.Equal("Supe i čorbe", dto.FoodCategoryName);
        Assert.Equal("supe-i-corbe", dto.FoodCategorySlug);
        Assert.Equal(5m, dto.CategoryDiscountPercent);
    }

    [Fact]
    public async Task GetVisibleFoodsAsync_NullImage_UsesFallbackImage()
    {
        var foods = new FakeFoodRepository();
        foods.Foods.Add(JunecaSupa(image: null));
        var service = Service(foods, new FakeFoodCategoryRepository(), new FakeComboMenuRepository());

        var dto = (await service.GetVisibleFoodsAsync(null)).Single();

        Assert.Equal("/images/no_image.png", dto.ImageUrl);
    }

    [Fact]
    public async Task GetVisibleCombosAsync_MapsSellingPriceAndItems()
    {
        var combos = new FakeComboMenuRepository();
        combos.Combos.Add(RucakCombo());
        var service = Service(new FakeFoodRepository(), new FakeFoodCategoryRepository(), combos);

        var dto = (await service.GetVisibleCombosAsync()).Single();

        Assert.Equal(720m, dto.SellingPrice);
        var item = Assert.Single(dto.Items);
        Assert.Equal(5, item.FoodId);
        Assert.Equal("Ćevapi", item.FoodName);
        Assert.Equal(2, item.Amount);
    }

    [Fact]
    public async Task GetFoodAsync_UnknownId_ReturnsNull()
    {
        var service = Service(new FakeFoodRepository(), new FakeFoodCategoryRepository(), new FakeComboMenuRepository());
        Assert.Null(await service.GetFoodAsync(999));
    }

    [Fact]
    public async Task GetComboAsync_UnknownId_ReturnsNull()
    {
        var service = Service(new FakeFoodRepository(), new FakeFoodCategoryRepository(), new FakeComboMenuRepository());
        Assert.Null(await service.GetComboAsync(999));
    }

    [Fact]
    public async Task CreateFoodAsync_CopiesFieldsAndReturnsAssignedId()
    {
        var foods = new FakeFoodRepository();
        var service = Service(foods, new FakeFoodCategoryRepository(), new FakeComboMenuRepository());
        var dto = new FoodDto
        {
            Name = "Ćevapi",
            UnitOfMeasurement = "g",
            FoodCategoryId = 2,
            IsVisible = true,
            Ingredients = "Mleveno meso",
            Price = 400m,
            DiscountPercent = 5m,
            ImageFileName = "cevapi.jpg"
        };

        var id = await service.CreateFoodAsync(dto);
        var entity = Assert.Single(foods.Foods);

        Assert.Equal(id, entity.Id);
        Assert.Equal("Ćevapi", entity.Name);
        Assert.Equal("g", entity.UnitOfMeasurement);
        Assert.Equal(2, entity.FoodCategoryId);
        Assert.True(entity.IsVisible);
        Assert.Equal("Mleveno meso", entity.Ingredients);
        Assert.Equal(400m, entity.Price);
        Assert.Equal(5m, entity.DiscountPercent);
        Assert.Equal("cevapi.jpg", entity.ImageFileName);
    }

    [Fact]
    public async Task UpdateFoodAsync_UnknownId_DoesNotCallUpdate()
    {
        var foods = new FakeFoodRepository();
        var service = Service(foods, new FakeFoodCategoryRepository(), new FakeComboMenuRepository());

        await service.UpdateFoodAsync(new FoodDto { Id = 999, Name = "Nepostojeće" });

        Assert.Equal(0, foods.UpdateCalls);
    }

    [Fact]
    public async Task CreateCategoryAsync_GeneratesSlugFromName()
    {
        var categories = new FakeFoodCategoryRepository();
        var service = Service(new FakeFoodRepository(), categories, new FakeComboMenuRepository());

        var id = await service.CreateCategoryAsync(new FoodCategoryDto { Name = "Supe i čorbe", DiscountPercent = 10m });
        var category = Assert.Single(categories.Categories);

        Assert.Equal(id, category.Id);
        Assert.Equal("supe-i-corbe", category.Slug);
        Assert.Equal(10m, category.DiscountPercent);
    }

    [Fact]
    public async Task UpdateCategoryDiscountAsync_UpdatesDiscountPercent()
    {
        var categories = new FakeFoodCategoryRepository();
        categories.Categories.Add(new FoodCategory { Id = 1, Name = "Supe", Slug = "supe", DiscountPercent = 0m });
        var service = Service(new FakeFoodRepository(), categories, new FakeComboMenuRepository());

        await service.UpdateCategoryDiscountAsync(1, 15m);

        Assert.Equal(15m, categories.Categories[0].DiscountPercent);
        Assert.Equal(1, categories.UpdateCalls);
    }
}
