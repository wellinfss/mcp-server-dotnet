using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using MCPServer.Application.Services;
using ModelContextProtocol.Server;
using MCPServer.Infrastructure.Context;
using MCPServer.Domain.Interfaces;
using MCPServer.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

var builder = Host.CreateEmptyApplicationBuilder(settings: null);

// Configure Logging
builder.Services.AddLogging(logging =>
{
    logging.AddConsole();
    logging.SetMinimumLevel(LogLevel.Debug);
});

// Get logger
var loggerFactory = LoggerFactory.Create(builder =>
{
    builder.AddConsole();
    builder.SetMinimumLevel(LogLevel.Debug);
});
var logger = loggerFactory.CreateLogger("Program");

logger.LogInformation("Iniciando servidor MCP...");

// Configure Database
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql("Host=localhost;Database=mcpserver;Username=mcpuser;Password=mcppass"));

logger.LogInformation("Banco de dados configurado.");

// Register Dependencies
builder.Services.AddScoped<IVisitanteRepository, VisitanteRepository>();
builder.Services.AddScoped<VisitanteService>();

logger.LogInformation("Dependências registradas.");

// Configure MCP Server
builder.Services.AddMcpServer()
    .WithStdioServerTransport()
    .WithToolsFromAssembly();

logger.LogInformation("Servidor MCP configurado.");

var app = builder.Build();

logger.LogInformation("Servidor construído, iniciando...");

await app.RunAsync();