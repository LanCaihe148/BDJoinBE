

namespace BDJoinSN.Application.Contracts.Identity
{
    public interface IUserUpdateService
    {
        Task UpdateUserDisplayNameAsync(string userId, string displayName);
        Task UpdateUserProfileImageAsync(string userId, string imageUrl);
        Task<bool> UpdateUserInfoAsync(string userId, string displayName, string? profileImageUrl = null);
    }
}
