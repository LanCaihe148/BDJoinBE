using BDJoinSN.Application.Contracts.Identity;
using BDJoinSN.Application.Contracts.Persistance;
using BDJoinSN.Application.Models.Identity;
using Microsoft.AspNetCore.Mvc;

namespace BDJoinSN.API.Controllers
{
    [ApiController]
    [Route("Api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly IProfileCreationService _profileCreationService;

        public AuthController(IAuthService authService, IProfileCreationService profileCreationService)
        {
            _authService = authService;
            _profileCreationService = profileCreationService;
        }

        [HttpPost("Login")]
        [ProducesResponseType(typeof(AuthResponse), StatusCodes.Status200OK)]
        public async Task<ActionResult<AuthResponse>> Login([FromBody] AuthRequest request)
        {
            var response = await _authService.Login(request);
            return Ok(response);
        }

        [HttpPost("Register")]
        [ProducesResponseType(typeof(RegistrationResponse), StatusCodes.Status200OK)]
        public async Task<ActionResult<RegistrationResponse>> Register([FromBody] RegistrationRequest request)
        {
            var response = await _authService.Register(request);

            await _profileCreationService.CreateProfileAsync(
                response.UserId,
                request.Name,
                request.Lastname,
                request.DisplayName
            );
            return Ok(response);
        }
    }
}
