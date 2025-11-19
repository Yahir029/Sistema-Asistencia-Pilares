namespace AsistenciaAPI.Infrastructure.Services
{
    public interface ISeeder
    {
        /// <summary>
        /// Execute seeding logic. Implementations should be idempotent.
        /// </summary>
        void Seed();
    }
}
