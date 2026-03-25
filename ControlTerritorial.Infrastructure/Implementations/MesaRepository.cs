using ControlTerritorial.Application.Contracts;
using ControlTerritorial.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace ControlTerritorial.Infrastructure.Implementations
{
    public class MesaRepository : IMesaRepository
    {
        private readonly DbContextControlTerritorial _context;

        public MesaRepository(DbContextControlTerritorial context)
        {
            _context = context;
        }

        public async Task<Mesa?> GetByIdAsync(int id)
        {
            return await _context.Mesas.FindAsync(id);
        }

        public async Task<IEnumerable<Mesa>> GetAllAsync()
        {
            return await _context.Mesas.ToListAsync();
        }

        public async Task<ControlTerritorial.Domain.Common.Result<Mesa>> AddAsync(Mesa entity)
        {
            await _context.Mesas.AddAsync(entity);
            await _context.SaveChangesAsync();
            return ControlTerritorial.Domain.Common.Result<Mesa>.Success(entity);
        }

        public async Task UpdateAsync(Mesa entity)
        {
            _context.Mesas.Update(entity);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var entity = await _context.Mesas.FindAsync(id);
            if (entity is null) return;
            _context.Mesas.Remove(entity);
            await _context.SaveChangesAsync();
        }

        public async Task<Mesa?> GetByEscuelaIdAndNroMesaAsync(int escuelaId, int nroMesa)
        {
            return await _context.Mesas.FirstOrDefaultAsync(m => m.EscuelaId == escuelaId && m.NroMesa == nroMesa);
        }
    }
}

