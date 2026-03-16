using ControlTerritorial.Domain.Entities;
using ControlTerritorial.Domain.Enum;

namespace ControlTerritorial.Application.Contracts
{
    public interface IPersonaRepository : IRepository<Persona>
    {
        Task<Persona?> GetByDniAsync(string dni);
        Task<IEnumerable<Persona>> BuscarPorNombreOApellidoAsync(string? nombre, string? apellido);
        Task<IEnumerable<Persona>> BuscarPorRolAsync(PersonRole rol);
        Task<bool> TieneSubordinadosAsync(int liderId);        
    }
}
