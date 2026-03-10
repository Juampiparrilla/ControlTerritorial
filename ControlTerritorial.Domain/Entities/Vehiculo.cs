namespace ControlTerritorial.Domain.Entities
{
    public class Vehiculo 
    {
        public int Id { get; private set; }
        public string Dominio { get; private set; }
        public Persona Chofer { get; private set; }
        public Vehiculo(string dominio, Persona chofer) 
        {
            if (string.IsNullOrWhiteSpace(dominio))
                throw new ArgumentException("El dominio no puede ser vacío.", nameof(dominio));

            Dominio = dominio;  
            Chofer = chofer;
        }
    }
}
