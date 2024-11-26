using MovieBookingApp.Data;

public class SeatLockService
{
    private readonly Dictionary<int, DateTime> _seatLocks = new();

    public bool LockSeat(int seatId)
    {
        if (_seatLocks.TryGetValue(seatId, out var lockExpiration) && lockExpiration > DateTime.UtcNow)
        {
            return false; 
            
        }

        _seatLocks[seatId] = DateTime.UtcNow.AddMinutes(5);
        return true;
    }

    public void UnlockSeat(int seatId)
    {
        _seatLocks.Remove(seatId);
    }

    public bool IsSeatLocked(int seatId)
    {
        return _seatLocks.TryGetValue(seatId, out var lockExpiration) && lockExpiration > DateTime.UtcNow;
    }

    public void CleanupExpiredLocks()
    {
        var expiredSeats = _seatLocks
            .Where(kv => kv.Value <= DateTime.UtcNow)
            .Select(kv => kv.Key)
            .ToList();

        foreach (var seatId in expiredSeats)
        {
            _seatLocks.Remove(seatId);
        }
    }
}
