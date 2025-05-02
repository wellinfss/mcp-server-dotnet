using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MCPServer.Application.DTOs;
using MCPServer.Application.Services;

namespace MCPServer.API.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class VisitanteController : ControllerBase
    {
        private readonly VisitanteService _visitanteService;

        public VisitanteController(VisitanteService visitanteService)
        {
            _visitanteService = visitanteService;
        }

        [HttpPost]
        public async Task<ActionResult<VisitanteDTO>> CreateVisitante(CreateVisitanteDTO createDto)
        {
            var usuarioCriacao = User.FindFirst(ClaimTypes.Name)?.Value;
            if (string.IsNullOrEmpty(usuarioCriacao))
            {
                return BadRequest("Usuário não identificado");
            }

            var result = await _visitanteService.CreateVisitanteAsync(createDto, usuarioCriacao);
            return CreatedAtAction(nameof(GetVisitantes), new { id = result.Id }, result);
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<VisitanteDTO>>> GetVisitantes()
        {
            var visitantes = await _visitanteService.GetAllVisitantesAsync();
            return Ok(visitantes);
        }
    }
}