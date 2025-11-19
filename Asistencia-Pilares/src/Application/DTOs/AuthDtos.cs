namespace AsistenciaAPI.Application.DTOs
{
    public record LoginRequestDto(string IdEmpleadoExterno, string Password);

    public record LoginResponseDto(string Token, DateTime Expiration);
}
