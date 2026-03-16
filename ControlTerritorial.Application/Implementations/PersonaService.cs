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
            // Validar unicidad de DNI
            var existenteConMismoDni = await _personaRepository.GetByDniAsync(personaDto.DNI);
            if (existenteConMismoDni is not null)
            {
                return Result<Persona>.Fail("Ya existe una persona registrada con ese DNI.");
            }

            // Validar reglas de jerarquía según rol
            var validacionJerarquia = await ValidarJerarquiaAsync(personaDto.Rol, personaDto.LiderId);
            if (validacionJerarquia.IsFail)
            {
                return Result<Persona>.Fail(validacionJerarquia.ErrorMessage!);
            }

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

            // Validar unicidad de DNI (permitiendo que la misma persona conserve su DNI)
            var existenteConMismoDni = await _personaRepository.GetByDniAsync(personaDto.DNI);
            if (existenteConMismoDni is not null && existenteConMismoDni.Id != id)
            {
                return Result<Persona>.Fail("Ya existe otra persona registrada con ese DNI.");
            }

            // Validar reglas de jerarquía según rol
            var validacionJerarquia = await ValidarJerarquiaAsync(personaDto.Rol, personaDto.LiderId);
            if (validacionJerarquia.IsFail)
            {
                return Result<Persona>.Fail(validacionJerarquia.ErrorMessage!);
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

            // No permitir borrar si tiene subordinados asociados
            var tieneSubordinados = await _personaRepository.TieneSubordinadosAsync(id);
            if (tieneSubordinados)
            {
                return Result<bool>.Fail("No se puede eliminar la persona porque tiene subordinados asociados.");
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

        private async Task<Result<bool>> ValidarJerarquiaAsync(PersonRole rol, int? liderId)
        {
            // Administrador: no requiere líder
            if (rol == PersonRole.Administrador)
            {
                if (liderId.HasValue)
                {
                    return Result<bool>.Fail("Un administrador no debe tener líder asignado.");
                }

                return Result<bool>.Success(true);
            }

            // Para el resto de roles (Grupo, Referente, Puntero, Votante, Chofer) si se especifica líder, validamos que exista
            if (!liderId.HasValue)
            {
                // Para Referente, Puntero y Votante exigimos líder
                if (rol == PersonRole.Grupo || rol == PersonRole.Chofer)
                {
                    return Result<bool>.Success(true);
                }

                return Result<bool>.Fail("El rol seleccionado requiere un líder asociado.");
            }

            var lider = await _personaRepository.GetByIdAsync(liderId.Value);
            if (lider is null)
            {
                return Result<bool>.Fail("El líder asociado no existe.");
            }

            // Reglas específicas de jerarquía
            switch (rol)
            {
                case PersonRole.Grupo:
                    if (lider.Rol != PersonRole.Administrador)
                    {
                        return Result<bool>.Fail("Un grupo debe tener como líder a un administrador.");
                    }
                    break;

                case PersonRole.Referente:
                    if (lider.Rol != PersonRole.Grupo)
                    {
                        return Result<bool>.Fail("Un referente debe tener como líder a un grupo.");
                    }
                    break;

                case PersonRole.Puntero:
                    if (lider.Rol != PersonRole.Referente)
                    {
                        return Result<bool>.Fail("Un puntero debe tener como líder a un referente.");
                    }
                    break;

                case PersonRole.Votante:
                    if (lider.Rol != PersonRole.Puntero)
                    {
                        return Result<bool>.Fail("Un votante debe tener como líder a un puntero.");
                    }
                    break;

                case PersonRole.Chofer:
                    // Chofer puede tener como líder a cualquier rol, por ahora no restringimos más
                    break;
            }

            return Result<bool>.Success(true);
        }
    }
}
