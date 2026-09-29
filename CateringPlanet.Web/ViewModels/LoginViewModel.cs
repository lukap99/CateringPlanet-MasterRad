using System.ComponentModel.DataAnnotations;

namespace CateringPlanet.Web.ViewModels;

/// <summary>
/// Input model for the account login form.
/// </summary>
public class LoginViewModel
{
    /// <summary>User's e-mail address (doubles as the Identity user name).</summary>
    [Required]
    [EmailAddress]
    [Display(Name = "E-mail")]
    public string Email { get; set; } = null!;

    /// <summary>Plain-text password captured by the form.</summary>
    [Required]
    [DataType(DataType.Password)]
    [Display(Name = "Password")]
    public string Password { get; set; } = null!;

    /// <summary>Whether the sign-in cookie should persist across browser sessions.</summary>
    [Display(Name = "Remember me")]
    public bool RememberMe { get; set; }

    /// <summary>Optional local URL to redirect to after a successful login.</summary>
    public string? ReturnUrl { get; set; }
}
