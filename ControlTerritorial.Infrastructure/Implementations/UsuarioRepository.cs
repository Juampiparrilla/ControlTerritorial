using ControlTerritorial.Application.Contracts;
using ControlTerritorial.Application.Exceptions;
using ControlTerritorial.Domain.Entities;
using ControlTerritorial.Infrastructure;
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

        public async Task<Usuario?> GetByDniAndTenantAsync(string dni, string tenantId, CancellationToken cancellationToken = default)
        {
            return await _context.Usuarios
                .AsNoTracking()
                .FirstOrDefaultAsync(
                    u => u.Dni == dni && u.TenantId == tenantId,
                    cancellationToken)
                .ConfigureAwait(false);
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
                throw new UserAlreadyExistsException("Ya existe un usuario con ese DNI en este tenant.");
            }
        }

        /// <summary>Violación de índice único en PostgreSQL (23505).</summary>
        private static bool IsUniqueConstraintViolation(DbUpdateException ex)
        {
            return ex.InnerException is PostgresException pg && pg.SqlState == PostgresErrorCodes.UniqueViolation;
        }
    }
}
