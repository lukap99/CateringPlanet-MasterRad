using System.ComponentModel.DataAnnotations;

namespace CateringPlanet.Api.DTOs;

/// <summary>
/// Credentials posted to the login endpoint.
/// </summary>
public class LoginRequestDto
{
    /// <summary>User's registered e-mail address.</summary>
    [Required]
    [EmailAddress]
    public string Email { get; set; } = null!;

    /// <summary>User's password.</summary>
    [Required]
    public string Password { get; set; } = null!;
}
