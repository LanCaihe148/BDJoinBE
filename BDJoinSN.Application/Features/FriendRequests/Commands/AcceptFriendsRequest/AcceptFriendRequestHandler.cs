using BDJoinSN.Application.Contracts.Persistance;
using BDJoinSN.Application.Exceptions;
using BDJoinSN.Domain;
using MediatR;
using Microsoft.Extensions.Logging;

namespace BDJoinSN.Application.Features.FriendRequests.Commands.AccepFriendsRequest
{
    public class AcceptFriendRequestHandler : IRequestHandler<AcceptFriendRequestCommand, bool>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<AcceptFriendRequestHandler> _logger;

        public AcceptFriendRequestHandler(IUnitOfWork unitOfWork, ILogger<AcceptFriendRequestHandler> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task<bool> Handle(AcceptFriendRequestCommand request, CancellationToken cancellationToken)
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
                    throw new BadRequestException("No tienes permiso para aceptar esta solicitud.");

                
                if (friendRequest.Status != FriendRequestStatus.Pending)
                    throw new BadRequestException("Esta solicitud ya ha sido procesada.");

                
                var areFriends = await _unitOfWork.FriendRepository.AreFriendsAsync(
                    friendRequest.SenderId,
                    friendRequest.ReceiverId);

                if (areFriends)
                    throw new BadRequestException("Ya son amigos.");

                
                friendRequest.Status = FriendRequestStatus.Accepted;
                friendRequest.UpdatedAt = DateTime.UtcNow;

                
                _unitOfWork.FriendRepository.UpdateFR(friendRequest);
                await _unitOfWork.Complete();

                _logger.LogInformation($"Solicitud de amistad {requestId} aceptada por usuario {request.UserId}");

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al aceptar solicitud de amistad {RequestId}", request.RequestId);
                throw;
            }
        }    
    }
}
