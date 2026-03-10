namespace ControlTerritorial.Domain.Enum
{
    public enum RolePersona
    {
        Administrador,         // Puede gestionar usuarios, roles, permisos y ver toda la Información de todos los grupos.
        Grupo,                 // Puede gestionar su propia Información, ver la Información de sus miembros (Referentes y Punteros).
        Referente,             // Puede gestionar su propia Información, ver la Información de sus miembros (Punteros).
        Puntero,               // Solo puede ver y gestionar su propia Información y la de su listado.
        Votante,               // Solo puede ver su propia Información y votar en elecciones.
        Chofer
    }

    public enum RoleAuto
    {
        Auto,
        Colectivo,
        Traffic
    }
}

