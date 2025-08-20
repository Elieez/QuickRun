using QuickRun.Api.Models;

namespace QuickRun.Api.Services.Interfaces;

public interface IGraphHopperClient
{
    Task<string> RoundTripRawAsync(RoundTripRequest request, CancellationToken cancellationToken);
}
