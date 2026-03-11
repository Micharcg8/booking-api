namespace Booking.Contracts.Search;

/// <summary>
/// Request parameters for trip search (query string or body).
/// </summary>
public record TripSearchRequest
{
    public string? Origin { get; init; }
    public string? Destination { get; init; }
    public DateOnly? DateFrom { get; init; }
    public DateOnly? DateTo { get; init; }
    public int? Passengers { get; init; }
    public decimal? MaxPrice { get; init; }
    public string? TravelType { get; init; }
    public string? Provider { get; init; }
}
