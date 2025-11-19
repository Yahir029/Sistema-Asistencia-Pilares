using System;

namespace AsistenciaAPI.Application.DTOs
{
    public record RolDto(Guid Id, string Nombre);
    public record CreateRolDto(string Nombre);
}
