using ControlTerritorial.Application.Contracts;
using ControlTerritorial.Application.Exceptions;
using ControlTerritorial.Domain.Entities;

namespace ControlTerritorial.Application.Implementations
{
    public class UserService : IUserService
    {
        public const string DefaultTenantId = "default";
        private const string DefaultRole = "User";

        private readonly IUsuarioRepository _usuarioRepository;
        private readonly IPasswordHasher _passwordHasher;

        public UserService(IUsuarioRepository usuarioRepository, IPasswordHasher passwordHasher)
        {
            _usuarioRepository = usuarioRepository;
            _passwordHasher = passwordHasher;
        }

        public Task<Usuario?> GetByDniAsync(string dni, CancellationToken cancellationToken = default)
        {
            return _usuarioRepository.GetByDniAndTenantAsync(dni, DefaultTenantId, cancellationToken);
        }

        public async Task<bool> ValidateUserAsync(string dni, string password, CancellationToken cancellationToken = default)
        {
            var usuario = await GetByDniAsync(dni, cancellationToken).ConfigureAwait(false);
            if (usuario is null)
            {
                return false;
            }

            return _passwordHasher.Verify(password, usuario.PasswordHash);
        }

        public async Task<Usuario> CreateUserAsync(string dni, string password, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(dni))
            {
                throw new ArgumentException("El DNI es obligatorio.", nameof(dni));
            }

            if (string.IsNullOrEmpty(password))
            {
                throw new ArgumentException("La contraseña es obligatoria.", nameof(password));
            }

            var existente = await _usuarioRepository
                .GetByDniAndTenantAsync(dni.Trim(), DefaultTenantId, cancellationToken)
                .ConfigureAwait(false);
            if (existente is not null)
            {
                throw new UserAlreadyExistsException("Ya existe un usuario con ese DNI en este tenant.");
            }

            var usuario = new Usuario
            {
                Dni = dni.Trim(),
                PasswordHash = _passwordHasher.Hash(password),
                Role = DefaultRole,
                TenantId = DefaultTenantId,
                CreatedAt = DateTime.UtcNow,
            };

            await _usuarioRepository.AddAsync(usuario, cancellationToken).ConfigureAwait(false);
            return usuario;
        }
    }
}
