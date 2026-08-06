using BDJoinSN.Application.Models;
using BDJoinSN.Application.Models.Pagination;
using MediatR;

namespace BDJoinSN.Application.Features.FriendRequests.Queries.GetPendingRequests
{
    public class GetPendingRequestQuery : IRequest<PaginatedResult<PendingFriendRequestDto>>
    {
        public string UserId { get; set; } = string.Empty;

        public int PageIndex { get; set; } = 1;

        public int PageSize { get; set; } = 10;
    }
}
