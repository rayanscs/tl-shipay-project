using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
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
            services.AddScoped<IAuthAppService, AuthAppService>();
            services.AddScoped<IClienteAppService, ClienteAppService>();
            return services;
        }

        public static IServiceCollection AddServices(this IServiceCollection services)
        {
            services.AddScoped<IAuthService, AuthService>();
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

        public static IServiceCollection AddConfigAuthenticationApi(this IServiceCollection services, IConfiguration configuration)
        {
            var jwtSettings = configuration.GetSection("Jwt");
            var key = Encoding.ASCII.GetBytes(jwtSettings["Key"]!);

            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)

            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidIssuer = jwtSettings["Issuer"],

                    ValidateAudience = true,
                    ValidAudience = jwtSettings["Audience"],

                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.Zero, // Sem margem de erro

                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key)
                };

                // Tratamento de eventos de autenticação (opcional)
                options.Events = new JwtBearerEvents
                {
                    OnAuthenticationFailed = context =>
                    {
                        Console.WriteLine($"Falha na autenticação: {context.Exception.Message}");
                        return Task.CompletedTask;
                    },
                    OnTokenValidated = context =>
                    {
                        Console.WriteLine("Token validado com sucesso");
                        return Task.CompletedTask;
                    }
                };
            });

            services.AddAuthorization();
            return services;
        }
    }
}
