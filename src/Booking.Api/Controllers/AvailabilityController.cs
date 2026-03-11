using Microsoft.AspNetCore.Mvc;
using Booking.Contracts.Availability;

namespace Booking.Api.Controllers;

/// <summary>
/// Check availability for trip options (Availability bounded context).
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class AvailabilityController : ControllerBase
{
    /// <summary>
    /// Check if a trip option is still available.
    /// </summary>
    /// <param name="tripId">Trip option id from search.</param>
    /// <response code="200">Availability result.</response>
    [HttpGet]
    [ProducesResponseType(typeof(AvailabilityCheckResponse), StatusCodes.Status200OK)]
    public ActionResult<AvailabilityCheckResponse> Check([FromQuery] string tripId)
    {
        if (string.IsNullOrWhiteSpace(tripId))
            return BadRequest("tripId is required");

        return Ok(new AvailabilityCheckResponse
        {
            TripId = tripId,
            Available = true
        });
    }
}
