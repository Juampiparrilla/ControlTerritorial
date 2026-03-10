namespace ControlTerritorial.Domain.Entities
{
    public class Escuela
    {
        public int Id { get; private set; }
        public string NombreEstablecimiento { get; private set; }
        public Mesa Mesa { get; private set; }
        public string? Direccion { get; private set; }
        public Escuela()
        {
        }

        public Escuela(string nombreEstablecimiento, Mesa mesa, string? direccion)
        {
            if (string.IsNullOrEmpty(nombreEstablecimiento))
            {
                throw new ArgumentException("El nombre de la esculea no puede ser vacio.", nameof(nombreEstablecimiento));
            }

            NombreEstablecimiento = nombreEstablecimiento;
            Mesa = mesa; 
            Direccion = direccion;
        }

    }
}
