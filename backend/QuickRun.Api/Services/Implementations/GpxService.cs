using QuickRun.Api.Models;
using QuickRun.Api.Services.Interfaces;
using System.Globalization;
using System.Text;

namespace QuickRun.Api.Services.Implementations;

public sealed class GpxService : IGpxService
{
    public string ToGpx(GpxRequest request)
    {
        var sb = new StringBuilder();
        sb.AppendLine("<?xml version=\"1.0\" encoding=\"UTF-8\"?>");
        sb.AppendLine("<gpx version=\"1.1\" creator=\"runloops\">");
        sb.AppendLine($"  <trk><name>{System.Security.SecurityElement.Escape(request.Name)}</name><trkseg>");
        foreach (var c in request.Coordinates)
        {
            if (c.Length < 2) continue;
            var lat = c[0].ToString(CultureInfo.InvariantCulture);
            var lon = c[1].ToString(CultureInfo.InvariantCulture);
            sb.AppendLine($"    <trkpt lat=\"{lat}\" lon=\"{lon}\"></trkpt>");
        }
        sb.AppendLine("  </trkseg></trk></gpx>");
        return sb.ToString();
    }
}
