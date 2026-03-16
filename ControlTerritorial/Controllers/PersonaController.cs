using ControlTerritorial.Application.Contracts;
using ControlTerritorial.Application.DTOs;
using ControlTerritorial.Application.Implementations;
using Microsoft.AspNetCore.Mvc;

namespace ControlTerritorial.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PersonaController : ControllerBase
    {

        private readonly IPersonaService _personaService;
        public PersonaController(IPersonaService personaService)
        {
            _personaService = personaService;
        }

        [HttpPost]
        public async Task<IActionResult> CreatePersona([FromBody] CreatePersonaDTO persona)
        {
            var result = await _personaService.CrearPersonaAsync(persona);
            if (result.IsSuccess)
            {
                return Ok(result.Value);
            }
            else
            {
                return BadRequest(result.ErrorMessage);
            }

        }
    }
}
