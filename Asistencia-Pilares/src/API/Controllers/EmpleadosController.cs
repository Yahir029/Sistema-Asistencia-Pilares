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
            try
            {
                var id = await _svc.CrearEmpleadoAsync(dto);
                return Created($"/empleados/{id}", new { id });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { mensaje = ex.Message });
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] CrearEmpleadoDto dto)
        {
            try
            {
                await _svc.ActualizarEmpleadoAsync(id, dto);
                return NoContent();
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { mensaje = ex.Message });
            }
        }

        [HttpPatch("{id}/estado")]
        public async Task<IActionResult> ChangeState(Guid id, [FromQuery] bool activo)
        {
            try
            {
                await _svc.CambiarEstadoAsync(id, activo);
                return NoContent();
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { mensaje = ex.Message });
            }
        }
    }
}
