using System;

namespace AsistenciaAPI.Application.DTOs
{
    public record ResultadoAsistenciaDto
    {
        public bool Exito { get; init; }
        public string Mensaje { get; init; } = string.Empty;
        public DateTime FechaHora { get; init; }
        public string NombreEmpleado { get; init; } = string.Empty;
        // Tipo de marcación realizada: "Entrada" | "Salida" | string.Empty
        public string Tipo { get; init; } = string.Empty;
    }
}
