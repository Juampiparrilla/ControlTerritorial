using ControlTerritorial.Application.Contracts;
using ControlTerritorial.Domain.Common;
using ControlTerritorial.Domain.Entities;
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

        public Task DeleteAsync(int id)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<Persona>> GetAllAsync()
        {
            throw new NotImplementedException();
        }

        public Task<Persona?> GetByIdAsync(int id)
        {
            throw new NotImplementedException();
        }

        public Task UpdateAsync(Persona entity)
        {
            throw new NotImplementedException();
        }
    }
}
