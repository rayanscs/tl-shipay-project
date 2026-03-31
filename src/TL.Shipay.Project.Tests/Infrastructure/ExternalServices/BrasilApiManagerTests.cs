using Bogus;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using Moq.Protected;
using System.Net;
using System.Text.Json;
using TL.Shipay.Project.Domain.Models.Responses.BrasilApi.DadosCep;
using TL.Shipay.Project.Domain.Models.Responses.BrasilApi.DadosCnpj;
using TL.Shipay.Project.Infrastructure;
using TL.Shipay.Project.Infrastructure.ExternalServices;
using TL.Shipay.Project.Tests.Utils;
using Xunit;

namespace TL.Shipay.Project.Tests.Infrastructure.ExternalServices
{
    public class BrasilApiManagerTests
    {

        private readonly Mock<ILogger<BrasilApiManager>> _loggerMock;
        private readonly Mock<IOptions<ApiManagerUrlOptions>> _optionsMock;
        private readonly ApiManagerUrlOptions _apiManagerUrlOptions;

        public BrasilApiManagerTests()
        {
            _loggerMock = new();
            _optionsMock = new();

            _apiManagerUrlOptions = new ApiManagerUrlOptions
            {
                BrasilApi = new BrasilApi
                {
                    BaseUrl = "https://api.brasil.io/api/v2",
                    DadosCnpj = "/cnpj/v1/{0}",
                    DadosCep = "/cep/v2/{0}"
                }
            };

            _optionsMock.Setup(x => x.Value).Returns(_apiManagerUrlOptions);
        }        

        #region ObterDadosEmpresaBrasilApiAsync Tests - GET

            [Fact]
        public async Task ObterDadosEmpresaBrasilApiAsync_DeveRetornarSucesso_QuandoRequisicaoTemSucesso()
        {
            #region Arrange
            var cnpj = TestsExtensions.GerarCnpj();
            var cep = TestsExtensions.GerarCepSimples();

            var dadosEmpresa = new Faker<DadosCnpjBrasilApiResponse>("pt_BR")
                .RuleFor(e => e.Cnpj, cnpj)
                .RuleFor(e => e.Cep, cep)
                .RuleFor(e => e.Municipio, f => f.Address.City())
                .RuleFor(e => e.Logradouro, f => f.Address.StreetName())
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
                    Content = new StringContent(JsonSerializer.Serialize(dadosEmpresa))
                });

            var httpClientMock = TestsExtensions.CreateResilientHttpClient(mockHttpMessageHandler);

            var brasilApiManager = new BrasilApiManager(httpClientMock, _loggerMock.Object, _optionsMock.Object);
            #endregion

            #region Act
            var result = await brasilApiManager.ObterDadosEmpresaBrasilApiAsync(cnpj, CancellationToken.None);
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
        public async Task ObterDadosEmpresaBrasilApiAsync_DeveRetornarErro_QuandoStatusCodeNaoESucesso()
        {
            #region Arrange
            var cnpj = TestsExtensions.GerarCnpj();

            var mockHttpMessageHandler = new Mock<HttpMessageHandler>();
            mockHttpMessageHandler
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.Is<HttpRequestMessage>(req => req.Method == HttpMethod.Get),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.NotFound));

            var httpClientMock = TestsExtensions.CreateResilientHttpClient(mockHttpMessageHandler);
            var brasilApiManager = new BrasilApiManager(httpClientMock, _loggerMock.Object, _optionsMock.Object);
            #endregion

            #region Act
            var result = await brasilApiManager.ObterDadosEmpresaBrasilApiAsync(cnpj, CancellationToken.None);
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
        public async Task ObterDadosEmpresaBrasilApiAsync_DeveRetornarErro_QuandoRespostaEhNula()
        {
            #region Arrange
            var cnpj = TestsExtensions.GerarCnpj();

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

            var httpClientMock = TestsExtensions.CreateResilientHttpClient(mockHttpMessageHandler);
            var brasilApiManager = new BrasilApiManager(httpClientMock, _loggerMock.Object, _optionsMock.Object);
            #endregion

            #region Act 
            var result = await brasilApiManager.ObterDadosEmpresaBrasilApiAsync(cnpj, CancellationToken.None);
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
        public async Task ObterDadosEmpresaBrasilApiAsync_DeveRetornarErro_QuandoExcecaoEhLancada()
        {
            #region Arrange
            var cnpj = TestsExtensions.GerarCnpj();

            var mockHttpMessageHandler = new Mock<HttpMessageHandler>();
            mockHttpMessageHandler
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.Is<HttpRequestMessage>(req => req.Method == HttpMethod.Get),
                    ItExpr.IsAny<CancellationToken>())
                .ThrowsAsync(new HttpRequestException("Erro de conexão"));

            var httpClient = TestsExtensions.CreateHttpClient(mockHttpMessageHandler);

            var brasilApiManager = new BrasilApiManager(httpClient, _loggerMock.Object, _optionsMock.Object);
            #endregion

            #region Act
            var result = await brasilApiManager.ObterDadosEmpresaBrasilApiAsync(cnpj, CancellationToken.None);
            #endregion

            #region Assert
            Assert.False(result.Sucesso);
            Assert.NotEmpty(result.Notifications);
            #endregion
        }

        [Fact]
        public async Task ObterDadosEmpresaBrasilApiAsync_DeveRetentar_QuandoPrimeiraRequisicaoFalha()
        {
            #region Arrange
            var cnpj = TestsExtensions.GerarCnpj();
            var dadosEmpresa = new DadosCnpjBrasilApiResponse
            {
                Municipio = "São Paulo",
                Logradouro = "Avenida Paulista"
            };

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
                    Content = new StringContent(JsonSerializer.Serialize(dadosEmpresa))
                });

            var httpClient = TestsExtensions.CreateResilientHttpClient(mockHttpMessageHandler);

            var brasilApiManager = new BrasilApiManager(httpClient, _loggerMock.Object, _optionsMock.Object);
            #endregion

            #region Act 
            var result = await brasilApiManager.ObterDadosEmpresaBrasilApiAsync(cnpj, CancellationToken.None);
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
        public async Task ObterDadosEmpresaBrasilApiAsync_DeveRetornarErro_QuandoTimeoutEhExcedido()
        {
            #region Arrange
            var cnpj = TestsExtensions.GerarCnpj();

            var mockHttpMessageHandler = new Mock<HttpMessageHandler>();
            mockHttpMessageHandler
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.Is<HttpRequestMessage>(req => req.Method == HttpMethod.Get),
                    ItExpr.IsAny<CancellationToken>())
                .ThrowsAsync(new OperationCanceledException("Timeout"));

            var httpClient = TestsExtensions.CreateHttpClient(mockHttpMessageHandler);
            var brasilApiManager = new BrasilApiManager(httpClient, _loggerMock.Object, _optionsMock.Object);
            #endregion

            #region Act & Assert
            await Assert.ThrowsAsync<OperationCanceledException>(
                () => brasilApiManager.ObterDadosEmpresaBrasilApiAsync(cnpj, CancellationToken.None));
            #endregion
        }

        #endregion

        #region ObterEnderecoPorCepBrasilApiAsync Tests

        [Fact]
        public async Task ObterEnderecoPorCepBrasilApiAsync_DeveRetornarSucesso_QuandoRequisicaoTemSucesso()
        {
            #region Arrange 
            var cep = TestsExtensions.GerarCepSimples();
            var dadosEndereco = new Faker<BrasilApiCepResponse>("pt_BR")
                .RuleFor(e => e.Street, f => f.Address.StreetName())
                .RuleFor(e => e.City, f => f.Address.City())
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

            var httpClient = TestsExtensions.CreateHttpClient(mockHttpMessageHandler);
            var brasilApiManager = new BrasilApiManager(httpClient, _loggerMock.Object, _optionsMock.Object);
            #endregion

            #region Act 
            var result = await brasilApiManager.ObterEnderecoPorCepBrasilApiAsync(cep, CancellationToken.None);
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
        public async Task ObterEnderecoPorCepBrasilApiAsync_DeveRetornarErro_QuandoCepNaoEncontrado()
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

            var httpClient = TestsExtensions.CreateHttpClient(mockHttpMessageHandler);
            var brasilApiManager = new BrasilApiManager(httpClient, _loggerMock.Object, _optionsMock.Object);
            #endregion

            #region Act
            var result = await brasilApiManager.ObterEnderecoPorCepBrasilApiAsync(cep, CancellationToken.None);
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
        public async Task ObterEnderecoPorCepBrasilApiAsync_DeveRetornarErro_QuandoRespostaEhNula()
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

            var httpClient = TestsExtensions.CreateHttpClient(mockHttpMessageHandler);
            var brasilApiManager = new BrasilApiManager(httpClient, _loggerMock.Object, _optionsMock.Object);
            #endregion

            #region Act
            var result = await brasilApiManager.ObterEnderecoPorCepBrasilApiAsync(cep, CancellationToken.None);
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
        public async Task ObterEnderecoPorCepBrasilApiAsync_DeveRetentar_QuandoErroTemporarioOcorre()
        {
            #region Arrange
            var cep = TestsExtensions.GerarCepSimples();
            var endereco = new BrasilApiCepResponse
            {
                Street = "Avenida Paulista",
                City = "São Paulo"
            };

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
                    Content = new StringContent(JsonSerializer.Serialize(endereco))
                });

            var httpClient = TestsExtensions.CreateResilientHttpClient(mockHttpMessageHandler);
            var brasilApiManager = new BrasilApiManager(httpClient, _loggerMock.Object, _optionsMock.Object);
            #endregion

            #region Act
            var result = await brasilApiManager.ObterEnderecoPorCepBrasilApiAsync(cep, CancellationToken.None);
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

        [Fact]
        public async Task ObterEnderecoPorCepBrasilApiAsync_DeveRetornarErro_QuandoExcecaoEhLancada()
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

            var httpClient = TestsExtensions.CreateHttpClient(mockHttpMessageHandler);
            var brasilApiManager = new BrasilApiManager(httpClient, _loggerMock.Object, _optionsMock.Object);
            #endregion

            #region Act
            var result = await brasilApiManager.ObterEnderecoPorCepBrasilApiAsync(cep, CancellationToken.None);
            #endregion

            #region Assert
            Assert.False(result.Sucesso);
            Assert.NotEmpty(result.Notifications);
            #endregion
        }

        #endregion

        #region Resiliência Tests

        [Fact]
        public async Task ObterDadosEmpresaBrasilApiAsync_DeveRegistrarLog_QuandoFalhaEOcorre()
        {
            #region Arrange
            var cnpj = TestsExtensions.GerarCnpj();

            var mockHttpMessageHandler = new Mock<HttpMessageHandler>();
            mockHttpMessageHandler
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.Is<HttpRequestMessage>(req => req.Method == HttpMethod.Get),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.ServiceUnavailable));

            var httpClient = TestsExtensions.CreateHttpClient(mockHttpMessageHandler);
            var brasilApiManager = new BrasilApiManager(httpClient, _loggerMock.Object, _optionsMock.Object);
            #endregion

            #region Act
            await brasilApiManager.ObterDadosEmpresaBrasilApiAsync(cnpj, CancellationToken.None);
            #endregion

            #region Assert
            _loggerMock.Verify(
                x => x.Log(
                    LogLevel.Warning,
                    It.IsAny<EventId>(),
                    It.IsAny<It.IsAnyType>(),
                    It.IsAny<Exception>(),
                    It.IsAny<Func<It.IsAnyType, Exception, string>>()),
                Times.Once);
            #endregion
        }

        [Fact]
        public async Task ObterEnderecoPorCepBrasilApiAsync_DeveRegistrarLogDeErro_QuandoExcecaoOcorre()
        {
            #region Arrange
            var cep = TestsExtensions.GerarCepSimples();
            var exception = new HttpRequestException("Erro de conexão");

            var mockHttpMessageHandler = new Mock<HttpMessageHandler>();
            mockHttpMessageHandler
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.Is<HttpRequestMessage>(req => req.Method == HttpMethod.Get),
                    ItExpr.IsAny<CancellationToken>())
                .ThrowsAsync(exception);

            var httpClient = TestsExtensions.CreateHttpClient(mockHttpMessageHandler);
            var brasilApiManager = new BrasilApiManager(httpClient, _loggerMock.Object, _optionsMock.Object);
            #endregion

            #region Act
            await brasilApiManager.ObterEnderecoPorCepBrasilApiAsync(cep, CancellationToken.None);
            #endregion

            #region Assert
            _loggerMock.Verify(
                x => x.Log(
                    LogLevel.Error,
                    It.IsAny<EventId>(),
                    It.IsAny<It.IsAnyType>(),
                    It.IsAny<Exception>(),
                    It.IsAny<Func<It.IsAnyType, Exception, string>>()),
                Times.Once);
            #endregion
        }


        [Theory]
        [InlineData(HttpStatusCode.InternalServerError)]
        [InlineData(HttpStatusCode.BadGateway)]
        [InlineData(HttpStatusCode.ServiceUnavailable)]
        [InlineData(HttpStatusCode.GatewayTimeout)]
        public async Task ObterEnderecoPorCepBrasilApiAsync_DeveRetentar_QuandoStatusCodeEhErroDoServicoExternoBrasilApi(HttpStatusCode statusCode)
        {
            #region Arrange
            var cep = TestsExtensions.GerarCepSimples();
            var endereco = new BrasilApiCepResponse
            {
                Street = "Rua Teste",
                City = "Teste"
            };

            var mockHttpMessageHandler = new Mock<HttpMessageHandler>();
            mockHttpMessageHandler
                .Protected()
                .SetupSequence<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.Is<HttpRequestMessage>(req => req.Method == HttpMethod.Get),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(TestsExtensions.CriarResponsePorHttpStatusCode(statusCode, endereco))
                .ReturnsAsync(TestsExtensions.CriarResponsePorHttpStatusCode(statusCode, endereco))
                .ReturnsAsync(TestsExtensions.CriarResponsePorHttpStatusCode(statusCode, endereco))
                .ReturnsAsync(TestsExtensions.CriarResponsePorHttpStatusCode(statusCode, endereco));

            var httpClient = TestsExtensions.CreateResilientHttpClient(mockHttpMessageHandler);
            var brasilApiManager = new BrasilApiManager(httpClient, _loggerMock.Object, _optionsMock.Object);
            #endregion

            #region Act
            var result = await brasilApiManager.ObterEnderecoPorCepBrasilApiAsync(cep, CancellationToken.None);
            #endregion

            #region Assert
            Assert.False(result.Sucesso);
            Assert.NotEmpty(result.Notifications);
            #endregion
        }

        #endregion
    }
}
