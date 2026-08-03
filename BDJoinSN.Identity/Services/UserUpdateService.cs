

using BDJoinSN.Application.Contracts.Identity;
using BDJoinSN.Application.Exceptions;
using BDJoinSN.Identity.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;

namespace BDJoinSN.Identity.Services
{
    public class UserUpdateService : IUserUpdateService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ILogger<UserUpdateService> _logger;

        public async Task UpdateUserDisplayNameAsync(string userId, string displayName)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
                throw new NotFoundException("Usuario", userId);

            
            user.DisplayName = displayName;
            var result = await _userManager.UpdateAsync(user);

            if (!result.Succeeded)
            {
                var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                throw new BadRequestException($"Error al actualizar DisplayName: {errors}");
            }

            _logger.LogInformation($"DisplayName actualizado para usuario {userId}");
        }

        public async Task UpdateUserProfileImageAsync(string userId, string imageUrl)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
                throw new NotFoundException("Usuario", userId);

            user.ProfileImageUrl = imageUrl;
            var result = await _userManager.UpdateAsync(user);

            if (!result.Succeeded)
            {
                var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                throw new BadRequestException($"Error al actualizar imagen de perfil: {errors}");
            }

            _logger.LogInformation($"Imagen de perfil actualizada para usuario {userId}");
        }

        public async Task<bool> UpdateUserInfoAsync(string userId, string displayName, string? profileImageUrl = null)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
                throw new NotFoundException("Usuario", userId);

            
            if (!string.IsNullOrEmpty(displayName) && user.DisplayName != displayName)
            {
                user.DisplayName = displayName;
            }

            if (!string.IsNullOrEmpty(profileImageUrl) && user.ProfileImageUrl != profileImageUrl)
            {
                user.ProfileImageUrl = profileImageUrl;
            }

            var result = await _userManager.UpdateAsync(user);

            if (!result.Succeeded)
            {
                var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                throw new BadRequestException($"Error al actualizar usuario: {errors}");
            }

            _logger.LogInformation($"Usuario {userId} actualizado correctamente");
            return true;
        }
    }
}
