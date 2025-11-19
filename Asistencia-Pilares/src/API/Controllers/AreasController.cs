using System;
using System.Threading.Tasks;
using AsistenciaAPI.Application.Common.Interfaces;
using AsistenciaAPI.Application.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AsistenciaAPI.API.Controllers
{
    [ApiController]
    [Route("areas")]
    [Authorize(Policy = "AdminOnly")]
    public class AreasController : ControllerBase
    {
        private readonly IAreaService _svc;

        public AreasController(IAreaService svc)
        {
            _svc = svc;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll() => Ok(await _svc.ObtenerTodosAsync());

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var a = await _svc.ObtenerPorIdAsync(id);
            return a is null ? NotFound() : Ok(a);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateAreaDto dto)
        {
            var id = await _svc.CrearAreaAsync(dto);
            return Created($"/areas/{id}", new { id });
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] CreateAreaDto dto)
        {
            await _svc.ActualizarAreaAsync(id, dto);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            await _svc.EliminarAreaAsync(id);
            return NoContent();
        }
    }
}
