using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace BDJoinSN.Domain.Common
{
    public abstract class BaseDomainModel<TId>
    {
        public TId Id { get; set; } = default!;

        public DateTimeOffset? CreatedDate { get; set; }
        public string? CreatedBy { get; set; }
        public DateTimeOffset? LastModifiedDate { get; set; }
        public string? LastModifiedBy { get; set; }

    }
}
