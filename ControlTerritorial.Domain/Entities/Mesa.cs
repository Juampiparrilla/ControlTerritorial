namespace ControlTerritorial.Domain.Entities
{
    public class Mesa
    {
        public int Id { get; private set; }
        public int NroMesa { get; private set; }
        public string Orden { get; private set; }      
        public Mesa()
        {
        }

        public Mesa(int nroMesa, string orden)
        {
            if (nroMesa < 1)
            {
                throw new ArgumentException("El numero de mesa no puede ser negativo.", nameof(nroMesa));
            }

            if (string.IsNullOrEmpty(orden))
            {
                throw new ArgumentException("El Orden no puede ser vacio.", nameof(orden));
            }

            NroMesa = nroMesa;
            Orden = orden;           
        }
    }
}
