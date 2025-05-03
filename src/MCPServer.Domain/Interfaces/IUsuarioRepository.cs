using System.Threading.Tasks;
using MCPServer.Domain.Entities;

namespace MCPServer.Domain.Interfaces
{
    public interface IUsuarioRepository
    {
        Task<Usuario> GetByIdAsync(int id);
        Task<Usuario> GetByEmailAsync(string email);
        Task<Usuario> CreateAsync(Usuario usuario);
        Task<Usuario> UpdateAsync(Usuario usuario);
        Task<bool> DeleteAsync(int id);
        Task<bool> EmailExistsAsync(string email);
    }
}