using ControlTerritorial.Domain.Enum;

namespace ControlTerritorial.Domain.Entities
{
    public class Vehiculo 
    {
        public int Id { get; private set; }
        public string Dominio { get; private set; }
        // Chofer
        public int ChoferId { get; private set; }
        public Persona Chofer { get; private set; }

        // EF - Relacion N : 1 con Persona (Chofer)
        public int RegistradoPorId { get; private set; }
        public Persona RegistradoPor { get; private set; }
        
        public VehicleType TipoVehiculo { get; private set; }
        private Vehiculo()
        {            
        }
        public Vehiculo(string dominio, Persona chofer, VehicleType tipoVehiculo, int registradoPorId) 
        {
            if (string.IsNullOrWhiteSpace(dominio))
                throw new ArgumentException("El dominio no puede ser vacío.", nameof(dominio));

            Dominio = dominio;  
            Chofer = chofer;
            TipoVehiculo = tipoVehiculo;
            RegistradoPorId = registradoPorId;
        }
    }
}
