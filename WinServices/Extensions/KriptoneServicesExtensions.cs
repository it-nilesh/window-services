using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace WinServices
{
    public static class WinServicesExtensions
    {
        public static IServiceCollection RegisterWinServices<T>(this IServiceCollection services) where T : WinHosServicetLietime
        {
            services.AddScoped<IHostLifetime>(x => {
                var serviceBaseLifetime = ActivatorUtilities.GetServiceOrCreateInstance<T>(x);
                var hostApplicationLifetime = x.GetRequiredService<IHostApplicationLifetime>();
                serviceBaseLifetime.Inject(hostApplicationLifetime);
                return serviceBaseLifetime;
            });

            return services;
        }
    }
}
