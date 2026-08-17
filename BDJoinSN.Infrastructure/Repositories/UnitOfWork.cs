

using BDJoinSN.Application.Contracts.Persistance;
using BDJoinSN.Domain.Common;
using BDJoinSN.Identity.Models;
using BDJoinSN.Identity.Repositories;
using BDJoinSN.Infrastructure.Persistance;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Collections;

namespace BDJoinSN.Infrastructure.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private Hashtable _repositories;
        private readonly BDJoinDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private IPostRepository _postRepository;
        private IUserRepository _userRepository;
        private IProfileRepository _profileRepository;
        private IFriendRepository _friendRepository;


        // profiles definition 
        public IPostRepository PostRepository => _postRepository ??= new PostRepository(_context);
        public IProfileRepository ProfileRepository => _profileRepository ??= new ProfileRepository(_context);
        public IUserRepository UserRepository => _userRepository ??= new UserRepository(_userManager, ProfileRepository);

        public IFriendRepository FriendRepository => _friendRepository ??= new FriendRepository(_context);

       
        public UnitOfWork(BDJoinDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public IAsyncRepository<TEntity, TId> Repository<TEntity, TId>() where TEntity : BaseDomainModel<TId>
        {
            if(_repositories == null){
                _repositories = new Hashtable();
            }

            var type = typeof(TEntity).Name;

            if (!_repositories.ContainsKey(type)){
                var repositoryType = typeof(RepositoryBase<,>);
                var repositoryInstace = Activator.CreateInstance(repositoryType.MakeGenericType(typeof(TEntity), typeof(TId)), _context);
                _repositories.Add(type, repositoryInstace);
            }

            return (IAsyncRepository<TEntity, TId>)_repositories[type];
        }

        public async Task<int> Complete()
        {
            return await _context.SaveChangesAsync();
        }

        public async Task DeleteUserRelatedDataAsync(string userId)
        {
            
            await FriendRepository.DeleteFriendRequestsByUserIdAsync(userId);

            
            var posts = await _context.Posts
                .Where(p => p.UserId == userId)
                .ToListAsync();

            if (posts.Any())
            {
                _context.Posts.RemoveRange(posts);
            }

            
            var profile = await _context.UserProfiles.FirstOrDefaultAsync(p => p.Id == userId);
            if (profile != null)
            {
                _context.UserProfiles.Remove(profile);
            }

           
            await _context.SaveChangesAsync();
        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }
}
