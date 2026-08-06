
using BDJoinSN.Application.Models;
using BDJoinSN.Application.Models.Pagination;
using MediatR;

namespace BDJoinSN.Application.Features.FriendRequests.Queries.GetSentRequest
{
    public class GetSentRequestQuery : IRequest<PaginatedResult<SentFriendsDto>>
    {
        public string UserId { get; set; } = string.Empty;

        public int PageIndex { get; set; } = 1;

        public int PageSize { get; set; } = 10;
    }
}
