using ControlTerritorial.Domain.Enum;

namespace ControlTerritorial.Application.DTOs
{
    using System.Text.Json.Serialization;

    public class CreateUserDTO
    {
        public string Username { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;

        [JsonPropertyName("role")]
        public SystemRole SystemRole { get; set; }
        public int? PersonaId { get; set; }
        public bool IsActive { get; set; } = true;
    }
}
