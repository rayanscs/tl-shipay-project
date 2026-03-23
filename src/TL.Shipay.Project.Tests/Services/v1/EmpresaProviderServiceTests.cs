using AutoFixture;
using AutoMapper;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using TL.Shipay.Project.Application.Services;
using TL.Shipay.Project.Domain.Interfaces.ApiManager;
using TL.Shipay.Project.Domain.Models;
using TL.Shipay.Project.Domain.Models.Http;
using TL.Shipay.Project.Infrastructure;
using Xunit;

namespace TL.Shipay.Project.Tests.Services.v1
{
    public class EmpresaProviderServiceTests
    {
        private readonly EmpresaProviderService _empresaProviderService;
        private readonly Fixture _fixture = new();

        private readonly Mock<IBrasilApiManager> _brasilApiManagerMock = new();
        private readonly Mock<IViaCepManager> _viaCepApiManagerMock = new();
        private readonly Mock<IOptions<ResilienciaConfig>> _resConfigMock = new();
        private readonly Mock<ILogger<EmpresaProviderService>> _loggerMock = new();
        private readonly Mock<IMapper> _mapperMock = new();

        public EmpresaProviderServiceTests()
        {
            _resConfigMock.Setup(x => x.Value)
                .Returns(new ResilienciaConfig
                {
                    Services = new()
                    {
                        ServicePrincipal = "BrasilApi"
                    }
                });

            _empresaProviderService = new(
                    _brasilApiManagerMock.Object,
                    _viaCepApiManagerMock.Object,
                    _resConfigMock.Object,
                    _loggerMock.Object,
                    _mapperMock.Object
                );
        }

        [Fact]
        public async Task ProcessarValidacao_DeveRetornarSucesso_QuandoEnderecosCoincidem()
        {
            var cnpj = "123";
            var cep = "123";

            var empresaResponse = new Response();
            var enderecoResponse = new Response();

            var empresa = new DadosEmpresa
            {
                Municipio = "Curitiba",
                Logradouro = "Rua A"
            };

            var endereco = new Endereco
            {
                Cidade = "Curitiba",
                Logradouro = "Rua A"
            };

            _brasilApiManagerMock
                .Setup(x => x.ObterDadosEmpresaBrasilApiAsync(cnpj, It.IsAny<CancellationToken>()))
                .ReturnsAsync(empresaResponse);

            _brasilApiManagerMock
                .Setup(x => x.ObterEnderecoPorCepBrasilApiAsync(cep, It.IsAny<CancellationToken>()))
                .ReturnsAsync(enderecoResponse);

            _mapperMock
                .Setup(x => x.Map<DadosEmpresa>(empresaResponse))
                .Returns(empresa);

            _mapperMock
                .Setup(x => x.Map<Endereco>(It.IsAny<object>()))
                .Returns(endereco);

            enderecoResponse.SetData(new { street = "Rua A", cidade = "Curitiba" });

            var result = await _empresaProviderService.ProcessarValidacaoDadosEmpresaAsync(cnpj, cep, CancellationToken.None);

            Assert.True(result.Notifications.Count == 0);
            Assert.Contains("coincidem", result.MensagemPrincipal, StringComparison.OrdinalIgnoreCase);
        }

        [Fact]
        public async Task ProcessarValidacao_DeveRetornarFalha_QuandoEnderecosNaoCoincidem()
        {
            var cnpj = "123";
            var cep = "123";

            var empresaResponse = new Response();
            var enderecoResponse = new Response();

            var empresa = new DadosEmpresa
            {
                Municipio = "Curitiba",
                Logradouro = "Rua A"
            };

            var endereco = new Endereco
            {
                Cidade = "São Paulo",
                Logradouro = "Rua B"
            };

            _brasilApiManagerMock
                .Setup(x => x.ObterDadosEmpresaBrasilApiAsync(cnpj, It.IsAny<CancellationToken>()))
                .ReturnsAsync(empresaResponse);

            _brasilApiManagerMock
                .Setup(x => x.ObterEnderecoPorCepBrasilApiAsync(cep, It.IsAny<CancellationToken>()))
                .ReturnsAsync(enderecoResponse);

            _mapperMock
                .Setup(x => x.Map<DadosEmpresa>(empresaResponse))
                .Returns(empresa);

            _mapperMock
                .Setup(x => x.Map<Endereco>(It.IsAny<object>()))
                .Returns(endereco);

            enderecoResponse.SetData(new { street = "Rua B", cidade = "São Paulo" });

            var result = await _empresaProviderService.ProcessarValidacaoDadosEmpresaAsync(cnpj, cep, CancellationToken.None);

            Assert.NotEmpty(result.Notifications);
        }

        [Fact]
        public async Task ProcessarValidacao_DeveRetornarErro_QuandoEmpresaFalhar()
        {
            var cnpj = "123";
            var cep = "123";

            var empresaResponse = new Response();
            empresaResponse.AddNotification("erro");

            _brasilApiManagerMock
                .Setup(x => x.ObterDadosEmpresaBrasilApiAsync(cnpj, It.IsAny<CancellationToken>()))
                .ReturnsAsync(empresaResponse);

            var result = await _empresaProviderService.ProcessarValidacaoDadosEmpresaAsync(cnpj, cep, CancellationToken.None);

            Assert.False(result.Sucesso);
        }

        [Fact]
        public async Task ProcessarValidacao_DeveUsarFallback_QuandoBrasilApiFalhar()
        {
            var cnpj = "123";
            var cep = "123";

            var empresaResponse = new Response();
            var falhaCep = new Response();
            falhaCep.AddNotification("erro");
            var sucessoViaCep = new Response();

            var empresa = new DadosEmpresa
            {
                Municipio = "Curitiba",
                Logradouro = "Rua A"
            };

            var endereco = new Endereco
            {
                Cidade = "Curitiba",
                Logradouro = "Rua A"
            };

            _brasilApiManagerMock
                .Setup(x => x.ObterDadosEmpresaBrasilApiAsync(cnpj, It.IsAny<CancellationToken>()))
                .ReturnsAsync(empresaResponse);

            _brasilApiManagerMock
                .Setup(x => x.ObterEnderecoPorCepBrasilApiAsync(cep, It.IsAny<CancellationToken>()))
                .ReturnsAsync(falhaCep);

            _viaCepApiManagerMock
                .Setup(x => x.ObterEnderecoViaCepAsync(cep, It.IsAny<CancellationToken>()))
                .ReturnsAsync(sucessoViaCep);

            _mapperMock
                .Setup(x => x.Map<DadosEmpresa>(empresaResponse))
                .Returns(empresa);

            _mapperMock
                .Setup(x => x.Map<Endereco>(It.IsAny<object>()))
                .Returns(endereco);

            sucessoViaCep.SetData(new { logradouro = "Rua A", localidade = "Curitiba" });

            var result = await _empresaProviderService.ProcessarValidacaoDadosEmpresaAsync(cnpj, cep, CancellationToken.None);

            Assert.Empty(result.Notifications);

            _viaCepApiManagerMock.Verify(x =>
                x.ObterEnderecoViaCepAsync(cep, It.IsAny<CancellationToken>()
            ), Times.Once);
        }
    }
}
