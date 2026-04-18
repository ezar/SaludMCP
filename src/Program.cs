using SaludMcp.Clients;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddHttpClient("cima", client =>
{
    client.BaseAddress = new Uri("https://cima.aemps.es/cima/rest/");
    client.DefaultRequestHeaders.UserAgent.ParseAdd("salud-mcp/1.0");
    client.Timeout = TimeSpan.FromSeconds(10);
});

builder.Services.AddScoped<CimaClient>();

builder.Services
    .AddMcpServer()
    .WithHttpTransport()
    .WithToolsFromAssembly();

var app = builder.Build();

app.MapMcp();

await app.RunAsync();
