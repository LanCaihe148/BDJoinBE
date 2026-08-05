using AutoMapper;
using BDJoinSN.Application.Contracts.Identity;
using BDJoinSN.Application.Contracts.Persistance;
using BDJoinSN.Application.Exceptions;
using MediatR;
using Microsoft.Extensions.Logging;

namespace BDJoinSN.Application.Features.Users.Commands.UpdateProfilesCommand
{
    public class UpdateUserProfileCommandHandler : IRequestHandler<UpdateUserProfileCommand, bool>
    {
         private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IUserUpdateService _userUpdateService; 
        private readonly ILogger<UpdateUserProfileCommandHandler> _logger;

        public UpdateUserProfileCommandHandler(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            IUserUpdateService userUpdateService,
            ILogger<UpdateUserProfileCommandHandler> logger)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _userUpdateService = userUpdateService;
            _logger = logger;
        }

        public async Task<bool> Handle(UpdateUserProfileCommand request, CancellationToken cancellationToken)
        {
            try
            {

                if (string.IsNullOrEmpty(request.UserId))
                    throw new BadRequestException("El ID del usuario es requerido.");


                var profile = await _unitOfWork.ProfileRepository.GetByUserIdAsync(request.UserId);
                if (profile == null)
                    throw new NotFoundException("Perfil", request.UserId);


                if (request.Birthday.HasValue)
                {
                    profile.Birthday = request.Birthday;
                }

                if (!string.IsNullOrEmpty(request.Biography))
                {
                    profile.Biography = request.Biography;
                }
                    
                if (!string.IsNullOrEmpty(request.Location))
                {
                    profile.Location = request.Location;
                }

                if (!string.IsNullOrEmpty(request.ProfileImageUrl))
                {
                    profile.ProfileImageUrl = request.ProfileImageUrl;
                }

                if (!string.IsNullOrEmpty(request.Username))
                {
                    profile.UserName = request.Username;
                }

                profile.UpdatedAt = DateTime.UtcNow;

                
                _unitOfWork.ProfileRepository.UpdateEntity(profile);
                await _unitOfWork.Complete();
 

                _logger.LogInformation($"Perfil del usuario {request.UserId} actualizado correctamente");

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al actualizar perfil del usuario {UserId}", request.UserId);
                throw;
            }
        }
    }
}
