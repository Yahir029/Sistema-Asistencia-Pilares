using System.Threading.Tasks;
using AsistenciaAPI.Application.DTOs;

namespace AsistenciaAPI.Application.Common.Interfaces
{
    public interface IAsistenciaService
    {
        Task<ResultadoAsistenciaDto> RegistrarEntradaAsync(string idEmpleadoExterno);
        Task<ResultadoAsistenciaDto> RegistrarSalidaAsync(string idEmpleadoExterno);
        Task<ResultadoAsistenciaDto> RegistrarMarcacionAsync(string idEmpleadoExterno);
    }
}
