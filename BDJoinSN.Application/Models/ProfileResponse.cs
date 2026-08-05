using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BDJoinSN.Application.Models
{
    public class ProfileResponse
    {
        public string UserId { get; set; } = string.Empty;
        public string UserName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string? Name { get; set; }
        public string? LastName { get; set; }
        public string? Biography { get; set; }
        public string? ProfileImageUrl { get; set; }

        public string? City { get; set; }
        public DateTime? Birthday { get; set; }
        public List<FriendSummaryResponse> RecentFriends { get; set; } = new();
        public DateTime CreatedAt { get; set; }
    }
}
