using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AsistenciaAPI.Application.DTOs;

namespace AsistenciaAPI.Application.Common.Interfaces
{
    public interface IEmpleadoService
    {
        Task<Guid> CrearEmpleadoAsync(CrearEmpleadoDto dto);
        Task<EmpleadoDto?> ObtenerPorIdAsync(Guid id);
        Task<List<EmpleadoDto>> ObtenerTodosAsync();
        Task ActualizarEmpleadoAsync(Guid id, CrearEmpleadoDto dto);
        Task CambiarEstadoAsync(Guid id, bool activo);
    }
}
