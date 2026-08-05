using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BDJoinSN.Application.Exceptions
{
    public class NullOrEmptyException : ApplicationException
    {
        public NullOrEmptyException(string? paramName) : base($"the {paramName} value can't be NULL or EMPTY")
        {
        }
    }
}
