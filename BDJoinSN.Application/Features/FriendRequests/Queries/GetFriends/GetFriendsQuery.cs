
using BDJoinSN.Application.Models;
using BDJoinSN.Application.Models.Pagination;
using MediatR;

namespace BDJoinSN.Application.Features.FriendRequests.Queries.GetFriends
{
    public class GetFriendsQuery : IRequest<PaginatedResult<FriendDto>>
    {
        public string UserId { get; set; } = string.Empty;
        public int PageIndex { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public string? SearchTerm { get; set; }
        public string? SortBy { get; set; } = "Username";
        public bool SortDescending { get; set; } = false;
    }
}
