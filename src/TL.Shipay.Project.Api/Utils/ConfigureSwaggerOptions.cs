using Asp.Versioning.ApiExplorer;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Diagnostics.CodeAnalysis;

namespace TL.Shipay.Project.Api.Utils
{
    [ExcludeFromCodeCoverage]
    public class ConfigureSwaggerOptions : IConfigureOptions<SwaggerGenOptions>
    {
        private readonly IApiVersionDescriptionProvider _provider;

        // Injeta o provedor que sabe quais versões da API existem
        public ConfigureSwaggerOptions(IApiVersionDescriptionProvider provider)
        {
            _provider = provider;
        }

        public void Configure(SwaggerGenOptions options)
        {
            // Cria um documento do Swagger para cada versão descoberta
            foreach (var description in _provider.ApiVersionDescriptions)
            {
                options.SwaggerDoc(description.GroupName, CreateInfoForApiVersion(description));
            }
        }

        private static OpenApiInfo CreateInfoForApiVersion(ApiVersionDescription description)
        {
            var info = new OpenApiInfo
            {
                Title = "Shipay - TechLeader Test API",
                Version = description.ApiVersion.ToString(),
                Description = "Documentação interativa da API."
            };

            // Adiciona um aviso visual se a versão estiver marcada como obsoleta
            if (description.IsDeprecated)
            {
                info.Description += " **Atenção: Esta versão da API está obsoleta.**";
            }

            return info;
        }
    }
}
