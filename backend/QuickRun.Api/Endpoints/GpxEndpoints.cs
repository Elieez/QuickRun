using Microsoft.AspNetCore.Mvc;
using QuickRun.Api.Models;
using QuickRun.Api.Services.Interfaces;

namespace QuickRun.Api.Endpoints;

public static class GpxEndpoints
{
    public static IEndpointRouteBuilder MapGpxEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapPost("/api/gpx", ([FromBodyAttribute] GpxRequest request, IGpxService gpx) =>
        {
            if (request.Coordinates is null || request.Coordinates.Length == 0)
                return Results.BadRequest(new { error = "coordinates required" });

            var content = gpx.ToGpx(request);
            return Results.Text(content, "application/gpx+xml");
        }).RequireRateLimiting("api");

        return app;
    }
}
