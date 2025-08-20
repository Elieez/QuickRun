using QuickRun.Api.Models;

namespace QuickRun.Api.Services.Interfaces;

public interface IRouteService
{
    Task<RoundTripResponse> GetRoundTripAsync(RoundTripRequest request, CancellationToken cancellationToken);
}
