namespace ControlTerritorial.Domain.Entities
{
    public class Usuario
    {
        public int Id { get; set; }
        public string Dni { get; set; } = string.Empty;
        public string PasswordHash { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
        public string TenantId { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
    }
}
