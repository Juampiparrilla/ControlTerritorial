using ControlTerritorial.Domain.Entities;

namespace ControlTerritorial.Application.Contracts
{
    public interface IPersonaRepository : IRepository<Persona>
    {
        //Task<Persona?> GetByDniAsync(string dni);
        //Task<Persona?> GetWithSubordinadosAsync(int id);
    }
}
