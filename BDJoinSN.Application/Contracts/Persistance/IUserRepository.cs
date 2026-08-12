

using BDJoinSN.Application.Features.Users.Commands.UpdateProfilesCommand;
using BDJoinSN.Application.Models;
using BDJoinSN.Application.Models.Identity;
using BDJoinSN.Application.Models.Pagination;

namespace BDJoinSN.Application.Contracts.Persistance
{
    public interface IUserRepository
    {
        Task<PaginatedResult<UserSearchResult>> SearchUsersAsync(
            string searchTerm,
            string currentUserId,
            int pageIndex = 1,
            int pageSize = 10);

        Task<UserDto?> GetByUsernameAsync(string username);
        Task<bool> UpdateAppUser(UpdateUserProfileCommand request);
        Task<bool> ExistsByUsernameAsync(string username);
    }
}
