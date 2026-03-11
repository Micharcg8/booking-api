using Booking.Api.Services;
using Booking.Contracts.Availability;
using Booking.Contracts.Booking;
using Microsoft.AspNetCore.Mvc;

namespace Booking.Api.Controllers;

/// <summary>
/// Manage bookings (Booking bounded context).
/// </summary>
[ApiController]
[Route("api/bookings")]
[Produces("application/json")]
public class BookingController : ControllerBase
{
    private readonly InMemoryBookingStore _bookingStore;
    private readonly InMemoryHoldStore _holdStore;

    public BookingController(InMemoryBookingStore bookingStore, InMemoryHoldStore holdStore)
    {
        _bookingStore = bookingStore;
        _holdStore = holdStore;
    }

    /// <summary>
    /// Create a booking in Pending state for a given trip and hold.
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(CreateBookingResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public ActionResult<CreateBookingResponse> Create([FromBody] CreateBookingRequest request)
    {
        if (request == null ||
            string.IsNullOrWhiteSpace(request.TripId) ||
            string.IsNullOrWhiteSpace(request.HoldId) ||
            string.IsNullOrWhiteSpace(request.CustomerName) ||
            string.IsNullOrWhiteSpace(request.CustomerEmail))
        {
            return BadRequest("TripId, HoldId, CustomerName and CustomerEmail are required.");
        }

        var hold = _holdStore.Get(request.HoldId);
        if (hold == null || hold.Status != HoldStatus.Active)
        {
            return BadRequest("Hold is not valid or has expired.");
        }

        var booking = _bookingStore.CreatePending(request.TripId, request.HoldId);

        var response = new CreateBookingResponse
        {
            BookingId = booking.BookingId,
            Status = booking.Status
        };

        return CreatedAtAction(nameof(GetById), new { bookingId = booking.BookingId }, response);
    }

    /// <summary>
    /// Get booking details including current status.
    /// </summary>
    [HttpGet("{bookingId}")]
    [ProducesResponseType(typeof(BookingDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public ActionResult<BookingDto> GetById(string bookingId)
    {
        var booking = _bookingStore.Get(bookingId);
        if (booking == null)
        {
            return NotFound();
        }

        return Ok(booking);
    }
}

