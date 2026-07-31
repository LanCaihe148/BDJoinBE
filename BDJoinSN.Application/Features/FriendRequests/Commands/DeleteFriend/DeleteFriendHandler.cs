using BDJoinSN.Application.Contracts.Persistance;
using BDJoinSN.Application.Exceptions;
using MediatR;
using Microsoft.Extensions.Logging;

namespace BDJoinSN.Application.Features.FriendRequests.Commands.DeleteFriend
{
    public class DeleteFriendHandler : IRequestHandler<DeleteFriendCommand, bool>
    {

        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<DeleteFriendHandler> _logger;

        public DeleteFriendHandler(IUnitOfWork unitOfWork, ILogger<DeleteFriendHandler> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task<bool> Handle(DeleteFriendCommand request, CancellationToken cancellationToken)
        {
            try
            {
                
                if (request == null)
                    throw new BadRequestException("La solicitud no puede estar vacía.");

                
                if (string.IsNullOrEmpty(request.UserId))
                    throw new BadRequestException("El ID del usuario es requerido.");

                
                if (string.IsNullOrEmpty(request.FriendId))
                    throw new BadRequestException("El ID del amigo es requerido.");

                
                if (request.UserId == request.FriendId)
                    throw new BadRequestException("No puedes eliminar tu propia amistad.");

                
                var friendRequest = await _unitOfWork.FriendRepository.GetAcceptedFriendRequestAsync(
                    request.UserId,
                    request.FriendId);

                
                if (friendRequest == null)
                    throw new NotFoundException("Amistad", $"{request.UserId} - {request.FriendId}");

                
                if (friendRequest.SenderId != request.UserId && friendRequest.ReceiverId != request.UserId)
                    throw new BadRequestException("No tienes permiso para eliminar esta amistad.");

                
                _unitOfWork.FriendRepository.DeleteFR(friendRequest);
                await _unitOfWork.Complete();

                
                _logger.LogInformation($"Amistad entre {request.UserId} y {request.FriendId} eliminada por {request.UserId}");

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al eliminar amistad entre {UserId} y {FriendId}",
                    request.UserId, request.FriendId);
                throw;
            }
        }
    }
}
