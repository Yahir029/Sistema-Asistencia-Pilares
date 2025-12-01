using System;
using System.Collections.Generic;

namespace AsistenciaAPI.Application.DTOs
{
    public record ReporteRequestDto
    {
        public DateTime FechaInicio { get; init; }
        public DateTime FechaFin { get; init; }
        public Guid? EmpleadoId { get; init; }
        // Nuevo: soporte para múltiples empleados
        public List<Guid>? EmpleadoIds { get; init; }
    }
}
