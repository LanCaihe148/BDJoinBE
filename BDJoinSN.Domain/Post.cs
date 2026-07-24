using BDJoinSN.Domain.Common;

namespace BDJoinSN.Domain
{
    public class Post : BaseDomainModel<int>
    {
        public string UserId { get; set; } = string.Empty;
        public string? Author { get; set; }
        public string? Content { get; set; }

    }
}
