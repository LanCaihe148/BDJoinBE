
namespace BDJoinSN.Application.Contracts.Identity
{
    public interface ITokenBlacklistService
    {
        Task AddToBlacklistAsync(string token, DateTime expiry);
        Task<bool> IsTokenBlacklistedAsync(string token);
        Task InvalidateUserTokensAsync(string userId);
    }
}
