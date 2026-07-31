using MediatR;


namespace BDJoinSN.Application.Features.FriendRequests.Commands.DeleteFriend
{
    public class DeleteFriendCommand : IRequest<bool>
    {
        public string UserId { get; set; } = string.Empty;

        public string FriendId { get; set; } = string.Empty;

    }
}
