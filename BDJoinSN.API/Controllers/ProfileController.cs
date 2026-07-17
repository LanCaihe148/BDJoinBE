using BDJoinSN.Application.Contracts.Identity;
using BDJoinSN.Application.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace BDJoinSN.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class ProfileController : ControllerBase
    {
        private readonly IProfileService _profileService;

        public ProfileController(IProfileService profileService)
        {
            _profileService = profileService;
        }

        
        [HttpGet("me")]
        [ProducesResponseType(typeof(ProfileResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ProfileResponse>> GetMyProfile()
        {
            var userId = User.FindFirstValue("uid")
                ?? throw new UnauthorizedAccessException("No se pudo identificar al usuario.");

            var profile = await _profileService.GetOwnProfileAsync(userId);
            return Ok(profile);
        }

       
        [HttpGet("{username}")]
        [AllowAnonymous] // Este endpoint puede ser público
        [ProducesResponseType(typeof(PublicProfileResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<PublicProfileResponse>> GetPublicProfile(string username)
        {
            var currentUserId = User?.FindFirstValue(ClaimTypes.NameIdentifier);
            var profile = await _profileService.GetPublicProfileAsync(username, currentUserId);
            return Ok(profile);
        }
    }
}
