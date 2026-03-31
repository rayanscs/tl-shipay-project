using Microsoft.Extensions.DependencyInjection;
using Moq;
using TL.Shipay.Project.Infrastructure;

namespace TL.Shipay.Project.Tests.Utils
{
    public static class TestsExtensions
    {
        private static readonly Random _random = new Random();

        public static string GerarCnpj()
        {
            int soma = 0, resto = 0;
            int[] multiplicador1 = new int[12] { 5, 4, 3, 2, 9, 8, 7, 6, 5, 4, 3, 2 };
            int[] multiplicador2 = new int[13] { 6, 5, 4, 3, 2, 9, 8, 7, 6, 5, 4, 3, 2 };

            Random rnd = new Random();
            string semente = rnd.Next(10000000, 99999999).ToString() + "0001";

            for (int i = 0; i < 12; i++)
                soma += int.Parse(semente[i].ToString()) * multiplicador1[i];

            resto = soma % 11;
            if (resto < 2)
                resto = 0;
            else
                resto = 11 - resto;

            semente = semente + resto;
            soma = 0;

            for (int i = 0; i < 13; i++)
                soma += int.Parse(semente[i].ToString()) * multiplicador2[i];

            resto = soma % 11;

            if (resto < 2)
                resto = 0;
            else
                resto = 11 - resto;

            semente = semente + resto;
            return semente;
        }

        public static string GerarCepFormatado()
        {
            int parte1 = _random.Next(10000, 99999);
            int parte2 = _random.Next(0, 999);
            return $"{parte1:D5}-{parte2:D3}";
        }

        public static string GerarCepSimples() => _random.Next(10000000, 99999999).ToString();

        public static HttpClient CreateHttpClient(Mock<HttpMessageHandler> mockHttpMessageHandler) => new HttpClient(mockHttpMessageHandler.Object);

        public static HttpClient CreateResilientHttpClient(Mock<HttpMessageHandler> mockHttpMessageHandler, ResilienciaConfig? config = null)
        {
            var resiliencia = config ?? new ResilienciaConfig
            {
                RetryCount = 2,
                RetryDelaySeconds = 2,
                RetryUseJitter = true,
                CircuitBreakerFailureRatio = 0.5,
                CircuitBreakerMinimumThroughput = 3,
                CircuitBreakerSamplingDuration = 20,
                CircuitBreakerBreakDuration = 30
            };

            var services = new ServiceCollection();
            services.AddLogging();
            services.AddHttpClient("resilient")
                .ConfigurePrimaryHttpMessageHandler(() => mockHttpMessageHandler.Object)
                .AddStandardResilienceHandler(options =>
                {
                    options.Retry.MaxRetryAttempts = resiliencia.RetryCount;
                    options.Retry.Delay = TimeSpan.FromSeconds(resiliencia.RetryDelaySeconds);
                    options.Retry.BackoffType = Polly.DelayBackoffType.Constant;
                    options.Retry.UseJitter = resiliencia.RetryUseJitter;
                    options.CircuitBreaker.FailureRatio = resiliencia.CircuitBreakerFailureRatio;
                    options.CircuitBreaker.MinimumThroughput = resiliencia.CircuitBreakerMinimumThroughput;
                    options.CircuitBreaker.SamplingDuration = TimeSpan.FromSeconds(resiliencia.CircuitBreakerSamplingDuration);
                    options.CircuitBreaker.BreakDuration = TimeSpan.FromSeconds(resiliencia.CircuitBreakerBreakDuration);
                    options.TotalRequestTimeout.Timeout = TimeSpan.FromSeconds(60);
                    options.AttemptTimeout.Timeout = TimeSpan.FromSeconds(10);
                });

            var provider = services.BuildServiceProvider();
            var factory = provider.GetRequiredService<IHttpClientFactory>();
            return factory.CreateClient("resilient");
        }
    }
}
