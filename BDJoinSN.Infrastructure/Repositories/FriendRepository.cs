using System.Security.Principal;
using BDJoinSN.Application.Contracts.Persistance;
using BDJoinSN.Application.Exceptions;
using BDJoinSN.Application.Features.FriendRequests.Commands.CreateFriendRequests;
using BDJoinSN.Application.Models;
using BDJoinSN.Domain;
using BDJoinSN.Infrastructure.Persistance;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;

namespace BDJoinSN.Infrastructure.Repositories
{
    public class FriendRepository : RepositoryBase<FriendRequest, int>, IFriendRepository
    {
        private readonly IUserInfoService _userinfoservice;

        
        public FriendRepository(BDJoinDbContext context, IUserInfoService userinfo) : base(context)
        {
            _userinfoservice = userinfo;
        }

        public async Task<bool> HasPendingRequestAsync(string senderId, string receiverId)
        {
            return await _context.Set<FriendRequest>()
                .AnyAsync(fr =>
                    (fr.SenderId == senderId && fr.ReceiverId == receiverId && fr.Status == FriendRequestStatus.Pending) ||
                    (fr.SenderId == receiverId && fr.ReceiverId == senderId && fr.Status == FriendRequestStatus.Pending));
        }

        public async Task<bool> AreFriendsAsync(string userId1, string userId2)
        {
            return await _context.Set<FriendRequest>()
                .AnyAsync(fr =>
                    (fr.SenderId == userId1 && fr.ReceiverId == userId2 && fr.Status == FriendRequestStatus.Accepted) ||
                    (fr.SenderId == userId2 && fr.ReceiverId == userId1 && fr.Status == FriendRequestStatus.Accepted));
        }

        public async Task<IReadOnlyList<FriendRequest>> GetPendingRequestsForUserAsync(string userId)
        {
            return await _context.Set<FriendRequest>()
                .Where(fr => fr.ReceiverId == userId && fr.Status == FriendRequestStatus.Pending)
                .Include(fr => fr.Sender)
                .ToListAsync();
        }

        public void AddFriendRequest(FriendRequest friendRequest)
        {
            _context.Set<FriendRequest>().Add(friendRequest);
        }

        public void UpdateFR(FriendRequest friendRequest)
        {
            _context.FriendRequests.Update(friendRequest);
        }

        public async Task<FriendRequest?> GetByIdAsnc(int id)
        {
            return await _context.FriendRequests
               .FirstOrDefaultAsync(fr => fr.Id == id);
        }

        public async Task<FriendRequest?> GetAcceptedFriendRequestAsync(string userId1, string userId2)
        {
            return await _context.FriendRequests
                .FirstOrDefaultAsync(fr =>
                    ((fr.SenderId == userId1 && fr.ReceiverId == userId2) ||
                     (fr.SenderId == userId2 && fr.ReceiverId == userId1)) &&
                    fr.Status == FriendRequestStatus.Accepted);
        }


        public async Task<IReadOnlyList<FriendRequest>> GetAllFriendsAsync(string userId)
        {
            return await _context.FriendRequests
                .Where(fr =>
                    (fr.SenderId == userId || fr.ReceiverId == userId) &&
                    fr.Status == FriendRequestStatus.Accepted)
                .Include(fr => fr.Sender)
                .Include(fr => fr.Receiver)
                .ToListAsync();
        }

        public void DeleteFR(FriendRequest friendRequest)
        {
            _context.FriendRequests.Remove(friendRequest);
        }

        public async Task<IReadOnlyList<FriendRequest>> GetSentRequestAsync(string userId)
        {
            return await _context.FriendRequests
                .Where(fr => fr.SenderId == userId && fr.Status == FriendRequestStatus.Pending)
                .Include(fr => fr.Receiver)  
                .OrderByDescending(fr => fr.CreatedAt)  
                .ToListAsync();

        }

        public async Task<IReadOnlyList<FriendRequest>> GetReceivedRequestAsync(string userId)
        {
            return await _context.FriendRequests
                .Where(fr => fr.ReceiverId == userId && fr.Status == FriendRequestStatus.Pending)
                .Include(fr => fr.SenderId)
                .OrderByDescending(fr => fr.CreatedAt)
                .ToListAsync();

        }

        public async Task DeleteFriendRequestsByUserIdAsync(string userId)
        {
            var friendRequests = await _context.FriendRequests
                .Where(fr => fr.SenderId == userId || fr.ReceiverId == userId)
                .ToListAsync();

            if (friendRequests.Any())
            {
                _context.FriendRequests.RemoveRange(friendRequests);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<IReadOnlyList<FriendRequest>> GetFriendSuggestions(string userId)
        {
            var ownFriends = await GetAllFriendsAsync(userId);

            var ownFriendsIds = ownFriends.Select(fr => fr.SenderId == userId ? fr.ReceiverId : fr.SenderId).ToList();

            if (!ownFriends.Any())
            {
                return new List<FriendRequest>();
            }

            var sugg = new List<FriendRequest>();

            var suggUsersId = new HashSet<string>();

            for(int i=0; i<ownFriendsIds.Count(); i++)
            {
                var friendId = ownFriendsIds[i];

                var friendsOfFriends = await GetAllFriendsAsync(friendId);
                
                for(int j=0; j<friendsOfFriends.Count(); j++)
                {
                    var fr = friendsOfFriends[j];
                    var potentialFriendId = fr.SenderId == friendId ? fr.ReceiverId : fr.SenderId;

                    if (potentialFriendId == userId)
                    {
                        continue;
                    }
                    if (ownFriendsIds.Contains(potentialFriendId))
                    {
                        continue;
                    }
                    if (suggUsersId.Contains(potentialFriendId))
                    {
                        continue;
                    }
                    suggUsersId.Add(potentialFriendId);

                    var suggestion= new FriendRequest
                    {
                        SenderId = friendId,
                        ReceiverId = potentialFriendId,
                        Status = FriendRequestStatus.Pending,
                        CreatedAt = DateTime.UtcNow
                    };

                    sugg.Add(suggestion);

                    
                }
            }
            return sugg;

        }

        public async Task<List<string>> GetFriendSugg(string userId)
        {
            var ownFriends = await GetAllFriendsAsync(userId);

            var ownFriendsIds = ownFriends.Select(fr => fr.SenderId == userId ? fr.ReceiverId : fr.SenderId).ToList();

            if (!ownFriends.Any())
            {
                return new List<string>();
            }

            var sugg = new List<string>();

            var suggUsersId = new HashSet<string>();

            for (int i = 0; i < ownFriendsIds.Count(); i++)
            {
                var friendId = ownFriendsIds[i];

                var friendsOfFriends = await GetAllFriendsAsync(friendId);

                for (int j = 0; j < friendsOfFriends.Count(); j++)
                {
                    var fr = friendsOfFriends[j];
                    var potentialFriendId = fr.SenderId == friendId ? fr.ReceiverId : fr.SenderId;

                    if (potentialFriendId == userId)
                    {
                        continue;
                    }
                    if (ownFriendsIds.Contains(potentialFriendId))
                    {
                        continue;
                    }
                    if (suggUsersId.Contains(potentialFriendId))
                    {
                        continue;
                    }
                    suggUsersId.Add(potentialFriendId);

                    //var suggestion = new FriendRequest
                    //{
                    //    SenderId = friendId,
                    //    ReceiverId = potentialFriendId,
                    //    Status = FriendRequestStatus.Pending,
                    //    CreatedAt = DateTime.UtcNow
                    //};

                    // var suggestion = potentialFriendId;
                    sugg.Add(potentialFriendId);


                }
            }
            return sugg;

        }

        public async Task<List<FriendDto>> GetFriendSuggDtos(string userId)
        {
            var friends = await GetFriendSugg(userId);
            var fSuggsDtos = new List<FriendDto>();

            if (!friends.Any())
            {
                var userFriends = await GetAllFriendsAsync(userId);
                for(int i = 0; i<userFriends.Count(); i++)
                {
                    var s = userFriends[i];
                    var userFDto = await _userinfoservice.GetUserInfoAsync(s.ReceiverId);
                    fSuggsDtos.Add(new FriendDto
                    {
                        UserId = userFDto.UserId,
                        Username = userFDto.Username,
                        DisplayName = userFDto.DisplayName,
                        AvatarUrl = userFDto.AvatarUrl
                    });
                }
                return fSuggsDtos;
            }
            for(int i = 0; i<friends.Count(); i++)
            {
                var et = friends[i];
                var fDto = await _userinfoservice.GetUserInfoAsync(et);
                    
                    fSuggsDtos.Add(new FriendDto{
                        UserId = et,
                        Username = fDto.Username,
                        DisplayName = fDto.DisplayName,
                        AvatarUrl = fDto.AvatarUrl
                    });
            }

            return fSuggsDtos;
        }

    }
}
