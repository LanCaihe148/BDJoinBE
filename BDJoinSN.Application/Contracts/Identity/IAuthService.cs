using BDJoinSN.Application.Models.Identity;

namespace BDJoinSN.Application.Contracts.Identity
{
    public interface IAuthService
    {
        Task<AuthResponse> Login(AuthRequest request);

        Task<RegistrationResponse> Register(RegistrationRequest request);
    }
}
