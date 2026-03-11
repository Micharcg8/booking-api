using Microsoft.AspNetCore.Mvc;
using Booking.Contracts.Availability;
using Booking.Api.Services;

namespace Booking.Api.Controllers;

/// <summary>
/// Create and query temporary holds (Availability bounded context).
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class HoldsController : ControllerBase
{
    private readonly InMemoryHoldStore _store;

    public HoldsController(InMemoryHoldStore store)
    {
        _store = store;
    }

    /// <summary>
    /// Create a temporary hold on a trip option. Hold expires after 15 minutes.
    /// </summary>
    /// <response code="201">Hold created.</response>
    /// <response code="400">Invalid request.</response>
    [HttpPost]
    [ProducesResponseType(typeof(CreateHoldResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public ActionResult<CreateHoldResponse> Create([FromBody] CreateHoldRequest request)
    {
        if (request == null || string.IsNullOrWhiteSpace(request.TripId))
            return BadRequest("TripId is required");

        var (holdId, expiresAt) = _store.Create(request.TripId);
        var response = new CreateHoldResponse
        {
            HoldId = holdId,
            TripId = request.TripId,
            ExpiresAt = expiresAt,
            Status = HoldStatus.Active
        };
        return CreatedAtAction(nameof(Get), new { holdId }, response);
    }

    /// <summary>
    /// Get hold details. Status is updated to Expired if past ExpiresAt.
    /// </summary>
    /// <param name="holdId">Hold id returned from create.</param>
    /// <response code="200">Hold details.</response>
    /// <response code="404">Hold not found.</response>
    [HttpGet("{holdId}")]
    [ProducesResponseType(typeof(HoldDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public ActionResult<HoldDto> Get(string holdId)
    {
        var hold = _store.Get(holdId);
        if (hold == null)
            return NotFound();
        return Ok(hold);
    }

    /// <summary>
    /// Release a hold before expiration.
    /// </summary>
    [HttpDelete("{holdId}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public IActionResult Release(string holdId)
    {
        var hold = _store.Get(holdId);
        if (hold == null)
            return NotFound();
        _store.Release(holdId);
        return NoContent();
    }
}
