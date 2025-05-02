using System.ComponentModel;
using System.Threading.Tasks;
using MCPServer.Application.Services;
using MCPServer.Application.DTOs;
using ModelContextProtocol.Server;

namespace MCPServer.MCP.Tools
{
    [McpServerToolType]
    public class VisitanteTools
    {
        private readonly VisitanteService _visitanteService;

        public VisitanteTools(VisitanteService visitanteService)
        {
            _visitanteService = visitanteService;
        }

        [McpServerTool]
        [Description("Lista todos os visitantes registrados")]
        public async Task<string> ListarVisitantes()
        {
            var visitantes = await _visitanteService.GetAllVisitantesAsync();
            return System.Text.Json.JsonSerializer.Serialize(visitantes);
        }

        [McpServerTool]
        [Description("Cria um novo visitante")]
        public async Task<string> CriarVisitante(string nome, string dataVisita, string usuarioCriacao, string? observacao = null)
        {
            var createDto = new CreateVisitanteDTO
            {
                Nome = nome,
                DataVisita = System.DateTime.Parse(dataVisita),
                Observacao = observacao
            };

            var result = await _visitanteService.CreateVisitanteAsync(createDto, usuarioCriacao);
            return System.Text.Json.JsonSerializer.Serialize(result);
        }
    }
} 