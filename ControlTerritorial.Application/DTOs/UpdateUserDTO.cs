using ControlTerritorial.Domain.Enum;

namespace ControlTerritorial.Application.DTOs
{
    public class UpdateUserDTO
    {
        public string Username { get; set; } = string.Empty;
        public SystemRole Role { get; set; }
        public int? PersonaId { get; set; }
        public bool IsActive { get; set; }
    }
}
