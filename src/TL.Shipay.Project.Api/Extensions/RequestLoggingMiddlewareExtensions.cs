using TL.Shipay.Project.Infrastructure.LogConfig;

namespace TL.Shipay.Project.Api.Extensions
{
    public static class RequestLoggingMiddlewareExtensions
    {
        public static IApplicationBuilder UseRequestLogging(this IApplicationBuilder builder)
            => builder.UseMiddleware<RequestLoggingMiddleware>();
    }
}
