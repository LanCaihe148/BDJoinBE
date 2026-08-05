using BDJoinSN.Application.Features.Posts.Commands;
using BDJoinSN.Domain;

namespace BDJoinSN.Application.Contracts.Persistance
{
    public interface IPostRepository : IAsyncRepository<Post, int>
    {
        Task<IReadOnlyList<Post>> GetFeedAsync(
        List<string> friendIds,
        string currentUserId,
        int pageIndex,
        int pageSize);

        Task<int> GetFeedCountAsync(List<string> friendIds, string currentUserId);

        Task<List<Post>> GetAllPostByUsername(string username, int pageIndex, int pageSize);

        Task<int> CountByUsernameAsync(string username);
    }
}
