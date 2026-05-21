using ControlTerritorial.Domain.Entities;

namespace ControlTerritorial.Application.Contracts
{
    public interface IUsuarioRepository
    {
        Task<Usuario?> GetByDniAndTenantAsync(string dni, string tenantId, CancellationToken cancellationToken = default);
        Task AddAsync(Usuario usuario, CancellationToken cancellationToken = default);
    }
}
