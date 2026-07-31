using BDJoinSN.Application.Contracts.Identity;
using System.Collections.Concurrent;

namespace BDJoinSN.Identity.Services
{
    public class TokenBlacklistService : ITokenBlacklistService
    {
        private static readonly ConcurrentDictionary<string, DateTime> _blacklist = new();

        public Task AddToBlacklistAsync(string token, DateTime expiry)
        {
            _blacklist.TryAdd(token, expiry);
            return Task.CompletedTask;
        }

        public async Task InvalidateUserTokensAsync(string userId)
        {
            //_logger.LogInformation($"Tokens invalidados para usuario {userId}");
        }

        public Task<bool> IsTokenBlacklistedAsync(string token)
        {
            if(_blacklist.TryGetValue(token, out var expiry)){
                if(expiry < DateTime.UtcNow)
                {
                    _blacklist.TryRemove(token, out _);
                    return Task.FromResult(false);
                }
                return Task.FromResult(true);
            }
            return Task.FromResult(false);
        }
    }
}
