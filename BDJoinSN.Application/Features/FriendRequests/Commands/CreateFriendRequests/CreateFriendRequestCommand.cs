using BDJoinSN.Domain;
using MediatR;

namespace BDJoinSN.Application.Features.FriendRequests.Commands.CreateFriendRequests
{
    public class CreateFriendRequestCommand : IRequest<FriendRequestStatus>
    {
        public string SenderId { get; set; } = string.Empty;
        public string ReceiverId { get; set; } = string.Empty;
        public FriendRequestStatus Status { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        

       
    }
}
