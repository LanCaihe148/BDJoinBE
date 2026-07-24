using MediatR;

namespace BDJoinSN.Application.Features.Posts.Commands.CreatePost
{
    public class CreatePostCommand : IRequest<int>
    {
        public string? UserId { get; set; }
        public string? Author { get; set; }

        public string? Content { get; set; }

    }
}
