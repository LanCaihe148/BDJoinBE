using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BDJoinSN.Application.Models
{
    public class PublicProfileResponse
    {
        public string UserName { get; set; } = string.Empty;
        public string? Name { get; set; }
        public string? LastName { get; set; }
        public string? Biography { get; set; }
        public string? City { get; set; }
        public string? ProfileImageUrl { get; set; }

        public int FriendsCount { get; set; }
        public List<FriendSummaryResponse> RecentFriends { get; set; } = new();
        public RelationshipStatus? RelationshipStatus { get; set; }
    }
}
