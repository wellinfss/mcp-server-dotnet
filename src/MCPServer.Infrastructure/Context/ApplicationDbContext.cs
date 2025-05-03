using Microsoft.EntityFrameworkCore;
using MCPServer.Domain.Entities;

namespace MCPServer.Infrastructure.Context
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Visitante> Visitantes { get; set; }
        public DbSet<Usuario> Usuarios { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Visitante>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Nome).IsRequired();
                entity.Property(e => e.DataVisita).IsRequired();
                entity.Property(e => e.DataCriacao).IsRequired();
                entity.Property(e => e.UsuarioCriacao).IsRequired();
                entity.Property(e => e.Ativo).IsRequired();
            });

            modelBuilder.Entity<Usuario>()
                .HasIndex(u => u.Email)
                .IsUnique();
        }
    }
}