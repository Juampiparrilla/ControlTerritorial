using ControlTerritorial.Domain.Enum;

namespace ControlTerritorial.Application.DTOs
{
    public class PersonaResponseDTO
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string Apellido { get; set; } = string.Empty;
        public string DNI { get; set; } = string.Empty;
        public PersonRole Rol { get; set; }
        public string? Telefono { get; set; }

        public int? EscuelaId { get; set; }
        public string? EscuelaNombre { get; set; }

        public int? MesaId { get; set; }
        public int? NroMesa { get; set; }

        public int? LiderId { get; set; }
        public string? LiderNombre { get; set; }
    }
}

