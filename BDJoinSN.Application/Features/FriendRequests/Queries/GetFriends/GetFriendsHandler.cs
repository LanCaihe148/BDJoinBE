
using AutoMapper;
using BDJoinSN.Application.Contracts.Persistance;
using BDJoinSN.Application.Exceptions;
using BDJoinSN.Application.Models;
using BDJoinSN.Application.Models.Pagination;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;

namespace BDJoinSN.Application.Features.FriendRequests.Queries.GetFriends
{
    public class GetFriendsHandler : IRequestHandler<GetFriendsQuery, PaginatedResult<FriendDto>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger<GetFriendsHandler> _logger;

        public GetFriendsHandler(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            ILogger<GetFriendsHandler> logger)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<PaginatedResult<FriendDto>> Handle(GetFriendsQuery request, CancellationToken cancellationToken)
        {
            try
            {
                
                if (string.IsNullOrEmpty(request.UserId))
                    throw new BadRequestException("El ID del usuario es requerido.");

                
                var friendRequests = await _unitOfWork.FriendRepository.GetAllFriendsAsync(request.UserId);

                if (!friendRequests.Any())
                {
                    return new PaginatedResult<FriendDto>(new List<FriendDto>(), 0, request.PageIndex, request.PageSize);
                }

                
                var friends = new List<FriendDto>();

                foreach (var fr in friendRequests)
                {
                    
                    var friendId = fr.SenderId == request.UserId ? fr.ReceiverId : fr.SenderId;

                    
                    var profile = await _unitOfWork.ProfileRepository.GetByUserIdAsync(friendId);
                    if (profile == null) continue;

                    friends.Add(new FriendDto
                    {
                        UserId = profile.Id,
                        Username = profile.UserName ?? string.Empty,
                        DisplayName = profile.DisplayName ?? profile.UserName,
                        AvatarUrl = profile.ProfileImageUrl,
                        BecameFriendsAt = fr.UpdatedAt ?? fr.CreatedAt
                    });
                }

                
                if (!string.IsNullOrWhiteSpace(request.SearchTerm))
                {
                    var term = request.SearchTerm.ToLowerInvariant();
                    friends = friends.Where(f =>
                        f.Username.ToLowerInvariant().Contains(term) ||
                        (f.DisplayName != null && f.DisplayName.ToLowerInvariant().Contains(term))
                    ).ToList();
                }

                
                friends = request.SortBy?.ToLower() switch
                {
                    "displayname" => request.SortDescending
                        ? friends.OrderByDescending(f => f.DisplayName).ToList()
                        : friends.OrderBy(f => f.DisplayName).ToList(),
                    "becamefriendsat" => request.SortDescending
                        ? friends.OrderByDescending(f => f.BecameFriendsAt).ToList()
                        : friends.OrderBy(f => f.BecameFriendsAt).ToList(),
                    _ => request.SortDescending
                        ? friends.OrderByDescending(f => f.Username).ToList()
                        : friends.OrderBy(f => f.Username).ToList()
                };

                
                var totalCount = friends.Count;
                var pagedItems = friends
                    .Skip((request.PageIndex - 1) * request.PageSize)
                    .Take(request.PageSize)
                    .ToList();

                _logger.LogInformation($"Lista de amigos obtenida para usuario {request.UserId}: {pagedItems.Count} de {totalCount}");

                return new PaginatedResult<FriendDto>(pagedItems, totalCount, request.PageIndex, request.PageSize);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener lista de amigos para usuario {UserId}", request.UserId);
                throw;
            }
        }
    }
}
