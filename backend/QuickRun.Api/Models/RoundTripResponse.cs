namespace QuickRun.Api.Models;

public sealed class RoundTripResponse
{
    public required double DistanceMeters { get; init; }
    public required long DurationMs { get; init; }
    public required double[] Bbox { get; init; } // [minLon,minLat,maxLon,maxLat]
    public required double[][] Coordinates { get; init; } // [ [lat,lng], ... ]
    public object Raw { get; init; } = default!;
}
