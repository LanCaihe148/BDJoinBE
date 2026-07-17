using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BDJoinSN.Application.Models
{
    public class FriendSummaryResponse
    {
        public string UserName { get; set; } = string.Empty;
        public string? Name { get; set; }
        public string? ProfileImageUrl { get; set; }
    }
}
