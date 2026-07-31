using BDJoinSN.Application.Contracts.Identity;
using BDJoinSN.Application.Contracts.Persistance;
using BDJoinSN.Application.Exceptions;
using BDJoinSN.Application.Models.Identity;
using BDJoinSN.Identity.Features.Auth.Commands.ChangePassword;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace BDJoinSN.API.Controllers
{
    [ApiController]
    [Route("Api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly IProfileCreationService _profileCreationService;
        private readonly ITokenBlacklistService _tokenBlacklistService;
        private readonly ILogger<AuthController> _logger;
        private readonly IMediator _mediator;

        public AuthController(IAuthService authService, IProfileCreationService profileCreationService, ITokenBlacklistService tokenBlacklistService, ILogger<AuthController> logger, IMediator mediator)
        {
            _authService = authService;
            _profileCreationService = profileCreationService;
            _tokenBlacklistService = tokenBlacklistService;
            _logger = logger;
            _mediator = mediator;
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

            //await _profileCreationService.CreateProfileAsync(
            //    response.UserId,
            //    request.Name,
            //    request.Lastname,
            //    request.DisplayName,
            //    request.Username
            //);
            return Ok(response);
        }


        [HttpPost("logout")]
        [Authorize]
        public async Task<IActionResult> Logout()
        {
            try
            {
                
                var authHeader = Request.Headers["Authorization"].ToString();

                if (string.IsNullOrEmpty(authHeader))
                {
                    return BadRequest(new { error = "No se proporcionó token" });
                }

                
                var token = authHeader.StartsWith("Bearer ")
                    ? authHeader.Substring(7)
                    : authHeader;

                if (string.IsNullOrEmpty(token))
                {
                    return BadRequest(new { error = "Token inválido" });
                }

                
                var handler = new JwtSecurityTokenHandler();
                if (handler.CanReadToken(token))
                {
                    var jwtToken = handler.ReadJwtToken(token);
                    var expiry = jwtToken.ValidTo;

                    
                    await _tokenBlacklistService.AddToBlacklistAsync(token, expiry);
                }

                _logger.LogInformation($"Usuario {User.Identity?.Name} cerró sesión");
                return Ok(new { message = "Sesión cerrada exitosamente" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al cerrar sesión");
                return StatusCode(500, new { error = "Error al cerrar sesión" });
            }
        
        }

        [HttpPost("Change-password")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordRequest request)
        {
            try
            {
               
                if (string.IsNullOrWhiteSpace(request.CurrentPassword))
                    return BadRequest(new { error = "La contraseña actual es requerida." });

                if (string.IsNullOrWhiteSpace(request.NewPassword))
                    return BadRequest(new { error = "La nueva contraseña es requerida." });

                if (string.IsNullOrWhiteSpace(request.ConfirmNewPassword))
                    return BadRequest(new { error = "Debes confirmar la nueva contraseña." });

                
                if (request.NewPassword != request.ConfirmNewPassword)
                    return BadRequest(new { error = "La nueva contraseña y la confirmación no coinciden." });

                if (request.CurrentPassword == request.NewPassword)
                    return BadRequest(new { error = "La nueva contraseña no puede ser igual a la actual." });

                
                if (request.NewPassword.Length < 6)
                    return BadRequest(new { error = "La nueva contraseña debe tener al menos 6 caracteres." });

                
                var userId = User.FindFirstValue("uid")
                    ?? User.FindFirstValue(ClaimTypes.NameIdentifier)
                    ?? throw new UnauthorizedAccessException("Usuario no autenticado");

                
                var command = new ChangePasswordCommand
                {
                    UserId = userId,
                    CurrentPassword = request.CurrentPassword,
                    NewPassword = request.NewPassword,
                    InvalidateOtherSessions = request.InvalidateOtherSessions
                };

                
                var result = await _mediator.Send(command);

                
                return Ok(new
                {
                    message = "Contraseña actualizada exitosamente",
                    success = result,
                    
                });
            }
            catch (NotFoundException)
            {
                return NotFound(new { error = "Usuario no encontrado." });
            }
            catch (BadRequestException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al cambiar contraseña");
                return StatusCode(500, new { error = "Error al cambiar la contraseña" });
            }
        }
    }
}
