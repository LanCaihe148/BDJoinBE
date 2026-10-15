using BDJoinSN.Application.Contracts.Persistance;
using BDJoinSN.Application.Exceptions;
using BDJoinSN.Application.Models;
using BDJoinSN.Application.Models.Pagination;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace BDJoinSN.Application;

public class GetFriendSuggestionHandler : IRequestHandler<GetFriendSuggestionsQuery, PaginatedResult<FriendDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<GetFriendSuggestionHandler> _logger;

    public GetFriendSuggestionHandler(IUnitOfWork unitOfWork, ILogger<GetFriendSuggestionHandler> logger)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<PaginatedResult<FriendDto>> Handle(GetFriendSuggestionsQuery request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrEmpty(request.Id))
        {
            throw new NullOrEmptyException(request.Id);
        }

        var friends = await _unitOfWork.FriendRepository.GetFriendSuggDtos(request.Id);
        
        var results = new PaginatedResult<FriendDto>
        {
            PageIndex = request.pageIndex,
            PageSize = request.pageSize,
            TotalCount = friends.Count(),
            Items = friends
        };

        return results;
    
        
    }
}
