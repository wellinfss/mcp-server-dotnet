using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace MCPServer.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [AllowAnonymous]
    public class TesteController : ControllerBase
    {
        [HttpGet]
        public IActionResult Get()
        {
            return Ok(new { message = "API está funcionando!" });
        }

        [HttpPost]
        public IActionResult Post([FromBody] TesteRequest request)
        {
            return Ok(new { message = $"Recebido: {request.Nome}" });
        }
    }

    public class TesteRequest
    {
        public string Nome { get; set; } = string.Empty;
    }
}