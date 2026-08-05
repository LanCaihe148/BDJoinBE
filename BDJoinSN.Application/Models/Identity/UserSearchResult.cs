
namespace BDJoinSN.Application.Models.Identity
{
    public class UserSearchResult
    {
        public string UserId { get; set; } = string.Empty;
        public string Username { get; set; } = string.Empty;
        public string? DisplayName { get; set; }
        public string? Name { get; set; }
        public string? LastName { get; set; }
        public string? Biography { get; set; }
        public string? ProfileImageUrl { get; set; }
        public string? RelationshipStatus { get; set; } 
        public bool IsDeleted { get; set; }
    }
}
