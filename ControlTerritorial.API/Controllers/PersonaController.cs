using ControlTerritorial.Application.Contracts;
using ControlTerritorial.Application.DTOs;
using ControlTerritorial.Domain.Entities;
using ControlTerritorial.Domain.Enum;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ControlTerritorial.API.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class PersonaController : ControllerBase
    {

        private readonly IPersonaService _personaService;
        public PersonaController(IPersonaService personaService)
        {
            _personaService = personaService;
        }

        [HttpGet("search-by-name")]
        public async Task<IActionResult> BuscarPorNombreOApellido([FromQuery] string? nombre, [FromQuery] string? apellido)
        {
            var result = await _personaService.BuscarPorNombreOApellidoAsync(nombre, apellido);

            if (!result.IsSuccess)
            {
                return BadRequest(result.ErrorMessage);
            }

            var responses = result.Value!
                .Select(entity => MapToResponse(entity));

            return Ok(responses);
        }

        [HttpGet("search-by-role/{rol}")]
        public async Task<IActionResult> BuscarPorRol(PersonRole rol)
        {
            var result = await _personaService.BuscarPorRolAsync(rol);

            if (!result.IsSuccess)
            {
                return BadRequest(result.ErrorMessage);
            }

            var responses = result.Value!
                .Select(entity => MapToResponse(entity));

            return Ok(responses);
        }

        [HttpGet("search-by-dni/{dni}")]
        public async Task<IActionResult> GetByDni(string dni)
        {
            var result = await _personaService.ObtenerPorDniAsync(dni);
            if (!result.IsSuccess)
            {
                return NotFound(result.ErrorMessage);
            }

            var entity = result.Value!;
            return Ok(MapToResponse(entity));
        }

        [HttpGet("getAll")]
        public async Task<IActionResult> GetAll()
        {
            var result = await _personaService.ObtenerTodasAsync();

            if (!result.IsSuccess)
            {
                return BadRequest(result.ErrorMessage);
            }

            var responses = result.Value!
                .Select(entity => MapToResponse(entity));

            return Ok(responses);
        }

        [HttpPost("create")]
        public async Task<IActionResult> CreatePersona([FromBody] CreatePersonaDTO persona)
        {
            var result = await _personaService.CrearPersonaAsync(persona);
            if (!result.IsSuccess)
            {
                return BadRequest(result.ErrorMessage);
            }

            var entity = result.Value!;
            return Ok(MapToResponse(entity));
        }

        [HttpPut("update/{id:int}")]
        public async Task<IActionResult> EditarPersona(int id, [FromBody] CreatePersonaDTO persona)
        {
            var result = await _personaService.EditarPersonaAsync(id, persona);
            if (!result.IsSuccess)
            {
                return BadRequest(result.ErrorMessage);
            }

            var entity = result.Value!;
            return Ok(MapToResponse(entity));
        }

        [HttpDelete("delete/{id:int}")]
        public async Task<IActionResult> EliminarPersona(int id)
        {
            var result = await _personaService.EliminarPersonaAsync(id);

            if (!result.IsSuccess)
            {
                return BadRequest(result.ErrorMessage);
            }

            return NoContent();
        }

        private static PersonaResponseDTO MapToResponse(Persona entity)
        {
            return new PersonaResponseDTO
            {
                Id = entity.Id,
                Nombre = entity.Nombre,
                Apellido = entity.Apellido,
                DNI = entity.DNI,
                Rol = entity.Rol,
                Telefono = entity.Telefono,
                EscuelaId = entity.EscuelaId,
                EscuelaNombre = entity.Escuela?.NombreEstablecimiento,
                MesaId = entity.MesaId,
                NroMesa = entity.Mesa?.NroMesa,
                LiderId = entity.LiderId,
                LiderNombre = entity.Lider != null ? $"{entity.Lider.Nombre} {entity.Lider.Apellido}".Trim() : null
            };
        }
    }
}
