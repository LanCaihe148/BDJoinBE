namespace BDJoinSN.Domain
{
    public class UserProfile
    {
        public string Id { get; set; } = string.Empty; 
        public string? Name { get; set; }
        public string? LastName { get; set; }
        public string? Biography { get; set; }
        public string? ProfileImageUrl { get; set; }
        public string? DisplayName { get; set; }
        public string? Location { get; set; }
        public string? Website { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }

        
        public virtual ICollection<FriendRequest> SentFriendRequests { get; set; } = new List<FriendRequest>();
        public virtual ICollection<FriendRequest> ReceivedFriendRequests { get; set; } = new List<FriendRequest>();
    }
}
