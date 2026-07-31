
using System.Net;

namespace BDJoinSN.Application.Exceptions
{
    public class ForbiddenException : Exception
    {
        public HttpStatusCode StatusCode { get; }

        public ForbiddenException(string message)
            : base(message)
        {
            StatusCode = HttpStatusCode.Forbidden;
        }
    }
}
