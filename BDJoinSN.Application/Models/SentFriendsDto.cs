
namespace BDJoinSN.Application.Models
{
    public class SentFriendsDto
    {
        public int RequestId { get; set; }
        public string ReceiverId { get; set; } = string.Empty;
        public string ReceiverUsername { get; set; } = string.Empty;
        public string? ReceiverDisplayName { get; set; }
        public string? ReceiverProfileImageUrl { get; set; }
        public DateTime CreatedAt { get; set; }
        public string Status { get; set; } = string.Empty;
    }
}
