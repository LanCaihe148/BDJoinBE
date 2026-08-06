
using BDJoinSN.Application.Models;
using BDJoinSN.Domain;

namespace BDJoinSN.Application.Contracts.Persistance
{
    public interface IProfileRepository : IAsyncRepository<UserProfile, string>
    {
        Task<UserProfile?> GetByUserIdAsync(string userId);
        Task<bool> ExistsByUserIdAsync(string userId);
        Task UpdateProfileImageAsync(string userId, string imageUrl);
        Task UpdateDisplayNameAsync(string userId, string displayName);

        Task<UserInfoDto> GetUserInfoByIdAsync(string userId);

    }
}
