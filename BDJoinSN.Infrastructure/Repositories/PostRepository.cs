using BDJoinSN.Application.Contracts.Persistance;
using BDJoinSN.Domain;
using BDJoinSN.Infrastructure.Persistance;
using Microsoft.EntityFrameworkCore;

namespace BDJoinSN.Infrastructure.Repositories
{
    public class PostRepository : RepositoryBase<Post, int>, IPostRepository
    {
        public PostRepository(BDJoinDbContext context) : base(context)
        {
        }

        public async Task<int> CountByUsernameAsync(string username)
        {
            return await _context.Posts
                .CountAsync(p => p.Author == username);
        }

        public async Task<List<Post>> GetAllPostByUsername(string username, int pageIndex, int pageSize)
        {
            return await _context.Posts.Where(p => p.Author == username).OrderByDescending(p => p.CreatedDate)
                .Skip((pageIndex - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync(); 
        }

        public async Task<IReadOnlyList<Post>> GetFeedAsync(List<string> friendIds, int pageIndex, int pageSize)
        {
            return await _context.Posts
                .Where(p => friendIds.Contains(p.UserId))
                .OrderByDescending(p => p.CreatedDate)
                .Skip((pageIndex - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }

        public async Task<int> GetFeedCountAsync(List<string> friendIds)
        {
            return await _context.Posts
                .CountAsync(p => friendIds.Contains(p.UserId));
        }
    }
}
