using BDJoinSN.Application.Features.FriendRequests.Commands.CreateFriendRequests;
using BDJoinSN.Domain;

namespace BDJoinSN.Application.Contracts.Persistance
{
    public interface IFriendRepository
    {
        Task<bool> HasPendingRequestAsync(string senderId, string receiverId);
        Task<bool> AreFriendsAsync(string userId1, string userId2);
        Task<IReadOnlyList<FriendRequest>> GetPendingRequestsForUserAsync(string userId);
        void AddFriendRequest(FriendRequest friendRequest);

    }
}
