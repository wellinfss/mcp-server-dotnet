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
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Configure logging
builder.Logging.AddConsole();
builder.Logging.SetMinimumLevel(LogLevel.Debug);

// Configure services
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"),
        b => b.MigrationsAssembly("MCPServer.MCP")));

builder.Services.AddScoped<IVisitanteRepository, VisitanteRepository>();
builder.Services.AddScoped<IUsuarioRepository, UsuarioRepository>();
builder.Services.AddScoped<VisitanteService>();
builder.Services.AddScoped<AuthService>();

// Configure JWT Authentication
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"] ?? throw new InvalidOperationException("JWT Key not found"))),
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            ValidateLifetime = true,
            ClockSkew = TimeSpan.Zero
        };
    });

// Configure MCP
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

var app = builder.Build();

// Configure pipeline
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

// Auth endpoints
app.MapPost("/auth/login", async (LoginRequestDTO request, AuthService authService) =>
{
    try
    {
        var response = await authService.LoginAsync(request);
        return Results.Ok(response);
    }
    catch (UnauthorizedAccessException ex)
    {
        return Results.Unauthorized();
    }
    catch (Exception ex)
    {
        return Results.Problem(ex.Message);
    }
});

app.MapPost("/auth/register", async (RegisterRequestDTO request, AuthService authService) =>
{
    try
    {
        var response = await authService.RegisterAsync(request);
        return Results.Ok(response);
    }
    catch (InvalidOperationException ex)
    {
        return Results.BadRequest(ex.Message);
    }
    catch (Exception ex)
    {
        return Results.Problem(ex.Message);
    }
});

// Protected endpoints
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
}).RequireAuthorization();

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
}).RequireAuthorization();

// Add tools discovery endpoint
app.MapGet("/tools", () => Results.Ok(new
{
    Tools = new object[] {
        new {
            Name = "Login",
            Path = "/auth/login",
            Method = "POST",
            Example = new LoginRequestDTO
            {
                Email = "usuario@exemplo.com",
                Senha = "senha123"
            }
        },
        new {
            Name = "Register",
            Path = "/auth/register",
            Method = "POST",
            Example = new RegisterRequestDTO
            {
                Nome = "Novo Usuário",
                Email = "novo@exemplo.com",
                Senha = "senha123"
            }
        },
        new {
            Name = "ListarVisitantes",
            Path = "/mcp/ListarVisitantes",
            Method = "GET",
            RequiresAuth = true
        },
        new {
            Name = "RegistrarVisitante",
            Path = "/mcp/RegistrarVisitante",
            Method = "POST",
            RequiresAuth = true,
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
app.Urls.Add("http://0.0.0.0:7001");

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