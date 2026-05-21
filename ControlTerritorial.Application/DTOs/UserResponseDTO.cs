using ControlTerritorial.Domain.Enum;

namespace ControlTerritorial.Application.DTOs
{
    public class UserResponseDTO
    {
        public int Id { get; set; }
        public string Username { get; set; } = string.Empty;
        public SystemRole Role { get; set; }
        public bool IsActive { get; set; }
        public int? PersonaId { get; set; }
        public string? PersonaNombre { get; set; }
        public string? PersonaApellido { get; set; }
        public string? PersonaDni { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}
