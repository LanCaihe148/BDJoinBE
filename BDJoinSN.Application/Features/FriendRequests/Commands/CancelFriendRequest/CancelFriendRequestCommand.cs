

using MediatR;

namespace BDJoinSN.Application.Features.FriendRequests.Commands.CancelFriendRequest
{
    public class CancelFriendRequestCommand : IRequest<bool>
    {
        public string RequestId { get; set; } = string.Empty;

        public string UserId { get; set; } = string.Empty;


    }
}
