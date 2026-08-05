
using AutoMapper;
using BDJoinSN.Application.Contracts.Persistance;
using BDJoinSN.Application.Exceptions;
using BDJoinSN.Application.Features.Posts.Queries.GetPostsById;
using BDJoinSN.Application.Models.Pagination;
using MediatR;

namespace BDJoinSN.Application.Features.Posts.Queries.GetAllPostByUsername
{
    public class GetAllPostByUsernameHandler : IRequestHandler<GetAllPostByUsernameQuery, PaginatedResult<PostDto>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        public GetAllPostByUsernameHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<PaginatedResult<PostDto>> Handle(GetAllPostByUsernameQuery request, CancellationToken cancellationToken)
        {

            if (string.IsNullOrEmpty(request.Username)){
                throw new NullOrEmptyException(nameof(request.Username));
            }

            var username = await _unitOfWork.UserRepository.GetByUsernameAsync(request.Username);
            if (username == null)
                throw new NotFoundException("Usuario", request.Username);

            var totalCount = await _unitOfWork.PostRepository.CountByUsernameAsync(request.Username);

            var posts = await _unitOfWork.PostRepository.GetAllPostByUsername(request.Username, request.PageIndex, request.PageSize);

            var postDtos = _mapper.Map<List<PostDto>>(posts);
            var totalPages = (int)Math.Ceiling((double)totalCount / request.PageSize);
            return new PaginatedResult<PostDto>(postDtos, totalCount, request.PageIndex, request.PageSize);
        }
    }
}
