using System;

namespace AsistenciaAPI.Application.DTOs
{
    public record AreaDto(Guid Id, string Nombre);
    public record CreateAreaDto(string Nombre);
}
