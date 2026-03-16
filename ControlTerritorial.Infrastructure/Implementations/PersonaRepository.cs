using ControlTerritorial.Application.Contracts;
using ControlTerritorial.Domain.Common;
using ControlTerritorial.Domain.Entities;
using ControlTerritorial.Domain.Enum;
using Microsoft.EntityFrameworkCore;

namespace ControlTerritorial.Infrastructure.Implementations
{
    public class PersonaRepository : IPersonaRepository
    {
        private readonly DbContextControlTerritorial _context;

        public PersonaRepository(DbContextControlTerritorial context)
        {
            _context = context;
        }

        public async Task<Result<Persona>> AddAsync(Persona entity)
        {
            try
            {
                await _context.Personas.AddAsync(entity);
                await _context.SaveChangesAsync();
                return Result<Persona>.Success(entity);
            }
            catch (Exception ex)
            {
                return Result<Persona>.Fail("No se creo la persona " + ex.Message);
            }
        }

        public async Task UpdateAsync(Persona entity)
        {
            _context.Personas.Update(entity);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var persona = await _context.Personas.FindAsync(id);
            if (persona is null)
            {
                return;
            }

            _context.Personas.Remove(persona);
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<Persona>> GetAllAsync()
        {
            return await _context.Personas.ToListAsync();
        }

        public async Task<Persona?> GetByIdAsync(int id)
        {
            return await _context.Personas.FindAsync(id);
        }

        public async Task<Persona?> GetByDniAsync(string dni)
        {
            return await _context.Personas.FirstOrDefaultAsync(p => p.DNI == dni);
        }

        public async Task<IEnumerable<Persona>> BuscarPorNombreOApellidoAsync(string? nombre, string? apellido)
        {
            IQueryable<Persona> query = _context.Personas;

            if (!string.IsNullOrWhiteSpace(nombre))
            {
                query = query.Where(p => p.Nombre.Contains(nombre));
            }

            if (!string.IsNullOrWhiteSpace(apellido))
            {
                query = query.Where(p => p.Apellido.Contains(apellido));
            }

            return await query.ToListAsync();
        }

        public async Task<IEnumerable<Persona>> BuscarPorRolAsync(PersonRole rol)
        {
            return await _context.Personas
                .Include(p => p.Lider)
                .Where(p => p.Rol == rol)
                .ToListAsync();
        }

        public async Task<bool> TieneSubordinadosAsync(int liderId)
        {
            return await _context.Personas.AnyAsync(p => p.LiderId == liderId);
        }
    }
}
