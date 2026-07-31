

using MediatR;

namespace BDJoinSN.Application.Features.Posts.Commands.UpdatePost
{
    public class UpdatePostCommand : IRequest<bool>
    {
        public int Id { get; set; }
        public string UserId { get; set; } = string.Empty; 
        public string? Content { get; set; }
        public string? Author { get; set; }
    }
}
