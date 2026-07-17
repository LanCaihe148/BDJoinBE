using BDJoinSN.Application.Contracts;
using BDJoinSN.Domain;
using BDJoinSN.Infrastructure.Persistance;

namespace BDJoinSN.Infrastructure.Repositories
{
    public class ProfileCreationService : IProfileCreationService
    {
        private readonly BDJoinDbContext _context;

        public ProfileCreationService(BDJoinDbContext context)
        {
            _context = context;
        }

        public async Task CreateProfileAsync(string userId, string name, string lastName)
        {
            var profile = new UserProfile
            {
                Id = userId,
                Name = name,
                LastName = lastName,
                CreatedAt = DateTime.UtcNow
            };

            await _context.UserProfiles.AddAsync(profile);
            await _context.SaveChangesAsync();
        }
    }
}
