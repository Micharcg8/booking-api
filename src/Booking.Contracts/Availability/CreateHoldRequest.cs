namespace Booking.Contracts.Availability;

/// <summary>
/// Request to create a temporary hold on a trip option.
/// </summary>
public record CreateHoldRequest
{
    public required string TripId { get; init; }
    public int Passengers { get; init; } = 1;
}
