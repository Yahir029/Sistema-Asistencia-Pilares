using System;

namespace AsistenciaAPI.Infrastructure.Services
{
    public interface IMigrationService
    {
        /// <summary>
        /// Applies migrations or ensures DB creation and runs seed logic depending on environment.
        /// Implementations should be idempotent and safe to call at startup.
        /// </summary>
        void ApplyMigrationsAndSeed();
    }
}
