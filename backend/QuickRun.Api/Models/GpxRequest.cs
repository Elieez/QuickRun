namespace QuickRun.Api.Models;

public sealed class GpxRequest
{
    public required double[][] Coordinates { get; init; } = Array.Empty<double[]>();
    public string Name { get; init; } = "Route";
}
