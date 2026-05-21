using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using ControlTerritorial.Application.Configuration;
using ControlTerritorial.Application.Contracts;
using ControlTerritorial.Domain.Entities;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace ControlTerritorial.Infrastructure.Security
{
    public sealed class JwtTokenService : IJwtTokenService
    {
        private readonly JwtSettings _settings;

        public JwtTokenService(IOptions<JwtSettings> options)
        {
            _settings = options.Value;
        }

        public (string Token, DateTime ExpiresAtUtc) GenerateToken(Usuario usuario)
        {
            if (string.IsNullOrWhiteSpace(_settings.Key))
            {
                throw new InvalidOperationException("Jwt:Key no está configurada.");
            }

            var expiresAtUtc = DateTime.UtcNow.AddMinutes(_settings.ExpirationMinutes);

            var claims = new List<Claim>
            {
                new(ClaimTypes.NameIdentifier, usuario.Id.ToString()),
                new("userId", usuario.Id.ToString()),
                new("dni", usuario.Dni),
                new(ClaimTypes.Role, usuario.Role),
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_settings.Key));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _settings.Issuer,
                audience: _settings.Audience,
                claims: claims,
                expires: expiresAtUtc,
                signingCredentials: credentials);

            var tokenString = new JwtSecurityTokenHandler().WriteToken(token);
            return (tokenString, expiresAtUtc);
        }
    }
}
