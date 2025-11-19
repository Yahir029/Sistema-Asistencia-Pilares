using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AsistenciaAPI.Application.DTOs;

namespace AsistenciaAPI.Application.Common.Interfaces
{
    public interface IRolService
    {
        Task<List<RolDto>> ObtenerTodosAsync();
        Task<RolDto?> ObtenerPorIdAsync(Guid id);
        Task<Guid> CrearRolAsync(CreateRolDto dto);
        Task ActualizarRolAsync(Guid id, CreateRolDto dto);
        Task EliminarRolAsync(Guid id);
    }
}
