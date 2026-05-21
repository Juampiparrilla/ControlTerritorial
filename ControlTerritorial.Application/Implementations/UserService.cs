using ControlTerritorial.Application.Contracts;
using ControlTerritorial.Application.DTOs;
using ControlTerritorial.Application.Exceptions;
using ControlTerritorial.Domain.Entities;
using ControlTerritorial.Domain.Enum;

namespace ControlTerritorial.Application.Implementations
{
    public class UserService : IUserService
    {
        private const int MinPasswordLength = 6;

        private readonly IUsuarioRepository _usuarioRepository;
        private readonly IPersonaRepository _personaRepository;
        private readonly IPasswordHasher _passwordHasher;

        public UserService(
            IUsuarioRepository usuarioRepository,
            IPersonaRepository personaRepository,
            IPasswordHasher passwordHasher)
        {
            _usuarioRepository = usuarioRepository;
            _personaRepository = personaRepository;
            _passwordHasher = passwordHasher;
        }

        public Task<Usuario?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            return _usuarioRepository.GetByIdAsync(id, cancellationToken);
        }

        public Task<Usuario?> GetByUsernameAsync(string username, CancellationToken cancellationToken = default)
        {
            return _usuarioRepository.GetByUsernameAsync(username.Trim(), cancellationToken);
        }

        public async Task<bool> ValidateUserAsync(string username, string password, CancellationToken cancellationToken = default)
        {
            var usuario = await GetByUsernameAsync(username, cancellationToken).ConfigureAwait(false);
            if (usuario is null || !usuario.IsActive)
            {
                return false;
            }

            return _passwordHasher.Verify(password, usuario.PasswordHash);
        }

        public async Task<IReadOnlyList<UserResponseDTO>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            var usuarios = await _usuarioRepository.GetAllAsync(cancellationToken).ConfigureAwait(false);
            var list = new List<UserResponseDTO>();
            foreach (var u in usuarios)
            {
                list.Add(await MapToResponseAsync(u, cancellationToken).ConfigureAwait(false));
            }

            return list;
        }

        public async Task<UserResponseDTO> CreateAsync(CreateUserDTO dto, CancellationToken cancellationToken = default)
        {
            ValidatePassword(dto.Password);
            var username = NormalizeUsername(dto.Username);

            if (await _usuarioRepository.ExistsUsernameAsync(username, cancellationToken: cancellationToken).ConfigureAwait(false))
            {
                throw new UserAlreadyExistsException("Ya existe un usuario con ese nombre de usuario.");
            }

            if (dto.PersonaId.HasValue)
            {
                await EnsurePersonaLinkValidAsync(dto.PersonaId.Value, null, cancellationToken).ConfigureAwait(false);
            }

            var usuario = new Usuario
            {
                Username = username,
                PasswordHash = _passwordHasher.Hash(dto.Password),
                SystemRole = dto.SystemRole,
                PersonaId = dto.PersonaId,
                IsActive = dto.IsActive,
                CreatedAt = DateTime.UtcNow,
            };

            await _usuarioRepository.AddAsync(usuario, cancellationToken).ConfigureAwait(false);
            return await MapToResponseAsync(usuario, cancellationToken).ConfigureAwait(false);
        }

        public async Task<UserResponseDTO> UpdateAsync(int id, UpdateUserDTO dto, CancellationToken cancellationToken = default)
        {
            var usuario = await _usuarioRepository.GetByIdAsync(id, cancellationToken).ConfigureAwait(false)
                ?? throw new KeyNotFoundException("Usuario no encontrado.");

            var username = NormalizeUsername(dto.Username);
            if (await _usuarioRepository.ExistsUsernameAsync(username, id, cancellationToken).ConfigureAwait(false))
            {
                throw new UserAlreadyExistsException("Ya existe un usuario con ese nombre de usuario.");
            }

            if (dto.PersonaId.HasValue)
            {
                await EnsurePersonaLinkValidAsync(dto.PersonaId.Value, id, cancellationToken).ConfigureAwait(false);
            }

            usuario.Username = username;
            usuario.SystemRole = dto.SystemRole;
            usuario.PersonaId = dto.PersonaId;
            usuario.IsActive = dto.IsActive;
            usuario.TouchUpdated();

            await _usuarioRepository.UpdateAsync(usuario, cancellationToken).ConfigureAwait(false);
            return await MapToResponseAsync(usuario, cancellationToken).ConfigureAwait(false);
        }

        public async Task DeleteAsync(int id, CancellationToken cancellationToken = default)
        {
            var usuario = await _usuarioRepository.GetByIdAsync(id, cancellationToken).ConfigureAwait(false)
                ?? throw new KeyNotFoundException("Usuario no encontrado.");

            if (usuario.SystemRole == SystemRole.AdminSistema)
            {
                var adminCount = await _usuarioRepository.CountByRoleAsync(SystemRole.AdminSistema, cancellationToken).ConfigureAwait(false);
                if (adminCount <= 1)
                {
                    throw new InvalidOperationException("No se puede eliminar el último administrador del sistema.");
                }
            }

            await _usuarioRepository.DeleteAsync(usuario, cancellationToken).ConfigureAwait(false);
        }

        public async Task ResetPasswordAsync(int id, string newPassword, CancellationToken cancellationToken = default)
        {
            ValidatePassword(newPassword);
            var usuario = await _usuarioRepository.GetByIdAsync(id, cancellationToken).ConfigureAwait(false)
                ?? throw new KeyNotFoundException("Usuario no encontrado.");

            usuario.PasswordHash = _passwordHasher.Hash(newPassword);
            usuario.TouchUpdated();
            await _usuarioRepository.UpdateAsync(usuario, cancellationToken).ConfigureAwait(false);
        }

        public async Task<UserResponseDTO> ToggleActiveAsync(int id, CancellationToken cancellationToken = default)
        {
            var usuario = await _usuarioRepository.GetByIdAsync(id, cancellationToken).ConfigureAwait(false)
                ?? throw new KeyNotFoundException("Usuario no encontrado.");

            if (usuario.SystemRole == SystemRole.AdminSistema && usuario.IsActive)
            {
                var activeAdmins = await _usuarioRepository.GetAllAsync(cancellationToken).ConfigureAwait(false);
                var activeAdminCount = activeAdmins.Count(u => u.SystemRole == SystemRole.AdminSistema && u.IsActive);
                if (activeAdminCount <= 1)
                {
                    throw new InvalidOperationException("No se puede desactivar el último administrador activo del sistema.");
                }
            }

            usuario.IsActive = !usuario.IsActive;
            usuario.TouchUpdated();
            await _usuarioRepository.UpdateAsync(usuario, cancellationToken).ConfigureAwait(false);
            return await MapToResponseAsync(usuario, cancellationToken).ConfigureAwait(false);
        }

        public async Task<UserResponseDTO> MapToResponseAsync(Usuario usuario, CancellationToken cancellationToken = default)
        {
            string? nombre = null;
            string? apellido = null;
            string? dni = null;

            if (usuario.PersonaId.HasValue)
            {
                var persona = usuario.Persona ?? await _personaRepository.GetByIdAsync(usuario.PersonaId.Value).ConfigureAwait(false);
                if (persona is not null)
                {
                    nombre = persona.Nombre;
                    apellido = persona.Apellido;
                    dni = persona.DNI;
                }
            }

            return new UserResponseDTO
            {
                Id = usuario.Id,
                Username = usuario.Username,
                SystemRole = usuario.SystemRole,
                IsActive = usuario.IsActive,
                PersonaId = usuario.PersonaId,
                PersonaNombre = nombre,
                PersonaApellido = apellido,
                PersonaDni = dni,
                CreatedAt = usuario.CreatedAt,
                UpdatedAt = usuario.UpdatedAt,
            };
        }

        private async Task EnsurePersonaLinkValidAsync(int personaId, int? excludeUserId, CancellationToken cancellationToken)
        {
            var persona = await _personaRepository.GetByIdAsync(personaId).ConfigureAwait(false);
            if (persona is null)
            {
                throw new ArgumentException("La persona vinculada no existe.");
            }

            if (await _usuarioRepository.ExistsPersonaLinkAsync(personaId, excludeUserId, cancellationToken).ConfigureAwait(false))
            {
                throw new UserAlreadyExistsException("Esa persona ya está vinculada a otro usuario.");
            }
        }

        private static string NormalizeUsername(string username)
        {
            if (string.IsNullOrWhiteSpace(username))
            {
                throw new ArgumentException("El nombre de usuario es obligatorio.");
            }

            return username.Trim().ToLowerInvariant();
        }

        private static void ValidatePassword(string password)
        {
            if (string.IsNullOrEmpty(password) || password.Length < MinPasswordLength)
            {
                throw new ArgumentException($"La contraseña debe tener al menos {MinPasswordLength} caracteres.");
            }
        }
    }
}
