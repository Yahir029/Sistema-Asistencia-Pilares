using System;
using System.Collections.Generic;

namespace AsistenciaAPI.Application.DTOs
{
    public record EmpleadoDto(Guid Id, string IdEmpleadoExterno, string Nombre, string NombreArea, string NombreRol, bool EstaActivo, List<HorarioDto> Horarios);
}
