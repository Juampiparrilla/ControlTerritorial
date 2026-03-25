using ControlTerritorial.Application.Contracts;
using ControlTerritorial.Domain.Common;
using ControlTerritorial.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace ControlTerritorial.Infrastructure.Implementations
{
    public class EscuelaRepository : IEscuelaRepository
    {
        private readonly DbContextControlTerritorial _context;

        public EscuelaRepository(DbContextControlTerritorial context)
        {
            _context = context;
        }

        public async Task<Result<Escuela>> AddAsync(Escuela entity)
        {
            await _context.Escuelas.AddAsync(entity);
            await _context.SaveChangesAsync();
            return Result<Escuela>.Success(entity);
        }

        public async Task UpdateAsync(Escuela entity)
        {
            _context.Escuelas.Update(entity);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var entity = await _context.Escuelas.FindAsync(id);
            if (entity is null) return;
            _context.Escuelas.Remove(entity);
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<Escuela>> GetAllAsync()
        {
            return await _context.Escuelas.ToListAsync();
        }

        public async Task<Escuela?> GetByIdAsync(int id)
        {
            return await _context.Escuelas.FindAsync(id);
        }

        public async Task<Escuela?> GetByNombreEstablecimientoAsync(string nombreEstablecimiento)
        {
            return await _context.Escuelas.FirstOrDefaultAsync(e => e.NombreEstablecimiento == nombreEstablecimiento);
        }
    }
}

