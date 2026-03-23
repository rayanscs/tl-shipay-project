using TL.Shipay.Project.Api.AppService.v1;
using TL.Shipay.Project.Api.AppService.v1.Interfaces;
using TL.Shipay.Project.Application.Services;
using TL.Shipay.Project.Domain.Interfaces.ApiManager;
using TL.Shipay.Project.Domain.Interfaces.Services;
using TL.Shipay.Project.Infrastructure.ExternalServices;

namespace TL.Shipay.Project.Api.Extensions
{
    public static class Injector
    {
        public static IServiceCollection AddExternalServices(this IServiceCollection services, IConfiguration config)
        {
            services.AddScoped<IBrasilApiManager, BrasilApiManager>();
            services.AddScoped<IViaCepManager, ViaCepManager>();

            return services;
        }

        public static IServiceCollection AddAppServices(this IServiceCollection services)
        {
            services.AddScoped<IClienteAppService, ClienteAppService>();
            return services;
        }

        public static IServiceCollection AddServices(this IServiceCollection services)
        {
            services.AddScoped<IDadosEmpresaProvider, DadosEmpresaProvider>();
            return services;
        }

        public static IServiceCollection AddMapperInjector(this IServiceCollection services)
        {
            services.AddScoped<IDadosEmpresaProvider, DadosEmpresaProvider>();
            return services;
        }
    }
}
