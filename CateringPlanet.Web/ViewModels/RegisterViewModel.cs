using System.ComponentModel.DataAnnotations;

namespace CateringPlanet.Web.ViewModels;

/// <summary>
/// Input model for the customer registration form.
/// </summary>
public class RegisterViewModel
{
    /// <summary>User's first name.</summary>
    [Required]
    [Display(Name = "First name")]
    public string FirstName { get; set; } = null!;

    /// <summary>User's last name.</summary>
    [Required]
    [Display(Name = "Last name")]
    public string LastName { get; set; } = null!;

    /// <summary>Contact phone number (optional).</summary>
    [RegularExpression(@"^(?:\+?[0-9\s()-]{6,20})?$", ErrorMessage = "Enter a valid phone number.")]
    [Display(Name = "Phone number")]
    public string? PhoneNumber { get; set; }

    /// <summary>Delivery address (optional).</summary>
    [Display(Name = "Address")]
    public string? Address { get; set; }

    /// <summary>User's e-mail address (doubles as the Identity user name).</summary>
    [Required]
    [EmailAddress]
    [Display(Name = "E-mail")]
    public string Email { get; set; } = null!;

    /// <summary>Password captured by the form.</summary>
    [Required]
    [DataType(DataType.Password)]
    [StringLength(100, MinimumLength = 6)]
    [Display(Name = "Password")]
    public string Password { get; set; } = null!;

    /// <summary>Password repetition used for validation.</summary>
    [DataType(DataType.Password)]
    [Compare(nameof(Password), ErrorMessage = "Passwords must match!")]
    [Display(Name = "Repeat password")]
    public string ConfirmPassword { get; set; } = null!;
}
