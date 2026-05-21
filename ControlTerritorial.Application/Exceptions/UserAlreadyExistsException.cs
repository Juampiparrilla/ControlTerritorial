namespace ControlTerritorial.Application.Exceptions
{
    /// <summary>
    /// Indica que ya existe un usuario con el mismo DNI en el tenant (regla de negocio o restricción única en BD).
    /// </summary>
    public sealed class UserAlreadyExistsException : Exception
    {
        public UserAlreadyExistsException(string message)
            : base(message)
        {
        }
    }
}
