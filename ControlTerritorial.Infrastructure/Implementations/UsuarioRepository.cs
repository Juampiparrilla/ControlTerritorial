using ControlTerritorial.Application.Contracts;
using ControlTerritorial.Application.Exceptions;
using ControlTerritorial.Domain.Entities;
using ControlTerritorial.Domain.Enum;
using Microsoft.EntityFrameworkCore;
using Npgsql;

namespace ControlTerritorial.Infrastructure.Implementations
{
    public class UsuarioRepository : IUsuarioRepository
    {
        private readonly DbContextControlTerritorial _context;

        public UsuarioRepository(DbContextControlTerritorial context)
        {
            _context = context;
        }

        public async Task<Usuario?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            return await _context.Usuarios
                .Include(u => u.Persona)
                .FirstOrDefaultAsync(u => u.Id == id, cancellationToken)
                .ConfigureAwait(false);
        }

        public async Task<Usuario?> GetByUsernameAsync(string username, CancellationToken cancellationToken = default)
        {
            var normalized = username.Trim().ToLowerInvariant();
            return await _context.Usuarios
                .Include(u => u.Persona)
                .FirstOrDefaultAsync(u => u.Username == normalized, cancellationToken)
                .ConfigureAwait(false);
        }

        public async Task<IReadOnlyList<Usuario>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            return await _context.Usuarios
                .Include(u => u.Persona)
                .OrderBy(u => u.Username)
                .AsNoTracking()
                .ToListAsync(cancellationToken)
                .ConfigureAwait(false);
        }

        public Task<int> CountByRoleAsync(SystemRole role, CancellationToken cancellationToken = default)
        {
            return _context.Usuarios.CountAsync(u => u.Role == role, cancellationToken);
        }

        public async Task AddAsync(Usuario usuario, CancellationToken cancellationToken = default)
        {
            try
            {
                await _context.Usuarios.AddAsync(usuario, cancellationToken).ConfigureAwait(false);
                await _context.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
            }
            catch (DbUpdateException ex) when (IsUniqueConstraintViolation(ex))
            {
                throw new UserAlreadyExistsException("Ya existe un usuario con ese nombre de usuario o persona vinculada.");
            }
        }

        public async Task UpdateAsync(Usuario usuario, CancellationToken cancellationToken = default)
        {
            try
            {
                _context.Usuarios.Update(usuario);
                await _context.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
            }
            catch (DbUpdateException ex) when (IsUniqueConstraintViolation(ex))
            {
                throw new UserAlreadyExistsException("Ya existe un usuario con ese nombre de usuario o persona vinculada.");
            }
        }

        public async Task DeleteAsync(Usuario usuario, CancellationToken cancellationToken = default)
        {
            _context.Usuarios.Remove(usuario);
            await _context.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
        }

        public Task<bool> ExistsUsernameAsync(string username, int? excludeUserId = null, CancellationToken cancellationToken = default)
        {
            var normalized = username.Trim().ToLowerInvariant();
            var query = _context.Usuarios.Where(u => u.Username == normalized);
            if (excludeUserId.HasValue)
            {
                query = query.Where(u => u.Id != excludeUserId.Value);
            }

            return query.AnyAsync(cancellationToken);
        }

        public Task<bool> ExistsPersonaLinkAsync(int personaId, int? excludeUserId = null, CancellationToken cancellationToken = default)
        {
            var query = _context.Usuarios.Where(u => u.PersonaId == personaId);
            if (excludeUserId.HasValue)
            {
                query = query.Where(u => u.Id != excludeUserId.Value);
            }

            return query.AnyAsync(cancellationToken);
        }

        private static bool IsUniqueConstraintViolation(DbUpdateException ex)
        {
            return ex.InnerException is PostgresException pg && pg.SqlState == PostgresErrorCodes.UniqueViolation;
        }
    }
}
