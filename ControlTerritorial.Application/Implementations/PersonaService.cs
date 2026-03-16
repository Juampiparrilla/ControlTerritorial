using ControlTerritorial.Application.Contracts;
using ControlTerritorial.Application.DTOs;
using ControlTerritorial.Domain.Common;
using ControlTerritorial.Domain.Entities;
using ControlTerritorial.Domain.Enum;

namespace ControlTerritorial.Application.Implementations
{
    public class PersonaService : IPersonaService
    {
        private readonly IPersonaRepository _personaRepository;
        public PersonaService(IPersonaRepository personaRepository)
        {
            _personaRepository = personaRepository;
        }
        public async Task<Result<Persona>> CrearPersonaAsync(CreatePersonaDTO personaDto)
        {
            Escuela? escuelaNueva = null;

            if (!string.IsNullOrWhiteSpace(personaDto.Escuela))
            {
                escuelaNueva = new Escuela(personaDto.Escuela, string.Empty);
            }

            var persona = new Persona(
                personaDto.Nombre,
                personaDto.Apellido,
                personaDto.DNI,
                personaDto.Rol,
                personaDto.Telefono,
                escuelaNueva
            );

            return await _personaRepository.AddAsync(persona);
        }

        public async Task<Result<Persona>> EditarPersonaAsync(int id, CreatePersonaDTO personaDto)
        {
            var persona = await _personaRepository.GetByIdAsync(id);

            if (persona is null)
            {
                return Result<Persona>.Fail("La persona no existe.");
            }

            Escuela? escuelaNueva = null;

            if (!string.IsNullOrWhiteSpace(personaDto.Escuela))
            {
                escuelaNueva = new Escuela(personaDto.Escuela, string.Empty);
            }

            persona.ActualizarDatos(
                personaDto.Nombre,
                personaDto.Apellido,
                personaDto.DNI,
                personaDto.Rol,
                personaDto.Telefono,
                escuelaNueva
            );

            await _personaRepository.UpdateAsync(persona);

            return Result<Persona>.Success(persona);
        }

        public async Task<Result<bool>> EliminarPersonaAsync(int id)
        {
            var persona = await _personaRepository.GetByIdAsync(id);

            if (persona is null)
            {
                return Result<bool>.Fail("La persona no existe.");
            }

            await _personaRepository.DeleteAsync(id);

            return Result<bool>.Success(true);
        }

        public async Task<Result<Persona>> ObtenerPorDniAsync(string dni)
        {
            var persona = await _personaRepository.GetByDniAsync(dni);

            if (persona is null)
            {
                return Result<Persona>.Fail("No se encontró una persona con ese DNI.");
            }

            return Result<Persona>.Success(persona);
        }

        public async Task<Result<IEnumerable<Persona>>> ObtenerTodasAsync()
        {
            var personas = await _personaRepository.GetAllAsync();

            return Result<IEnumerable<Persona>>.Success(personas);
        }

        public async Task<Result<IEnumerable<Persona>>> BuscarPorNombreOApellidoAsync(string? nombre, string? apellido)
        {
            var personas = await _personaRepository.BuscarPorNombreOApellidoAsync(nombre, apellido);
            return Result<IEnumerable<Persona>>.Success(personas);
        }

        public async Task<Result<IEnumerable<Persona>>> BuscarPorRolAsync(PersonRole rol)
        {
            var personas = await _personaRepository.BuscarPorRolAsync(rol);
            return Result<IEnumerable<Persona>>.Success(personas);
        }
    }
}
