using Asp.Versioning.ApiExplorer;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace TL.Shipay.Project.Api.Utils
{
    public sealed class ConfigureSwaggerOptions(IApiVersionDescriptionProvider provider) : IConfigureOptions<SwaggerGenOptions>
    {
        public void Configure(SwaggerGenOptions options)
            => provider.ApiVersionDescriptions.ToList()
                .ForEach(description => options.SwaggerDoc(
                    name: description.GroupName,
                    info: new OpenApiInfo
                    {
                        Title = $"{description.ApiVersion}",
                        Version = description.ApiVersion.ToString(),
                    }
                )
            );
    }
}
