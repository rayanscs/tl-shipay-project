using Asp.Versioning.ApiExplorer;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace TL.Shipay.Project.Api.Extensions
{
    public sealed class ConfigureSwaggerOptions(IApiVersionDescriptionProvider provider) : IConfigureOptions<SwaggerGenOptions>
    {
        public void Configure(SwaggerGenOptions options)
            => provider.ApiVersionDescriptions.ToList()
                .ForEach(description => options.SwaggerDoc(
                    name: description.GroupName,
                    info: new OpenApiInfo
                    {
                        Title = $"Shipay Project API Test",
                        Version = description.ApiVersion.ToString(),
                    }
                )
            );
    }
}
