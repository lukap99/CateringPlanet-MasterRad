using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace CateringPlanet.Api.Security;

/// <summary>
/// Creates signed JWT bearer tokens from the <c>Jwt</c> configuration section.
/// </summary>
public class JwtTokenService
{
    private readonly IConfiguration _configuration;

    public JwtTokenService(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    /// <summary>
    /// Builds a signed HS256 token carrying the user's identity claims
    /// (sub, e-mail, full name) and one role claim per role.
    /// </summary>
    /// <param name="userId">Identity user id (sub claim).</param>
    /// <param name="email">User e-mail (email claim).</param>
    /// <param name="fullName">Display name ("FirstName LastName").</param>
    /// <param name="roles">Roles to embed as role claims.</param>
    /// <returns>The serialized JWT and its UTC expiry time.</returns>
    public (string Token, DateTime ExpiresAt) CreateToken(string userId, string email, string fullName, IEnumerable<string> roles)
    {
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]!));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var expiryMinutes = double.TryParse(_configuration["Jwt:ExpiryMinutes"], out var minutes) ? minutes : 120;
        var expiresAt = DateTime.UtcNow.AddMinutes(expiryMinutes);

        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, userId),
            new(JwtRegisteredClaimNames.Email, email),
            new("name", fullName),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };
        claims.AddRange(roles.Select(role => new Claim(ClaimTypes.Role, role)));

        var token = new JwtSecurityToken(
            issuer: _configuration["Jwt:Issuer"],
            audience: _configuration["Jwt:Audience"],
            claims: claims,
            expires: expiresAt,
            signingCredentials: credentials);

        return (new JwtSecurityTokenHandler().WriteToken(token), expiresAt);
    }
}
