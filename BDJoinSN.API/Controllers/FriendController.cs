using BDJoinSN.Application.Contracts.Persistance;
using BDJoinSN.Application.Exceptions;
using BDJoinSN.Application.Features.FriendRequests.Commands.AccepFriendsRequest;
using BDJoinSN.Application.Features.FriendRequests.Commands.CancelFriendRequest;
using BDJoinSN.Application.Features.FriendRequests.Commands.CreateFriendRequests;
using BDJoinSN.Application.Features.FriendRequests.Commands.DeleteFriend;
using BDJoinSN.Application.Features.FriendRequests.Commands.RejectFriendRequests;
using BDJoinSN.Application.Features.FriendRequests.Queries.GetFriends;
using BDJoinSN.Application.Models;
using BDJoinSN.Application.Models.Pagination;
using BDJoinSN.Domain;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace BDJoinSN.API.Controllers
{
    [ApiController]
    [Route("Api/[controller]")]
    [Authorize]
    public class FriendController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ILogger<FriendController> _logger;

        public FriendController(IMediator mediator, ILogger<FriendController> logger)
        {
            _mediator = mediator;
            _logger = logger;
        }

        [HttpPost("request")]
        [ProducesResponseType(typeof(FriendRequestStatus), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<FriendRequestStatus>> SendFriendRequest(
            [FromBody] CreateFriendRequestCommand request)
        {
            try
            {
                
                var senderId = User.FindFirstValue("uid")
                    ?? throw new UnauthorizedAccessException("No se pudo identificar al usuario.");

                request.SenderId = senderId;

                var result = await _mediator.Send(request);
                return Ok(new { status = result, message = "Solicitud de amistad enviada." });
            }
            catch (BadRequestException ex)
            {
                _logger.LogWarning(ex, "Error al enviar solicitud de amistad");
                return BadRequest(new { error = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error interno al enviar solicitud de amistad");
                return StatusCode(500, new { error = "Error al procesar la solicitud." });
            }
        }

        [HttpPost("accept/{requestId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> AcceptFriendRequest(int requestId)
        {
            try
            {
                var userId = User.FindFirstValue("uid")
                    ?? User.FindFirstValue(ClaimTypes.NameIdentifier)
                    ?? throw new UnauthorizedAccessException("Usuario no autenticado");

                var command = new AcceptFriendRequestCommand
                {
                    RequestId = requestId.ToString(),
                    UserId = userId
                };

                var result = await _mediator.Send(command);

                return Ok(new { message = "Solicitud de amistad aceptada", success = result });
            }
            catch (NotFoundException)
            {
                return NotFound(new { error = "Solicitud de amistad no encontrada" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al aceptar solicitud de amistad {RequestId}", requestId);
                return BadRequest(new { error = ex.Message });
            }
        }

        [HttpPost("reject/{requestId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> RejectFriendRequest(int requestId)
        {
            try
            {
                var userId = User.FindFirstValue("uid")
                    ?? User.FindFirstValue(ClaimTypes.NameIdentifier)
                    ?? throw new UnauthorizedAccessException("Usuario no autenticado");

                var command = new RejectFriendRequestCommand
                {
                    RequestId = requestId.ToString(),
                    UserId = userId
                };

                var result = await _mediator.Send(command);

                return Ok(new { message = "Solicitud de amistad rechazada", success = result });
            }
            catch (NotFoundException)
            {
                return NotFound(new { error = "Solicitud de amistad no encontrada" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al rechazar solicitud de amistad {RequestId}", requestId);
                return BadRequest(new { error = ex.Message });
            }
        }

        [HttpPost("cancel/{requestId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> CancelFriendRequest(int requestId)
        {
            try
            {
                var userId = User.FindFirstValue("uid")
                    ?? User.FindFirstValue(ClaimTypes.NameIdentifier)
                    ?? throw new UnauthorizedAccessException("Usuario no autenticado");

                var command = new CancelFriendRequestCommand
                {
                    RequestId = requestId.ToString(),
                    UserId = userId
                };

                var result = await _mediator.Send(command);

                return Ok(new
                {
                    message = "Solicitud de amistad cancelada",
                    success = result
                });
            }
            catch (NotFoundException)
            {
                return NotFound(new { error = "Solicitud de amistad no encontrada" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al cancelar solicitud de amistad {RequestId}", requestId);
                return BadRequest(new { error = ex.Message });
            }
        }


        [HttpDelete("{friendId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteFriend(string friendId)
        {
            try
            {
                var userId = User.FindFirstValue("uid")
                    ?? User.FindFirstValue(ClaimTypes.NameIdentifier)
                    ?? throw new UnauthorizedAccessException("Usuario no autenticado");

                if (string.IsNullOrEmpty(friendId))
                    return BadRequest(new { error = "El ID del amigo es requerido." });

                if (userId == friendId)
                    return BadRequest(new { error = "No puedes eliminarte a ti mismo." });

                var command = new DeleteFriendCommand
                {
                    UserId = userId,
                    FriendId = friendId
                };

                var result = await _mediator.Send(command);

                return Ok(new
                {
                    message = "Amistad eliminada correctamente",
                    success = result
                });
            }
            catch (NotFoundException)
            {
                return NotFound(new { error = "No existe una amistad entre estos usuarios." });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al eliminar amistad con {FriendId}", friendId);
                return BadRequest(new { error = ex.Message });
            }
        }

        [HttpGet("pending")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> GetPendingRequests()
        {
            try
            {
                var userId = User.FindFirstValue("uid")
                    ?? User.FindFirstValue(ClaimTypes.NameIdentifier)
                    ?? throw new UnauthorizedAccessException("Usuario no autenticado");

                

                return Ok(new { message = "Endpoint para obtener solicitudes pendientes" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener solicitudes pendientes");
                return StatusCode(500, new { error = "Error al obtener solicitudes" });
            }
        }


        [HttpGet("list")]
        [ProducesResponseType(typeof(PaginatedResult<FriendDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<PaginatedResult<FriendDto>>> GetFriendsList(
            [FromQuery] int pageIndex = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] string? searchTerm = null,
            [FromQuery] string? sortBy = "Username",
            [FromQuery] bool sortDescending = false)
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

                
                var query = new GetFriendsQuery
                {
                    UserId = userId,
                    PageIndex = pageIndex,
                    PageSize = pageSize,
                    SearchTerm = searchTerm,
                    SortBy = sortBy,
                    SortDescending = sortDescending
                };

             
                var result = await _mediator.Send(query);

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener lista de amigos");
                return StatusCode(500, new { error = "Error al obtener la lista de amigos" });
            }
        }
    }
}
