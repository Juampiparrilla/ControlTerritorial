namespace ControlTerritorial.Application.DTOs
{
    public class PadronRowImportDTO
    {
        public string DNI { get; set; } = string.Empty;
        public string Nombre { get; set; } = string.Empty;
        public string Apellido { get; set; } = string.Empty;
        public string EscuelaNombre { get; set; } = string.Empty;
        public int MesaNro { get; set; }
        public string? Orden { get; set; }
    }
}

