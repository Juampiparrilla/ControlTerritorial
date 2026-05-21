using ControlTerritorial.Domain.Entities;

namespace ControlTerritorial.Application.Contracts
{
    public interface IJwtTokenService
    {
        (string Token, DateTime ExpiresAtUtc) GenerateToken(Usuario usuario);
    }
}
