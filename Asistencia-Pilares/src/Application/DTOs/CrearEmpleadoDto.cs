using System;
using System.Collections.Generic;

namespace AsistenciaAPI.Application.DTOs
{
    public record CrearEmpleadoDto(string IdEmpleadoExterno, string Nombre, string NombreArea, string NombreRol, List<HorarioDto> Horarios, bool EsAdmin = false, string? Password = null);
}
