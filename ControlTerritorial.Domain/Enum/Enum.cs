namespace ControlTerritorial.Domain.Enum
{
    public enum PersonRole
    {
        [Obsolete("Rol territorial obsoleto. Usar SystemRole en Usuario para accesos al sistema.")]
        Administrador = 0,
        Grupo,                 // Puede gestionar su propia Información, ver la Información de sus miembros (Referentes y Punteros).
        Referente,             // Puede gestionar su propia Información, ver la Información de sus miembros (Punteros).
        Puntero,               // Solo puede ver y gestionar su propia Información y la de su listado.
        Votante,               // Solo puede ver su propia Información y votar en elecciones.
        Chofer
    }

    public enum VehicleType
    {
        Auto,
        Colectivo,
        Traffic
    }
}

