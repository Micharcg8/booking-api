namespace Booking.Contracts.Search;

/// <summary>
/// A single trip option returned by search.
/// </summary>
public record TripOptionDto
{
    public required string Id { get; init; }
    public required string Origin { get; init; }
    public required string Destination { get; init; }
    public required DateOnly DepartureDate { get; init; }
    public required decimal Price { get; init; }
    public string? TravelType { get; init; }
    public string? Provider { get; init; }
}
