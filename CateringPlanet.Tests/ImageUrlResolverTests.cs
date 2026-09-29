using CateringPlanet.Application.Helpers;

namespace CateringPlanet.Tests;

/// <summary>
/// Unit tests for <see cref="ImageUrlResolver"/> — image URL composition.
/// </summary>
public class ImageUrlResolverTests
{
    [Fact]
    public void ForFood_NullFileName_UsesFallbackImage()
    {
        Assert.Equal("/images/no_image.png", ImageUrlResolver.ForFood(null, "supe-i-corbe"));
    }

    [Fact]
    public void ForFood_WithFileName_BuildsCategoryScopedUrl()
    {
        Assert.Equal(
            "/images/food/supe-i-corbe/juneca_supa.jpg",
            ImageUrlResolver.ForFood("juneca_supa.jpg", "supe-i-corbe"));
    }

    [Fact]
    public void ForFood_CustomBasePath_TrimsTrailingSlash()
    {
        Assert.Equal("/img/food/cat/x.jpg", ImageUrlResolver.ForFood("x.jpg", "cat", "/img/"));
    }

    [Fact]
    public void ForCombo_NullFileName_UsesFallbackImage()
    {
        Assert.Equal("/images/no_image.png", ImageUrlResolver.ForCombo(null));
    }

    [Fact]
    public void ForCombo_WithFileName_BuildsComboUrl()
    {
        Assert.Equal("/images/combo/combo1.jpg", ImageUrlResolver.ForCombo("combo1.jpg"));
    }

    [Fact]
    public void Defaults_ExposeExpectedConstants()
    {
        Assert.Equal("/images", ImageUrlResolver.DefaultBasePath);
        Assert.Equal("no_image.png", ImageUrlResolver.DefaultImage);
    }
}
