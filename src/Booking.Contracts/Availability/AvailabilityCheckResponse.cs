namespace Booking.Contracts.Availability;

/// <summary>
/// Result of an availability check for a trip option.
/// </summary>
public record AvailabilityCheckResponse
{
    public required string TripId { get; init; }
    public required bool Available { get; init; }
}
