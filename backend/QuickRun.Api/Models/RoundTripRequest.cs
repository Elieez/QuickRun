namespace QuickRun.Api.Models;

public sealed class RoundTripRequest
{
    public required double Lat { get; init; }
    public required double Lng { get; init; }
    public required double Km { get; init; }
    public string Mode { get; init; } = "foot"; // foot|bike
    public int? Seed { get; init; }
}
