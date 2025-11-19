using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using AsistenciaAPI.Application.Common.Interfaces;
using AsistenciaAPI.Application.DTOs;
using ClosedXML.Excel;
using Microsoft.EntityFrameworkCore;

namespace AsistenciaAPI.Infrastructure.Services
{
    public class ReporteService : IReporteService
    {
        private readonly IApplicationDbContext _db;

        public ReporteService(IApplicationDbContext db)
        {
            _db = db ?? throw new ArgumentNullException(nameof(db));
        }

        public async Task<List<ReporteFilaDto>> ObtenerReporteAsync(ReporteRequestDto request)
        {
            if (request is null) throw new ArgumentNullException(nameof(request));

            // Filtrar en base de datos por rango de fechas y opcionalmente por empleado.
            var query = _db.RegistrosAsistencia
                .AsNoTracking()
                .Include(r => r.Empleado)
                .Where(r => r.Fecha >= request.FechaInicio.Date && r.Fecha <= request.FechaFin.Date);

            if (request.EmpleadoId.HasValue)
            {
                query = query.Where(r => r.EmpleadoId == request.EmpleadoId.Value);
            }

            // Traer a memoria y proyectar a DTO para simplificar manejo de TimeSpan y combinaciones fecha+hora.
            // SQLite no soporta ORDER BY sobre TimeSpan, por lo que hacemos la ordenación por HoraEntrada en memoria.
            var registrosDb = await query
                // ordenar por campos que sí soporta el proveedor en la consulta
                .OrderBy(r => r.Empleado.Nombre)
                .ThenBy(r => r.Fecha)
                .ToListAsync();

            // Ordenación final en memoria (TimeSpan comparaciones funcionan en LINQ-to-Objects)
            var registros = registrosDb
                .OrderBy(r => r.Empleado?.Nombre ?? string.Empty)
                .ThenBy(r => r.Fecha)
                .ThenBy(r => r.HoraEntrada ?? TimeSpan.Zero)
                .ToList();

            var filas = registros.Select(r => new ReporteFilaDto
            {
                NombreEmpleado = r.Empleado?.Nombre ?? string.Empty,
                Fecha = r.Fecha.Date,
                HoraEntrada = r.HoraEntrada.HasValue ? r.Fecha.Date.Add(r.HoraEntrada.Value) : r.Fecha.Date,
                HoraSalida = r.HoraSalida.HasValue ? r.Fecha.Date.Add(r.HoraSalida.Value) : (DateTime?)null
            })
            .ToList();

            return filas;
        }

        public async Task<byte[]> ExportarReporteExcelAsync(ReporteRequestDto request)
        {
            var filas = await ObtenerReporteAsync(request);

            using var workbook = new XLWorkbook();
            var ws = workbook.Worksheets.Add("Reporte");

            // Encabezados
            ws.Cell(1, 1).Value = "Empleado";
            ws.Cell(1, 2).Value = "Fecha";
            ws.Cell(1, 3).Value = "Hora Entrada";
            ws.Cell(1, 4).Value = "Hora Salida";

            // Datos
            var row = 2;
            foreach (var f in filas)
            {
                ws.Cell(row, 1).Value = f.NombreEmpleado;
                ws.Cell(row, 2).Value = f.Fecha;
                ws.Cell(row, 3).Value = f.HoraEntrada;
                ws.Cell(row, 4).Value = f.HoraSalida;
                row++;
            }

            // Formatos
            ws.Column(2).Style.DateFormat.Format = "yyyy-mm-dd";
            ws.Column(3).Style.DateFormat.Format = "HH:mm:ss";
            ws.Column(4).Style.DateFormat.Format = "HH:mm:ss";

            ws.Columns().AdjustToContents();

            using var ms = new MemoryStream();
            workbook.SaveAs(ms);
            return ms.ToArray();
        }
    }
}
