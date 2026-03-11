using Booking.Contracts.Booking;

namespace Booking.Contracts.Payments;

/// <summary>
/// Request to initiate payment for a booking.
/// </summary>
public record InitiatePaymentRequest
{
    public required string BookingId { get; init; }
    public decimal Amount { get; init; }
}

/// <summary>
/// Response after initiating payment.
/// </summary>
public record InitiatePaymentResponse
{
    public required string PaymentId { get; init; }
    public required string BookingId { get; init; }
    public required PaymentStatus Status { get; init; }
    public required string RedirectUrl { get; init; }
}

