using Microsoft.Extensions.DependencyInjection;
using AsistenciaAPI.Application.Common.Interfaces;
using AsistenciaAPI.Application.Services;

namespace AsistenciaAPI.Application
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddApplication(this IServiceCollection services)
        {
            services.AddScoped<IEmpleadoService, EmpleadoService>();
            services.AddScoped<IAreaService, AreaService>();
            services.AddScoped<IRolService, RolService>();
            return services;
        }
    }
}
