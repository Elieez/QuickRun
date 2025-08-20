using QuickRun.Api.Common;
using QuickRun.Api.Config;

namespace QuickRun.Api.Extensions;

public static class ApplicationBuilderExtensions
{
    public static IApplicationBuilder UseAppErrorHandling(this IApplicationBuilder app)
        => app.UseMiddleware<ErrorHandlingMiddleware>();

    public static IApplicationBuilder UseAppCors(this IApplicationBuilder app)
        => app.UseCors(CorsOptions.PolicyName);

    public static IApplicationBuilder UseAppRateLimiting(this IApplicationBuilder app)
        => app.UseRateLimiter();
}
