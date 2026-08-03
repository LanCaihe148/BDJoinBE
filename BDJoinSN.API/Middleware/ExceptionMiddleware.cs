using BDJoinSN.API.Errors;
using BDJoinSN.Application.Exceptions;
using Newtonsoft.Json;
using System;
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
                        statusCode = (int)HttpStatusCode.NotFound; // 404
                        message = ex.Message;
                        break;

                    case ValidationException validationException:
                        statusCode = (int)HttpStatusCode.BadRequest; // 400
                        message = ex.Message;
                        details = JsonConvert.SerializeObject(validationException.Errors);
                        break;

                    case BadRequestException:
                        statusCode = (int)HttpStatusCode.BadRequest; // 400
                        message = ex.Message;
                        break;

                    case ForbiddenException:
                        statusCode = (int)HttpStatusCode.Forbidden; // 403
                        message = ex.Message;
                        break;

                    case UnauthorizedAccessException:
                        statusCode = (int)HttpStatusCode.Unauthorized; // 401
                        message = ex.Message;
                        break;

                    default:
                        statusCode = (int)HttpStatusCode.InternalServerError; // 500
                        if (_env.IsDevelopment())
                        {
                            message = ex.Message;
                            details = ex.StackTrace;
                        }
                        else
                        {
                            message = "Ocurrió un error interno. Intenta de nuevo más tarde.";
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
