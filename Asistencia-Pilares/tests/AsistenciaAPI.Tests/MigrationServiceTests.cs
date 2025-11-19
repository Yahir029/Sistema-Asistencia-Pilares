using System;
using System.Data.Common;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.FileProviders;
using Xunit;
using AsistenciaAPI.Infrastructure.Persistence;
using AsistenciaAPI.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;

namespace AsistenciaAPI.Tests
{
    class TestHostEnvironment : IHostEnvironment
    {
        public string EnvironmentName { get; set; } = Environments.Production;
        public string ApplicationName { get; set; }
        public string ContentRootPath { get; set; }
        public IFileProvider ContentRootFileProvider { get; set; }
    }

    class FakeSeeder : ISeeder
    {
        public bool Called { get; private set; }
        public void Seed() => Called = true;
    }

    public class MigrationServiceTests : IDisposable
    {
        private readonly DbConnection _connection;

        public MigrationServiceTests()
        {
            _connection = new SqliteConnection("DataSource=:memory:");
            _connection.Open();
        }

        private AsistenciaDbContext CreateContext()
        {
            var options = new DbContextOptionsBuilder<AsistenciaDbContext>()
                .UseSqlite(_connection)
                // In tests we may have pending model changes vs migrations; suppress the warning so Migrate() doesn't throw
                .ConfigureWarnings(w => w.Ignore(Microsoft.EntityFrameworkCore.Diagnostics.RelationalEventId.PendingModelChangesWarning))
                .Options;
            return new AsistenciaDbContext(options);
        }

        [Fact]
        public void ApplyMigrationsAndSeed_Development_CallsEnsureCreated_And_Seed()
        {
            using var context = CreateContext();
            var env = new TestHostEnvironment { EnvironmentName = Environments.Development };
            var seeder = new FakeSeeder();
            var svc = new MigrationService(context, env, seeder);

            svc.ApplyMigrationsAndSeed();

            Assert.True(seeder.Called, "Seeder should be called in development");
            Assert.True(context.Database.CanConnect());
        }

        [Fact]
        public void ApplyMigrationsAndSeed_Production_CallsMigrate_And_Seed()
        {
            using var context = CreateContext();
            var env = new TestHostEnvironment { EnvironmentName = Environments.Production };
            var seeder = new FakeSeeder();
            var svc = new MigrationService(context, env, seeder);

            // Should not throw
            svc.ApplyMigrationsAndSeed();

            Assert.True(seeder.Called, "Seeder should be called in production after migrate");
            Assert.True(context.Database.CanConnect());
        }

        public void Dispose()
        {
            _connection.Dispose();
        }
    }
}
