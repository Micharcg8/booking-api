using Booking.Contracts.Booking;

namespace Booking.Api.Services;

/// <summary>
/// In-memory store for payments (iteration 6). Replace with gateway/infrastructure later.
/// </summary>
public class InMemoryPaymentStore
{
    private readonly Dictionary<string, PaymentEntry> _payments = new();
    private readonly object _lock = new();

    public (string PaymentId, string RedirectUrl) CreatePending(string bookingId, decimal amount)
    {
        var paymentId = Guid.NewGuid().ToString("N")[..12];
        var redirectUrl = $"https://fake-payments.example.com/checkout/{paymentId}";

        lock (_lock)
        {
            _payments[paymentId] = new PaymentEntry
            {
                PaymentId = paymentId,
                BookingId = bookingId,
                Amount = amount,
                Status = PaymentStatus.Pending
            };
        }

        return (paymentId, redirectUrl);
    }

    public PaymentEntry? Get(string paymentId)
    {
        lock (_lock)
        {
            return _payments.TryGetValue(paymentId, out var entry) ? entry : null;
        }
    }

    public void UpdateStatus(string paymentId, PaymentStatus status)
    {
        lock (_lock)
        {
            if (_payments.TryGetValue(paymentId, out var entry))
            {
                entry.Status = status;
            }
        }
    }

    public class PaymentEntry
    {
        public required string PaymentId { get; init; }
        public required string BookingId { get; init; }
        public decimal Amount { get; init; }
        public PaymentStatus Status { get; set; }
    }
}

