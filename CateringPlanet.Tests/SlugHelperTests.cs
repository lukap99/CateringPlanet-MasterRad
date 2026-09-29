using CateringPlanet.Application.Helpers;

namespace CateringPlanet.Tests;

/// <summary>
/// Unit tests for <see cref="SlugHelper.Slugify"/> — URL-safe filename generation.
/// </summary>
public class SlugHelperTests
{
    [Fact]
    public void Slugify_SpacesAndDiacritics_ProducesAsciiSlug()
    {
        Assert.Equal("supe-i-corbe", SlugHelper.Slugify("Supe i čorbe"));
    }

    [Fact]
    public void Slugify_DiacriticsAndSymbols_ReplacesWithDashes()
    {
        Assert.Equal("sargarepa---duvec-", SlugHelper.Slugify("Šargarepa & Đuveč!"));
    }

    [Fact]
    public void Slugify_LettersAndDigits_KeepsThem()
    {
        Assert.Equal("cappuccino-2x", SlugHelper.Slugify("Cappuccino 2x"));
    }

    [Fact]
    public void Slugify_EmptyString_ReturnsEmpty()
    {
        Assert.Equal(string.Empty, SlugHelper.Slugify(string.Empty));
    }

    [Fact]
    public void Slugify_MultipleSpaces_ProducesMultipleDashes()
    {
        // Documents current behavior: consecutive separators are NOT collapsed.
        Assert.Equal("a--b", SlugHelper.Slugify("a  b"));
    }
}
