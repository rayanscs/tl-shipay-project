using Microsoft.Extensions.DependencyInjection;
using Moq;
using System.Net;
using System.Text;
using System.Text.Json;
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
                CircuitBreakerFailureRatio = 0.5,       // Percentual de falhas permitido antes do Circuit Breaker abrir.
                CircuitBreakerMinimumThroughput = 3,    // Número mínimo de requisições antes do Circuit Breaker começar a monitorar falhas.
                CircuitBreakerSamplingDuration = 20,    // Período de tempo (em segundos) em que o Circuit Breaker analisa as requisições
                CircuitBreakerBreakDuration = 10        // Tempo (em segundos) que o circuito fica ABERTO (rejeitando requisições).
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

        public static HttpResponseMessage CriarResponsePorHttpStatusCode(HttpStatusCode statusCode, object dados)
        {
            return statusCode switch
            {
                HttpStatusCode.OK => new HttpResponseMessage(statusCode)
                {
                    Content = new StringContent(
                        JsonSerializer.Serialize(dados),
                        Encoding.UTF8,
                        "application/json")
                },
                HttpStatusCode.NotFound => new HttpResponseMessage(statusCode)
                {
                    Content = new StringContent(
                        JsonSerializer.Serialize(new { error = "Recurso não encontrado" }),
                        Encoding.UTF8,
                        "application/json"),
                    ReasonPhrase = "Not Found"
                },
                HttpStatusCode.BadRequest => new HttpResponseMessage(statusCode)
                {
                    Content = new StringContent(
                        JsonSerializer.Serialize(new { error = "Requisição inválida" }),
                        Encoding.UTF8,
                        "application/json")
                },
                HttpStatusCode.Unauthorized => new HttpResponseMessage(statusCode)
                {
                    Content = new StringContent(
                        JsonSerializer.Serialize(new { error = "Não autorizado" }),
                        Encoding.UTF8,
                        "application/json")
                },
                HttpStatusCode.BadGateway => new HttpResponseMessage(statusCode)
                {
                    Content = new StringContent(
                        JsonSerializer.Serialize(new { error = "Bad Gateway - Servidor intermediário inválido" }),
                        Encoding.UTF8,
                        "application/json"),
                            ReasonPhrase = "Bad Gateway"
                },
                HttpStatusCode.ServiceUnavailable => new HttpResponseMessage(statusCode)
                {
                    Content = new StringContent(
                        JsonSerializer.Serialize(new { error = "Serviço indisponível - Tente novamente mais tarde" }),
                        Encoding.UTF8,
                        "application/json"),
                            ReasonPhrase = "Service Unavailable",
                            Headers = { { "Retry-After", "1" } }
                },
                HttpStatusCode.GatewayTimeout => new HttpResponseMessage(statusCode)
                {
                    Content = new StringContent(
                        JsonSerializer.Serialize(new { error = "Timeout - Serviço demorou muito para responder" }),
                        Encoding.UTF8,
                        "application/json"),
                            ReasonPhrase = "Gateway Timeout"
                },
                _ => new HttpResponseMessage(HttpStatusCode.InternalServerError)
                {
                    Content = new StringContent(
                        JsonSerializer.Serialize(new { error = "Erro desconhecido" }),
                        Encoding.UTF8,
                        "application/json")
                }
            };
        }
    }
}
