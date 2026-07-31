
using BDJoinSN.Application.Contracts.Identity;
using BDJoinSN.Application.Exceptions;
using BDJoinSN.Identity.Models;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;

namespace BDJoinSN.Identity.Features.Auth.Commands.ChangePassword
{
    public class ChangePasswordHandler : IRequestHandler<ChangePasswordCommand, bool>
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ILogger<ChangePasswordHandler> _logger;
        private readonly ITokenBlacklistService _tokenBlacklistService;

        public ChangePasswordHandler(UserManager<ApplicationUser> userManager, ILogger<ChangePasswordHandler> logger, ITokenBlacklistService tokenBlacklistService)
        {
            _userManager = userManager;
            _logger = logger;
            _tokenBlacklistService = tokenBlacklistService;
        }

        public async Task<bool> Handle(ChangePasswordCommand request, CancellationToken cancellationToken)
        {
            try
            {
               
                if (request == null)
                    throw new BadRequestException("La solicitud no puede estar vacía.");

                if (string.IsNullOrEmpty(request.UserId))
                    throw new BadRequestException("El ID del usuario es requerido.");

                if (string.IsNullOrEmpty(request.CurrentPassword))
                    throw new BadRequestException("La contraseña actual es requerida.");

                if (string.IsNullOrEmpty(request.NewPassword))
                    throw new BadRequestException("La nueva contraseña es requerida.");

                
                if (request.CurrentPassword == request.NewPassword)
                    throw new BadRequestException("La nueva contraseña no puede ser igual a la actual.");

                
                var user = await _userManager.FindByIdAsync(request.UserId);
                if (user == null)
                    throw new NotFoundException(nameof(ApplicationUser), request.UserId);

                
                var isCurrentPasswordValid = await _userManager.CheckPasswordAsync(user, request.CurrentPassword);
                if (!isCurrentPasswordValid)
                    throw new BadRequestException("La contraseña actual es incorrecta.");

                
                var result = await _userManager.ChangePasswordAsync(user, request.CurrentPassword, request.NewPassword);

                if (!result.Succeeded)
                {
                    var errors = string.Join("; ", result.Errors.Select(e => e.Description));
                    _logger.LogWarning($"Error al cambiar contraseña para usuario {request.UserId}: {errors}");
                    throw new BadRequestException($"Error al cambiar la contraseña: {errors}");
                }

                
                if (request.InvalidateOtherSessions)
                {
                    
                    await _userManager.UpdateSecurityStampAsync(user);

                    _logger.LogInformation($"Sesiones invalidadas para usuario {request.UserId}");
                }

                _logger.LogInformation($"Contraseña actualizada exitosamente para usuario {request.UserId}");

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al cambiar contraseña para usuario {UserId}", request.UserId);
                throw;
            }
        }
    }
}
