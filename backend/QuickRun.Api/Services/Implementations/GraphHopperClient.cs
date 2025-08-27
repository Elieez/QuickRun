using Microsoft.Extensions.Caching.Memory;
using QuickRun.Api.Models;
using QuickRun.Api.Services.Interfaces;
using System.Globalization;

namespace QuickRun.Api.Services.Implementations;

public sealed class GraphHopperClient(
    IHttpClientFactory httpClientFactory,
    GraphHopperOptions options,
    IMemoryCache cache,
    ILogger<GraphHopperClient> logger) : IGraphHopperClient
{
    public async Task<string> RoundTripRawAsync(RoundTripRequest request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(options.ApiKey))
            throw new InvalidOperationException("GraphHopper API key is not configured");
        if (request.Km <= 0 || request.Km > 100) throw new ArgumentOutOfRangeException(nameof(request.Km));
        var profile = request.Mode?.ToLowerInvariant() == "bike" ? "bike" : "foot";
        var key = $"rt:{request.Lat:F5}:{request.Lng:F5}:{request.Km:F2}:{profile}:{(request.Seed?.ToString() ?? "-")}";

        if (cache.TryGetValue(key, out string cached)) return cached;
        
        var qs = new List<string>
        {
            $"key={options.ApiKey}",
            $"profile={profile}",
            "points_encoded=false",
            "algorithm=round_trip",
            $"round_trip.distance={(int)(request.Km * 1000)}",
            "ch.disable=true",
            $"point={request.Lat.ToString(CultureInfo.InvariantCulture)}," +
            $"{request.Lng.ToString(CultureInfo.InvariantCulture)}"
        };
        if (request.Seed.HasValue) qs.Add($"round_trip.seed={request.Seed.Value}");

        var url = $"{options.BaseUrl}?{string.Join("&", qs)}";
        var client = httpClientFactory.CreateClient("router");

        for (int i = 0; i < 2; i++)
        {
            using var response = await client.GetAsync(url, cancellationToken);
            if(response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync(cancellationToken);
                cache.Set(key, json, TimeSpan.FromMinutes(5));
                return json; 
            }
            logger.LogWarning("GraphHopper try {try} failed with {StatusCode}", i + 1, response.StatusCode);
            await Task.Delay(250, cancellationToken);
        }
        throw new InvalidOperationException("Routing failed");
    }
}
