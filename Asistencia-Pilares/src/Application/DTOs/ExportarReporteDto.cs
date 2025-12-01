using System;
using System.Collections.Generic;

namespace AsistenciaAPI.Application.DTOs
{
    public record ExportarReporteDto
    {
        public DateTime FechaInicio { get; init; }
        public DateTime FechaFin { get; init; }
        public List<ReporteFilaDto> Datos { get; init; } = new();
    }
}
