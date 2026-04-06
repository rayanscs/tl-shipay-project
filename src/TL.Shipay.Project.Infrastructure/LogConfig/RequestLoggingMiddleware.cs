using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.Diagnostics;

namespace TL.Shipay.Project.Infrastructure.LogConfig
{
    public sealed class RequestLoggingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<RequestLoggingMiddleware> _logger;

        public RequestLoggingMiddleware(RequestDelegate next, ILogger<RequestLoggingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var stopwatch = Stopwatch.StartNew();
            var method = context.Request.Method;
            var path = context.Request.Path;

            try
            {
                await _next(context);
                stopwatch.Stop();

                var statusCode = context.Response.StatusCode;

                using (_logger.BeginScope(new Dictionary<string, object>
                {
                    ["RequestMethod"] = method,
                    ["RequestPath"] = path.ToString(),
                    ["StatusCode"] = statusCode,
                    ["Elapsed"] = stopwatch.Elapsed.TotalMilliseconds
                }))
                {
                    if (statusCode >= 500)
                    {
                        _logger.LogError(
                            "HTTP {Method} {Path} respondeu {StatusCode} em {Elapsed:0.00}ms",
                            method, path, statusCode, stopwatch.Elapsed.TotalMilliseconds);
                    }
                    else
                    {
                        _logger.LogInformation(
                            "HTTP {Method} {Path} respondeu {StatusCode} em {Elapsed:0.00}ms",
                            method, path, statusCode, stopwatch.Elapsed.TotalMilliseconds);
                    }
                }
            }
            catch (Exception ex)
            {
                stopwatch.Stop();

                using (_logger.BeginScope(new Dictionary<string, object>
                {
                    ["RequestMethod"] = method,
                    ["RequestPath"] = path.ToString(),
                    ["Elapsed"] = stopwatch.Elapsed.TotalMilliseconds
                }))
                {
                    _logger.LogError(ex,
                        "HTTP {Method} {Path} falhou após {Elapsed:0.00}ms",
                        method, path, stopwatch.Elapsed.TotalMilliseconds);
                }

                throw; // Re-throw para manter o comportamento padrão
            }
        }
    }
}
