using System;

namespace MCPServer.Domain.Entities
{
    public class Visitante
    {
        public int Id { get; set; }
        public string Nome { get; set; }
        public DateTime DataVisita { get; set; }
        public DateTime DataCriacao { get; set; }
        public string UsuarioCriacao { get; set; }
        public bool Ativo { get; set; }
        public string? Observacao { get; set; }
    }
}