namespace Booking.Contracts.Auth;

/// <summary>
/// JWT token response for login/token endpoint.
/// </summary>
public record TokenResponse
{
    public required string Token { get; init; }
    public required DateTime ExpiresAt { get; init; }
}
