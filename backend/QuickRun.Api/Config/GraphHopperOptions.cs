namespace QuickRun.Api.Models;

public sealed class GraphHopperOptions
{
    public const string SectionName = "GraphHopper";
    public string ApiKey { get; set; } = string.Empty;
    public string BaseUrl { get; set; } = "http://graphhopper.com/api/1/route";
}
