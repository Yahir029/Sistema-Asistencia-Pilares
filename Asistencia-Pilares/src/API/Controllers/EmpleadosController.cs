using System;
using System.Threading.Tasks;
using AsistenciaAPI.Application.Common.Interfaces;
using AsistenciaAPI.Application.DTOs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace AsistenciaAPI.API.Controllers
{
    [ApiController]
    [Route("empleados")]
    [Authorize(Policy = "AdminOnly")]
    public class EmpleadosController : ControllerBase
    {
        private readonly IEmpleadoService _svc;

        public EmpleadosController(IEmpleadoService svc)
        {
            _svc = svc;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
            => Ok(await _svc.ObtenerTodosAsync());

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var e = await _svc.ObtenerPorIdAsync(id);
            return e is null ? NotFound() : Ok(e);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CrearEmpleadoDto dto)
        {
            var id = await _svc.CrearEmpleadoAsync(dto);
            return Created($"/empleados/{id}", new { id });
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] CrearEmpleadoDto dto)
        {
            await _svc.ActualizarEmpleadoAsync(id, dto);
            return NoContent();
        }

        [HttpPatch("{id}/estado")]
        public async Task<IActionResult> ChangeState(Guid id, [FromQuery] bool activo)
        {
            await _svc.CambiarEstadoAsync(id, activo);
            return NoContent();
        }
    }
}
