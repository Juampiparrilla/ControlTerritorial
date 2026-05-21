namespace ControlTerritorial.Application.Contracts
{
    /// <summary>
    /// Abstrae el algoritmo de hash de contraseñas (implementación concreta en Infrastructure).
    /// </summary>
    public interface IPasswordHasher
    {
        string Hash(string password);
        bool Verify(string password, string passwordHash);
    }
}
