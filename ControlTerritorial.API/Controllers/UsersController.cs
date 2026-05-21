using ControlTerritorial.Application.Configuration;
using ControlTerritorial.Application.Contracts;
using ControlTerritorial.Application.DTOs;
using ControlTerritorial.Application.Exceptions;
using ControlTerritorial.Domain.Enum;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.Extensions.Options;

namespace ControlTerritorial.API.Controllers
{
    [Route("api/users")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly IJwtTokenService _jwtTokenService;
        private readonly FeatureSettings _features;

        public UsersController(
            IUserService userService,
            IJwtTokenService jwtTokenService,
            IOptions<FeatureSettings> features)
        {
            _userService = userService;
            _jwtTokenService = jwtTokenService;
            _features = features.Value;
        }

        [Authorize(Roles = nameof(SystemRole.AdminSistema))]
        [HttpGet]
        public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
        {
            var users = await _userService.GetAllAsync(cancellationToken).ConfigureAwait(false);
            return Ok(users);
        }

        [Authorize(Roles = nameof(SystemRole.AdminSistema))]
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id, CancellationToken cancellationToken)
        {
            var usuario = await _userService.GetByIdAsync(id, cancellationToken).ConfigureAwait(false);
            if (usuario is null)
            {
                return NotFound();
            }

            var response = await _userService.MapToResponseAsync(usuario, cancellationToken).ConfigureAwait(false);
            return Ok(response);
        }

        [Authorize(Roles = nameof(SystemRole.AdminSistema))]
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateUserDTO request, CancellationToken cancellationToken)
        {
            if (request is null)
            {
                return BadRequest("El cuerpo de la solicitud es obligatorio.");
            }

            try
            {
                var created = await _userService.CreateAsync(request, cancellationToken).ConfigureAwait(false);
                return Ok(created);
            }
            catch (UserAlreadyExistsException ex)
            {
                return Conflict(ex.Message);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Error al crear el usuario.");
            }
        }

        [Authorize(Roles = nameof(SystemRole.AdminSistema))]
        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateUserDTO request, CancellationToken cancellationToken)
        {
            if (request is null)
            {
                return BadRequest("El cuerpo de la solicitud es obligatorio.");
            }

            try
            {
                var updated = await _userService.UpdateAsync(id, request, cancellationToken).ConfigureAwait(false);
                return Ok(updated);
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
            catch (UserAlreadyExistsException ex)
            {
                return Conflict(ex.Message);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Error al actualizar el usuario.");
            }
        }

        [Authorize(Roles = nameof(SystemRole.AdminSistema))]
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
        {
            try
            {
                await _userService.DeleteAsync(id, cancellationToken).ConfigureAwait(false);
                return NoContent();
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Error al eliminar el usuario.");
            }
        }

        [Authorize(Roles = nameof(SystemRole.AdminSistema))]
        [HttpPatch("{id:int}/reset-password")]
        public async Task<IActionResult> ResetPassword(int id, [FromBody] ResetPasswordDTO request, CancellationToken cancellationToken)
        {
            if (request is null || string.IsNullOrEmpty(request.Password))
            {
                return BadRequest("La contraseña es obligatoria.");
            }

            try
            {
                await _userService.ResetPasswordAsync(id, request.Password, cancellationToken).ConfigureAwait(false);
                return NoContent();
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Error al restablecer la contraseña.");
            }
        }

        [Authorize(Roles = nameof(SystemRole.AdminSistema))]
        [HttpPatch("{id:int}/toggle-active")]
        public async Task<IActionResult> ToggleActive(int id, CancellationToken cancellationToken)
        {
            try
            {
                var updated = await _userService.ToggleActiveAsync(id, cancellationToken).ConfigureAwait(false);
                return Ok(updated);
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Error al cambiar el estado del usuario.");
            }
        }

        [AllowAnonymous]
        [EnableRateLimiting("login")]
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequestDto request, CancellationToken cancellationToken)
        {
            if (request is null)
            {
                return BadRequest("El cuerpo de la solicitud es obligatorio.");
            }

            var username = ResolveUsername(request);
            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrEmpty(request.Password))
            {
                return BadRequest("Usuario y contraseña son obligatorios.");
            }

            try
            {
                var valido = await _userService
                    .ValidateUserAsync(username, request.Password, cancellationToken)
                    .ConfigureAwait(false);

                if (!valido)
                {
                    return Unauthorized();
                }

                var usuario = await _userService.GetByUsernameAsync(username, cancellationToken).ConfigureAwait(false);
                if (usuario is null)
                {
                    return Unauthorized();
                }

                var (token, expiresAtUtc) = _jwtTokenService.GenerateToken(usuario);
                return Ok(new LoginResponseDto
                {
                    Token = token,
                    ExpiresAtUtc = expiresAtUtc,
                });
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Error al iniciar sesión.");
            }
        }

        [AllowAnonymous]
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequestDto request, CancellationToken cancellationToken)
        {
            if (!_features.AllowPublicRegistration)
            {
                return NotFound();
            }

            if (request is null)
            {
                return BadRequest("El cuerpo de la solicitud es obligatorio.");
            }

            var username = ResolveUsername(request.Username, request.Dni);
            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrEmpty(request.Password))
            {
                return BadRequest("Usuario y contraseña son obligatorios.");
            }

            try
            {
                await _userService.CreateAsync(
                    new CreateUserDTO
                    {
                        Username = username,
                        Password = request.Password,
                        Role = request.Role,
                        IsActive = true,
                    },
                    cancellationToken).ConfigureAwait(false);
                return Ok();
            }
            catch (UserAlreadyExistsException ex)
            {
                return Conflict(ex.Message);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Error al registrar el usuario.");
            }
        }

        private static string ResolveUsername(LoginRequestDto request)
        {
            return ResolveUsername(request.Username, request.Dni);
        }

        private static string ResolveUsername(string username, string? legacyDni)
        {
            if (!string.IsNullOrWhiteSpace(username))
            {
                return username.Trim();
            }

            return legacyDni?.Trim() ?? string.Empty;
        }
    }
}
