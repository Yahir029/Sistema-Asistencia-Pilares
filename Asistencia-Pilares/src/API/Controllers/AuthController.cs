using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using AsistenciaAPI.Application.Common.Interfaces;
using AsistenciaAPI.Application.DTOs;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace AsistenciaAPI.API.Controllers
{
    [ApiController]
    [Route("auth")]
    public class AuthController : ControllerBase
    {
        private readonly IApplicationDbContext _db;
        private readonly IPasswordHasher _hasher;
        private readonly IConfiguration _config;

        public AuthController(IApplicationDbContext db, IPasswordHasher hasher, IConfiguration config)
        {
            _db = db;
            _hasher = hasher;
            _config = config;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequestDto dto)
        {
            var empleado = await _db.Empleados.FirstOrDefaultAsync(e => e.IdEmpleadoExterno == dto.IdEmpleadoExterno);
            if (empleado == null) return Unauthorized();

            if (!empleado.EsAdmin)
                return Forbid(); // only admins can login

            // Verificar que el empleado tiene contraseña y que coincida
            if (string.IsNullOrWhiteSpace(empleado.PasswordHash))
                return Unauthorized();

            // Verificar la contraseña usando el hasher
            if (!_hasher.Verify(empleado.PasswordHash, dto.Password))
                return Unauthorized();

            var jwt = _config.GetSection("Jwt");
            var key = Encoding.UTF8.GetBytes(jwt["Key"] ?? string.Empty);
            var expires = DateTime.UtcNow.AddMinutes(int.Parse(jwt["ExpireMinutes"] ?? "60"));

            var claims = new[] {
                new Claim(JwtRegisteredClaimNames.Sub, empleado.Id.ToString()),
                new Claim(ClaimTypes.Name, empleado.Nombre),
                new Claim(ClaimTypes.Role, "Admin")
            };

            var creds = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256);
            var token = new JwtSecurityToken(
                issuer: jwt["Issuer"],
                audience: jwt["Audience"],
                claims: claims,
                expires: expires,
                signingCredentials: creds);

            var tokenString = new JwtSecurityTokenHandler().WriteToken(token);

            return Ok(new LoginResponseDto(tokenString, expires));
        }
    }
}
