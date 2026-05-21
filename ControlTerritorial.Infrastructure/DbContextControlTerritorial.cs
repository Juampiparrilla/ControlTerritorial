using ControlTerritorial.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace ControlTerritorial.Infrastructure
{
    public class DbContextControlTerritorial : DbContext
    {
        public DbContextControlTerritorial(DbContextOptions<DbContextControlTerritorial> options) : base(options)
        {
        }

        public DbSet<Persona> Personas { get; set; }
        public DbSet<Escuela> Escuelas { get; set; }
        public DbSet<Mesa> Mesas { get; set; }
        public DbSet<Vehiculo> Vehiculos { get; set; }
        public DbSet<PadronImport> PadronesImportados { get; set; }
        public DbSet<PadronRow> PadronFilas { get; set; }
        public DbSet<Usuario> Usuarios { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Usuario>(entity =>
            {
                entity.HasIndex(e => new { e.Dni, e.TenantId }).IsUnique();
                entity.Property(e => e.Dni).HasMaxLength(32);
                entity.Property(e => e.PasswordHash).HasMaxLength(200);
                entity.Property(e => e.Role).HasMaxLength(64);
                entity.Property(e => e.TenantId).HasMaxLength(64);
            });
        }
    }
}
