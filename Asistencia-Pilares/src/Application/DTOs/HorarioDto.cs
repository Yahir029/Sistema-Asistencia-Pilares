using System;

namespace AsistenciaAPI.Application.DTOs
{
    public record HorarioDto(System.DayOfWeek Dia, TimeSpan? HoraInicio, TimeSpan? HoraFin);
}
