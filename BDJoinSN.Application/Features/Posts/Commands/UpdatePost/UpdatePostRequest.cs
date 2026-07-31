

namespace BDJoinSN.Application.Features.Posts.Commands.UpdatePost
{
    public class UpdatePostRequest
    {
        public int Id { get; set; }
        public string? Content { get; set; }
        public string? Author { get; set; }
    }
}
