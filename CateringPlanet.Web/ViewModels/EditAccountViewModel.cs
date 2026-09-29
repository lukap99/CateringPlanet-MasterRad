using System.ComponentModel.DataAnnotations;

namespace CateringPlanet.Web.ViewModels;

/// <summary>
/// Model for the "edit account" page: names are editable,
/// e-mail and loyalty points are shown read-only.
/// </summary>
public class EditAccountViewModel
{
    /// <summary>Identity user id (hidden, never edited).</summary>
    public string Id { get; set; } = null!;

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

    /// <summary>Read-only e-mail display value.</summary>
    [Display(Name = "E-mail")]
    public string Email { get; set; } = null!;

    /// <summary>Read-only loyalty points display value.</summary>
    [Display(Name = "Points")]
    public int Points { get; set; }
}
