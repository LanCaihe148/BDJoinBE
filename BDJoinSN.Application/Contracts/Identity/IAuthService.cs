using BDJoinSN.Application.Models.Identity;
using BDJoinSN.Application.Models.Pagination;

namespace BDJoinSN.Application.Contracts.Identity
{
    public interface IAuthService
    {
        Task<AuthResponse> Login(AuthRequest request);

        Task<RegistrationResponse> Register(RegistrationRequest request);

        Task<bool> Delete(string UserId);
    }
}
