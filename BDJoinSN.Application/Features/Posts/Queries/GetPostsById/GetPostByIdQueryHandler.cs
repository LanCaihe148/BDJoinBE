

using AutoMapper;
using BDJoinSN.Application.Contracts.Persistance;
using BDJoinSN.Application.Exceptions;
using BDJoinSN.Domain;
using MediatR;
using Microsoft.Extensions.Logging;

namespace BDJoinSN.Application.Features.Posts.Queries.GetPostsById
{
    public class GetPostByIdQueryHandler : IRequestHandler<GetPostByIdQuery, PostDto>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger<GetPostByIdQueryHandler> _logger;

        public GetPostByIdQueryHandler(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            ILogger<GetPostByIdQueryHandler> logger)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<PostDto> Handle(GetPostByIdQuery request, CancellationToken cancellationToken)
        {
            try
            {
                
                var post = await _unitOfWork.PostRepository.GetByIdAsync(request.Id);

                if (post == null)
                {
                    _logger.LogWarning("Post con ID {PostId} no encontrado", request.Id);
                    throw new NotFoundException(nameof(Post), request.Id);
                }

                
                var postDto = _mapper.Map<PostDto>(post);

                _logger.LogInformation("Post {PostId} obtenido exitosamente", request.Id);
                return postDto;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener post con ID {PostId}", request.Id);
                throw;
            }
        }
    }
}
