

namespace BDJoinSN.Application.Models
{
    public class UserInfoDto
    {
        public string Id { get; set; } = string.Empty;
        public string Username { get; set; } = string.Empty;
        public string? DisplayName { get; set; }
        public string? ProfileImageUrl { get; set; }
    }
}
