using BDJoinSN.Application.Features.FriendRequests.Commands.CreateFriendRequests;
using BDJoinSN.Domain;

namespace BDJoinSN.Application.Contracts.Persistance
{
    public interface IFriendRepository
    {
        Task<bool> HasPendingRequestAsync(string senderId, string receiverId);
        Task<bool> AreFriendsAsync(string userId1, string userId2);
        Task<FriendRequest?> GetByIdAsnc(int id);
        Task<IReadOnlyList<FriendRequest>> GetPendingRequestsForUserAsync(string userId);
        Task<FriendRequest?> GetAcceptedFriendRequestAsync(string userId1, string userId2);
        Task<IReadOnlyList<FriendRequest>> GetAllFriendsAsync(string userId);

        Task<IReadOnlyList<FriendRequest>> GetSentRequestAsync(string userId);

        Task<IReadOnlyList<FriendRequest>> GetReceivedRequestAsync(string userId);
        void AddFriendRequest(FriendRequest friendRequest);
        void UpdateFR(FriendRequest friendRequest);
        void DeleteFR(FriendRequest friendRequest);
        Task DeleteFriendRequestsByUserIdAsync(string userId);

    }
}
