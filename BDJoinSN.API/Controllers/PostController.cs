using BDJoinSN.Application.Contracts.Persistance;
using BDJoinSN.Application.Exceptions;
using BDJoinSN.Application.Features.Posts.Commands.CreatePost;
using BDJoinSN.Application.Features.Posts.Commands.DeletePost;
using BDJoinSN.Application.Features.Posts.Commands.UpdatePost;
using BDJoinSN.Application.Features.Posts.Queries.GetAllPostByUsername;
using BDJoinSN.Application.Features.Posts.Queries.GetPostsById;
using BDJoinSN.Application.Models.Pagination;
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

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult> CreatePost([FromBody]CreatePostCommand request)
        {
            var userId = User.FindFirstValue("uid")
                ?? User.FindFirstValue(ClaimTypes.NameIdentifier);
            var author = User.FindFirstValue("username")
                ?? User.FindFirstValue(ClaimTypes.NameIdentifier);

            request.Author = author;
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

        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeletePost(int id)
        {
            try
            {
                var userId = User.FindFirstValue("uid")
                    ?? User.FindFirstValue(ClaimTypes.NameIdentifier)
                    ?? throw new UnauthorizedAccessException("Usuario no autenticado");

                if (id <= 0)
                    return BadRequest(new { error = "El ID debe ser un número positivo" });

                var command = new DeletePostCommand
                {
                    PostId = id,
                    UserId = userId
                };

                var result = await _mediator.Send(command);

                return Ok(new
                {
                    message = "Post eliminado correctamente",
                    success = result
                });
            }
            catch (NotFoundException)
            {
                return NotFound(new { error = $"Post con ID {id} no encontrado" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al eliminar post {PostId}", id);
                return BadRequest(new { error = ex.Message });
            }
        }

        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)] 
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UpdatePost(int id, [FromBody] UpdatePostRequest request)
        {
            try
            {
                
                if (id != request.Id)
                    return BadRequest(new { error = "El ID de la URL no coincide con el ID del cuerpo." });

                if (id <= 0)
                    return BadRequest(new { error = "El ID debe ser un número positivo." });

                
                if (string.IsNullOrWhiteSpace(request.Content))
                    return BadRequest(new { error = "El contenido del post no puede estar vacío." });

                
                var userId = User.FindFirstValue("uid")
                    ?? User.FindFirstValue(ClaimTypes.NameIdentifier)
                    ?? throw new UnauthorizedAccessException("Usuario no autenticado");

                
                var command = new UpdatePostCommand
                {
                    Id = id,
                    UserId = userId,
                    Content = request.Content,
                    Author = request.Author
                };

           
                var result = await _mediator.Send(command);

                return Ok(new
                {
                    message = "Post actualizado correctamente",
                    success = result
                });
            }
            catch (NotFoundException)
            {
                return NotFound(new { error = $"Post con ID {id} no encontrado" });
            }
            catch (ForbiddenException ex)
            {
                return StatusCode(403, new { error = ex.Message });
            }
            catch (BadRequestException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al actualizar post {PostId}", id);
                return StatusCode(500, new { error = "Error al actualizar el post" });
            }
        }

        [HttpGet("user/{username}")]
        [ProducesResponseType(typeof(PaginatedResult<PostDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<PaginatedResult<PostDto>>> GetPostsByUsername(
            string username,
            [FromQuery] int pageIndex = 1,
            [FromQuery] int pageSize = 10)
        {
            try
            {
                
                if (pageIndex < 1)
                    return BadRequest(new { error = "pageIndex debe ser mayor o igual a 1" });

                if (pageSize < 1 || pageSize > 50)
                    return BadRequest(new { error = "pageSize debe estar entre 1 y 50" });

                
                var query = new GetAllPostByUsernameQuery
                {
                    Username = username,
                    PageIndex = pageIndex,
                    PageSize = pageSize
                };

                
                var result = await _mediator.Send(query);

                return Ok(result);
            }
            catch (NotFoundException)
            {
                return NotFound(new { error = $"Usuario '{username}' no encontrado" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener posts del usuario {Username}", username);
                return StatusCode(500, new { error = "Error al obtener los posts" });
            }
        }

    }
}
