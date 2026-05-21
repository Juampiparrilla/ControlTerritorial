using ControlTerritorial.Application.Configuration;
using ControlTerritorial.Application.Contracts;
using ControlTerritorial.Application.DTOs;
using ControlTerritorial.Application.Exceptions;
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

        [AllowAnonymous]
        [EnableRateLimiting("login")]
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequestDto request, CancellationToken cancellationToken)
        {
            if (request is null)
            {
                return BadRequest("El cuerpo de la solicitud es obligatorio.");
            }

            if (string.IsNullOrWhiteSpace(request.Dni) || string.IsNullOrEmpty(request.Password))
            {
                return BadRequest("DNI y contraseña son obligatorios.");
            }

            try
            {
                var dni = request.Dni.Trim();
                var valido = await _userService
                    .ValidateUserAsync(dni, request.Password, cancellationToken)
                    .ConfigureAwait(false);

                if (!valido)
                {
                    return Unauthorized();
                }

                var usuario = await _userService.GetByDniAsync(dni, cancellationToken).ConfigureAwait(false);
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

            if (string.IsNullOrWhiteSpace(request.Dni) || string.IsNullOrEmpty(request.Password))
            {
                return BadRequest("DNI y contraseña son obligatorios.");
            }

            if (request.Password.Length < 6)
            {
                return BadRequest("La contraseña debe tener al menos 6 caracteres.");
            }

            try
            {
                await _userService
                    .CreateUserAsync(request.Dni.Trim(), request.Password, cancellationToken)
                    .ConfigureAwait(false);
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
    }
}
