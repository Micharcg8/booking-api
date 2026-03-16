namespace Booking.Contracts.Cities;

/// <summary>
/// City suggestion for search autocomplete.
/// </summary>
public record CityDto
{
    public required string Code { get; init; }
    public required string Name { get; init; }
}
