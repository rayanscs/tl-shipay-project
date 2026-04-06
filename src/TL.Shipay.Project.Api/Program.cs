using Asp.Versioning;
using Asp.Versioning.ApiExplorer;
using FluentValidation;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Serilog;
using Swashbuckle.AspNetCore.Filters;
using System.Text;
using TL.Shipay.Project.Api.Extensions;
using TL.Shipay.Project.Application.Filters;
using TL.Shipay.Project.Application.Validators;
using TL.Shipay.Project.Infrastructure;
using TL.Shipay.Project.Infrastructure.LogConfig;

var builder = WebApplication.CreateBuilder(args);

builder.Services.Configure<ApiManagerUrlOptions>(builder.Configuration.GetSection("ApiManagerUrlOptions"));
builder.Services.Configure<InfrastructureOptions>(builder.Configuration.GetSection("InfrastructureOptions"));
builder.Services.Configure<Jwt>(builder.Configuration.GetSection("Jwt"));

builder.Services.AddAppServices();
builder.Services.AddServices();
builder.Services.AddMapperProfiles();
builder.Services.AddHttpClientFactory(builder.Configuration);
builder.Services.AddExternalServices();

builder.Services.AddValidatorsFromAssemblyContaining<ClienteValidator>();

#region Configurações de logging
// ============================================================
// Configuração do Serilog (v4.3.1)
// - Console: para debug local
// - NamedPipe: envia para o Console Application listener
// ============================================================
Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Information()
    .MinimumLevel.Override("Microsoft.AspNetCore", Serilog.Events.LogEventLevel.Warning)
    .Enrich.FromLogContext()
    .Enrich.WithProperty("Application", "SerilogPipeDemo.Api")
    .WriteTo.Console(
        outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] {Message:lj}{NewLine}{Exception}")
    .WriteTo.NamedPipe(PipeConstants.PipeName) // <-- Nosso sink customizado
    .CreateLogger();

// Substitui o logging padrão do ASP.NET Core pelo Serilog
builder.Host.UseSerilog();
#endregion

builder.Services.AddControllers(options =>
{
    options.Filters.Add<FluentValidationFilter>();
});

builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(options =>
{
    var xmlFile = $"{System.Reflection.Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    options.IncludeXmlComments(xmlPath);
    options.ExampleFilters();
});

builder.Services.AddApiVersioning(options =>
{
    options.DefaultApiVersion = new ApiVersion(1, 0);
    options.AssumeDefaultVersionWhenUnspecified = true;
    options.ReportApiVersions = true;
})
.AddApiExplorer(options =>
{
    options.GroupNameFormat = "'v'VVV";
    options.SubstituteApiVersionInUrl = true;
});

builder.Services.AddSwaggerExamplesFromAssemblyOf<Program>();
builder.Services.ConfigureOptions<ConfigureSwaggerOptions>();
builder.Services.Configure<RouteOptions>(options => options.LowercaseUrls = true);

builder.Services.AddConfigAuthenticationApi(builder.Configuration);

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    var provider = app.Services.GetRequiredService<IApiVersionDescriptionProvider>();

    app.UseSwagger();
    app.UseSwaggerUI(options => provider.ApiVersionDescriptions.ToList()
    .ForEach(
        description => options.SwaggerEndpoint(
            url: $"/swagger/{description.GroupName}/swagger.json",
            name: description.GroupName.ToUpperInvariant()))
    );

    app.UseReDoc(options =>
    {
        options.DocumentTitle = "Shipay TechLeader Test - Documentação API";
        options.SpecUrl = "/swagger/v1/swagger.json";
    });
}

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.UseRequestLogging();
app.MapControllers();

try
{
    Log.Information("Iniciando a API...");
    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "API terminou inesperadamente");
}
finally
{
    Log.CloseAndFlush();
}
