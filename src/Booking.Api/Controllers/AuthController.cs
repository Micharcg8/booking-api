using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Booking.Contracts.Auth;

namespace Booking.Api.Controllers;

/// <summary>
/// Authentication (token issuance for iteration 7; no real user store yet).
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class AuthController : ControllerBase
{
    private readonly IConfiguration _config;

    public AuthController(IConfiguration config)
    {
        _config = config;
    }

    /// <summary>
    /// Issue a JWT for development/testing. In production, validate credentials against a user store.
    /// </summary>
    [HttpPost("token")]
    [ProducesResponseType(typeof(TokenResponse), StatusCodes.Status200OK)]
    public ActionResult<TokenResponse> Token()
    {
        var key = _config["Jwt:Key"];
        var issuer = _config["Jwt:Issuer"] ?? "booking-api";
        var audience = _config["Jwt:Audience"] ?? "booking-fe";
        var expMinutes = int.TryParse(_config["Jwt:ExpirationMinutes"], out var m) ? m : 60;

        if (string.IsNullOrEmpty(key) || key.Length < 32)
        {
            return StatusCode(500, "Jwt:Key must be at least 32 characters.");
        }

        var keyBytes = Encoding.UTF8.GetBytes(key);
        var creds = new SigningCredentials(
            new SymmetricSecurityKey(keyBytes),
            SecurityAlgorithms.HmacSha256);

        var expires = DateTime.UtcNow.AddMinutes(expMinutes);
        var token = new JwtSecurityToken(
            issuer: issuer,
            audience: audience,
            claims: new[]
            {
                new Claim(ClaimTypes.NameIdentifier, "dev-user"),
                new Claim(ClaimTypes.Name, "Test User"),
                new Claim(ClaimTypes.Role, "User")
            },
            expires: expires,
            signingCredentials: creds);

        var tokenString = new JwtSecurityTokenHandler().WriteToken(token);

        return Ok(new TokenResponse
        {
            Token = tokenString,
            ExpiresAt = expires
        });
    }
}
