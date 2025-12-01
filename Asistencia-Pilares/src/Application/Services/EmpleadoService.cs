using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AsistenciaAPI.Application.Common.Interfaces;
using AsistenciaAPI.Application.DTOs;
using Core.Entities;
using Microsoft.EntityFrameworkCore;

namespace AsistenciaAPI.Application.Services
{
    

    public class EmpleadoService : IEmpleadoService
    {
        private readonly IApplicationDbContext _context;
        private readonly IPasswordHasher _hasher;

        public EmpleadoService(IApplicationDbContext context, IPasswordHasher hasher)
        {
            _context = context;
            _hasher = hasher;
        }
        public async Task<Guid> CrearEmpleadoAsync(CrearEmpleadoDto dto)
        {
            var empleado = new Empleado
            {
                Id = Guid.NewGuid(),
                IdEmpleadoExterno = dto.IdEmpleadoExterno,
                Nombre = dto.Nombre,
                EstaActivo = true,
                Email = string.IsNullOrWhiteSpace(dto.Email) ? "N/A" : dto.Email,
                Telefono = string.IsNullOrWhiteSpace(dto.Telefono) ? "N/A" : dto.Telefono
            };

            // admin flag and password handling
            empleado.EsAdmin = dto.EsAdmin;
            if (dto.EsAdmin)
            {
                if (string.IsNullOrWhiteSpace(dto.Password))
                    throw new InvalidOperationException("Los usuarios administradores requieren una contraseña.");

                empleado.PasswordHash = _hasher.Hash(dto.Password);
            }

            // Resolver o crear Area por nombre (si se proporciona)
            if (!string.IsNullOrWhiteSpace(dto.NombreArea))
            {
                var nombreArea = dto.NombreArea.Trim();
                var area = await _context.Areas
                    .FirstOrDefaultAsync(a => a.Nombre.ToLower() == nombreArea.ToLower());

                if (area == null)
                {
                    area = new Area { Id = Guid.NewGuid(), Nombre = nombreArea };
                    _context.Areas.Add(area);
                }

                empleado.AreaId = area.Id;
            }
            else
            {
                // Crear área "N/A" si no existe y asignarla
                var areaNa = await _context.Areas
                    .FirstOrDefaultAsync(a => a.Nombre == "N/A");
                
                if (areaNa == null)
                {
                    areaNa = new Area { Id = Guid.NewGuid(), Nombre = "N/A" };
                    _context.Areas.Add(areaNa);
                }
                
                empleado.AreaId = areaNa.Id;
            }

            // Resolver o crear Rol por nombre (si se proporciona)
            if (!string.IsNullOrWhiteSpace(dto.NombreRol))
            {
                var nombreRol = dto.NombreRol.Trim();
                var rol = await _context.Roles
                    .FirstOrDefaultAsync(r => r.Nombre.ToLower() == nombreRol.ToLower());

                if (rol == null)
                {
                    rol = new Rol { Id = Guid.NewGuid(), Nombre = nombreRol };
                    _context.Roles.Add(rol);
                }

                empleado.RolId = rol.Id;
            }
            else
            {
                // Crear rol "N/A" si no existe y asignarlo
                var rolNa = await _context.Roles
                    .FirstOrDefaultAsync(r => r.Nombre == "N/A");
                
                if (rolNa == null)
                {
                    rolNa = new Rol { Id = Guid.NewGuid(), Nombre = "N/A" };
                    _context.Roles.Add(rolNa);
                }
                
                empleado.RolId = rolNa.Id;
            }

            var horarios = dto.Horarios?.Select(h => new HorarioLaboral
            {
                Id = Guid.NewGuid(),
                Dia = h.Dia,
                HoraInicio = h.HoraInicio,
                HoraFin = h.HoraFin,
                EmpleadoId = empleado.Id
            }).ToList();

            if (horarios != null && horarios.Count > 0)
                empleado.Horarios = horarios;

            _context.Empleados.Add(empleado);
            await _context.SaveChangesAsync();

            return empleado.Id;
        }

        public async Task<EmpleadoDto?> ObtenerPorIdAsync(Guid id)
        {
            var e = await _context.Empleados
                .Include(x => x.Area)
                .Include(x => x.Rol)
                .Include(x => x.Horarios)
                .FirstOrDefaultAsync(x => x.Id == id);

            if (e == null) return null;

            var horarios = e.Horarios.Select(h => new HorarioDto(h.Dia, h.HoraInicio, h.HoraFin)).ToList();

            return new EmpleadoDto(
                e.Id, 
                e.IdEmpleadoExterno, 
                e.Nombre, 
                e.Area?.Nombre ?? "N/A", 
                e.Rol?.Nombre ?? "N/A", 
                e.EstaActivo, 
                horarios,
                e.Email ?? "N/A",
                e.Telefono ?? "N/A"
            );
        }

        public async Task<List<EmpleadoDto>> ObtenerTodosAsync()
        {
            var list = await _context.Empleados
                .Include(x => x.Area)
                .Include(x => x.Rol)
                .Include(x => x.Horarios)
                .ToListAsync();

            return list.Select(e => new EmpleadoDto(
                e.Id,
                e.IdEmpleadoExterno,
                e.Nombre,
                e.Area?.Nombre ?? "N/A",
                e.Rol?.Nombre ?? "N/A",
                e.EstaActivo,
                e.Horarios.Select(h => new HorarioDto(h.Dia, h.HoraInicio, h.HoraFin)).ToList(),
                e.Email ?? "N/A",
                e.Telefono ?? "N/A"
            )).ToList();
        }

        public async Task ActualizarEmpleadoAsync(Guid id, CrearEmpleadoDto dto)
        {
            var empleado = await _context.Empleados
                .Include(x => x.Horarios)
                .FirstOrDefaultAsync(x => x.Id == id);

            if (empleado == null) throw new InvalidOperationException("Empleado no encontrado");

            empleado.Nombre = dto.Nombre;
            empleado.IdEmpleadoExterno = dto.IdEmpleadoExterno;
            empleado.Email = string.IsNullOrWhiteSpace(dto.Email) ? "N/A" : dto.Email;
            empleado.Telefono = string.IsNullOrWhiteSpace(dto.Telefono) ? "N/A" : dto.Telefono;

            // Resolver o crear Area por nombre y asignar AreaId
            if (!string.IsNullOrWhiteSpace(dto.NombreArea))
            {
                var nombreArea = dto.NombreArea.Trim();
                var area = await _context.Areas
                    .FirstOrDefaultAsync(a => a.Nombre.ToLower() == nombreArea.ToLower());

                if (area == null)
                {
                    area = new Area { Id = Guid.NewGuid(), Nombre = nombreArea };
                    _context.Areas.Add(area);
                }

                empleado.AreaId = area.Id;
            }
            else
            {
                // Asignar área "N/A"
                var areaNa = await _context.Areas
                    .FirstOrDefaultAsync(a => a.Nombre == "N/A");
                
                if (areaNa == null)
                {
                    areaNa = new Area { Id = Guid.NewGuid(), Nombre = "N/A" };
                    _context.Areas.Add(areaNa);
                }
                
                empleado.AreaId = areaNa.Id;
            }

            // Resolver o crear Rol por nombre y asignar RolId
            if (!string.IsNullOrWhiteSpace(dto.NombreRol))
            {
                var nombreRol = dto.NombreRol.Trim();
                var rol = await _context.Roles
                    .FirstOrDefaultAsync(r => r.Nombre.ToLower() == nombreRol.ToLower());

                if (rol == null)
                {
                    rol = new Rol { Id = Guid.NewGuid(), Nombre = nombreRol };
                    _context.Roles.Add(rol);
                }

                empleado.RolId = rol.Id;
            }
            else
            {
                // Asignar rol "N/A"
                var rolNa = await _context.Roles
                    .FirstOrDefaultAsync(r => r.Nombre == "N/A");
                
                if (rolNa == null)
                {
                    rolNa = new Rol { Id = Guid.NewGuid(), Nombre = "N/A" };
                    _context.Roles.Add(rolNa);
                }
                
                empleado.RolId = rolNa.Id;
            }

            // Reemplazar horarios: eliminar existentes y añadir nuevos
            if (empleado.Horarios != null && empleado.Horarios.Any())
            {
                _context.HorariosLaborales.RemoveRange(empleado.Horarios);
            }

            var nuevos = dto.Horarios?.Select(h => new HorarioLaboral
            {
                Id = Guid.NewGuid(),
                Dia = h.Dia,
                HoraInicio = h.HoraInicio,
                HoraFin = h.HoraFin,
                EmpleadoId = empleado.Id
            }).ToList();

            if (nuevos != null && nuevos.Any())
            {
                empleado.Horarios = nuevos;
                _context.HorariosLaborales.AddRange(nuevos);
            }

            // actualizar EsAdmin / Password en edición
            empleado.EsAdmin = dto.EsAdmin;
            if (dto.EsAdmin)
            {
                if (!string.IsNullOrWhiteSpace(dto.Password))
                {
                    empleado.PasswordHash = _hasher.Hash(dto.Password);
                }
                else if (string.IsNullOrWhiteSpace(empleado.PasswordHash))
                {
                    // If now admin but had no password and no new password provided -> invalid
                    throw new InvalidOperationException("Los usuarios administradores requieren una contraseña.");
                }
            }
            else
            {
                // Not admin: clear password
                empleado.PasswordHash = null;
            }

            await _context.SaveChangesAsync();
        }

        public async Task CambiarEstadoAsync(Guid id, bool activo)
        {
            var empleado = await _context.Empleados.FindAsync(id);
            if (empleado == null) throw new InvalidOperationException("Empleado no encontrado");
            empleado.EstaActivo = activo;
            await _context.SaveChangesAsync();
        }
    }
}
