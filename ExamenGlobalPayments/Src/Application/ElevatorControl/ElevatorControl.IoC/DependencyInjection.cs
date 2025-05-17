
using ElevatorControl.Application.Interface;
using ElevatorControl.Application.Services;
using Microsoft.Extensions.DependencyInjection;

namespace ElevatorControl.IoC
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddElevatorDependencies(this IServiceCollection services)
        {
            services.AddSingleton<IElevatorService, ElevatorService>();
            return services;
        }
    }
}
