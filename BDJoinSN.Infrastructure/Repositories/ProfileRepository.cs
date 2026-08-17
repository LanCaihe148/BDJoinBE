
using System.Diagnostics;
using BDJoinSN.Application.Contracts.Persistance;
using BDJoinSN.Application.Exceptions;
using BDJoinSN.Application.Models;
using BDJoinSN.Domain;
using BDJoinSN.Identity.Models;
using BDJoinSN.Infrastructure.Persistance;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace BDJoinSN.Infrastructure.Repositories
{
    public class ProfileRepository : RepositoryBase<UserProfile, string>, IProfileRepository
    {

        
        public ProfileRepository(BDJoinDbContext context) : base(context)
        {
           
        }

        public async Task<UserProfile?> GetByUserIdAsync(string userId)
        {
            return await _context.Set<UserProfile>()
                .FirstOrDefaultAsync(p => p.Id == userId);
        }

        

        public async Task<bool> ExistsByUserIdAsync(string userId)
        {
            return await _context.Set<UserProfile>()
                .AnyAsync(p => p.Id == userId);
        }




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

        public async Task<UserInfoDto> GetUserInfoByIdAsync(string userId)
        {
            var profile = await _context.UserProfiles.FirstOrDefaultAsync(p => p.Id == userId);

            if(profile == null)
            {
                return null;
            }

            return new UserInfoDto
            {
                Id = profile.Id,
                Username = profile.UserName ?? string.Empty,
                DisplayName = profile.DisplayName ?? profile.UserName,
                ProfileImageUrl = profile.ProfileImageUrl
            };
        }

        public async Task<bool> DeleteProfile(string userId)
        {
             
            if (string.IsNullOrEmpty(userId)){
                throw new BadRequestException("El ID del usuario no puede estar vacío");
            }
            
            var profile = await _context.UserProfiles
                .FirstOrDefaultAsync(p => p.Id == userId);
            
            
            if (profile != null)
            {
                _context.UserProfiles.Remove(profile);
                await _context.SaveChangesAsync();
                return true;
            }

            return false;
        }
    }
}
