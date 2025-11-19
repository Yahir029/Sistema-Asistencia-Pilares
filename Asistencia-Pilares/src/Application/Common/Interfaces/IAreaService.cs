using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AsistenciaAPI.Application.DTOs;

namespace AsistenciaAPI.Application.Common.Interfaces
{
    public interface IAreaService
    {
        Task<List<AreaDto>> ObtenerTodosAsync();
        Task<AreaDto?> ObtenerPorIdAsync(Guid id);
        Task<Guid> CrearAreaAsync(CreateAreaDto dto);
        Task ActualizarAreaAsync(Guid id, CreateAreaDto dto);
        Task EliminarAreaAsync(Guid id);
    }
}
