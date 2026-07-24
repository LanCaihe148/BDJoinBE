using BDJoinSN.Domain.Common;



namespace BDJoinSN.Application.Contracts.Persistance
{
    public interface IUnitOfWork : IDisposable
    {
        IPostRepository PostRepository { get; }

        IAsyncRepository<TEntity, TId> Repository<TEntity, TId>() where TEntity : BaseDomainModel<TId>;

        IProfileRepository ProfileRepository { get; }

        IFriendRepository FriendRepository { get; }

        //IUserRepository UserRepository { get; }
        Task<int> Complete();
    }
}
