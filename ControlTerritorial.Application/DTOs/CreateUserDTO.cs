using ControlTerritorial.Domain.Enum;

namespace ControlTerritorial.Application.DTOs
{
    public class CreateUserDTO
    {
        public string Username { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public SystemRole Role { get; set; }
        public int? PersonaId { get; set; }
        public bool IsActive { get; set; } = true;
    }
}
