using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using MCPServer.Domain.Entities;
using MCPServer.Domain.Interfaces;
using MCPServer.Infrastructure.Context;

namespace MCPServer.Infrastructure.Repositories
{
    public class VisitanteRepository : IVisitanteRepository
    {
        private readonly ApplicationDbContext _context;

        public VisitanteRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Visitante> GetByIdAsync(int id)
        {
            return await _context.Visitantes.FindAsync(id);
        }

        public async Task<IEnumerable<Visitante>> GetAllAsync()
        {
            return await _context.Visitantes.ToListAsync();
        }

        public async Task<Visitante> AddAsync(Visitante visitante)
        {
            _context.Visitantes.Add(visitante);
            await _context.SaveChangesAsync();
            return visitante;
        }

        public async Task UpdateAsync(Visitante visitante)
        {
            _context.Entry(visitante).State = EntityState.Modified;
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var visitante = await _context.Visitantes.FindAsync(id);
            if (visitante != null)
            {
                _context.Visitantes.Remove(visitante);
                await _context.SaveChangesAsync();
            }
        }
    }
}