using BDJoinSN.Application.Contracts.Persistance;
using BDJoinSN.Application.Exceptions;
using BDJoinSN.Domain;
using MediatR;

namespace BDJoinSN.Application.Features.FriendRequests.Commands.CreateFriendRequests
{
    public class CreateFriendRequestHandler : IRequestHandler<CreateFriendRequestCommand, FriendRequestStatus>
    {
        private readonly IUnitOfWork _unitOfWork;

        public CreateFriendRequestHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<FriendRequestStatus> Handle(CreateFriendRequestCommand request, CancellationToken cancellationToken)
        {
            
            if (request == null)
                throw new BadRequestException("La solicitud no puede estar vacía.");

            if (string.IsNullOrEmpty(request.SenderId))
                throw new BadRequestException("El ID del remitente es requerido.");

            if (string.IsNullOrEmpty(request.ReceiverId))
                throw new BadRequestException("El ID del destinatario es requerido.");

            if (request.SenderId == request.ReceiverId)
                throw new BadRequestException("No puedes enviarte una solicitud de amistad a ti mismo.");

           
            var areFriends = await _unitOfWork.FriendRepository.AreFriendsAsync(
                request.SenderId,
                request.ReceiverId);

            if (areFriends)
                throw new BadRequestException("Ya son amigos.");

            
            var hasPending = await _unitOfWork.FriendRepository.HasPendingRequestAsync(
                request.SenderId,
                request.ReceiverId);

            if (hasPending)
                throw new BadRequestException("Ya hay una solicitud de amistad pendiente.");

           
            var friendRequest = new FriendRequest
            {
                SenderId = request.SenderId,
                ReceiverId = request.ReceiverId,
                Status = FriendRequestStatus.Pending,
                CreatedAt = DateTime.UtcNow
            };

            
            _unitOfWork.FriendRepository.AddFriendRequest(friendRequest);

            
            await _unitOfWork.Complete();

            return FriendRequestStatus.Pending;
        }
    }
}
