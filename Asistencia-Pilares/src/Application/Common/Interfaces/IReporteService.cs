using System.Collections.Generic;
using System.Threading.Tasks;
using AsistenciaAPI.Application.DTOs;

namespace AsistenciaAPI.Application.Common.Interfaces
{
    public interface IReporteService
    {
        Task<List<ReporteFilaDto>> ObtenerReporteAsync(ReporteRequestDto request);

        Task<byte[]> ExportarReporteExcelAsync(ReporteRequestDto request);
    }
}
