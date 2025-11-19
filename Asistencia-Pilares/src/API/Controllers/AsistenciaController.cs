using System;
using System.Threading.Tasks;
using AsistenciaAPI.Application.Common.Interfaces;
using AsistenciaAPI.Application.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace AsistenciaAPI.API.Controllers
{
    [ApiController]
    [Route("asistencia")]
    public class AsistenciaController : ControllerBase
    {
        private readonly IAsistenciaService _svc;

        public AsistenciaController(IAsistenciaService svc)
        {
            _svc = svc;
        }

        [HttpPost("entrada")]
        public async Task<IActionResult> RegistrarEntrada([FromBody] RegistroAsistenciaDto dto)
        {
            var resultado = await _svc.RegistrarEntradaAsync(dto.IdEmpleado);
            if (!resultado.Exito)
            {
                if (resultado.Mensaje?.Contains("no encontrado", StringComparison.OrdinalIgnoreCase) == true)
                    return NotFound(resultado);
                return BadRequest(resultado);
            }

            return Created(string.Empty, resultado);
        }

        [HttpPost("salida")]
        public async Task<IActionResult> RegistrarSalida([FromBody] RegistroAsistenciaDto dto)
        {
            var resultado = await _svc.RegistrarSalidaAsync(dto.IdEmpleado);
            if (!resultado.Exito)
            {
                if (resultado.Mensaje?.Contains("no encontrado", StringComparison.OrdinalIgnoreCase) == true)
                    return NotFound(resultado);
                return BadRequest(resultado);
            }

            return Ok(resultado);
        }

        [HttpPost("marcar")]
        public async Task<IActionResult> RegistrarMarcacion([FromBody] RegistroAsistenciaDto dto)
        {
            var resultado = await _svc.RegistrarMarcacionAsync(dto.IdEmpleado);
            if (!resultado.Exito)
            {
                if (resultado.Mensaje?.Contains("no encontrado", StringComparison.OrdinalIgnoreCase) == true)
                    return NotFound(resultado);
                return BadRequest(resultado);
            }

            if (string.Equals(resultado.Tipo, "Entrada", StringComparison.OrdinalIgnoreCase))
                return Created(string.Empty, resultado);

            return Ok(resultado);
        }
    }
}
