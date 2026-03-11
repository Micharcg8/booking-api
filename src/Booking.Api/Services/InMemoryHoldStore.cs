using Booking.Contracts.Availability;

namespace Booking.Api.Services;

/// <summary>
/// In-memory store for holds (iteration 5). Replace with persistence (e.g. PostgreSQL + Redis TTL) later.
/// </summary>
public class InMemoryHoldStore
{
    private static readonly TimeSpan HoldDuration = TimeSpan.FromMinutes(15);
    private readonly Dictionary<string, HoldEntry> _holds = new();
    private readonly object _lock = new();

    public (string HoldId, DateTime ExpiresAt) Create(string tripId)
    {
        var holdId = Guid.NewGuid().ToString("N")[..12];
        var expiresAt = DateTime.UtcNow.Add(HoldDuration);
        lock (_lock)
        {
            _holds[holdId] = new HoldEntry { TripId = tripId, ExpiresAt = expiresAt, Status = HoldStatus.Active };
        }
        return (holdId, expiresAt);
    }

    public HoldDto? Get(string holdId)
    {
        lock (_lock)
        {
            if (!_holds.TryGetValue(holdId, out var entry))
                return null;

            var status = entry.Status;
            if (status == HoldStatus.Active && DateTime.UtcNow >= entry.ExpiresAt)
            {
                entry.Status = HoldStatus.Expired;
                status = HoldStatus.Expired;
            }

            return new HoldDto
            {
                HoldId = holdId,
                TripId = entry.TripId,
                ExpiresAt = entry.ExpiresAt,
                Status = status
            };
        }
    }

    public void Release(string holdId)
    {
        lock (_lock)
        {
            if (_holds.TryGetValue(holdId, out var entry) && entry.Status == HoldStatus.Active)
                entry.Status = HoldStatus.Released;
        }
    }

    public void MarkConvertedToBooking(string holdId)
    {
        lock (_lock)
        {
            if (_holds.TryGetValue(holdId, out var entry) && entry.Status == HoldStatus.Active)
            {
                entry.Status = HoldStatus.ConvertedToBooking;
            }
        }
    }

    private class HoldEntry
    {
        public required string TripId { get; init; }
        public DateTime ExpiresAt { get; init; }
        public HoldStatus Status { get; set; }
    }
}
