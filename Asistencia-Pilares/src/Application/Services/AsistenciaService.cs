using System;
using System.Linq;
using System.Threading.Tasks;
using AsistenciaAPI.Application.Common.Interfaces;
using AsistenciaAPI.Application.DTOs;
using Core.Entities;
using Microsoft.EntityFrameworkCore;

namespace AsistenciaAPI.Application.Services
{
    public class AsistenciaService : IAsistenciaService
    {
        private readonly IApplicationDbContext _context;

        public AsistenciaService(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<ResultadoAsistenciaDto> RegistrarEntradaAsync(string idEmpleadoExterno)
        {
            var ahora = DateTime.Now;

            var empleado = await _context.Empleados
                .Include(e => e.Asistencias)
                .FirstOrDefaultAsync(e => e.IdEmpleadoExterno == idEmpleadoExterno);

            if (empleado == null || !empleado.EstaActivo)
            {
                return new ResultadoAsistenciaDto
                {
                    Exito = false,
                    Mensaje = "Empleado no encontrado o inactivo",
                    FechaHora = ahora,
                    NombreEmpleado = string.Empty
                };
            }

            // Verificar si existe una sesión abierta (HoraSalida == null)
            var tieneSesionAbierta = empleado.Asistencias != null && empleado.Asistencias.Any(a => a.HoraSalida == null);
            if (tieneSesionAbierta)
            {
                return new ResultadoAsistenciaDto
                {
                    Exito = false,
                    Mensaje = "Ya tienes una entrada registrada. Debes registrar salida primero.",
                    FechaHora = ahora,
                    NombreEmpleado = empleado.Nombre
                };
            }

            var registro = new RegistroAsistencia
            {
                Fecha = ahora.Date,
                HoraEntrada = ahora.TimeOfDay,
                HoraSalida = null,
                EmpleadoId = empleado.Id
            };

            _context.RegistrosAsistencia.Add(registro);
            await _context.SaveChangesAsync();

            return new ResultadoAsistenciaDto
            {
                Exito = true,
                Mensaje = "Entrada registrada",
                Tipo = "Entrada",
                FechaHora = ahora,
                NombreEmpleado = empleado.Nombre
            };
        }

        public async Task<ResultadoAsistenciaDto> RegistrarSalidaAsync(string idEmpleadoExterno)
        {
            var ahora = DateTime.Now;

            var empleado = await _context.Empleados
                .FirstOrDefaultAsync(e => e.IdEmpleadoExterno == idEmpleadoExterno);

            if (empleado == null)
            {
                return new ResultadoAsistenciaDto
                {
                    Exito = false,
                    Mensaje = "Empleado no encontrado",
                    FechaHora = ahora,
                    NombreEmpleado = string.Empty
                };
            }

            // Buscar la última sesión abierta para este empleado
            // Nota: SQLite no soporta ordenar por TimeSpan en SQL, por lo que traemos los candidatos
            // y ordenamos en memoria (LINQ to Objects) para evitar NotSupportedException.
            var candidatos = await _context.RegistrosAsistencia
                .Where(r => r.EmpleadoId == empleado.Id && r.HoraSalida == null)
                .ToListAsync();

            var registro = candidatos
                .OrderByDescending(r => r.Fecha)
                .ThenByDescending(r => r.HoraEntrada)
                .FirstOrDefault();

            if (registro == null)
            {
                return new ResultadoAsistenciaDto
                {
                    Exito = false,
                    Mensaje = "No tienes una entrada registrada. Registra tu entrada primero.",
                    FechaHora = ahora,
                    NombreEmpleado = empleado.Nombre
                };
            }

            registro.HoraSalida = ahora.TimeOfDay;
            await _context.SaveChangesAsync();

            return new ResultadoAsistenciaDto
            {
                Exito = true,
                Mensaje = "Salida registrada",
                Tipo = "Salida",
                FechaHora = ahora,
                NombreEmpleado = empleado.Nombre
            };
        }

        public async Task<ResultadoAsistenciaDto> RegistrarMarcacionAsync(string idEmpleadoExterno)
        {
            // Intent: si no hay sesión abierta -> registrar entrada; si hay sesión abierta -> registrar salida
            var ahora = DateTime.Now;

            var empleado = await _context.Empleados
                .Include(e => e.Asistencias)
                .FirstOrDefaultAsync(e => e.IdEmpleadoExterno == idEmpleadoExterno);

            if (empleado == null || !empleado.EstaActivo)
            {
                return new ResultadoAsistenciaDto
                {
                    Exito = false,
                    Mensaje = "Empleado no encontrado o inactivo",
                    FechaHora = ahora,
                    NombreEmpleado = string.Empty
                };
            }

            // ¿Tiene sesión abierta?
            var tieneSesionAbierta = empleado.Asistencias != null && empleado.Asistencias.Any(a => a.HoraSalida == null);

            if (!tieneSesionAbierta)
            {
                // Registrar entrada
                var registro = new RegistroAsistencia
                {
                    Fecha = ahora.Date,
                    HoraEntrada = ahora.TimeOfDay,
                    HoraSalida = null,
                    EmpleadoId = empleado.Id
                };

                _context.RegistrosAsistencia.Add(registro);
                await _context.SaveChangesAsync();

                return new ResultadoAsistenciaDto
                {
                    Exito = true,
                    Mensaje = "Entrada registrada",
                    Tipo = "Entrada",
                    FechaHora = ahora,
                    NombreEmpleado = empleado.Nombre
                };
            }

            // Si tiene sesión abierta, registrar salida sobre la última sesión abierta
            var candidatos = await _context.RegistrosAsistencia
                .Where(r => r.EmpleadoId == empleado.Id && r.HoraSalida == null)
                .ToListAsync();

            var registroSalida = candidatos
                .OrderByDescending(r => r.Fecha)
                .ThenByDescending(r => r.HoraEntrada)
                .FirstOrDefault();

            if (registroSalida == null)
            {
                // Este caso es improbable porque ya detectamos una sesión abierta, pero lo manejamos defensivamente
                return new ResultadoAsistenciaDto
                {
                    Exito = false,
                    Mensaje = "No se pudo encontrar la entrada abierta para cerrar.",
                    FechaHora = ahora,
                    NombreEmpleado = empleado.Nombre
                };
            }

            registroSalida.HoraSalida = ahora.TimeOfDay;
            await _context.SaveChangesAsync();

            return new ResultadoAsistenciaDto
            {
                Exito = true,
                Mensaje = "Salida registrada",
                Tipo = "Salida",
                FechaHora = ahora,
                NombreEmpleado = empleado.Nombre
            };
        }
    }
}
