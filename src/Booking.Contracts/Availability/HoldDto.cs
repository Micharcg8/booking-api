namespace Booking.Contracts.Availability;

/// <summary>
/// Hold status in the workflow.
/// </summary>
public enum HoldStatus
{
    Active,
    Expired,
    Released,
    ConvertedToBooking
}

/// <summary>
/// Hold information returned by the API.
/// </summary>
public record HoldDto
{
    public required string HoldId { get; init; }
    public required string TripId { get; init; }
    public required DateTime ExpiresAt { get; init; }
    public required HoldStatus Status { get; init; }
}
