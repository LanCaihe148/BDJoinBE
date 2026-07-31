using BDJoinSN.Application.Contracts.Identity;

namespace BDJoinSN.API.Middleware
{
    public class TokenBlacklistMiddleware
    {
        private readonly RequestDelegate _next;

        public TokenBlacklistMiddleware(RequestDelegate next)
        {
            _next = next;
        }


        public async Task InvokeAsync(HttpContext context, ITokenBlacklistService blacklistservice)
        {
            var token = context.Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();

            if (!string.IsNullOrEmpty(token))
            {
                if (await blacklistservice.IsTokenBlacklistedAsync(token))
                {
                    context.Response.StatusCode = 401;
                    await context.Response.WriteAsync("Token invalidio o expirado");
                    return;
                }


            }
            await _next(context);
        }

    }
}
