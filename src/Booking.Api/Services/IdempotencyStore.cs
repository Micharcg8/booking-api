namespace Booking.Api.Services;

/// <summary>
/// In-memory store for idempotency keys (e.g. payment callback). Prevents duplicate processing.
/// </summary>
public class IdempotencyStore
{
    private readonly HashSet<string> _processed = new();
    private readonly object _lock = new();

    public bool TryMarkProcessed(string key)
    {
        if (string.IsNullOrWhiteSpace(key)) return false;
        lock (_lock)
        {
            if (_processed.Contains(key))
                return false;
            _processed.Add(key);
            return true;
        }
    }

    public bool HasBeenProcessed(string key)
    {
        if (string.IsNullOrWhiteSpace(key)) return false;
        lock (_lock)
            return _processed.Contains(key);
    }
}
