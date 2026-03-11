using Booking.Contracts.Booking;

namespace Booking.Api.Services;

/// <summary>
/// In-memory store for bookings (iteration 6). Replace with proper persistence later.
/// </summary>
public class InMemoryBookingStore
{
    private readonly Dictionary<string, BookingEntry> _bookings = new();
    private readonly object _lock = new();

    public BookingDto CreatePending(string tripId, string holdId)
    {
        var bookingId = Guid.NewGuid().ToString("N")[..12];

        lock (_lock)
        {
            _bookings[bookingId] = new BookingEntry
            {
                BookingId = bookingId,
                TripId = tripId,
                HoldId = holdId,
                Status = BookingStatus.Pending,
                PaymentStatus = PaymentStatus.Pending
            };
        }

        return MapToDto(_bookings[bookingId]);
    }

    public BookingDto? Get(string bookingId)
    {
        lock (_lock)
        {
            if (!_bookings.TryGetValue(bookingId, out var entry))
                return null;

            return MapToDto(entry);
        }
    }

    public void UpdateStatus(string bookingId, BookingStatus status, PaymentStatus? paymentStatus = null)
    {
        lock (_lock)
        {
            if (_bookings.TryGetValue(bookingId, out var entry))
            {
                entry.Status = status;
                if (paymentStatus.HasValue)
                {
                    entry.PaymentStatus = paymentStatus.Value;
                }
            }
        }
    }

    private static BookingDto MapToDto(BookingEntry entry) =>
        new()
        {
            BookingId = entry.BookingId,
            TripId = entry.TripId,
            HoldId = entry.HoldId,
            Status = entry.Status,
            PaymentStatus = entry.PaymentStatus
        };

    private class BookingEntry
    {
        public required string BookingId { get; init; }
        public required string TripId { get; init; }
        public required string HoldId { get; init; }
        public BookingStatus Status { get; set; }
        public PaymentStatus PaymentStatus { get; set; }
    }
}

