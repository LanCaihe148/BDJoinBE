

using BDJoinSN.Application.Contracts.Identity;
using BDJoinSN.Application.Exceptions;
using BDJoinSN.Application.Models;
using BDJoinSN.Domain;
using BDJoinSN.Identity.Models;
using BDJoinSN.Infrastructure.Persistance;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace BDJoinSN.Infrastructure.Repositories
{
    public class ProfileService : IProfileService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly BDJoinDbContext _appDbContext;
        private readonly ILogger<ProfileService> _logger;

        public ProfileService(UserManager<ApplicationUser> userManager, BDJoinDbContext appDbContext, ILogger<ProfileService> logger)
        {
            _userManager = userManager;
            _appDbContext = appDbContext;
            _logger = logger;
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

            try
            {
                
                var user = await _userManager.FindByNameAsync(username);
                if (user == null)
                    throw new NotFoundException("Username", username);

                
                _logger.LogInformation("=== PUBLIC PROFILE DEBUG ===");
                _logger.LogInformation("CurrentUserId: {CurrentUserId}", currentUserId ?? "NULL");
                _logger.LogInformation("TargetUserId: {TargetUserId}", user.Id);
                _logger.LogInformation("TargetUserName: {TargetUserName}", user.UserName);

                
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
                    _logger.LogInformation("Verificando relación entre CurrentUser y TargetUser");

                    
                    var existingRequest = await _appDbContext.FriendRequests
                        .FirstOrDefaultAsync(fr =>
                            (fr.SenderId == currentUserId && fr.ReceiverId == user.Id) ||
                            (fr.SenderId == user.Id && fr.ReceiverId == currentUserId));

                    
                    if (existingRequest != null)
                    {
                        _logger.LogInformation("=== Friend Request Found ===");
                        _logger.LogInformation("RequestId: {RequestId}", existingRequest.Id);
                        _logger.LogInformation("SenderId: {SenderId}", existingRequest.SenderId);
                        _logger.LogInformation("ReceiverId: {ReceiverId}", existingRequest.ReceiverId);
                        _logger.LogInformation("Status: {Status} ({StatusName})",
                            existingRequest.Status,
                            existingRequest.Status.ToString());

                        
                        if (existingRequest.Status == FriendRequestStatus.Pending)
                        {
                            relationship = existingRequest.SenderId == currentUserId
                                ? RelationshipStatus.PendingSent
                                : RelationshipStatus.PendingReceived;

                            _logger.LogInformation("Relationship: {Relationship}", relationship.ToString());
                        }
                        else if (existingRequest.Status == FriendRequestStatus.Accepted)
                        {
                            relationship = RelationshipStatus.Friends;
                            _logger.LogInformation("✅ Relationship: Friends - ¡SON AMIGOS!");
                        }
                        else if (existingRequest.Status == FriendRequestStatus.Rejected)
                        {
                            relationship = RelationshipStatus.None;
                            _logger.LogInformation("Relationship: None (Rejected)");
                        }
                    }
                    else
                    {
                        _logger.LogInformation("❌ No se encontró ninguna solicitud de amistad entre los usuarios");
                        relationship = RelationshipStatus.None;
                    }
                }
                else
                {
                    _logger.LogInformation("CurrentUserId es NULL o es el mismo usuario");
                }

                
                _logger.LogInformation("=== FINAL RESULT ===");
                _logger.LogInformation("FriendsCount: {FriendsCount}", friendsCount);
                _logger.LogInformation("RelationshipStatus: {RelationshipStatus}", relationship?.ToString() ?? "NULL");
                _logger.LogInformation("RelationshipStatus Value: {RelationshipValue}", (int?)relationship ?? -1);

                
                return new PublicProfileResponse
                {
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
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener perfil público para {Username}", username);
                throw;
            }
        }
    }
}
