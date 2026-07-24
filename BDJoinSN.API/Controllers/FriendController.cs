using BDJoinSN.Application.Contracts.Persistance;
using BDJoinSN.Application.Exceptions;
using BDJoinSN.Application.Features.FriendRequests.Commands.CreateFriendRequests;
using BDJoinSN.Domain;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace BDJoinSN.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
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

        [HttpPost("send")]
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
    }
}
