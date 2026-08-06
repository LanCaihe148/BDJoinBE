using AutoMapper;
using BDJoinSN.Application.Contracts.Persistance;
using BDJoinSN.Application.Exceptions;
using BDJoinSN.Application.Models;
using BDJoinSN.Application.Models.Pagination;
using MediatR;
using Microsoft.Extensions.Logging;

namespace BDJoinSN.Application.Features.FriendRequests.Queries.GetSentRequest
{
    public class GetSentRequestHandler : IRequestHandler<GetSentRequestQuery, PaginatedResult<SentFriendsDto>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger<GetSentRequestHandler> _logger;

        public GetSentRequestHandler(IUnitOfWork unitOfWork, IMapper mapper, ILogger<GetSentRequestHandler> logger)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<PaginatedResult<SentFriendsDto>> Handle(GetSentRequestQuery request, CancellationToken cancellationToken)
        {
            try
            {
                if (string.IsNullOrEmpty(request.UserId))
                {
                    throw new BadRequestException("El Id del usuario es requerido");
                }

                var sentRequest = await _unitOfWork.FriendRepository.GetSentRequestAsync(request.UserId);

                if (!sentRequest.Any())
                {
                    return new PaginatedResult<SentFriendsDto>(
                        new List<SentFriendsDto>(),
                        0,
                        request.PageIndex,
                        request.PageSize);
                }

                var result = new List<SentFriendsDto>();
                foreach (var fr in sentRequest)
                {
                    var receiverInfo = await _unitOfWork.ProfileRepository.GetUserInfoByIdAsync(fr.ReceiverId);

                    if (receiverInfo == null)
                    {
                        continue;
                    }

                    result.Add(new SentFriendsDto
                    {
                        RequestId = fr.Id,
                        ReceiverId = fr.ReceiverId,
                        ReceiverUsername = receiverInfo.Username ?? string.Empty,
                        ReceiverDisplayName = receiverInfo.DisplayName ?? receiverInfo.Username,
                        ReceiverProfileImageUrl = receiverInfo.ProfileImageUrl,
                        CreatedAt = fr.CreatedAt,
                        Status = fr.Status.ToString()
                    });

                }
                    result = result.OrderByDescending(r => r.CreatedAt).ToList();

                    var totalCount = result.Count;
                    var pagedItems = result.
                        Skip((request.PageIndex - 1) * request.PageSize).Take(request.PageSize).ToList();

                    _logger.LogInformation($"Solicitudes salientes obtenidas para usuario {request.UserId}: {pagedItems.Count} de {totalCount}");

                    return new PaginatedResult<SentFriendsDto>(
                        pagedItems,
                        totalCount,
                        request.PageIndex,
                        request.PageSize);
                }
                catch (Exception ex) {
                _logger.LogError(ex, "Error al obtener solicitudes salientes para usuario {UserId}", request.UserId);
                throw;
                }   

                
            
        }
    }
}
