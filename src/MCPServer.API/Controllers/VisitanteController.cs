using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MCPServer.Application.DTOs;
using MCPServer.Application.Services;
using Microsoft.AspNetCore.Authorization;

namespace MCPServer.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class VisitanteController : ControllerBase
    {
        private readonly VisitanteService _visitanteService;

        public VisitanteController(VisitanteService visitanteService)
        {
            _visitanteService = visitanteService;
        }

        [HttpGet]
        public async Task<IActionResult> ListarVisitantes()
        {
            var visitantes = await _visitanteService.GetAllVisitantesAsync();
            return Ok(visitantes);
        }

        [HttpPost]
        public async Task<IActionResult> CriarVisitante([FromBody] CreateVisitanteDTO dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var usuarioCriacao = User.FindFirst(ClaimTypes.Name)?.Value;
            if (string.IsNullOrEmpty(usuarioCriacao))
            {
                return BadRequest("Usuário não identificado no token.");
            }

            var result = await _visitanteService.CreateVisitanteAsync(dto, usuarioCriacao);
            return CreatedAtAction(nameof(ListarVisitantes), result);
        }
    }
}