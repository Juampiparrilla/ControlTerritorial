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
    }
}
