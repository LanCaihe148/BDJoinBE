using BDJoinSN.Application.Models;
using BDJoinSN.Application.Models.Pagination;
using BDJoinSN.Domain;
using MediatR;

namespace BDJoinSN.Application;

public class GetFriendSuggestionsQuery : IRequest<PaginatedResult<FriendDto>>
{
    public string Id {get; set;} = string.Empty;

    public int pageIndex {get; set;} 
    public int pageSize {get; set;}
    public int totalCount {get; set;} 
}