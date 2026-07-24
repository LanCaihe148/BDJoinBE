

using MediatR;

namespace BDJoinSN.Application.Features.Posts.Queries.GetPostsById
{
    public class GetPostByIdQuery : IRequest<PostDto>
    {
        public int Id { get; set; }

        public GetPostByIdQuery(int id)
        {
            Id = id;
        }
    }
}
