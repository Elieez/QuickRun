using QuickRun.Api.Models;

namespace QuickRun.Api.Services.Interfaces;

public interface IGpxService
{
    string ToGpx(GpxRequest request);
}
