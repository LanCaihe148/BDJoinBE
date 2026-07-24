using BDJoinSN.Application.Features.Posts.Commands;
using BDJoinSN.Domain;

namespace BDJoinSN.Application.Contracts.Persistance
{
    public interface IPostRepository : IAsyncRepository<Post, int>
    {

        

    }
}
