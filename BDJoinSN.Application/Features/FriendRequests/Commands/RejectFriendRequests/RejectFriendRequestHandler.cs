using BDJoinSN.Application.Contracts.Persistance;
using BDJoinSN.Application.Exceptions;
using BDJoinSN.Domain;
using MediatR;
using Microsoft.Extensions.Logging;

namespace BDJoinSN.Application.Features.FriendRequests.Commands.RejectFriendRequests
{
    public class RejectFriendRequestHandler : IRequestHandler<RejectFriendRequestCommand, bool>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<RejectFriendRequestHandler> _logger;

        public RejectFriendRequestHandler(IUnitOfWork unitOfWork, ILogger<RejectFriendRequestHandler> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task<bool> Handle(RejectFriendRequestCommand request, CancellationToken cancellationToken)
        {
            try
            {
                
                if (request == null)
                    throw new BadRequestException("La solicitud no puede estar vacía.");

                
                if (string.IsNullOrEmpty(request.RequestId))
                    throw new BadRequestException("El ID de la solicitud es requerido.");

                
                if (string.IsNullOrEmpty(request.UserId))
                    throw new BadRequestException("El ID del usuario es requerido.");

                
                if (!int.TryParse(request.RequestId, out int requestId))
                    throw new BadRequestException("El ID de la solicitud debe ser un número válido.");

                var friendRequest = await _unitOfWork.FriendRepository.GetByIdAsnc(requestId);

                
                if (friendRequest == null)
                    throw new NotFoundException(nameof(FriendRequest), requestId);

                
                if (friendRequest.ReceiverId != request.UserId)
                {
                    _logger.LogWarning($"Usuario {request.UserId} intentó rechazar solicitud {requestId} sin ser el receptor. Receptor real: {friendRequest.ReceiverId}");
                    throw new BadRequestException("No tienes permiso para rechazar esta solicitud. Solo el destinatario puede rechazarla.");
                }

                
                if (friendRequest.Status != FriendRequestStatus.Pending)
                {
                    _logger.LogWarning($"Usuario {request.UserId} intentó rechazar solicitud {requestId} que no está pendiente. Estado actual: {friendRequest.Status}");
                    throw new BadRequestException($"Esta solicitud ya ha sido procesada. Estado actual: {friendRequest.Status}");
                }

                
                friendRequest.Status = FriendRequestStatus.Rejected;
                friendRequest.UpdatedAt = DateTime.UtcNow;

                
                _unitOfWork.FriendRepository.UpdateFR(friendRequest);
                await _unitOfWork.Complete();

                _logger.LogInformation($"Solicitud de amistad {requestId} rechazada por usuario {request.UserId}");

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al rechazar solicitud de amistad {RequestId}", request.RequestId);
                throw;
            }

        }
    }
}
