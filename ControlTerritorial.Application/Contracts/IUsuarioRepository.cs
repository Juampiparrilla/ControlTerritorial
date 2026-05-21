using ControlTerritorial.Domain.Entities;

namespace ControlTerritorial.Application.Contracts
{
    public interface IUsuarioRepository
    {
        Task<Usuario?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
        Task<Usuario?> GetByUsernameAsync(string username, CancellationToken cancellationToken = default);
        Task<IReadOnlyList<Usuario>> GetAllAsync(CancellationToken cancellationToken = default);
        Task<int> CountByRoleAsync(Domain.Enum.SystemRole role, CancellationToken cancellationToken = default);
        Task AddAsync(Usuario usuario, CancellationToken cancellationToken = default);
        Task UpdateAsync(Usuario usuario, CancellationToken cancellationToken = default);
        Task DeleteAsync(Usuario usuario, CancellationToken cancellationToken = default);
        Task<bool> ExistsUsernameAsync(string username, int? excludeUserId = null, CancellationToken cancellationToken = default);
        Task<bool> ExistsPersonaLinkAsync(int personaId, int? excludeUserId = null, CancellationToken cancellationToken = default);
    }
}
