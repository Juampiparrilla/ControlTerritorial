using ControlTerritorial.Application.DTOs;
using ControlTerritorial.Domain.Entities;

namespace ControlTerritorial.Application.Contracts
{
    public interface IUserService
    {
        Task<Usuario?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
        Task<Usuario?> GetByUsernameAsync(string username, CancellationToken cancellationToken = default);
        Task<bool> ValidateUserAsync(string username, string password, CancellationToken cancellationToken = default);
        Task<IReadOnlyList<UserResponseDTO>> GetAllAsync(CancellationToken cancellationToken = default);
        Task<UserResponseDTO> CreateAsync(CreateUserDTO dto, CancellationToken cancellationToken = default);
        Task<UserResponseDTO> UpdateAsync(int id, UpdateUserDTO dto, CancellationToken cancellationToken = default);
        Task DeleteAsync(int id, CancellationToken cancellationToken = default);
        Task ResetPasswordAsync(int id, string newPassword, CancellationToken cancellationToken = default);
        Task<UserResponseDTO> ToggleActiveAsync(int id, CancellationToken cancellationToken = default);
        Task<UserResponseDTO> MapToResponseAsync(Usuario usuario, CancellationToken cancellationToken = default);
    }
}
