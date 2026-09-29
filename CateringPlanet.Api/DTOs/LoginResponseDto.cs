namespace CateringPlanet.Api.DTOs;

/// <summary>
/// Successful login result: JWT bearer token plus a user summary.
/// </summary>
public class LoginResponseDto
{
    /// <summary>JWT bearer token to send in the Authorization header.</summary>
    public string Token { get; set; } = null!;

    /// <summary>Token expiry date and time (UTC).</summary>
    public DateTime ExpiresAt { get; set; }

    /// <summary>E-mail of the authenticated user.</summary>
    public string Email { get; set; } = null!;

    /// <summary>Display name of the authenticated user.</summary>
    public string FullName { get; set; } = null!;

    /// <summary>Roles assigned to the user.</summary>
    public IEnumerable<string> Roles { get; set; } = new List<string>();
}
