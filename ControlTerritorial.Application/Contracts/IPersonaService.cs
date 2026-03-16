using ControlTerritorial.Application.DTOs;
using ControlTerritorial.Domain.Common;
using ControlTerritorial.Domain.Entities;
using ControlTerritorial.Domain.Enum;

namespace ControlTerritorial.Application.Contracts
{
    public interface IPersonaService
    {
        Task<Result<Persona>> CrearPersonaAsync(CreatePersonaDTO personaDto);
        Task<Result<Persona>> EditarPersonaAsync(int id, CreatePersonaDTO personaDto);
        Task<Result<bool>> EliminarPersonaAsync(int id);
        Task<Result<Persona>> ObtenerPorDniAsync(string dni);
        Task<Result<IEnumerable<Persona>>> ObtenerTodasAsync();
        Task<Result<IEnumerable<Persona>>> BuscarPorNombreOApellidoAsync(string? nombre, string? apellido);
        Task<Result<IEnumerable<Persona>>> BuscarPorRolAsync(PersonRole rol);
    }
}
