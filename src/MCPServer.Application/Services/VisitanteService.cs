using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MCPServer.Application.DTOs;
using MCPServer.Domain.Entities;
using MCPServer.Domain.Interfaces;

namespace MCPServer.Application.Services
{
    public class VisitanteService
    {
        private readonly IVisitanteRepository _visitanteRepository;

        public VisitanteService(IVisitanteRepository visitanteRepository)
        {
            _visitanteRepository = visitanteRepository;
        }

        public async Task<VisitanteDTO> CreateVisitanteAsync(CreateVisitanteDTO createDto, string usuarioCriacao)
        {
            var visitante = new Visitante
            {
                Nome = createDto.Nome,
                DataVisita = createDto.DataVisita,
                DataCriacao = DateTime.UtcNow,
                UsuarioCriacao = usuarioCriacao,
                Ativo = true,
                Observacao = createDto.Observacao
            };

            var result = await _visitanteRepository.AddAsync(visitante);

            return new VisitanteDTO
            {
                Id = result.Id,
                Nome = result.Nome,
                DataVisita = result.DataVisita,
                DataCriacao = result.DataCriacao,
                UsuarioCriacao = result.UsuarioCriacao,
                Ativo = result.Ativo,
                Observacao = result.Observacao
            };
        }

        public async Task<IEnumerable<VisitanteDTO>> GetAllVisitantesAsync()
        {
            var visitantes = await _visitanteRepository.GetAllAsync();
            var visitanteDTOs = new List<VisitanteDTO>();

            foreach (var visitante in visitantes)
            {
                visitanteDTOs.Add(new VisitanteDTO
                {
                    Id = visitante.Id,
                    Nome = visitante.Nome,
                    DataVisita = visitante.DataVisita,
                    DataCriacao = visitante.DataCriacao,
                    UsuarioCriacao = visitante.UsuarioCriacao,
                    Ativo = visitante.Ativo,
                    Observacao = visitante.Observacao
                });
            }

            return visitanteDTOs;
        }
    }
}