using ControlTerritorial.Domain.Enum;

namespace ControlTerritorial.Domain.Entities
{
    public class Persona
    {
        public int Id { get; private set; }
        public string Nombre { get; private set; }
        public string Apellido { get; private set; }
        public string DNI { get; private set; }
        public PersonRole Rol { get; private set; }
        public string? Telefono { get; private set; }
        public Escuela? Escuela { get; private set; }

        public Persona()
        {
        }
        public Persona(string nombre, string apellido, string dni, PersonRole rol, string? telefono, Escuela? escuela)
        {
            if (string.IsNullOrEmpty(nombre))
            {
                throw new ArgumentException("El nombre no puede ser nulo o vacío.", nameof(nombre));
            }

            if (string.IsNullOrEmpty(apellido))
            {
                throw new ArgumentException("El Apellido no puede ser nulo o vacío.", nameof(apellido));
            }
             if (string.IsNullOrEmpty(dni))
            {
                throw new ArgumentException("El DNI no puede estar vacío.", nameof(dni));
            }

            Nombre = nombre;
            Apellido = apellido;
            DNI = dni;
            Rol = rol;
            Telefono = telefono;
            Escuela = escuela;
        }
    }
}


