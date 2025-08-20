using Microsoft.AspNetCore.RateLimiting;
using QuickRun.Api.Config;
using QuickRun.Api.Models;
using QuickRun.Api.Services.Implementations;
using QuickRun.Api.Services.Interfaces;
using System.Threading.RateLimiting;

namespace QuickRun.Api.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddAppOptions(this IServiceCollection services, IConfiguration cfg)
    {
        services.Configure<GraphHopperOptions>(cfg.GetSection(GraphHopperOptions.SectionName));
        services.AddSingleton(sp => sp.GetRequiredService<IConfiguration>()
                                      .GetSection(GraphHopperOptions.SectionName)
                                      .Get<GraphHopperOptions>()!);
        services.AddSingleton(new CorsOptions
        {
            AllowedOrigins = (cfg.GetSection("Cors:AllowedOrigins").Get<string[]>() ?? new[] { "http://localhost:5173" })
        });
        services.AddSingleton(new RateLimitOptions
        {
            WindowMinutes = cfg.GetValue("RateLimit:WindowMinutes", 5),
            PermitLimit = cfg.GetValue("RateLimit:PermitLimit", 60),
            QueueLimit = cfg.GetValue("RateLimit:QueueLimit", 20)
        });
        return services;
    }

    public static IServiceCollection AddAppCors(this IServiceCollection services)
    {
        services.AddCors(o => o.AddPolicy(CorsOptions.PolicyName, p =>
            p.WithOrigins(services.BuildServiceProvider()
              .GetRequiredService<CorsOptions>().AllowedOrigins)
             .AllowAnyHeader().AllowAnyMethod()
        ));
        return services;
    }

    public static IServiceCollection AddAppRateLimiting(this IServiceCollection services)
    {
        var opts = services.BuildServiceProvider().GetRequiredService<RateLimitOptions>();
        services.AddRateLimiter(_ => _.AddFixedWindowLimiter("api", o =>
        {
            o.Window = TimeSpan.FromMinutes(opts.WindowMinutes);
            o.PermitLimit = opts.PermitLimit;
            o.QueueLimit = opts.QueueLimit;
            o.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
        }));
        return services;
    }

    public static IServiceCollection AddAppHttpClients(this IServiceCollection services)
    {
        services.AddHttpClient("router", c => c.Timeout = TimeSpan.FromSeconds(30));
        return services;
    }

    public static IServiceCollection AddAppCaching(this IServiceCollection services)
    {
        services.AddMemoryCache();
        return services;
    }

    public static IServiceCollection AddAppServices(this IServiceCollection services)
    {
        services.AddSingleton<IGraphHopperClient, GraphHopperClient>();
        services.AddSingleton<IRouteService, RouteService>();
        services.AddSingleton<IGpxService, GpxService>();
        return services;
    }
}
