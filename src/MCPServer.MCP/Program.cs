using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using ModelContextProtocol.Server;
using ModelContextProtocol.AspNetCore;
using MCPServer.Application.Services;
using MCPServer.Infrastructure.Context;
using MCPServer.Domain.Interfaces;
using MCPServer.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using MCPServer.Application.DTOs;
using System.ComponentModel;
using Microsoft.Net.Http.Headers;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// Configure logging
builder.Logging.AddConsole();
builder.Logging.SetMinimumLevel(LogLevel.Debug);

// Configure services
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddScoped<IVisitanteRepository, VisitanteRepository>();
builder.Services.AddScoped<VisitanteService>();

// Configure MCP
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

var app = builder.Build();

// Configure pipeline
app.UseRouting();

// Map endpoints
app.MapGet("/mcp/ListarVisitantes", async (VisitanteService visitanteService) =>
{
    try
    {
        var result = await visitanteService.GetAllVisitantesAsync();
        return Results.Ok(result);
    }
    catch (Exception ex)
    {
        return Results.Problem(ex.Message);
    }
});

app.MapPost("/mcp/RegistrarVisitante", async (CreateVisitanteDTO visitante, VisitanteService visitanteService) =>
{
    try
    {
        // Converter para UTC
        visitante.DataVisita = DateTime.SpecifyKind(visitante.DataVisita, DateTimeKind.Utc);
        var result = await visitanteService.CreateVisitanteAsync(visitante, "API");
        return Results.Ok(result);
    }
    catch (Exception ex)
    {
        return Results.Problem(ex.Message);
    }
});

// Add tools discovery endpoint
app.MapGet("/tools", () => Results.Ok(new
{
    Tools = new object[] {
        new {
            Name = "ListarVisitantes",
            Path = "/mcp/ListarVisitantes",
            Method = "GET"
        },
        new {
            Name = "RegistrarVisitante",
            Path = "/mcp/RegistrarVisitante",
            Method = "POST",
            Example = new CreateVisitanteDTO
            {
                Nome = "João Silva",
                DataVisita = DateTime.UtcNow,
                Observacao = "Reunião com equipe de desenvolvimento"
            }
        }
    }
}));

// Start server
app.Urls.Clear();
app.Urls.Add("http://0.0.0.0:7000");

await app.RunAsync();

[McpServerToolType]
public static class VisitanteTool
{
    [McpServerTool, Description("Registra um novo visitante no sistema.")]
    public static async Task<VisitanteDTO> RegistrarVisitante(
        string nome,
        DateTime dataVisita,
        string observacao,
        [FromServices] VisitanteService visitanteService)
    {
        var createDto = new CreateVisitanteDTO
        {
            Nome = nome,
            DataVisita = dataVisita,
            Observacao = observacao
        };

        return await visitanteService.CreateVisitanteAsync(createDto, "MCP");
    }

    [McpServerTool, Description("Lista todos os visitantes registrados.")]
    public static async Task<IEnumerable<VisitanteDTO>> ListarVisitantes([FromServices] VisitanteService visitanteService)
    {
        return await visitanteService.GetAllVisitantesAsync();
    }
}