using TL.Shipay.Project.Domain.Interfaces.ApiManager;
using TL.Shipay.Project.Infrastructure;
using TL.Shipay.Project.Infrastructure.ExternalServices;

namespace TL.Shipay.Project.Api.Extensions
{
    public static class HttpClientExtensions
    {
        public static IServiceCollection AddHttpClientFactory(this IServiceCollection services, IConfiguration configuration)
        {
            var infrastructureOptions = configuration.GetSection("InfrastructureOptions").Get<InfrastructureOptions>()
                ?? throw new InvalidOperationException("A sessão InfrastructureOptions não possui valores.");

            services.AddHttpClient<IBrasilApiManager, BrasilApiManager>(client =>
            {
                client.DefaultRequestHeaders.Add("Accept", "application/json");
            }).AddStandardResilienceHandler(options =>
            {
                options.Retry.MaxRetryAttempts = infrastructureOptions.ResilienciaConfig.RetryCount; // número de tentativas de retry (1 + valor de retentativas)
                options.Retry.Delay = TimeSpan.FromSeconds(infrastructureOptions.ResilienciaConfig.RetryDelaySeconds); // delay entre as tentativas de retry
                options.Retry.BackoffType = Polly.DelayBackoffType.Constant; // tipo de backoff (constante)
                options.Retry.UseJitter = infrastructureOptions.ResilienciaConfig.RetryUseJitter; // habilitar jitter para evitar picos de tráfego
                options.CircuitBreaker.FailureRatio = infrastructureOptions.ResilienciaConfig.CircuitBreakerFailureRatio;         // 50% de falhas numa janela
                options.CircuitBreaker.MinimumThroughput = infrastructureOptions.ResilienciaConfig.CircuitBreakerMinimumThroughput;         // pelo menos 2 req durante a janela
                options.CircuitBreaker.SamplingDuration = TimeSpan.FromSeconds(infrastructureOptions.ResilienciaConfig.CircuitBreakerSamplingDuration); // janela de amostragem
                options.CircuitBreaker.BreakDuration = TimeSpan.FromSeconds(infrastructureOptions.ResilienciaConfig.CircuitBreakerBreakDuration);    // fica aberto 60s
           });

            services.AddHttpClient<IViaCepManager, ViaCepManager>(client =>
            {
                client.DefaultRequestHeaders.Add("Accept", "application/json");
            }).AddStandardResilienceHandler(options =>
            {
                options.Retry.MaxRetryAttempts = infrastructureOptions.ResilienciaConfig.RetryCount; // número de tentativas de retry (1 + valor de retentativas)
                options.Retry.Delay = TimeSpan.FromSeconds(infrastructureOptions.ResilienciaConfig.RetryDelaySeconds); // delay entre as tentativas de retry
                options.Retry.BackoffType = Polly.DelayBackoffType.Constant; // tipo de backoff (constante)
                options.Retry.UseJitter = infrastructureOptions.ResilienciaConfig.RetryUseJitter; // habilitar jitter para evitar picos de tráfego
                options.CircuitBreaker.FailureRatio = infrastructureOptions.ResilienciaConfig.CircuitBreakerFailureRatio;         // 50% de falhas numa janela
                options.CircuitBreaker.MinimumThroughput = infrastructureOptions.ResilienciaConfig.CircuitBreakerMinimumThroughput;         // pelo menos 2 req durante a janela
                options.CircuitBreaker.SamplingDuration = TimeSpan.FromSeconds(infrastructureOptions.ResilienciaConfig.CircuitBreakerSamplingDuration); // janela de amostragem
                options.CircuitBreaker.BreakDuration = TimeSpan.FromSeconds(infrastructureOptions.ResilienciaConfig.CircuitBreakerBreakDuration);    // fica aberto 60s
            });

            return services;
        }
    }
}
