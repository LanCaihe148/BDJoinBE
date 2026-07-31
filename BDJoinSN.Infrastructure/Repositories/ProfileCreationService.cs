using BDJoinSN.Application.Contracts.Persistance;
using BDJoinSN.Domain;
using BDJoinSN.Infrastructure.Persistance;
using Microsoft.EntityFrameworkCore;

namespace BDJoinSN.Infrastructure.Repositories
{
    public class ProfileCreationService : IProfileCreationService
    {
        private readonly BDJoinDbContext _context;

        public ProfileCreationService(BDJoinDbContext context)
        {
            _context = context;
        }

        public async Task CreateProfileAsync(string userId, string name, string lastName, string displayName, string userName)
        {
            var existingProfile = await _context.UserProfiles
       .FirstOrDefaultAsync(p => p.Id == userId);

            if (existingProfile != null)
            {
               
                existingProfile.Name = name;
                existingProfile.LastName = lastName;
                existingProfile.UserName = userName;
                existingProfile.DisplayName = displayName ?? $"{name} {lastName}";
                existingProfile.UpdatedAt = DateTime.UtcNow;

                _context.UserProfiles.Update(existingProfile);
                await _context.SaveChangesAsync();
                return;
            }

            
            var profile = new UserProfile
            {
                Id = userId,
                Name = name,
                LastName = lastName,
                UserName = userName,
                DisplayName = displayName ?? $"{name} {lastName}",
                CreatedAt = DateTime.UtcNow
            };

            await _context.UserProfiles.AddAsync(profile);
            await _context.SaveChangesAsync();
        }
    }
}
