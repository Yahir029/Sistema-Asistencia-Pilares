using Microsoft.Extensions.Hosting;
using Microsoft.EntityFrameworkCore;
using AsistenciaAPI.Infrastructure.Persistence;

namespace AsistenciaAPI.Infrastructure.Services
{
    public class MigrationService : IMigrationService
    {
    private readonly AsistenciaDbContext _db;
    private readonly IHostEnvironment _env;
    private readonly ISeeder _seeder;

        public MigrationService(AsistenciaDbContext db, IHostEnvironment env, ISeeder seeder)
        {
            _db = db;
            _env = env;
            _seeder = seeder;
        }

        public void ApplyMigrationsAndSeed()
        {
            if (_env.IsDevelopment())
            {
                // En dev usamos EnsureCreated para un flujo ligero
                _db.Database.EnsureCreated();
                _seeder.Seed();
            }
            else
            {
                // En prod/staging aplicamos migraciones
                _db.Database.Migrate();
                _seeder.Seed();
            }
        }
    }
}
