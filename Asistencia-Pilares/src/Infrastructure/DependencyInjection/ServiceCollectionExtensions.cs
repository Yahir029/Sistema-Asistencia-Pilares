using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using AsistenciaAPI.Infrastructure.Persistence;

namespace AsistenciaAPI.Infrastructure
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            var connectionString = configuration.GetConnectionString("DefaultConnection");

            // Si la cadena de conexión contiene DataSource=... asumimos SQLite para desarrollo.
            // En caso contrario, usamos SQL Server (producción).
            if (!string.IsNullOrEmpty(connectionString) && connectionString.Contains("DataSource=", StringComparison.OrdinalIgnoreCase))
            {
                services.AddDbContext<AsistenciaDbContext>(options =>
                    options.UseSqlite(connectionString));
            }
            else
            {
                services.AddDbContext<AsistenciaDbContext>(options =>
                    options.UseSqlServer(connectionString));
            }

            // Registrar servicio que aplica migraciones y realiza seed; es testeable e inyectable.
            services.AddScoped<Services.IMigrationService, Services.MigrationService>();

            // Registrar ISeeder (implementación DbSeeder) para permitir mocking en tests
            services.AddScoped<Services.ISeeder, Persistence.DbSeeder>();

            // Registrar la abstracción IApplicationDbContext para que la capa Application dependa de la interfaz
            services.AddScoped<Application.Common.Interfaces.IApplicationDbContext>(provider => provider.GetRequiredService<AsistenciaDbContext>());

            // Registrar el servicio de asistencias (Application layer)
            services.AddScoped<Application.Common.Interfaces.IAsistenciaService, AsistenciaAPI.Application.Services.AsistenciaService>();

            // Registrar el servicio de reportes (implementación en Infrastructure que usa ClosedXML)
            services.AddScoped<Application.Common.Interfaces.IReporteService, Services.ReporteService>();

            // Registrar password hasher (implementación en Infrastructure)
            services.AddSingleton<Application.Common.Interfaces.IPasswordHasher, Services.PasswordHasher>();

            return services;
        }
    }
}
