using ControlTerritorial.Domain.Enum;

namespace ControlTerritorial.Domain.Entities
{
    public class Usuario
    {
        public int Id { get; set; }
        public int? PersonaId { get; set; }
        public string Username { get; set; } = string.Empty;
        public string PasswordHash { get; set; } = string.Empty;
        public SystemRole SystemRole { get; set; }
        public bool IsActive { get; set; } = true;
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public Persona? Persona { get; set; }

        public void TouchUpdated() => UpdatedAt = DateTime.UtcNow;
    }
}
