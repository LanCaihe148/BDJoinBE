

using AutoMapper;
using BDJoinSN.Application.Contracts.Persistance;
using BDJoinSN.Application.Exceptions;
using BDJoinSN.Domain;
using MediatR;
using Microsoft.Extensions.Logging;

namespace BDJoinSN.Application.Features.Posts.Commands.CreatePost
{
    public class CreatePostCommandHandler : IRequestHandler<CreatePostCommand, int>
    {
        private readonly IUnitOfWork _unitOfWork;
        private IMapper _mapper;
        private readonly ILogger<CreatePostCommandHandler> _logger;

        public CreatePostCommandHandler(IUnitOfWork unitOfWork, IMapper mapper, ILogger<CreatePostCommandHandler> logger)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<int> Handle(CreatePostCommand request, CancellationToken cancellationToken)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(request.UserId))
                    throw new BadRequestException("El UserId es requerido");

                if (string.IsNullOrWhiteSpace(request.Content))
                    throw new BadRequestException("El contenido del post no puede estar vacío");

                var postEntity = _mapper.Map<Post>(request);

                _unitOfWork.PostRepository.AddEntity(postEntity);

                var result = await _unitOfWork.Complete();

                if (result <= 0)
                {
                    throw new Exception("No se pudo crear el post. No se guardaron cambios.");
                }

                _logger.LogInformation("Post {PostId} creado exitosamente por usuario {UserId}",
                    postEntity.Id, request.UserId);

                return postEntity.Id;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear post para usuario {UserId}", request.UserId);
                throw;
            }
        }
    }
}
