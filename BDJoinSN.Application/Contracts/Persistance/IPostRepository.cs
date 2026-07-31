using BDJoinSN.Application.Features.Posts.Commands;
using BDJoinSN.Domain;

namespace BDJoinSN.Application.Contracts.Persistance
{
    public interface IPostRepository : IAsyncRepository<Post, int>
    {
        Task<IReadOnlyList<Post>> GetFeedAsync(
        List<string> friendIds,
        int pageIndex,
        int pageSize);

        Task<int> GetFeedCountAsync(List<string> friendIds);

    }
}
