using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using MCPServer.Infrastructure.Context;

namespace MCPServer.Infrastructure
{
    public class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<ApplicationDbContext>
    {
        public ApplicationDbContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();
            optionsBuilder.UseNpgsql("Host=localhost;Database=mcpserver;Username=mcpuser;Password=mcppass");

            return new ApplicationDbContext(optionsBuilder.Options);
        }
    }
}