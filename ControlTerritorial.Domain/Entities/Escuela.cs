namespace ControlTerritorial.Domain.Entities
{
    public class Escuela
    {
        public int Id { get; private set; }
        public string? NombreEstablecimiento { get; private set; }
       
        // EF - Relacion 1 : N con Mesas
        public List<Mesa> Mesas { get; private set; } = new List<Mesa>();
        public string? Direccion { get; private set; }

        // EF - Relacion 1 : N con Persona
        public List<Persona> Personas { get; private set; } = new List<Persona>();
        
        private Escuela()
        {
        }

        public Escuela(string nombreEstablecimiento, string? direccion)
        {
            if (string.IsNullOrEmpty(nombreEstablecimiento))
            {
                throw new ArgumentException("El nombre de la esculea no puede ser vacio.", nameof(nombreEstablecimiento));
            }

            NombreEstablecimiento = nombreEstablecimiento;
            Mesas = new List<Mesa>(); 
            Direccion = direccion;
        }
    }
}
