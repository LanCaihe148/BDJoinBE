using AutoMapper;
using BDJoinSN.Application.Contracts.Persistance;
using BDJoinSN.Application.Exceptions;
using BDJoinSN.Application.Features.FriendRequests.Queries.GetSentRequest;
using BDJoinSN.Application.Models;
using BDJoinSN.Application.Models.Pagination;
using BDJoinSN.Domain;
using MediatR;  
using Microsoft.Extensions.Logging;

namespace BDJoinSN.Application.Features.FriendRequests.Queries.GetPendingRequests
{
    public class GetPendingRequestHandler : IRequestHandler<GetPendingRequestQuery, PaginatedResult<PendingFriendRequestDto>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger<GetPendingRequestHandler> _logger;

        public GetPendingRequestHandler(IUnitOfWork unitOfWork, IMapper mapper, ILogger<GetPendingRequestHandler> logger)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<PaginatedResult<PendingFriendRequestDto>> Handle(GetPendingRequestQuery request, CancellationToken cancellationToken)
        {
            try
            {
                if (string.IsNullOrEmpty(request.UserId))
                    throw new BadRequestException("El ID del usuario es requerido.");

                
                var pendingRequests = await _unitOfWork.FriendRepository.GetPendingRequestsForUserAsync(request.UserId);

                if (!pendingRequests.Any())
                {
                    return new PaginatedResult<PendingFriendRequestDto>(
                        new List<PendingFriendRequestDto>(),
                        0,
                        request.PageIndex,
                        request.PageSize);
                }

                
                var result = new List<PendingFriendRequestDto>();
                foreach (var fr in pendingRequests)
                {
                    var senderInfo = await _unitOfWork.ProfileRepository.GetUserInfoByIdAsync(fr.SenderId);

                    if (senderInfo == null) continue;

                    result.Add(new PendingFriendRequestDto
                    {
                        RequestId = fr.Id,
                        SenderId = fr.SenderId,
                        SenderUsername = senderInfo.Username ?? string.Empty,
                        SenderDisplayName = senderInfo.DisplayName ?? senderInfo.Username,
                        SenderProfileImageUrl = senderInfo.ProfileImageUrl,
                        CreatedAt = fr.CreatedAt,
                        Status = fr.Status.ToString()
                    });
                }

                
                result = result
                    .OrderByDescending(r => r.CreatedAt)
                    .ToList();

               
                var totalCount = result.Count;
                var pagedItems = result
                    .Skip((request.PageIndex - 1) * request.PageSize)
                    .Take(request.PageSize)
                    .ToList();

                _logger.LogInformation($"Solicitudes entrantes obtenidas para usuario {request.UserId}: {pagedItems.Count} de {totalCount}");

                return new PaginatedResult<PendingFriendRequestDto>(
                    pagedItems,
                    totalCount,
                    request.PageIndex,
                    request.PageSize);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener solicitudes entrantes para usuario {UserId}", request.UserId);
                throw;
            }
        }
    }
}
