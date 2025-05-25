using Caker.Data;
using Microsoft.EntityFrameworkCore;

namespace Caker.Services
{
    public class ExpiredTokenCleaner(CakerDbContext context) : IHostedService
    {
        private readonly CakerDbContext _context = context;
        private Timer? _timer;

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _timer = new Timer(DoWork, null, TimeSpan.Zero, TimeSpan.FromDays(1));
            return Task.CompletedTask;
        }

        private async void DoWork(object? state)
        {
            var expiredTokens = await _context
                .RefreshTokens.Where(rt => rt.Expires < DateTime.UtcNow && rt.Revoked == null)
                .ToListAsync();

            foreach (var token in expiredTokens)
            {
                _context.RefreshTokens.Remove(token);
            }

            await _context.SaveChangesAsync();
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _timer?.Change(Timeout.Infinite, 0);
            return Task.CompletedTask;
        }
    }
}
