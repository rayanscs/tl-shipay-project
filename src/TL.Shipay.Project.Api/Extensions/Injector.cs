using TL.Shipay.Project.Api.AppService.v1;
using TL.Shipay.Project.Api.AppService.v1.Interfaces;
using TL.Shipay.Project.Application.Mappings;
using TL.Shipay.Project.Application.Services;
using TL.Shipay.Project.Domain.Interfaces.ApiManager;
using TL.Shipay.Project.Domain.Interfaces.Services;
using TL.Shipay.Project.Infrastructure.ExternalServices;

namespace TL.Shipay.Project.Api.Extensions
{
    public static class Injector
    {
        public static IServiceCollection AddExternalServices(this IServiceCollection services)
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
            services.AddScoped<IEmpresaProviderService, EmpresaProviderService>();
            return services;
        }

        public static IServiceCollection AddMapperProfiles(this IServiceCollection services)
        {
            services.AddAutoMapper(mapperConfigurationExpression =>
            {
                mapperConfigurationExpression.AddProfile(typeof(DadosEmpresaMappings));
                mapperConfigurationExpression.AddProfile(typeof(EnderecoBrasilApiMappings));
                mapperConfigurationExpression.AddProfile(typeof(EnderecoViaCepMappings));
            });
            return services;
        }
    }
}
