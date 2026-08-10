using AutoMapper;
using BDJoinSN.Application.Contracts.Persistance;
using BDJoinSN.Application.Features.Posts.Queries.GetPostsById;
using MediatR;
using Microsoft.Extensions.Logging;


namespace BDJoinSN.Application.Features.Posts.Queries.GetPostsByFriend
{
    public class GetPostByFriendHandler : IRequestHandler<GetPostByFriendQuery, FeedResponse>
    {

        private readonly IUnitOfWork _unitOfWork;

        private readonly IMapper _mapper;

        private readonly ILogger<GetPostByFriendHandler> _logger;

        public GetPostByFriendHandler(IUnitOfWork unitOfWork, IMapper mapper, ILogger<GetPostByFriendHandler> logger)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<FeedResponse> Handle(GetPostByFriendQuery request, CancellationToken cancellationToken)
        {
            try
            {
                
                var friendIds = await GetFriendIdsAsync(request.UserId);

                if (!friendIds.Any())
                {
                    var minePosts = await _unitOfWork.PostRepository.GetAsync(p => p.UserId == request.UserId);
                    var mPostCount = await _unitOfWork.PostRepository.CountOwnPosts(request.UserId);

                    var totalOwnPostsPages = (int)Math.Ceiling((double)mPostCount / request.PageSize);
                    var mPostDtos = _mapper.Map<List<PostDto>>(minePosts);
                    _logger.LogInformation("El usuario {UserId} no tiene amigos", request.UserId);
                    return new FeedResponse
                    {
                        Items = mPostDtos,
                        TotalCount = mPostCount,
                        PageIndex = request.PageIndex,
                        PageSize = request.PageSize,
                        TotalPages = totalOwnPostsPages,
                        HasNextPage = request.PageIndex < totalOwnPostsPages,
                        HasPreviousPage = request.PageIndex > 1
                    };
                }

                
                var totalCount = await _unitOfWork.PostRepository.GetFeedCountAsync(friendIds, request.UserId);

                
                var posts = await _unitOfWork.PostRepository.GetFeedAsync(
                    friendIds,
                    request.UserId,
                    request.PageIndex,
                    request.PageSize);


               
                
                var postDtos = _mapper.Map<List<PostDto>>(posts);

                
                var totalPages = (int)Math.Ceiling((double)totalCount / request.PageSize);

                _logger.LogInformation("Feed obtenido para usuario {UserId}: {Count} posts (página {PageIndex} de {TotalPages})",
                    request.UserId, postDtos.Count, request.PageIndex, totalPages);

                return new FeedResponse
                {
                    Items = postDtos,
                    TotalCount = totalCount,
                    PageIndex = request.PageIndex,
                    PageSize = request.PageSize,
                    TotalPages = totalPages,
                    HasNextPage = request.PageIndex < totalPages,
                    HasPreviousPage = request.PageIndex > 1
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener feed para usuario {UserId}", request.UserId);
                throw;
            }
        }

        private async Task<List<string>> GetFriendIdsAsync(string userId)
        {
            var friendRequests = await _unitOfWork.FriendRepository.GetAllFriendsAsync(userId);

            var friendIds = friendRequests
                .Select(fr => fr.SenderId == userId ? fr.ReceiverId : fr.SenderId)
                .ToList();

            return friendIds;
        }
    }
}
