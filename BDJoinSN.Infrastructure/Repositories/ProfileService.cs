

using BDJoinSN.Application.Contracts.Identity;
using BDJoinSN.Application.Exceptions;
using BDJoinSN.Application.Models;
using BDJoinSN.Domain;
using BDJoinSN.Identity.Models;
using BDJoinSN.Infrastructure.Persistance;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace BDJoinSN.Infrastructure.Repositories
{
    public class ProfileService : IProfileService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly BDJoinDbContext _appDbContext;

        public ProfileService(UserManager<ApplicationUser> userManager, BDJoinDbContext appDbContext)
        {
            _userManager = userManager;
            _appDbContext = appDbContext;
        }

        public async Task<ProfileResponse> GetOwnProfileAsync(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
                throw new NotFoundException(nameof(ApplicationUser), userId);

            var profile = await _appDbContext.UserProfiles
                .FirstOrDefaultAsync(up => up.Id == userId);

            var friends = await _appDbContext.FriendRequests
                .Where(fr =>
                    (fr.SenderId == user.Id || fr.ReceiverId == user.Id) &&
                    fr.Status == FriendRequestStatus.Accepted)
                .Select(fr => fr.SenderId == user.Id ? fr.Receiver : fr.Sender)
                .ToListAsync();

            var friendsCount = friends.Count;
            var recentFriends = friends
                .Take(5)
                .Select(f => new FriendSummaryResponse
                {
                    UserName = _userManager.FindByIdAsync(f.Id).Result?.UserName ?? string.Empty,
                    Name = f.Name,
                    ProfileImageUrl = f.ProfileImageUrl
                })
                .ToList();
            return new ProfileResponse
            {
                UserId = user.Id,
                UserName = user.UserName ?? string.Empty,
                Email = user.Email ?? string.Empty,
                Name = profile?.Name,
                LastName = profile?.LastName,
                Biography = profile?.Biography,
                Birthday = profile?.Birthday,
                City = profile?.Location,
                FriendsCount = friendsCount,
                ProfileImageUrl = profile?.ProfileImageUrl,
                RecentFriends = recentFriends,
                CreatedAt = profile?.CreatedAt ?? DateTime.UtcNow
            };
        }

        public async Task<PublicProfileResponse> GetPublicProfileAsync(string username, string? currentUserId = null)
        {

            var user = await _userManager.FindByNameAsync(username);
            if (user == null)
                throw new NotFoundException("Username", username);

            var profile = await _appDbContext.UserProfiles
                .FirstOrDefaultAsync(up => up.Id == user.Id);

           
            var friends = await _appDbContext.FriendRequests
                .Where(fr =>
                    (fr.SenderId == user.Id || fr.ReceiverId == user.Id) &&
                    fr.Status == FriendRequestStatus.Accepted)
                .Select(fr => fr.SenderId == user.Id ? fr.Receiver : fr.Sender)
                .ToListAsync();

            var friendsCount = friends.Count;
            var recentFriends = friends
                .Take(5)
                .Select(f => new FriendSummaryResponse
                {
                    UserName = _userManager.FindByIdAsync(f.Id).Result?.UserName ?? string.Empty,
                    Name = f.Name,
                    ProfileImageUrl = f.ProfileImageUrl
                })
                .ToList();

            
            RelationshipStatus? relationship = null;
            if (!string.IsNullOrEmpty(currentUserId) && currentUserId != user.Id)
            {
               
                var existingRequest = await _appDbContext.FriendRequests
                    .FirstOrDefaultAsync(fr =>
                        (fr.SenderId == currentUserId && fr.ReceiverId == user.Id) ||
                        (fr.SenderId == user.Id && fr.ReceiverId == currentUserId));

                if (existingRequest == null)
                    relationship = RelationshipStatus.None;
                else if (existingRequest.Status == FriendRequestStatus.Pending)
                    relationship = existingRequest.SenderId == currentUserId
                        ? RelationshipStatus.PendingSent
                        : RelationshipStatus.PendingReceived;
                else if (existingRequest.Status == FriendRequestStatus.Accepted)
                    relationship = RelationshipStatus.Friends;
            }

            return new PublicProfileResponse
            {
                UserId = user.Id,
                UserName = user.UserName ?? string.Empty,
                Name = profile?.Name,
                LastName = profile?.LastName,
                Biography = profile?.Biography,
                City = profile?.Location,
                ProfileImageUrl = profile?.ProfileImageUrl,
                FriendsCount = friendsCount,
                RecentFriends = recentFriends,
                RelationshipStatus = relationship 
            };
        }
    }
}
