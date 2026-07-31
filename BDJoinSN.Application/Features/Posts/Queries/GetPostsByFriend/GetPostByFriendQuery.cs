using BDJoinSN.Application.Features.Posts.Queries.GetPostsById;
using BDJoinSN.Domain;
using MediatR;

namespace BDJoinSN.Application.Features.Posts.Queries.GetPostsByFriend
{
    public class GetPostByFriendQuery : IRequest<FeedResponse>
    {
        public string UserId { get; set; } = string.Empty;
        public int PageIndex { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }
}
