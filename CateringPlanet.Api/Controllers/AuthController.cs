using CateringPlanet.Api.DTOs;
using CateringPlanet.Api.Security;
using CateringPlanet.Application.Abstractions;
using Microsoft.AspNetCore.Mvc;

namespace CateringPlanet.Api.Controllers;

/// <summary>
/// Authentication endpoints for API clients (JWT bearer tokens).
/// </summary>
[Route("api/auth")]
[ApiController]
public class AuthController : ControllerBase
{
    private readonly IAccountService _accountService;
    private readonly JwtTokenService _jwtTokenService;

    public AuthController(IAccountService accountService, JwtTokenService jwtTokenService)
    {
        _accountService = accountService;
        _jwtTokenService = jwtTokenService;
    }

    /// <summary>Validates e-mail/password credentials and returns a JWT bearer token.</summary>
    /// <param name="request">Login credentials (e-mail and password).</param>
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequestDto request)
    {
        var user = await _accountService.ValidateCredentialsAsync(request.Email, request.Password);
        if (user is null)
        {
            return Unauthorized(new { message = "Invalid username or password." });
        }

        var roles = await _accountService.GetRolesAsync(user.Id);
        var (token, expiresAt) = _jwtTokenService.CreateToken(user.Id, user.Email, user.FullName, roles);

        return Ok(new LoginResponseDto
        {
            Token = token,
            ExpiresAt = expiresAt,
            Email = user.Email,
            FullName = user.FullName,
            Roles = roles.ToList()
        });
    }
}
