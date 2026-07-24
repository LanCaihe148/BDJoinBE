using AutoMapper;
using BDJoinSN.Application.Contracts.Persistance;
using BDJoinSN.Application.Exceptions;
using BDJoinSN.Domain;
using MediatR;
using Microsoft.Extensions.Logging;

namespace BDJoinSN.Application.Features.Users.Commands.UpdateProfilesCommand
{
    public class UpdateUserProfileCommandHandler : IRequestHandler<UpdateUserProfileCommand, Unit>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger<UpdateUserProfileCommandHandler> _logger;

        public UpdateUserProfileCommandHandler(IUnitOfWork unitOfWork, IMapper mapper, ILogger<UpdateUserProfileCommandHandler> logger)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<Unit> Handle(UpdateUserProfileCommand request, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(request.UserId))
                throw new BadRequestException("El UserId es requerido");

            var profile = await _unitOfWork.ProfileRepository.GetByUserIdAsync(request.UserId);

            if (profile == null)
                throw new NotFoundException(nameof(UserProfile), request.UserId);

            _mapper.Map(request, profile);

            profile.UpdatedAt = DateTime.UtcNow;
            profile.LastModifiedDate = DateTimeOffset.UtcNow;
            profile.LastModifiedBy = request.UserId;

            var result = await _unitOfWork.Complete();

            if (result <= 0)
                throw new Exception("No se pudo actualizar el perfil. No se guardaron cambios.");

            _logger.LogInformation("Perfil actualizado exitosamente para usuario {UserId}", request.UserId);

            return Unit.Value;
        }
    }
}
