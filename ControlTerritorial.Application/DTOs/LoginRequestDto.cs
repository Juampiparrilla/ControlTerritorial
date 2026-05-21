namespace ControlTerritorial.Application.DTOs
{
    public class LoginRequestDto
    {
        /// <summary>Nombre de usuario para iniciar sesión.</summary>
        public string Username { get; set; } = string.Empty;

        /// <summary>Compatibilidad temporal: si Username está vacío, se usa Dni como username.</summary>
        public string? Dni { get; set; }

        public string Password { get; set; } = string.Empty;
    }
}
