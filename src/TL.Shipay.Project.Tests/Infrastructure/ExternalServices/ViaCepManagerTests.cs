using Bogus;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using Moq.Protected;
using System.Net;
using System.Text.Json;
using TL.Shipay.Project.Domain.Models.Responses.ViaCep;
using TL.Shipay.Project.Infrastructure;
using TL.Shipay.Project.Infrastructure.ExternalServices;
using TL.Shipay.Project.Tests.Utils;
using Xunit;

namespace TL.Shipay.Project.Tests.Infrastructure.ExternalServices
{
    public class ViaCepManagerTests
    {
        private readonly Mock<ILogger<ViaCepManager>> _loggerMock;
        private readonly Mock<IOptions<ApiManagerUrlOptions>> _optionsMock;
        private readonly ApiManagerUrlOptions _apiManagerUrlOptions;

        public ViaCepManagerTests()
        {
            _loggerMock = new();
            _optionsMock = new();

            _apiManagerUrlOptions = new ApiManagerUrlOptions
            {
                ViaCep = new ViaCep
                {
                    BaseUrl = "https://viacep.com.br/ws",
                    DadosCep = "/{0}/json/"
                }
            };

            _optionsMock.Setup(x => x.Value).Returns(_apiManagerUrlOptions);
        }

        #region ObterEnderecoViaCepAsync Tests - GET

        [Fact]
        public async Task ObterEnderecoViaCepAsync_DeveRetornarSucesso_QuandoRequisicaoTemSucesso()
        {
            #region Arrange
            var cep = TestsExtensions.GerarCepSimples();

            var dadosEndereco = new Faker<ViaCepResponse>("pt_BR")
                .RuleFor(e => e.Cep, cep)
                .RuleFor(e => e.Logradouro, f => f.Address.StreetName())
                .RuleFor(e => e.Localidade, f => f.Address.City())
                .RuleFor(e => e.Bairro, f => f.Address.County())
                .RuleFor(e => e.Uf, f => f.Address.StateAbbr())
                .Generate();

            var mockHttpMessageHandler = new Mock<HttpMessageHandler>();
            mockHttpMessageHandler
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.Is<HttpRequestMessage>(req => req.Method == HttpMethod.Get),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = new StringContent(JsonSerializer.Serialize(dadosEndereco))
                });

            var httpClientMock = TestsExtensions.CreateHttpClient(mockHttpMessageHandler);
            var viaCepManager = new ViaCepManager(httpClientMock, _loggerMock.Object, _optionsMock.Object);
            #endregion

            #region Act
            var result = await viaCepManager.ObterEnderecoViaCepAsync(cep, CancellationToken.None);
            #endregion

            #region Assert
            Assert.True(result.Sucesso);
            Assert.Empty(result.Notifications);
            mockHttpMessageHandler.Protected().Verify(
                "SendAsync",
                Times.Once(),
                ItExpr.Is<HttpRequestMessage>(req => req.Method == HttpMethod.Get),
                ItExpr.IsAny<CancellationToken>());
            #endregion
        }

        [Fact]
        public async Task ObterEnderecoViaCepAsync_DeveRetornarErro_QuandoStatusCodeNaoESucesso()
        {
            #region Arrange
            var cep = TestsExtensions.GerarCepSimples();

            var mockHttpMessageHandler = new Mock<HttpMessageHandler>();
            mockHttpMessageHandler
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.Is<HttpRequestMessage>(req => req.Method == HttpMethod.Get),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.NotFound));

            var httpClientMock = TestsExtensions.CreateHttpClient(mockHttpMessageHandler);
            var viaCepManager = new ViaCepManager(httpClientMock, _loggerMock.Object, _optionsMock.Object);
            #endregion

            #region Act
            var result = await viaCepManager.ObterEnderecoViaCepAsync(cep, CancellationToken.None);
            #endregion

            #region Assert
            Assert.False(result.Sucesso);
            Assert.NotEmpty(result.Notifications);
            mockHttpMessageHandler.Protected().Verify(
                 "SendAsync",
                 Times.Once(),
                 ItExpr.Is<HttpRequestMessage>(req => req.Method == HttpMethod.Get),
                 ItExpr.IsAny<CancellationToken>());
            #endregion
        }

        [Fact]
        public async Task ObterEnderecoViaCepAsync_DeveRetornarErro_QuandoRespostaEhNula()
        {
            #region Arrange
            var cep = TestsExtensions.GerarCepSimples();

            var mockHttpMessageHandler = new Mock<HttpMessageHandler>();
            mockHttpMessageHandler
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.Is<HttpRequestMessage>(req => req.Method == HttpMethod.Get),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = new StringContent("null")
                });

            var httpClientMock = TestsExtensions.CreateHttpClient(mockHttpMessageHandler);
            var viaCepManager = new ViaCepManager(httpClientMock, _loggerMock.Object, _optionsMock.Object);
            #endregion

            #region Act
            var result = await viaCepManager.ObterEnderecoViaCepAsync(cep, CancellationToken.None);
            #endregion

            #region Assert
            Assert.False(result.Sucesso);
            Assert.NotEmpty(result.Notifications);
            mockHttpMessageHandler.Protected().Verify(
                "SendAsync",
                Times.Once(),
                ItExpr.Is<HttpRequestMessage>(req => req.Method == HttpMethod.Get),
                ItExpr.IsAny<CancellationToken>());
            #endregion
        }

        [Fact]
        public async Task ObterEnderecoViaCepAsync_DeveRetornarErro_QuandoCamposObrigatoriosVazios()
        {
            #region Arrange
            var cep = TestsExtensions.GerarCepSimples();

            var dadosEndereco = new Faker<ViaCepResponse>("pt_BR")
               .RuleFor(e => e.Cep, _ => "")
               .RuleFor(e => e.Logradouro, _ => "")
               .RuleFor(e => e.Localidade, _ => "")
               .Generate();

            var mockHttpMessageHandler = new Mock<HttpMessageHandler>();
            mockHttpMessageHandler
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.Is<HttpRequestMessage>(req => req.Method == HttpMethod.Get),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = new StringContent(JsonSerializer.Serialize(dadosEndereco))
                });

            var httpClientMock = TestsExtensions.CreateHttpClient(mockHttpMessageHandler);
            var viaCepManager = new ViaCepManager(httpClientMock, _loggerMock.Object, _optionsMock.Object);
            #endregion

            #region Act
            var result = await viaCepManager.ObterEnderecoViaCepAsync(cep, CancellationToken.None);
            #endregion

            #region Assert
            Assert.False(result.Sucesso);
            Assert.NotEmpty(result.Notifications);
            mockHttpMessageHandler.Protected().Verify(
                "SendAsync",
                Times.Once(),
                ItExpr.Is<HttpRequestMessage>(req => req.Method == HttpMethod.Get),
                ItExpr.IsAny<CancellationToken>());
            #endregion
        }

        [Fact]
        public async Task ObterEnderecoViaCepAsync_DeveRetornarErro_QuandoExcecaoEhLancada()
        {
            #region Arrange
            var cep = TestsExtensions.GerarCepSimples();

            var mockHttpMessageHandler = new Mock<HttpMessageHandler>();
            mockHttpMessageHandler
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.Is<HttpRequestMessage>(req => req.Method == HttpMethod.Get),
                    ItExpr.IsAny<CancellationToken>())
                .ThrowsAsync(new HttpRequestException("Erro de conexão"));

            var httpClient = TestsExtensions.CreateResilientHttpClient(mockHttpMessageHandler);
            var viaCepManager = new ViaCepManager(httpClient, _loggerMock.Object, _optionsMock.Object);
            #endregion

            #region Act
            var result = await viaCepManager.ObterEnderecoViaCepAsync(cep, CancellationToken.None);
            #endregion

            #region Assert
            Assert.False(result.Sucesso);
            Assert.NotEmpty(result.Notifications);
            #endregion
        }

        [Fact]
        public async Task ObterEnderecoViaCepAsync_DeveRetentar_QuandoPrimeiraRequisicaoFalha()
        {
            #region Arrange
            var cep = TestsExtensions.GerarCepSimples();
            var dadosEndereco = new Faker<ViaCepResponse>("pt_BR")
               .RuleFor(e => e.Cep, f => cep)
               .RuleFor(e => e.Logradouro, f => f.Address.StreetName())
               .RuleFor(e => e.Localidade, f => f.Address.City())
               .Generate();

            var mockHttpMessageHandler = new Mock<HttpMessageHandler>();
            mockHttpMessageHandler
                .Protected()
                .SetupSequence<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.Is<HttpRequestMessage>(req => req.Method == HttpMethod.Get),
                    ItExpr.IsAny<CancellationToken>())
                // Primeira tentativa: falha com timeout
                .ThrowsAsync(new HttpRequestException("Timeout", new TimeoutException()))
                // Segunda tentativa (retry): sucesso
                .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = new StringContent(JsonSerializer.Serialize(dadosEndereco))
                });

            var httpClient = TestsExtensions.CreateResilientHttpClient(mockHttpMessageHandler);
            var viaCepManager = new ViaCepManager(httpClient, _loggerMock.Object, _optionsMock.Object);
            #endregion

            #region Act
            var result = await viaCepManager.ObterEnderecoViaCepAsync(cep, CancellationToken.None);
            #endregion

            #region Assert
            Assert.True(result.Sucesso);
            // Verifica que foi chamado 2 vezes (1 inicial + 1 retry)
            mockHttpMessageHandler.Protected().Verify(
                "SendAsync",
                Times.Exactly(2),
                ItExpr.Is<HttpRequestMessage>(req => req.Method == HttpMethod.Get),
                ItExpr.IsAny<CancellationToken>());
            #endregion
        }

        [Fact]
        public async Task ObterEnderecoViaCepAsync_DeveRetornarErro_QuandoTimeoutEhExcedido()
        {
            #region Arrange
            var cep = TestsExtensions.GerarCepSimples();

            var mockHttpMessageHandler = new Mock<HttpMessageHandler>();
            mockHttpMessageHandler
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.Is<HttpRequestMessage>(req => req.Method == HttpMethod.Get),
                    ItExpr.IsAny<CancellationToken>())
                .ThrowsAsync(new OperationCanceledException("Timeout"));

            var httpClient = TestsExtensions.CreateResilientHttpClient(mockHttpMessageHandler);
            var viaCepManager = new ViaCepManager(httpClient, _loggerMock.Object, _optionsMock.Object);
            #endregion

            #region Act & Assert
            await Assert.ThrowsAsync<OperationCanceledException>(
                () => viaCepManager.ObterEnderecoViaCepAsync(cep, CancellationToken.None));
            #endregion
        }

        #endregion

        #region Resiliência Tests
        [Theory]
        [InlineData(HttpStatusCode.InternalServerError)]
        [InlineData(HttpStatusCode.BadGateway)]
        [InlineData(HttpStatusCode.ServiceUnavailable)]
        [InlineData(HttpStatusCode.GatewayTimeout)]
        public async Task ObterEnderecoViaCepAsync_DeveRetornarErro_QuandoStatusCodeEhErroDoServicoExternoViaCep(HttpStatusCode statusCode)
        {
            #region Arrange
            var cep = TestsExtensions.GerarCepSimples();
            var dadosEndereco = new Faker<ViaCepResponse>("pt_BR")
                .RuleFor(e => e.Cep, f => cep)
                .RuleFor(e => e.Logradouro, f => f.Address.StreetName())
                .RuleFor(e => e.Localidade, f => f.Address.City())
                .Generate();

            var mockHttpMessageHandler = new Mock<HttpMessageHandler>();
            mockHttpMessageHandler
                .Protected()
                .SetupSequence<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.Is<HttpRequestMessage>(req => req.Method == HttpMethod.Get),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(TestsExtensions.CriarResponsePorHttpStatusCode(statusCode, dadosEndereco))
                .ReturnsAsync(TestsExtensions.CriarResponsePorHttpStatusCode(statusCode, dadosEndereco))
                .ReturnsAsync(TestsExtensions.CriarResponsePorHttpStatusCode(statusCode, dadosEndereco))
                .ReturnsAsync(TestsExtensions.CriarResponsePorHttpStatusCode(statusCode, dadosEndereco));

            var httpClient = TestsExtensions.CreateResilientHttpClient(mockHttpMessageHandler);
            var viaCepManager = new ViaCepManager(httpClient, _loggerMock.Object, _optionsMock.Object);
            #endregion

            #region Act
            var result = await viaCepManager.ObterEnderecoViaCepAsync(cep, CancellationToken.None);
            #endregion

            #region Assert
            Assert.False(result.Sucesso);
            Assert.NotEmpty(result.Notifications);
            #endregion
        }

        [Fact]
        public async Task ObterEnderecoViaCepAsync_DeveRetentar_QuandoErroTemporarioOcorre()
        {
            #region Arrange
            var cep = TestsExtensions.GerarCepSimples();
            var dadosEndereco = new Faker<ViaCepResponse>("pt_BR")
                .RuleFor(e => e.Cep, f => cep)
                .RuleFor(e => e.Logradouro, f => f.Address.StreetName())
                .RuleFor(e => e.Localidade, f => f.Address.City())
                .Generate();

            var mockHttpMessageHandler = new Mock<HttpMessageHandler>();
            mockHttpMessageHandler
                .Protected()
                .SetupSequence<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.Is<HttpRequestMessage>(req => req.Method == HttpMethod.Get),
                    ItExpr.IsAny<CancellationToken>())
                // Primeira tentativa: erro de serviço indisponível
                .ThrowsAsync(new HttpRequestException("Service Unavailable", null, HttpStatusCode.ServiceUnavailable))
                // Segunda tentativa (retry): sucesso
                .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = new StringContent(JsonSerializer.Serialize(dadosEndereco))
                });

            var httpClient = TestsExtensions.CreateResilientHttpClient(mockHttpMessageHandler);
            var viaCepManager = new ViaCepManager(httpClient, _loggerMock.Object, _optionsMock.Object);
            #endregion

            #region Act
            var result = await viaCepManager.ObterEnderecoViaCepAsync(cep, CancellationToken.None);
            #endregion

            #region Assert
            Assert.True(result.Sucesso);
            mockHttpMessageHandler.Protected().Verify(
                "SendAsync",
                Times.Exactly(2),
                ItExpr.Is<HttpRequestMessage>(req => req.Method == HttpMethod.Get),
                ItExpr.IsAny<CancellationToken>());
            #endregion
        }

        #endregion
    }
}
