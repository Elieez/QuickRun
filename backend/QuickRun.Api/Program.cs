
using QuickRun.Api.Endpoints;
using QuickRun.Api.Extensions;

var builder = WebApplication.CreateBuilder(args);

// add services & options
builder.Services.AddAppOptions(builder.Configuration);
builder.Services.AddAppCors();
builder.Services.AddAppRateLimiting();
builder.Services.AddAppHttpClients();
builder.Services.AddAppCaching();
builder.Services.AddAppServices(); // DI for our services

var app = builder.Build();

// pipeline
app.UseAppErrorHandling();
app.UseAppCors();
app.UseAppRateLimiting();

// endpoints
app.MapHealthEndpoints();
app.MapRouteEndpoints();
app.MapGpxEndpoints();

app.Run();
