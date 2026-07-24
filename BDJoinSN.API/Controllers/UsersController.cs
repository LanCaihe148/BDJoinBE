using BDJoinSN.Application.Contracts.Identity;
using BDJoinSN.Application.Contracts.Persistance;
using BDJoinSN.Application.Features.Users.Queries.SearchUsersQuery;
using BDJoinSN.Application.Models;
using BDJoinSN.Application.Models.Identity;
using BDJoinSN.Application.Models.Pagination;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace BDJoinSN.API.Controllers
{
    [ApiController]
    [Route("api/[Controller]")]
    [Authorize]
    public class UsersController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly IProfileService _profileService;
        private readonly ILogger<UsersController> _logger;

        public UsersController(IMediator mediator, IProfileService profileService, ILogger<UsersController> logger)
        {
            _mediator = mediator;
            _profileService = profileService;
            _logger = logger;
        }

        [HttpGet("search")]
        [ProducesResponseType(typeof(PaginatedResult<UserSearchResult>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<PaginatedResult<UserSearchResult>>> SearchUsers(
            [FromQuery] string searchTerm,
            [FromQuery] int pageIndex = 1,
            [FromQuery] int pageSize = 10)
        {
            if (pageIndex < 1)
                return BadRequest(new { error = "pageIndex debe ser mayor o igual a 1" });

            if (pageSize < 1 || pageSize > 50)
                return BadRequest(new { error = "pageSize debe estar entre 1 y 50" });

            try
            {
                var userId = User.FindFirstValue("uid")
                    ?? User.FindFirstValue(ClaimTypes.NameIdentifier)
                    ?? throw new UnauthorizedAccessException("No se pudo identificar al usuario.");

                var query = new SearchUserQuery(searchTerm, userId, pageIndex, pageSize);
                var result = await _mediator.Send(query);

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al buscar usuarios con término: {SearchTerm}", searchTerm);
                return StatusCode(500, new { error = "Error al procesar la búsqueda" });
            }
        }

        [HttpGet("me")]
        [ProducesResponseType(typeof(ProfileResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ProfileResponse>> GetCurrentUser()
        {
            var userId = User.FindFirstValue("uid")
                ?? User.FindFirstValue(ClaimTypes.NameIdentifier)
                ?? throw new UnauthorizedAccessException("No se pudo identificar al usuario.");

            var profile = await _profileService.GetOwnProfileAsync(userId);
            return Ok(profile);
        }
    }
}
