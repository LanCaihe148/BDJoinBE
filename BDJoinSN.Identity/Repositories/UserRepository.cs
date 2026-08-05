
using BDJoinSN.Application.Contracts.Persistance;
using BDJoinSN.Application.Models;
using BDJoinSN.Application.Models.Identity;
using BDJoinSN.Application.Models.Pagination;
using BDJoinSN.Identity.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace BDJoinSN.Identity.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IProfileRepository _profileRepository;

        public UserRepository(
            UserManager<ApplicationUser> userManager,
            IProfileRepository profileRepository)
        {
            _userManager = userManager;
            _profileRepository = profileRepository;
        }

        public async Task<bool> ExistsByUsernameAsync(string username)
        {
            var user = await _userManager.FindByNameAsync(username);
            return user != null && !user.IsDeleted;
        }

        public async Task<UserDto?> GetByUsernameAsync(string username)
        {
            
            var user = await _userManager.FindByNameAsync(username);
            if (user == null)
                return null;

            
            var profile = await _profileRepository.GetByUserIdAsync(user.Id);

            
            return new UserDto
            {
                Id = user.Id,
                Username = user.UserName ?? string.Empty,
                DisplayName = profile?.DisplayName ?? user.DisplayName ?? user.UserName,
                ProfileImageUrl = profile?.ProfileImageUrl ?? user.ProfileImageUrl,
                Name = profile?.Name,
                LastName = profile?.LastName,
                IsDeleted = user.IsDeleted
            };
        }

        public async Task<PaginatedResult<UserSearchResult>> SearchUsersAsync(
            string searchTerm,
            string currentUserId,
            int pageIndex = 1,
            int pageSize = 10)
        {

            var query = _userManager.Users
                .Where(u => u.Id != currentUserId);

            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                var term = searchTerm.ToLowerInvariant();
                query = query.Where(u =>
                    (u.UserName != null && u.UserName.ToLowerInvariant().Contains(term)) ||
                    (u.DisplayName != null && u.DisplayName.ToLowerInvariant().Contains(term))
                );
            }
            var totalCount = await query.CountAsync();

            var users = await query
                .Skip((pageIndex - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var results = new List<UserSearchResult>();
            foreach (var user in users)
            {
                var profile = await _profileRepository.GetByUserIdAsync(user.Id);

                results.Add(new UserSearchResult
                {
                    UserId = user.Id,
                    Username = user.UserName ?? string.Empty,
                    DisplayName = user.DisplayName,
                    Name = user.Name,
                    LastName = user.LastName,
                    Biography = profile?.Biography,
                    ProfileImageUrl = profile?.ProfileImageUrl,
                    IsDeleted = false 
                });
            }

            return new PaginatedResult<UserSearchResult>(
                results,        
                totalCount,     
                pageIndex,
                pageSize);
        }
    }
}
