using Booking.Api.Services;
using Booking.Contracts.Booking;
using Booking.Contracts.Payments;
using Microsoft.AspNetCore.Mvc;

namespace Booking.Api.Controllers;

/// <summary>
/// Simulated payment endpoints (Payment bounded context).
/// </summary>
[ApiController]
[Route("api/payments")]
[Produces("application/json")]
public class PaymentsController : ControllerBase
{
    private readonly InMemoryPaymentStore _paymentStore;
    private readonly InMemoryBookingStore _bookingStore;
    private readonly InMemoryHoldStore _holdStore;

    public PaymentsController(
        InMemoryPaymentStore paymentStore,
        InMemoryBookingStore bookingStore,
        InMemoryHoldStore holdStore)
    {
        _paymentStore = paymentStore;
        _bookingStore = bookingStore;
        _holdStore = holdStore;
    }

    /// <summary>
    /// Initiate payment for a booking.
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(InitiatePaymentResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public ActionResult<InitiatePaymentResponse> Initiate([FromBody] InitiatePaymentRequest request)
    {
        if (request == null || string.IsNullOrWhiteSpace(request.BookingId))
        {
            return BadRequest("BookingId is required.");
        }

        var booking = _bookingStore.Get(request.BookingId);
        if (booking == null || booking.Status != BookingStatus.Pending)
        {
            return BadRequest("Booking must exist and be in Pending state.");
        }

        var (paymentId, redirectUrl) = _paymentStore.CreatePending(request.BookingId, request.Amount);

        // Reflect that payment has started
        _bookingStore.UpdateStatus(booking.BookingId, BookingStatus.Pending, PaymentStatus.Pending);

        var response = new InitiatePaymentResponse
        {
            PaymentId = paymentId,
            BookingId = booking.BookingId,
            Status = PaymentStatus.Pending,
            RedirectUrl = redirectUrl
        };

        return CreatedAtAction(nameof(Callback), new { paymentId, status = "Succeeded" }, response);
    }

    /// <summary>
    /// Simulated payment callback/webhook from the payment provider.
    /// </summary>
    /// <param name="paymentId">Payment identifier.</param>
    /// <param name="status">Result status: Succeeded or Failed.</param>
    [HttpPost("{paymentId}/callback")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public IActionResult Callback(string paymentId, [FromQuery] string status)
    {
        var entry = _paymentStore.Get(paymentId);
        if (entry is null)
        {
            return NotFound();
        }

        if (!Enum.TryParse<PaymentStatus>(status, ignoreCase: true, out var paymentStatus) ||
            (paymentStatus != PaymentStatus.Succeeded && paymentStatus != PaymentStatus.Failed))
        {
            return BadRequest("Status must be Succeeded or Failed.");
        }

        _paymentStore.UpdateStatus(paymentId, paymentStatus);

        var booking = _bookingStore.Get(entry.BookingId);
        if (booking == null)
        {
            return NotFound();
        }

        if (paymentStatus == PaymentStatus.Succeeded)
        {
            _bookingStore.UpdateStatus(booking.BookingId, BookingStatus.Confirmed, PaymentStatus.Succeeded);
            _holdStore.MarkConvertedToBooking(booking.HoldId);
        }
        else
        {
            _bookingStore.UpdateStatus(booking.BookingId, BookingStatus.Cancelled, PaymentStatus.Failed);
            _holdStore.Release(booking.HoldId);
        }

        return NoContent();
    }
}

