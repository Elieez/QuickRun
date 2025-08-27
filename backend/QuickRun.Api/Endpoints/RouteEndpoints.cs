using Microsoft.AspNetCore.Mvc;
using QuickRun.Api.Models;
using QuickRun.Api.Services.Interfaces;

namespace QuickRun.Api.Endpoints;

public static class RouteEndpoints
{
    public static IEndpointRouteBuilder MapRouteEndpoints(this IEndpointRouteBuilder app)
    {
        var grp = app.MapGroup("/api").RequireRateLimiting("api");

        grp.MapGet("/roundtrip", async (
            [FromQuery] double lat,
            [FromQuery] double lng,
            [FromQuery] double km,
            [FromQuery] string mode,
            [FromQuery] int? seed,
            IRouteService service,
            CancellationToken ct) =>
        {
            if (km <= 0 || km > 100) return Results.BadRequest(new { error = "km must be between 1 and 100" });

            try
            {
                var req = new RoundTripRequest { Lat = lat, Lng = lng, Km = km, Mode = mode, Seed = seed };
                var res = await service.GetRoundTripAsync(req, ct);
                return Results.Ok(res);
            }
            catch (Exception ex)
            {
                return Results.Problem(ex.Message);
            }
        });

        return app;
    }
}
