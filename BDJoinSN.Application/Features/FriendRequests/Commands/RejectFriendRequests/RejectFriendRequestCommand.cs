using MediatR;

namespace BDJoinSN.Application.Features.FriendRequests.Commands.RejectFriendRequests
{
    public class RejectFriendRequestCommand : IRequest<bool>
    {
        public string RequestId { get; set; } = string.Empty;

        public string UserId { get; set; } = string.Empty;

    }
}
