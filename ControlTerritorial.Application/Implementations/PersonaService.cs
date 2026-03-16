using ControlTerritorial.Application.Contracts;
using ControlTerritorial.Application.DTOs;
using ControlTerritorial.Domain.Common;
using ControlTerritorial.Domain.Entities;

namespace ControlTerritorial.Application.Implementations
{
    public class PersonaService : IPersonaService
    {
        private readonly IPersonaRepository _personaRepository;
        public PersonaService(IPersonaRepository personaRepository)
        {
            _personaRepository = personaRepository;
        }
        public Task<Result<Persona>> CrearPersonaAsync(CreatePersonaDTO personaDto)
        {

            if (!string.IsNullOrEmpty(personaDto.Escuela))
            {
                // Desarrollar lógica para validar la escuela, por ejemplo, verificar si existe en la base de datos

                var escuelaNueva = new Escuela(personaDto.Escuela, string.Empty);
            }

            if (!string.IsNullOrEmpty(personaDto.Mesa))
            {
                // Desarrollar lógica para validar la mesa, por ejemplo, verificar si existe en la base de datos

                var MesaNueva = new Mesa (int.Parse(personaDto.Mesa), string.Empty);
            }


            //var persona = new Persona
            //{
            //    Nombre = personaDto.Nombre,
            //    Apellido = personaDto.Apellido,
            //    DNI = personaDto.DNI,
            //    Rol = personaDto.Rol,
            //    Telefono = personaDto.Telefono,
            //    Escuela = escuelaNueva,
            //    EscuelaId = 0, // Asignar el ID de la escuela si es necesario
            //    Mesa = mesaNueva,
            //    MesaId = 0, // Asignar el ID de la mesa si es necesario
            //};

            throw new NotImplementedException();
        }
    }
}
