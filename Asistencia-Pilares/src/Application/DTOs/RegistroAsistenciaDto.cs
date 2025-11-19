using System;

namespace AsistenciaAPI.Application.DTOs
{
    public record RegistroAsistenciaDto
    {
        /// <summary>
        /// Representa el identificador externo del empleado (ej: "1025").
        /// </summary>
        public string IdEmpleado { get; init; } = string.Empty;
    }
}
