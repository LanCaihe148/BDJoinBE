
using BDJoinSN.Application.Contracts.Persistance;
using BDJoinSN.Infrastructure.Persistance;
using Microsoft.EntityFrameworkCore;

namespace BDJoinSN.Infrastructure.Repositories
{
    public class UserInfoService : IUserInfoService
    {
        private readonly BDJoinDbContext _context;

        public UserInfoService(BDJoinDbContext context)
        {
            _context = context;
        }

        public async Task<(string UserId, string Username, string? DisplayName, string? AvatarUrl)> GetUserInfoAsync(string userId)
        {
            
            var profile = await _context.UserProfiles
                .FirstOrDefaultAsync(p => p.Id == userId);

            if (profile == null)
                return (userId, string.Empty, null, null);

            return (
                profile.Id,
                profile.UserName ?? string.Empty, 
                profile.DisplayName,
                profile.ProfileImageUrl
            );
        }
    }
}
