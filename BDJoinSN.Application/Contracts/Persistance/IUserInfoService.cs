

namespace BDJoinSN.Application.Contracts.Persistance
{
    public interface IUserInfoService
    {
        Task<(string UserId, string Username, string? DisplayName, string? AvatarUrl)>
            GetUserInfoAsync(string userId);
    }
}
