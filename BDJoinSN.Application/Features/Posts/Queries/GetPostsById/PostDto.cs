
namespace BDJoinSN.Application.Features.Posts.Queries.GetPostsById
{
    public class PostDto
    {
        public int Id { get; set; }
        public string UserId { get; set; } = string.Empty;
        public string? Author { get; set; }
        public string? Content { get; set; }
        public DateTimeOffset? CreatedDate { get; set; }
        public string? CreatedBy { get; set; }
        public DateTimeOffset? LastModifiedDate { get; set; }
        public string? LastModifiedBy { get; set; }
    }
}
