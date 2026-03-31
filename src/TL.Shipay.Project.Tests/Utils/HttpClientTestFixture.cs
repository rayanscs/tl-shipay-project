using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TL.Shipay.Project.Domain.Interfaces.ApiManager;
using TL.Shipay.Project.Infrastructure;
using TL.Shipay.Project.Infrastructure.ExternalServices;

namespace TL.Shipay.Project.Tests.Utils
{
    public class HttpClientTestFixture : IDisposable
    {
        private readonly IServiceProvider _serviceProvider;
        public IServiceCollection Services { get; private set; }

        public HttpClientTestFixture()
        {
            Services = new ServiceCollection();

            var configuration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.Test.json")
                .Build();

            var infrastructureOptions = configuration.GetSection("InfrastructureOptions")
                .Get<InfrastructureOptions>()
                ?? throw new InvalidOperationException("InfrastructureOptions não configurado");

            // Configurar HttpClient com Resilience para testes
            Services.AddHttpClient<IBrasilApiManager, BrasilApiManager>(client =>
            {
                client.DefaultRequestHeaders.Add("Accept", "application/json");
            })
            .AddStandardResilienceHandler(options =>
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

            Services.AddHttpClient<IViaCepManager, ViaCepManager>(client =>
            {
                client.DefaultRequestHeaders.Add("Accept", "application/json");
            })
            .AddStandardResilienceHandler(options =>
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

            _serviceProvider = Services.BuildServiceProvider();
        }

        public T GetService<T>() where T : notnull
        {
            return _serviceProvider.GetRequiredService<T>();
        }

        public void Dispose()
        {
            (_serviceProvider as IDisposable)?.Dispose();
        }
    }
}
