using QuickRun.Api.Models;
using QuickRun.Api.Services.Interfaces;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace QuickRun.Api.Services.Implementations;

public sealed class RouteService(IGraphHopperClient gh) : IRouteService
{
    public async Task<RoundTripResponse> GetRoundTripAsync(RoundTripRequest request, CancellationToken cancellationToken)
    {
        var rawJson = await gh.RoundTripRawAsync(request, cancellationToken);
        using var doc = JsonDocument.Parse(rawJson);
        var root = doc.RootElement;

        if (!root.TryGetProperty("paths", out var paths) || paths.GetArrayLength() == 0)
        {
            var msg = root.TryGetProperty("message", out var m) ? m.GetString() : "No paths in response";
            throw new InvalidOperationException($"GraphHopper error: {msg}");
        }

        var path = doc.RootElement.GetProperty("paths")[0];

        var distance = path.GetProperty("distance").GetDouble();
        var time = path.GetProperty("time").GetInt64();
        var bbox = path.GetProperty("bbox").EnumerateArray().Select(x => x.GetDouble()).ToArray();

        var coordsLonLat = path.GetProperty("points").GetProperty("coordinates")
                            .EnumerateArray()
                            .Select(x => new double[] { x[1].GetDouble(), x[0].GetDouble() })
                            .ToArray();

        return new RoundTripResponse
        {
            DistanceMeters = distance,
            DurationMs = time,
            Bbox = bbox,
            Coordinates = coordsLonLat,
            Raw = JsonSerializer.Deserialize<object>(rawJson)!
        };
    }
}
