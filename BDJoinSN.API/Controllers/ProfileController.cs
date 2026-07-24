using BDJoinSN.Application.Contracts.Identity;
using BDJoinSN.Application.Features.Users.Commands.UpdateProfilesCommand;
using BDJoinSN.Application.Models;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace BDJoinSN.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    
    public class ProfileController : ControllerBase
    {
        private readonly IProfileService _profileService;
        private readonly IMediator _mediator;

        public ProfileController(IProfileService profileService, IMediator mediator)
        {
            _profileService = profileService;
            _mediator = mediator;
        }

        [HttpGet("me")]
        [ProducesResponseType(typeof(ProfileResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Authorize]
        public async Task<ActionResult<ProfileResponse>> GetMyProfile()
        {
            var userId = User.FindFirstValue("uid")
                ?? throw new UnauthorizedAccessException("No se pudo identificar al usuario.");

            var profile = await _profileService.GetOwnProfileAsync(userId);
            return Ok(profile);
        }

        [HttpPatch("me")]
        [ProducesResponseType(typeof(ProfileResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [Authorize]
        public async Task<ActionResult<ProfileResponse>> UpdateMyProfile([FromBody] UpdateUserProfileCommand request)
        {
            var userId = User.FindFirstValue("uid")
                ?? User.FindFirstValue(ClaimTypes.NameIdentifier)
                ?? throw new UnauthorizedAccessException("No se pudo identificar al usuario.");

            
            request.UserId = userId;

            await _mediator.Send(request);

            var updatedProfile = await _profileService.GetOwnProfileAsync(userId);
            return Ok(updatedProfile);
        }

        [HttpGet("{username}")]
        
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
