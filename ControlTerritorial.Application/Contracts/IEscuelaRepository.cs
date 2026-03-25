using ControlTerritorial.Domain.Entities;

namespace ControlTerritorial.Application.Contracts
{
    public interface IEscuelaRepository : IRepository<Escuela>
    {
        Task<Escuela?> GetByNombreEstablecimientoAsync(string nombreEstablecimiento);
    }
}
