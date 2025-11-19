using System;

namespace AsistenciaAPI.Application.DTOs
{
    public record ReporteFilaDto
    {
        public string NombreEmpleado { get; init; } = string.Empty;
        // Fecha (solo la parte de fecha)
        public DateTime Fecha { get; init; }
        // HoraEntrada como DateTime (fecha+hora) para facilidad de uso
        public DateTime HoraEntrada { get; init; }
        public DateTime? HoraSalida { get; init; }
    }
}
