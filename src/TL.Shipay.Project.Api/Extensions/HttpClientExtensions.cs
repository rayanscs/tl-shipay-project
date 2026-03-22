using TL.Shipay.Project.Infrastructure;
using TL.Shipay.Project.Infrastructure.ExternalServices;

namespace TL.Shipay.Project.Api.Extensions
{
    public static class HttpClientExtensions
    {
        public static IServiceCollection AddHttpClientFactory(this IServiceCollection services, IConfiguration configuration)
        {
            var apiManagerUrlOptions = configuration.GetSection("ResilienciaConfig").Get<ResilienciaConfig>() 
                ?? throw new InvalidOperationException("A sessão 'ResilienciaConfig' não possui valores."); ;

            services.AddHttpClient<BrasilApiManager>(client =>
            {
                client.DefaultRequestHeaders.Add("Accept", "application/json");
            }).AddStandardResilienceHandler(options =>
            {
                options.Retry.MaxRetryAttempts = apiManagerUrlOptions.RetryCount;
                options.CircuitBreaker.FailureRatio = 0.5;         // 50% de falhas numa janela
                options.CircuitBreaker.MinimumThroughput = 2;         // pelo menos 2 req durante a janela
                options.CircuitBreaker.SamplingDuration = TimeSpan.FromSeconds(10); // janela de amostragem
                options.CircuitBreaker.BreakDuration = TimeSpan.FromSeconds(30);    // fica aberto 60s
            });

            services.AddHttpClient<ViaCepManager>(client =>
            {
                client.DefaultRequestHeaders.Add("Accept", "application/json");
            }).AddStandardResilienceHandler(options =>
            {
                options.Retry.MaxRetryAttempts = apiManagerUrlOptions.RetryCount;
            });

            return services;
        }
    }
}
