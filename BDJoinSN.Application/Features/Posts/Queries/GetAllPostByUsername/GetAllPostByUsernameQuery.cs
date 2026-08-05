using BDJoinSN.Application.Features.Posts.Queries.GetPostsById;
using BDJoinSN.Application.Models.Pagination;
using MediatR;

namespace BDJoinSN.Application.Features.Posts.Queries.GetAllPostByUsername
{
    public class GetAllPostByUsernameQuery : IRequest<PaginatedResult<PostDto>>
    {
        public string? Username { get; set; }

        public int PageIndex { get; set; } = 1;
        public int PageSize { get; set; } = 10;

        
    }
}
