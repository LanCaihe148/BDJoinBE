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

        public async Task<int> CountExtPost(string userId)
        {

            //var posts = await _context.Posts.Where(p => p.UserId.Union()))


            return 1;
        }

        public async Task<int> CountOwnPosts(string userId)
        {
            return await _context.Posts.CountAsync(p => p.Author == userId);
        }

        public async Task<List<Post>> GetAllPostByUsername(string username, int pageIndex, int pageSize)
        {
            return await _context.Posts.Where(p => p.Author == username).OrderByDescending(p => p.CreatedDate)
                .Skip((pageIndex - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }

        public async Task<IReadOnlyList<Post>> GetFeedAsync(List<string> fOfIds, List<string> friendIds, string currentUserId, int pageIndex, int pageSize)
        {
            var userIds = new List<string>();
            var fofs = new List<string>();

            if (friendIds != null && friendIds.Any())
            {
                userIds.AddRange(friendIds);
            }

            if(fOfIds != null && fOfIds.Any())
            {
                fofs.AddRange(fOfIds);
            }

            userIds.Add(currentUserId);
            fofs.Add(currentUserId);

            return await _context.Posts
                .Where(p => userIds.Contains(p.UserId) || fofs.Contains(p.UserId))
                .OrderByDescending(p => p.CreatedDate)
                .Skip((pageIndex - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }

        public async Task<int> GetFeedCountAsync(List<string> fOfIds, List<string> friendIds, string currentUserId)
        {
            var fofs = new List<string>();
            if (fOfIds != null && fOfIds.Any())
            {
                fofs.AddRange(fOfIds);

            }
            fofs.Add(currentUserId);
            var userIds = new List<string>();
            if (friendIds != null && friendIds.Any()) 
            {
                userIds.AddRange(friendIds);
            }
              
            userIds.Add(currentUserId);

            return await _context.Posts
                .CountAsync(p => userIds.Contains(p.UserId) || fofs.Contains(p.UserId));
        }
 
    }
}
