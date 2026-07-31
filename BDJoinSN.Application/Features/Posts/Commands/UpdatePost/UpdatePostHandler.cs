

using AutoMapper;
using BDJoinSN.Application.Contracts.Persistance;
using BDJoinSN.Application.Exceptions;
using BDJoinSN.Domain;
using MediatR;
using Microsoft.Extensions.Logging;

namespace BDJoinSN.Application.Features.Posts.Commands.UpdatePost
{
    public class UpdatePostHandler : IRequestHandler<UpdatePostCommand, bool>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger<UpdatePostHandler> _logger;

        public UpdatePostHandler(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            ILogger<UpdatePostHandler> logger)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<bool> Handle(UpdatePostCommand request, CancellationToken cancellationToken)
        {
            try
            {
                
                if (request == null)
                    throw new BadRequestException("La solicitud no puede estar vacía.");

                
                if (request.Id <= 0)
                    throw new BadRequestException("El ID del post debe ser un número positivo.");

               
                if (string.IsNullOrEmpty(request.UserId))
                    throw new BadRequestException("El ID del usuario es requerido.");

                
                if (string.IsNullOrWhiteSpace(request.Content))
                    throw new BadRequestException("El contenido del post no puede estar vacío.");

                
                var post = await _unitOfWork.PostRepository.GetByIdAsync(request.Id);

                
                if (post == null)
                    throw new NotFoundException(nameof(Post), request.Id);

                if (post.UserId != request.UserId)
                {
                    _logger.LogWarning($"Usuario {request.UserId} intentó modificar el post {request.Id} sin ser el autor.");
                    throw new ForbiddenException("No tienes permiso para modificar este post. Solo el autor puede hacerlo.");
                }

                
                var oldContent = post.Content;

                
                post.Content = request.Content;

                
                if (!string.IsNullOrWhiteSpace(request.Author))
                {
                    post.Author = request.Author;
                }

                
                post.LastModifiedDate = DateTimeOffset.UtcNow;
                post.LastModifiedBy = request.UserId;

                
                _unitOfWork.PostRepository.UpdateEntity(post);
                await _unitOfWork.Complete();

                _logger.LogInformation($"Post {request.Id} actualizado por usuario {request.UserId}. Contenido anterior: {oldContent?.Length ?? 0} caracteres.");

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al actualizar post {PostId} por usuario {UserId}",
                    request.Id, request.UserId);
                throw;
            }
        }
    }
}
