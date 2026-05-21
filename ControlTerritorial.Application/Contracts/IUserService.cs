using ControlTerritorial.Domain.Entities;

namespace ControlTerritorial.Application.Contracts
{
    public interface IUserService
    {
        Task<Usuario?> GetByDniAsync(string dni, CancellationToken cancellationToken = default);
        Task<bool> ValidateUserAsync(string dni, string password, CancellationToken cancellationToken = default);
        Task<Usuario> CreateUserAsync(string dni, string password, CancellationToken cancellationToken = default);
    }
}
