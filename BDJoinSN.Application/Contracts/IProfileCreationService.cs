namespace BDJoinSN.Application.Contracts
{
    public interface IProfileCreationService
    {
        public Task CreateProfileAsync(string userId, string name, string lastName);
    }
}
