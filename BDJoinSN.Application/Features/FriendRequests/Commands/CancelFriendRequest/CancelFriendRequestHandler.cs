using BDJoinSN.Application.Contracts.Persistance;
using BDJoinSN.Application.Exceptions;
using BDJoinSN.Domain;
using MediatR;
using Microsoft.Extensions.Logging;

namespace BDJoinSN.Application.Features.FriendRequests.Commands.CancelFriendRequest
{
    public class CancelFriendRequestHandler : IRequestHandler<CancelFriendRequestCommand, bool>
    {
        private readonly IUnitOfWork _unitOfWork;

        private readonly ILogger<CancelFriendRequestHandler> _logger;

        public CancelFriendRequestHandler(IUnitOfWork unitOfWork, ILogger<CancelFriendRequestHandler> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task<bool> Handle(CancelFriendRequestCommand request, CancellationToken cancellationToken)
        {
            try
            {
                if (request == null)
                {
                    throw new BadRequestException("La solicitud no puede estar vacia.");
                }

                if (string.IsNullOrEmpty(request.RequestId))
                {
                    throw new BadRequestException("El ID de la solicitud es requerido");
                }

                if (string.IsNullOrEmpty(request.UserId))
                {
                    throw new BadRequestException("El User Id de la solicitud es requerido");
                }

                if (!int.TryParse(request.RequestId, out int requestId)) {
                    throw new BadRequestException("El ID de la solicitud debe ser un número válido.");
                }

                var friendRequest = await _unitOfWork.FriendRepository.GetByIdAsnc(requestId);
                
                if (friendRequest == null)
                    throw new NotFoundException(nameof(FriendRequest), requestId);

                
                if (friendRequest.SenderId != request.UserId)
                {
                    _logger.LogWarning($"Usuario {request.UserId} intentó cancelar solicitud {requestId} sin ser el remitente. Remitente real: {friendRequest.SenderId}");
                    throw new BadRequestException("No tienes permiso para cancelar esta solicitud. Solo el remitente puede hacerlo.");
                }

                
                if (friendRequest.Status != FriendRequestStatus.Pending)
                {
                    _logger.LogWarning($"Usuario {request.UserId} intentó cancelar solicitud {requestId} que no está pendiente. Estado actual: {friendRequest.Status}");
                    throw new BadRequestException($"Esta solicitud ya ha sido procesada. Estado actual: {friendRequest.Status}");
                }

                
                _unitOfWork.FriendRepository.DeleteFR(friendRequest);
                await _unitOfWork.Complete();

                _logger.LogInformation($"Solicitud de amistad {requestId} cancelada por usuario {request.UserId}");

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al cancelar solicitud de amistad {RequestId}", request.RequestId);
                throw;
            }

        }
    }
}
