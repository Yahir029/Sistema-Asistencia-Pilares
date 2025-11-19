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
    public class AreaService : IAreaService
    {
        private readonly IApplicationDbContext _db;

        public AreaService(IApplicationDbContext db)
        {
            _db = db;
        }

        public async Task<Guid> CrearAreaAsync(CreateAreaDto dto)
        {
            var area = new Area { Id = Guid.NewGuid(), Nombre = dto.Nombre };
            _db.Areas.Add(area);
            await _db.SaveChangesAsync();
            return area.Id;
        }

        public async Task EliminarAreaAsync(Guid id)
        {
            var area = await _db.Areas.FindAsync(id);
            if (area == null) throw new InvalidOperationException("Area no encontrada");
            _db.Areas.Remove(area);
            await _db.SaveChangesAsync();
        }

        public async Task<List<AreaDto>> ObtenerTodosAsync()
        {
            var list = await _db.Areas.ToListAsync();
            return list.Select(a => new AreaDto(a.Id, a.Nombre)).ToList();
        }

        public async Task<AreaDto?> ObtenerPorIdAsync(Guid id)
        {
            var a = await _db.Areas.FindAsync(id);
            if (a == null) return null;
            return new AreaDto(a.Id, a.Nombre);
        }

        public async Task ActualizarAreaAsync(Guid id, CreateAreaDto dto)
        {
            var area = await _db.Areas.FindAsync(id);
            if (area == null) throw new InvalidOperationException("Area no encontrada");
            area.Nombre = dto.Nombre;
            await _db.SaveChangesAsync();
        }
    }
}
