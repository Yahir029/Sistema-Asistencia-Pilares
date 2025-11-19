using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using AsistenciaAPI.Infrastructure.Services;

namespace AsistenciaAPI.API
{
    public static class HostingExtensions
    {
        /// <summary>
        /// Extension to apply migrations/EnsureCreated and run seed in a testable service.
        /// </summary>
        public static WebApplication ApplyMigrationsAndSeed(this WebApplication app)
        {
            using var scope = app.Services.CreateScope();
            var svc = scope.ServiceProvider.GetRequiredService<IMigrationService>();
            svc.ApplyMigrationsAndSeed();
            return app;
        }
    }
}
