using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TL.Shipay.Project.Application.Interfaces;
using TL.Shipay.Project.Application.Services;
using TL.Shipay.Project.Domain.Interfaces;
using TL.Shipay.Project.Domain.Models.Http;
using TL.Shipay.Project.Infrastructure.ExternalServices;

namespace TL.Shipay.Project.IoC.DependencyInjection
{
    public static class Injector
    {
        public static IServiceCollection RegisterExternalServices(this IServiceCollection services, IConfiguration config)
        {
            services.AddScoped<IBrasilApiManager,BrasilApiManager>();
            services.AddScoped<IViaCepManager, ViaCepManager>();

            return services;
        }

        public static IServiceCollection AddServices(this IServiceCollection services)
        {
            services.AddScoped<IBrasilApiService, BrasilApiService>();
            services.AddScoped<IViaCepService, ViaCepService>();

            return services;
        }

       
    }
}
