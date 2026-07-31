using BDJoinSN.Application.Features.Posts.Queries.GetPostsByFriend;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace BDJoinSN.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class FeedController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ILogger<FeedController> _logger;

        public FeedController(IMediator mediator, ILogger<FeedController> logger)
        {
            _mediator = mediator;
            _logger = logger;
        }

        
        [HttpGet]
        [ProducesResponseType(typeof(FeedResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<FeedResponse>> GetFeed(
            [FromQuery] int pageIndex = 1,
            [FromQuery] int pageSize = 10)
        {
            try
            {
                
                if (pageIndex < 1)
                    return BadRequest(new { error = "pageIndex debe ser mayor o igual a 1" });

                if (pageSize < 1 || pageSize > 50)
                    return BadRequest(new { error = "pageSize debe estar entre 1 y 50" });

                
                var userId = User.FindFirstValue("uid")
                    ?? User.FindFirstValue(ClaimTypes.NameIdentifier)
                    ?? throw new UnauthorizedAccessException("Usuario no autenticado");

                
                var query = new GetPostByFriendQuery
                {
                    UserId = userId,
                    PageIndex = pageIndex,
                    PageSize = pageSize
                };

                
                var response = await _mediator.Send(query);

                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener feed");
                return StatusCode(500, new { error = "Error al obtener el feed" });
            }
        }
    }
}
