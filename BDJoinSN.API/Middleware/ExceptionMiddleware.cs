using BDJoinSN.API.Errors;
using BDJoinSN.Application.Exceptions;
using Newtonsoft.Json;
using System.Net;

namespace BDJoinSN.API.Middleware
{
    public class ExceptionMiddleware
    {

        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionMiddleware> _logger;
        private readonly IHostEnvironment _env;

        public ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger, IHostEnvironment env)
        {
            _next = next;
            _logger = logger;
            _env = env;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                context.Response.ContentType = "application/json";
                var StatusCode = (int)HttpStatusCode.InternalServerError;
                var result = string.Empty;

                switch (ex)
                {
                    case NotFoundException notFoundException:
                        StatusCode = (int)HttpStatusCode.NotFound;
                        break;

                    case ValidationException validationException:
                        StatusCode = (int)HttpStatusCode.BadRequest;
                        var validationJson = JsonConvert.SerializeObject(validationException.Errors);
                        result = JsonConvert.SerializeObject(new CodeErrorException(StatusCode, ex.Message, validationJson));
                        break;

                    case BadRequestException badRequestException:
                        StatusCode = (int)HttpStatusCode.BadRequest;
                        break;

                    default:
                        break;
                }

                if (string.IsNullOrEmpty(result))
                {
                    result = JsonConvert.SerializeObject(new CodeErrorException(StatusCode, ex.Message, ex.StackTrace));

                }


                context.Response.StatusCode = StatusCode;

                await context.Response.WriteAsync(result);
            }
        }
    }
}
