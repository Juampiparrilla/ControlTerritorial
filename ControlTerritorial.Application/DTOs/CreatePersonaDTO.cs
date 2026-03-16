using ControlTerritorial.Domain.Enum;

namespace ControlTerritorial.Application.DTOs
{
    public class CreatePersonaDTO
    {
        public string Nombre { get; set; } = string.Empty;
        public string Apellido { get; set; } = string.Empty;
        public string DNI { get; set; } = string.Empty;
        public PersonRole Rol { get; set; }
        public string? Telefono { get; set; }
        public string? Escuela { get; set; }
        public string? Mesa { get; set; }
        public int? LiderId { get; set; }
    }
}
