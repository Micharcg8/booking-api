namespace Booking.Contracts.Availability;

/// <summary>
/// Response after creating a hold.
/// </summary>
public record CreateHoldResponse
{
    public required string HoldId { get; init; }
    public required string TripId { get; init; }
    public required DateTime ExpiresAt { get; init; }
    public HoldStatus Status { get; init; } = HoldStatus.Active;
}
