using ControlTerritorial.Domain.Entities;

namespace ControlTerritorial.Application.Contracts
{
    public interface IMesaRepository : IRepository<Mesa>
    {
        Task<Mesa?> GetByEscuelaIdAndNroMesaAsync(int escuelaId, int nroMesa);
    }
}
