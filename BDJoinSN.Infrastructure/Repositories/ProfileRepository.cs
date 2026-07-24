
using BDJoinSN.Application.Contracts.Persistance;
using BDJoinSN.Domain;
using BDJoinSN.Identity.Models;
using BDJoinSN.Infrastructure.Persistance;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace BDJoinSN.Infrastructure.Repositories
{
    public class ProfileRepository : RepositoryBase<UserProfile, string>, IProfileRepository
    {

        private readonly UserManager<UserProfile> _userManager;
        public ProfileRepository(BDJoinDbContext context) : base(context)
        {
        }

        public async Task<UserProfile?> GetByUserIdAsync(string userId)
        {
            return await _context.Set<UserProfile>()
                .FirstOrDefaultAsync(p => p.Id == userId);
        }

        //public async Task<UserProfile?> GetByUsernameAsync(string username)
        //{
        //    var user = await _userManager.FindByNameAsync(username);
        //    if (user == null) return null;

        //    // Luego buscar el perfil por el Id del usuario
        //    return await _context.Set<UserProfile>()
        //        .FirstOrDefaultAsync(p => p.Id == user.Id);
        //}

        public async Task<bool> ExistsByUserIdAsync(string userId)
        {
            return await _context.Set<UserProfile>()
                .AnyAsync(p => p.Id == userId);
        }

        //public async Task<bool> ExistsByUsernameAsync(string userId)
        //{
        //    return await _context.Set<UserProfile>()
        //        .AnyAsync(p => p.Id == userId);
        //}


        public async Task UpdateProfileImageAsync(string userId, string imageUrl)
        {
            var profile = await GetByUserIdAsync(userId);
            if (profile != null)
            {
                profile.ProfileImageUrl = imageUrl;
                profile.UpdatedAt = DateTime.UtcNow;
                await _context.SaveChangesAsync();
            }
        }

        public async Task UpdateDisplayNameAsync(string userId, string displayName)
        {
            var profile = await GetByUserIdAsync(userId);
            if (profile != null)
            {
                profile.DisplayName = displayName;
                profile.UpdatedAt = DateTime.UtcNow;
                await _context.SaveChangesAsync();
            }
        }
    }
}
