namespace QuickRun.Api.Config;

public sealed class RateLimitOptions
{
    public int WindowMinutes { get; init; } = 5;
    public int PermitLimit { get; init; } = 60;
    public int QueueLimit { get; init; } = 20;
}
