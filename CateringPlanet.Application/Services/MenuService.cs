using CateringPlanet.Application.Abstractions;
using CateringPlanet.Application.DTOs;
using CateringPlanet.Application.Helpers;
using CateringPlanet.Domain.Entities;
using CateringPlanet.Domain.Interfaces;

namespace CateringPlanet.Application.Services;

/// <summary>
/// Maps menu entities to DTOs and applies pricing/image resolution for view binding.
/// </summary>
public class MenuService : IMenuService
{
    private readonly IFoodRepository _foodRepository;
    private readonly IFoodCategoryRepository _categoryRepository;
    private readonly IComboMenuRepository _comboRepository;

    public MenuService(IFoodRepository foodRepository, IFoodCategoryRepository categoryRepository, IComboMenuRepository comboRepository)
    {
        _foodRepository = foodRepository;
        _categoryRepository = categoryRepository;
        _comboRepository = comboRepository;
    }

    /// <inheritdoc />
    public async Task<IEnumerable<FoodCategoryDto>> GetCategoriesAsync()
        => (await _categoryRepository.GetAllAsync()).Select(MapCategory).ToList();

    /// <inheritdoc />
    public async Task<IEnumerable<FoodDto>> GetVisibleFoodsAsync(int? categoryId)
    {
        IEnumerable<Food> foods = categoryId is > 0
            ? await _foodRepository.GetVisibleByCategoryAsync(categoryId.Value)
            : await _foodRepository.GetVisibleAsync();
        return foods.Select(MapFood).ToList();
    }

    /// <inheritdoc />
    public async Task<IEnumerable<FoodDto>> GetFoodsByCategoryAsync(int categoryId)
        => (await _foodRepository.GetByCategoryAsync(categoryId)).Select(MapFood).ToList();

    /// <inheritdoc />
    public async Task<IEnumerable<ComboMenuDto>> GetVisibleCombosAsync()
        => (await _comboRepository.GetVisibleAsync()).Select(MapCombo).ToList();

    /// <inheritdoc />
    public async Task<FoodDto?> GetFoodAsync(int id)
    {
        var food = await _foodRepository.GetByIdAsync(id);
        return food is null ? null : MapFood(food);
    }

    /// <inheritdoc />
    public async Task<ComboMenuDto?> GetComboAsync(int id)
    {
        var combo = await _comboRepository.GetByIdAsync(id);
        return combo is null ? null : MapCombo(combo);
    }

    /// <inheritdoc />
    public async Task<IEnumerable<FoodDto>> GetAllFoodsAsync()
        => (await _foodRepository.GetAllAsync()).Select(MapFood).ToList();

    /// <inheritdoc />
    public async Task<IEnumerable<ComboMenuDto>> GetAllCombosAsync()
        => (await _comboRepository.GetAllAsync()).Select(MapCombo).ToList();

    /// <inheritdoc />
    public async Task<int> CreateFoodAsync(FoodDto food)
    {
        var entity = new Food
        {
            Name = food.Name,
            UnitOfMeasurement = food.UnitOfMeasurement,
            FoodCategoryId = food.FoodCategoryId,
            IsVisible = food.IsVisible,
            Ingredients = food.Ingredients,
            Price = food.Price,
            DiscountPercent = food.DiscountPercent,
            ImageFileName = food.ImageFileName,
            CreatedAt = DateTime.Now,
            ModifiedAt = DateTime.Now
        };
        await _foodRepository.AddAsync(entity);
        return entity.Id;
    }

    /// <inheritdoc />
    public async Task UpdateFoodAsync(FoodDto food)
    {
        var entity = await _foodRepository.GetByIdAsync(food.Id);
        if (entity is null)
        {
            return;
        }

        entity.Name = food.Name;
        entity.UnitOfMeasurement = food.UnitOfMeasurement;
        entity.FoodCategoryId = food.FoodCategoryId;
        entity.IsVisible = food.IsVisible;
        entity.Ingredients = food.Ingredients;
        entity.Price = food.Price;
        entity.DiscountPercent = food.DiscountPercent;
        entity.ImageFileName = food.ImageFileName;
        entity.ModifiedAt = DateTime.Now;

        await _foodRepository.UpdateAsync(entity);
    }

    /// <inheritdoc />
    public async Task DeleteFoodAsync(int id)
        => await _foodRepository.DeleteAsync(id);

    /// <inheritdoc />
    public async Task<int> CreateCategoryAsync(FoodCategoryDto category)
    {
        var entity = new FoodCategory
        {
            Name = category.Name,
            Slug = SlugHelper.Slugify(category.Name),
            DiscountPercent = category.DiscountPercent,
            CreatedAt = DateTime.Now,
            ModifiedAt = DateTime.Now
        };
        await _categoryRepository.AddAsync(entity);
        return entity.Id;
    }

    /// <inheritdoc />
    public async Task UpdateCategoryDiscountAsync(int categoryId, decimal discountPercent)
    {
        var entity = await _categoryRepository.GetByIdAsync(categoryId);
        if (entity is null)
        {
            return;
        }

        entity.DiscountPercent = discountPercent;
        entity.ModifiedAt = DateTime.Now;
        await _categoryRepository.UpdateAsync(entity);
    }

    /// <inheritdoc />
    public async Task<int> CreateComboAsync(ComboMenuDto combo)
    {
        var entity = new ComboMenu
        {
            Name = combo.Name,
            DiscountPercent = combo.DiscountPercent,
            IsVisible = combo.IsVisible,
            Price = combo.Price,
            ImageFileName = combo.ImageFileName,
            CreatedAt = DateTime.Now,
            ModifiedAt = DateTime.Now
        };
        await _comboRepository.AddAsync(entity);
        return entity.Id;
    }

    /// <inheritdoc />
    public async Task UpdateComboAsync(ComboMenuDto combo)
    {
        var entity = await _comboRepository.GetByIdAsync(combo.Id);
        if (entity is null)
        {
            return;
        }

        entity.Name = combo.Name;
        entity.DiscountPercent = combo.DiscountPercent;
        entity.IsVisible = combo.IsVisible;
        entity.Price = combo.Price;
        entity.ImageFileName = combo.ImageFileName;
        entity.ModifiedAt = DateTime.Now;

        await _comboRepository.UpdateAsync(entity);
    }

    /// <inheritdoc />
    public async Task DeleteComboAsync(int id)
        => await _comboRepository.DeleteAsync(id);

    /// <inheritdoc />
    public async Task AddComboMenuItemAsync(int comboMenuId, int foodId, int amount)
        => await _comboRepository.AddItemAsync(new ComboMenuItem
        {
            ComboMenuId = comboMenuId,
            FoodId = foodId,
            Amount = amount
        });

    /// <inheritdoc />
    public async Task UpdateComboMenuItemAmountAsync(int comboMenuId, int comboMenuItemId, int amount)
        => await _comboRepository.UpdateItemAmountAsync(comboMenuItemId, amount);

    /// <inheritdoc />
    public async Task DeleteComboMenuItemAsync(int comboMenuId, int comboMenuItemId)
        => await _comboRepository.RemoveItemAsync(comboMenuItemId);

    private static FoodCategoryDto MapCategory(FoodCategory c) => new()
    {
        Id = c.Id,
        Name = c.Name,
        Slug = c.Slug,
        DiscountPercent = c.DiscountPercent
    };

    private static FoodDto MapFood(Food f) => new()
    {
        Id = f.Id,
        Name = f.Name,
        UnitOfMeasurement = f.UnitOfMeasurement,
        FoodCategoryId = f.FoodCategoryId,
        FoodCategoryName = f.FoodCategory.Name,
        FoodCategorySlug = f.FoodCategory.Slug,
        IsVisible = f.IsVisible,
        Ingredients = f.Ingredients,
        Price = f.Price,
        DiscountPercent = f.DiscountPercent,
        CategoryDiscountPercent = f.FoodCategory.DiscountPercent,
        SellingPrice = PricingService.FoodSellingPrice(f.Price, f.DiscountPercent, f.FoodCategory.DiscountPercent),
        ImageFileName = f.ImageFileName,
        ImageUrl = ImageUrlResolver.ForFood(f.ImageFileName, f.FoodCategory.Slug)
    };

    private static ComboMenuDto MapCombo(ComboMenu c) => new()
    {
        Id = c.Id,
        Name = c.Name,
        DiscountPercent = c.DiscountPercent,
        IsVisible = c.IsVisible,
        Price = c.Price,
        SellingPrice = PricingService.ComboSellingPrice(c.Price, c.DiscountPercent),
        ImageFileName = c.ImageFileName,
        ImageUrl = ImageUrlResolver.ForCombo(c.ImageFileName),
        Items = c.ComboMenuItems.Select(i => new ComboMenuItemDto
        {
            Id = i.Id,
            FoodId = i.FoodId,
            FoodName = i.Food.Name,
            Amount = i.Amount
        }).ToList()
    };
}
