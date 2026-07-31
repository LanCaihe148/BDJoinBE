using BDJoinSN.Application.Features.Posts.Queries.GetPostsById;

namespace BDJoinSN.Application.Features.Posts.Queries.GetPostsByFriend
{
    public class FeedResponse
    {
        public List<PostDto> Items { get; set; } = new();
        public int TotalCount { get; set; }
        public int PageIndex { get; set; }
        public int PageSize { get; set; }
        public int TotalPages { get; set; }
        public bool HasNextPage { get; set; }
        public bool HasPreviousPage { get; set; }
    }
}
