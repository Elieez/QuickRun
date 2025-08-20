namespace QuickRun.Api.Config;

public sealed class CorsOptions
{
    public const string PolicyName = "app";
    public string[] AllowedOrigins { get; set; } = Array.Empty<string>();
}
