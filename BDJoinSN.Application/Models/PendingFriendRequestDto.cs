
namespace BDJoinSN.Application.Models
{
    public class PendingFriendRequestDto
    {
        public int RequestId { get; set; }
        public string SenderId { get; set; } = string.Empty;
        public string SenderUsername { get; set; } = string.Empty;
        public string? SenderDisplayName { get; set; }
        public string? SenderProfileImageUrl { get; set; }
        public DateTime CreatedAt { get; set; }
        public string Status { get; set; } = string.Empty;
    }
}
