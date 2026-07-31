using MediatR;

namespace BDJoinSN.Application.Features.FriendRequests.Commands.AccepFriendsRequest
{
    public class AcceptFriendRequestCommand : IRequest<bool>
    {
        public string RequestId { get; set; } = string.Empty;

        public string UserId { get; set; } = string.Empty;


    }
}
