using Microsoft.AspNetCore.Mvc;
using Booking.Contracts.Search;

namespace Booking.Api.Controllers;

/// <summary>
/// Trip search (Search / Catalog bounded context).
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class SearchController : ControllerBase
{
    /// <summary>
    /// Search travel options by origin, destination, dates, passengers, price, travel type, and provider.
    /// </summary>
    /// <param name="origin">Origin location code or name.</param>
    /// <param name="destination">Destination location code or name.</param>
    /// <param name="dateFrom">Earliest departure date.</param>
    /// <param name="dateTo">Latest departure date.</param>
    /// <param name="passengers">Number of passengers.</param>
    /// <param name="maxPrice">Maximum price filter.</param>
    /// <param name="travelType">Travel type (e.g. Flight, Hotel, Package).</param>
    /// <param name="provider">Provider filter.</param>
    /// <response code="200">List of trip options (may be empty).</response>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<TripOptionDto>), StatusCodes.Status200OK)]
    public ActionResult<IEnumerable<TripOptionDto>> Search(
        [FromQuery] string? origin = null,
        [FromQuery] string? destination = null,
        [FromQuery] DateOnly? dateFrom = null,
        [FromQuery] DateOnly? dateTo = null,
        [FromQuery] int? passengers = null,
        [FromQuery] decimal? maxPrice = null,
        [FromQuery] string? travelType = null,
        [FromQuery] string? provider = null)
    {
        var request = new TripSearchRequest
        {
            Origin = origin,
            Destination = destination,
            DateFrom = dateFrom,
            DateTo = dateTo,
            Passengers = passengers,
            MaxPrice = maxPrice,
            TravelType = travelType,
            Provider = provider
        };

        var results = GetFakeSearchResults(request);
        return Ok(results);
    }

    private static List<TripOptionDto> GetFakeSearchResults(TripSearchRequest request)
    {
        var list = new List<TripOptionDto>
        {
            new TripOptionDto
            {
                Id = "trip-1",
                Origin = request.Origin ?? "MAD",
                Destination = request.Destination ?? "BCN",
                DepartureDate = request.DateFrom ?? DateOnly.FromDateTime(DateTime.UtcNow.AddDays(7)),
                Price = 89.50m,
                TravelType = request.TravelType ?? "Flight",
                Provider = request.Provider ?? "Demo"
            },
            new TripOptionDto
            {
                Id = "trip-2",
                Origin = request.Origin ?? "MAD",
                Destination = request.Destination ?? "BCN",
                DepartureDate = (request.DateFrom ?? DateOnly.FromDateTime(DateTime.UtcNow.AddDays(7))).AddDays(1),
                Price = 72.00m,
                TravelType = request.TravelType ?? "Flight",
                Provider = request.Provider ?? "Demo"
            },
            new TripOptionDto
            {
                Id = "trip-3",
                Origin = request.Origin ?? "MAD",
                Destination = request.Destination ?? "BCN",
                DepartureDate = (request.DateFrom ?? DateOnly.FromDateTime(DateTime.UtcNow.AddDays(7))).AddDays(2),
                Price = 105.00m,
                TravelType = request.TravelType ?? "Flight",
                Provider = request.Provider ?? "Demo"
            }
        };

        if (request.MaxPrice.HasValue)
            list = list.Where(t => t.Price <= request.MaxPrice!.Value).ToList();

        return list;
    }
}
