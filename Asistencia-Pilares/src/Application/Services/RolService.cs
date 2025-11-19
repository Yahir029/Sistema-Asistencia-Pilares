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
    public class RolService : IRolService
    {
        private readonly IApplicationDbContext _db;

        public RolService(IApplicationDbContext db)
        {
            _db = db;
        }

        public async Task<Guid> CrearRolAsync(CreateRolDto dto)
        {
            var rol = new Rol { Id = Guid.NewGuid(), Nombre = dto.Nombre };
            _db.Roles.Add(rol);
            await _db.SaveChangesAsync();
            return rol.Id;
        }

        public async Task EliminarRolAsync(Guid id)
        {
            var rol = await _db.Roles.FindAsync(id);
            if (rol == null) throw new InvalidOperationException("Rol no encontrado");
            _db.Roles.Remove(rol);
            await _db.SaveChangesAsync();
        }

        public async Task<List<RolDto>> ObtenerTodosAsync()
        {
            var list = await _db.Roles.ToListAsync();
            return list.Select(r => new RolDto(r.Id, r.Nombre)).ToList();
        }

        public async Task<RolDto?> ObtenerPorIdAsync(Guid id)
        {
            var r = await _db.Roles.FindAsync(id);
            if (r == null) return null;
            return new RolDto(r.Id, r.Nombre);
        }

        public async Task ActualizarRolAsync(Guid id, CreateRolDto dto)
        {
            var rol = await _db.Roles.FindAsync(id);
            if (rol == null) throw new InvalidOperationException("Rol no encontrado");
            rol.Nombre = dto.Nombre;
            await _db.SaveChangesAsync();
        }
    }
}
