

using BDJoinSN.Application.Contracts.Persistance;
using BDJoinSN.Application.Models.Identity;
using BDJoinSN.Application.Models.Pagination;
using MediatR;
using Microsoft.Extensions.Logging;

namespace BDJoinSN.Application.Features.Users.Queries.SearchUsersQuery
{
    public class SearchUserQueryHandler : IRequestHandler<SearchUserQuery, PaginatedResult<UserSearchResult>>
    {
        private readonly IUserRepository _userRepository;
        private readonly ILogger<SearchUserQueryHandler> _logger;

        public SearchUserQueryHandler(
            IUserRepository userRepository,
            ILogger<SearchUserQueryHandler> logger)
        {
            _userRepository = userRepository;
            _logger = logger;
        }

        public async Task<PaginatedResult<UserSearchResult>> Handle(
            SearchUserQuery request,
            CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogInformation("Buscando usuarios con término: {SearchTerm}", request.SearchTerm);

                var result = await _userRepository.SearchUsersAsync(
                    request.SearchTerm,
                    request.CurrentUserId,
                    request.PageIndex,
                    request.PageSize);

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al buscar usuarios con término: {SearchTerm}", request.SearchTerm);
                throw;
            }
        }
    }
}
