using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MCPServer.Domain.Entities;

namespace MCPServer.Domain.Interfaces
{
    public interface IUserRepository
    {
        Task<User> GetByIdAsync(Guid id);
        Task<User> GetByEmailAsync(string email);
        Task<User> GetByUsernameAsync(string username);
        Task<IEnumerable<User>> GetAllAsync();
        Task<User> CreateAsync(User user);
        Task UpdateAsync(User user);
        Task DeleteAsync(Guid id);
    }
}