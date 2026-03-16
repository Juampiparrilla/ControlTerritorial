using ControlTerritorial.Application.DTOs;
using ControlTerritorial.Domain.Common;
using ControlTerritorial.Domain.Entities;

namespace ControlTerritorial.Application.Contracts
{
    public interface IPersonaService
    {
        Task<Result<Persona>> CrearPersonaAsync(CreatePersonaDTO personaDto);
    }
}
