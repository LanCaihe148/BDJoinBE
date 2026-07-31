using BDJoinSN.Application.Contracts.Persistance;
using BDJoinSN.Application.Exceptions;
using BDJoinSN.Domain;
using MediatR;
using Microsoft.Extensions.Logging;

namespace BDJoinSN.Application.Features.Posts.Commands.DeletePost
{
    public class DeletePostHandler : IRequestHandler<DeletePostCommand, bool>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<DeletePostHandler> _logger;

        public DeletePostHandler(IUnitOfWork unitOfWork, ILogger<DeletePostHandler> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task<bool> Handle(DeletePostCommand request, CancellationToken cancellationToken)
        {
            try
            {
                
                if (request == null)
                    throw new BadRequestException("La solicitud no puede estar vacía.");

                
                if (request.PostId <= 0)
                    throw new BadRequestException("El ID del post debe ser un número positivo.");

                
                if (string.IsNullOrEmpty(request.UserId))
                    throw new BadRequestException("El ID del usuario es requerido.");

                
                var post = await _unitOfWork.PostRepository.GetByIdAsync(request.PostId);

                
                if (post == null)
                    throw new NotFoundException(nameof(Post), request.PostId);

                
                if (post.UserId != request.UserId)
                {
                    _logger.LogWarning($"Usuario {request.UserId} intentó eliminar el post {request.PostId} sin ser el autor.");
                    throw new BadRequestException("No tienes permiso para eliminar este post. Solo el autor puede hacerlo.");
                }

                
                _unitOfWork.PostRepository.DeleteEntity(post);
                await _unitOfWork.Complete();

                

                _logger.LogInformation($"Post {request.PostId} eliminado por usuario {request.UserId}");

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al eliminar post {PostId} por usuario {UserId}",
                    request.PostId, request.UserId);
                throw;
            }
        }
    }
}
