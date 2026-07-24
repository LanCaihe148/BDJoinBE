
using BDJoinSN.Domain.Common;

namespace BDJoinSN.Domain
{
    public class FriendRequest : BaseDomainModel<int>
    {
        public string SenderId { get; set; } = string.Empty; 
        public string ReceiverId { get; set; } = string.Empty; 
        public FriendRequestStatus Status { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }

        public virtual UserProfile Sender { get; set; } = null!;
        public virtual UserProfile Receiver { get; set; } = null!;
    }
}
