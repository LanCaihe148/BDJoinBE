namespace BDJoinSN.Application.Contracts.Persistance
{
    public interface IProfileCreationService
    {
        public Task CreateProfileAsync(string userId, string name, string lastName, string displayName ,string userName, string biography);
    }
}
