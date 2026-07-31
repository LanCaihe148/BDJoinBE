
using MediatR;

namespace BDJoinSN.Application.Features.Posts.Commands.DeletePost
{
    public class DeletePostCommand : IRequest<bool>
    {
        public string UserId { get; set; } = string.Empty;
        public int PostId { get; set; } 
    }

}
