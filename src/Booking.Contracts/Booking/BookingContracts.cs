namespace Booking.Contracts.Booking;

/// <summary>
/// Overall booking status in the workflow.
/// </summary>
public enum BookingStatus
{
    Pending,
    Confirmed,
    Cancelled
}

/// <summary>
/// Status of the associated payment.
/// </summary>
public enum PaymentStatus
{
    Pending,
    Succeeded,
    Failed
}

/// <summary>
/// Request to create a booking in Pending state.
/// </summary>
public record CreateBookingRequest
{
    public required string TripId { get; init; }
    public required string HoldId { get; init; }
    public required string CustomerName { get; init; }
    public required string CustomerEmail { get; init; }
}

/// <summary>
/// Response after creating a booking.
/// </summary>
public record CreateBookingResponse
{
    public required string BookingId { get; init; }
    public required BookingStatus Status { get; init; }
}

/// <summary>
/// Booking details with payment information.
/// </summary>
public record BookingDto
{
    public required string BookingId { get; init; }
    public required string TripId { get; init; }
    public required string HoldId { get; init; }
    public required BookingStatus Status { get; init; }
    public PaymentStatus PaymentStatus { get; init; }
}

