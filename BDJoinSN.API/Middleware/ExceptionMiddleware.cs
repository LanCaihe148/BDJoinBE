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

                var statusCode = (int)HttpStatusCode.InternalServerError;
                var message = "Ocurrió un error interno. Intenta de nuevo más tarde.";
                string? details = null;

                switch (ex)
                {
                    case NotFoundException:
                        statusCode = (int)HttpStatusCode.NotFound;
                        message = ex.Message;
                        break;

                    case ValidationException validationException:
                        statusCode = (int)HttpStatusCode.BadRequest;
                        message = ex.Message;
                        details = JsonConvert.SerializeObject(validationException.Errors);
                        break;

                    case BadRequestException:
                        statusCode = (int)HttpStatusCode.BadRequest;
                        message = ex.Message; 
                        break;

                    default:
                        
                        if (_env.IsDevelopment())
                        {
                            message = ex.Message;
                            details = ex.StackTrace;
                        }
                        break;
                }

                var result = JsonConvert.SerializeObject(new CodeErrorException(statusCode, message, details));

                context.Response.StatusCode = statusCode;
                await context.Response.WriteAsync(result);
            }
        }
    }
}
