using BDJoinSN.Domain.Common;

namespace BDJoinSN.Domain
{
    public class UserProfile : BaseDomainModel<string>
    {
        public string? Name { get; set; }
        public string? LastName { get; set; }

        public string? UserName { get; set; }
        public string? Biography { get; set; }
        public string? ProfileImageUrl { get; set; }
        public string? DisplayName { get; set; }
        public string? Location { get; set; }

        public DateTime? Birthday { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }

        
        public virtual ICollection<FriendRequest> SentFriendRequests { get; set; } = new List<FriendRequest>();
        public virtual ICollection<FriendRequest> ReceivedFriendRequests { get; set; } = new List<FriendRequest>();
    }
}
