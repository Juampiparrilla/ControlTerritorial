namespace ControlTerritorial.Application.DTOs
{
    public class LoginRequestDto
    {
        public string Dni { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }
}
