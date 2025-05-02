using System.Collections.Generic;
using System.Threading.Tasks;
using MCPServer.Domain.Entities;

namespace MCPServer.Domain.Interfaces
{
    public interface IVisitanteRepository
    {
        Task<Visitante> GetByIdAsync(int id);
        Task<IEnumerable<Visitante>> GetAllAsync();
        Task<Visitante> AddAsync(Visitante visitante);
        Task UpdateAsync(Visitante visitante);
        Task DeleteAsync(int id);
    }
}