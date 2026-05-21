using ControlTerritorial.Domain.Enum;

namespace ControlTerritorial.Application.DTOs
{
    using System.Text.Json.Serialization;

    public class RegisterRequestDto
    {
        public string Username { get; set; } = string.Empty;
        public string? Dni { get; set; }
        public string Password { get; set; } = string.Empty;

        [JsonPropertyName("role")]
        public SystemRole SystemRole { get; set; } = SystemRole.Operador;
    }
}
