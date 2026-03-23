using Asp.Versioning;
using FluentValidation;
using Swashbuckle.AspNetCore.Filters;
using TL.Shipay.Project.Api.Extensions;
using TL.Shipay.Project.Application.Validators;
using TL.Shipay.Project.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

builder.Services.Configure<ApiManagerUrlOptions>(builder.Configuration.GetSection("RouteOptions"));
builder.Services.Configure<InfrasctructureOptions>(builder.Configuration.GetSection("ResilienciaConfig"));

var configuration = builder.Configuration;

builder.Services.AddServices();
builder.Services.AddHttpClientFactory(configuration);
builder.Services.AddControllers();

builder.Services.AddValidatorsFromAssemblyContaining<ClienteValidator>();

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

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        // Set the Swagger UI to be at the app's root URL (optional)
        // c.RoutePrefix = string.Empty; 
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Shipay TechLeader Test");
    });

    app.UseReDoc(options =>
    {
        options.DocumentTitle = "Shipay TechLeader Test - Documentação API";
        options.SpecUrl = "/swagger/v1/swagger.json";
    });
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.Run();
