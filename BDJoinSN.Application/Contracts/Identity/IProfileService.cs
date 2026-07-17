
using BDJoinSN.Application.Models;

namespace BDJoinSN.Application.Contracts.Identity
{
    public interface IProfileService
    {
        public Task<ProfileResponse> GetOwnProfileAsync(string userId);

        public Task<PublicProfileResponse> GetPublicProfileAsync(string username, string? currentUserId = null);


    }
}
