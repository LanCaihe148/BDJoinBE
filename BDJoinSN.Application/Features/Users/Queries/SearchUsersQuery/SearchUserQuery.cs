using BDJoinSN.Application.Models.Identity;
using BDJoinSN.Application.Models.Pagination;
using MediatR;

namespace BDJoinSN.Application.Features.Users.Queries.SearchUsersQuery
{
    public class SearchUserQuery : IRequest<PaginatedResult<UserSearchResult>>
    {
        public string SearchTerm { get; set; } = string.Empty;
        public string CurrentUserId { get; set; } = string.Empty;
        public int PageIndex { get; set; } = 1;
        public int PageSize { get; set; } = 10;

        public SearchUserQuery()
        {
        }

        public SearchUserQuery(string searchTerm, string currentUserId, int pageIndex = 1, int pageSize = 10)
        {
            SearchTerm = searchTerm;
            CurrentUserId = currentUserId;
            PageIndex = pageIndex;
            PageSize = pageSize;
        }
    }
}
