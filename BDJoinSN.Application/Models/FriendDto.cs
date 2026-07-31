
namespace BDJoinSN.Application.Models
{
    public class FriendDto
    {
        public string UserId { get; set; } = string.Empty;
        public string Username { get; set; } = string.Empty;
        public string? DisplayName { get; set; }
        public string? AvatarUrl { get; set; }
        public DateTime? BecameFriendsAt { get; set; }
    }
}
