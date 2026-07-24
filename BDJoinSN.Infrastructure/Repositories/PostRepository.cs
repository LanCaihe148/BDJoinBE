using BDJoinSN.Application.Contracts.Persistance;
using BDJoinSN.Domain;
using BDJoinSN.Infrastructure.Persistance;

namespace BDJoinSN.Infrastructure.Repositories
{
    public class PostRepository : RepositoryBase<Post, int>, IPostRepository
    {
        public PostRepository(BDJoinDbContext context) : base(context)
        {
        }

        
    }
}
