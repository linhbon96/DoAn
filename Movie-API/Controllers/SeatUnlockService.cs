using MovieBookingApp.Data;

public class SeatUnlockService : IHostedService, IDisposable
{
    private Timer _timer;
    private readonly IServiceScopeFactory _scopeFactory;

    public SeatUnlockService(IServiceScopeFactory scopeFactory)
    {
        _scopeFactory = scopeFactory;
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        _timer = new Timer(UnlockExpiredSeats, null, TimeSpan.Zero, TimeSpan.FromSeconds(10)); // Tiến hành 10s 1 lần
        return Task.CompletedTask;
    }

    private async void UnlockExpiredSeats(object state)
    {
        using var scope = _scopeFactory.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        var expiredSeats = context.Seats.Where(s => s.LockedUntil.HasValue && s.LockedUntil < DateTime.UtcNow);
        foreach (var seat in expiredSeats)
        {
            seat.LockedUntil = null; // Bỏ khóa ghế
        }
        await context.SaveChangesAsync();
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        _timer?.Change(Timeout.Infinite, 0);
        return Task.CompletedTask;
    }

    public void Dispose()
    {
        _timer?.Dispose();
    }
}