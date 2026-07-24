using BDJoinSN.Application.Contracts.Persistance;
using BDJoinSN.Application.Exceptions;
using BDJoinSN.Application.Features.FriendRequests.Commands.CreateFriendRequests;
using BDJoinSN.Domain;
using BDJoinSN.Infrastructure.Persistance;
using Microsoft.EntityFrameworkCore;

namespace BDJoinSN.Infrastructure.Repositories
{
    public class FriendRepository : RepositoryBase<FriendRequest,int> ,IFriendRepository
    {
        public FriendRepository(BDJoinDbContext context) : base(context)
        {
            
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
    }
}
