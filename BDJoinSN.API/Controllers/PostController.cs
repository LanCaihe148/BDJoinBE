using BDJoinSN.Application.Contracts.Persistance;
using BDJoinSN.Application.Exceptions;
using BDJoinSN.Application.Features.Posts.Commands.CreatePost;
using BDJoinSN.Application.Features.Posts.Queries.GetPostsById;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace BDJoinSN.API.Controllers
{

    [ApiController]
    [Route("Api/[controller]")]
    [Authorize]
    public class PostController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ILogger<PostController> _logger;

        public PostController(IMediator mediator, ILogger<PostController> logger)
        {
            _mediator = mediator;
            _logger = logger;
        }

        [HttpPost("new")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult> CreatePost([FromBody]CreatePostCommand request)
        {
            var userId = User.FindFirstValue("uid")
                ?? User.FindFirstValue(ClaimTypes.NameIdentifier);
            request.UserId = userId;
            if (request.UserId == null)
            {
                throw new BadRequestException("La operacion no puede completarse por que el usuario no ha sido autenticado.");
            }

            var result = await _mediator.Send(request);

            return CreatedAtAction(nameof(GetPostById), new { id = result }, new { id = result });
        }

        [HttpGet("{id}")]
        [ProducesResponseType(typeof(PostDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<PostDto>> GetPostById(int id)
        {
            try
            {
              
                if (id <= 0)
                {
                    return BadRequest(new { error = "El ID debe ser un número positivo" });
                }

                
                var query = new GetPostByIdQuery(id);

                
                var post = await _mediator.Send(query);

                
                return Ok(post);
            }
            catch (NotFoundException)
            {
                return NotFound(new { error = $"Post con ID {id} no encontrado" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener post con ID {PostId}", id);
                return StatusCode(500, new { error = "Error al obtener el post" });
            }
        }

    }
}
