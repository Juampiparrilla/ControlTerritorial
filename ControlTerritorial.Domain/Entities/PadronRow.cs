using System;

namespace ControlTerritorial.Domain.Entities
{
    public class PadronRow
    {
        public int Id { get; private set; }
        public int PadronImportId { get; private set; }
        public PadronImport PadronImport { get; private set; } = null!;

        public string DNI { get; private set; } = string.Empty;
        public string Nombre { get; private set; } = string.Empty;
        public string Apellido { get; private set; } = string.Empty;
        public string EscuelaNombre { get; private set; } = string.Empty;
        public int MesaNro { get; private set; }
        public string? Orden { get; private set; }

        private PadronRow() { }

        public PadronRow(string dni, string nombre, string apellido, string escuelaNombre, int mesaNro, string? orden, int padronImportId)
        {
            if (string.IsNullOrWhiteSpace(dni)) throw new ArgumentException("DNI requerido", nameof(dni));
            if (string.IsNullOrWhiteSpace(nombre)) throw new ArgumentException("Nombre requerido", nameof(nombre));
            if (string.IsNullOrWhiteSpace(apellido)) throw new ArgumentException("Apellido requerido", nameof(apellido));
            if (string.IsNullOrWhiteSpace(escuelaNombre)) throw new ArgumentException("Escuela requerido", nameof(escuelaNombre));
            if (mesaNro < 1) throw new ArgumentException("MesaNro inválido", nameof(mesaNro));
            if (padronImportId < 1) throw new ArgumentException("padronImportId inválido", nameof(padronImportId));

            DNI = dni.Trim();
            Nombre = nombre.Trim();
            Apellido = apellido.Trim();
            EscuelaNombre = escuelaNombre.Trim();
            MesaNro = mesaNro;
            Orden = string.IsNullOrWhiteSpace(orden) ? null : orden.Trim();
            PadronImportId = padronImportId;
        }
    }
}

