using AutoFixture;
using AutoMapper;
using Bogus;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using TL.Shipay.Project.Application.Services;
using TL.Shipay.Project.Domain.Interfaces.ApiManager;
using TL.Shipay.Project.Domain.Models;
using TL.Shipay.Project.Domain.Models.Http;
using TL.Shipay.Project.Domain.Models.Responses.BrasilApi.DadosCnpj;
using TL.Shipay.Project.Infrastructure;
using TL.Shipay.Project.Tests.Utils;
using Xunit;

namespace TL.Shipay.Project.Tests.Services.v1
{
    public class EmpresaProviderServiceTests
    {
        private readonly EmpresaProviderService _empresaProviderService;
        private readonly Fixture _fixture = new();

        private readonly Mock<IBrasilApiManager> _brasilApiManagerMock = new();
        private readonly Mock<IViaCepManager> _viaCepApiManagerMock = new();
        private readonly Mock<IOptions<InfrastructureOptions>> _resConfigMock = new();
        private readonly Mock<ILogger<EmpresaProviderService>> _loggerMock = new();
        private readonly Mock<IMapper> _mapperMock = new();

        public EmpresaProviderServiceTests()
        {
            _resConfigMock.Setup(x => x.Value)
                .Returns(new InfrastructureOptions
                {   ResilienciaConfig = new ()
                    {
                        RetryCount = 3,
                        CircuitBreakerFailureRatio = 0.5,
                        CircuitBreakerMinimumThroughput = 10,
                        CircuitBreakerSamplingDuration = 60,
                        CircuitBreakerBreakDuration = 30,
                        Services = new()
                        {
                            ServicePrincipal = "BrasilApi"
                        }
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
            var cnpj = TestsExtensions.GerarCnpj();
            var cep = TestsExtensions.GerarCepSimples();

            var empresaResponse = new Response();
            var enderecoResponse = new Response();

            var empresa = new Faker<DadosEmpresa>("pt_BR")
                 .RuleFor(e => e.Municipio, f => f.Address.City())
                 .RuleFor(e => e.Logradouro, f => f.Address.StreetName())
                 .Generate();

            var dadosCnpjObj = new DadosCnpjBrasilApiResponse
            {
                Municipio = empresa.Municipio,
                Logradouro = empresa.Logradouro
            };
            empresaResponse.SetData(dadosCnpjObj);

            var endereco = new Faker<Endereco>("pt_BR")
                .RuleFor(e => e.Cep, _ => cep)
                .RuleFor(e => e.Cidade, _ => empresa.Municipio)
                .RuleFor(e => e.Logradouro, _ => empresa.Logradouro)
                .Generate();

            _brasilApiManagerMock
                .Setup(x => x.ObterDadosEmpresaBrasilApiAsync(cnpj, It.IsAny<CancellationToken>()))
                .ReturnsAsync(empresaResponse);

            _brasilApiManagerMock
                .Setup(x => x.ObterEnderecoPorCepBrasilApiAsync(cep, It.IsAny<CancellationToken>()))
                .ReturnsAsync(enderecoResponse);

            _mapperMock
                .Setup(x => x.Map<DadosEmpresa>(It.IsAny<DadosCnpjBrasilApiResponse>()))
                .Returns(empresa);

            _mapperMock
                .Setup(x => x.Map<Endereco>(It.IsAny<object>()))
                .Returns(endereco);

            enderecoResponse.SetData(new { street = empresa.Logradouro, cidade = empresa.Municipio });

            var result = await _empresaProviderService.ProcessaValidacaoDadosEmpresaAsync(cnpj, cep, CancellationToken.None);

            Assert.True(result.Notifications.Count == 0);
            Assert.Contains("coincidem", result.MensagemPrincipal, StringComparison.OrdinalIgnoreCase);
        }

        [Fact]
        public async Task ProcessarValidacao_DeveRetornarFalha_QuandoEnderecosNaoCoincidem()
        {
            var cnpj = TestsExtensions.GerarCnpj();
            var cep = TestsExtensions.GerarCepSimples();

            var empresaResponse = new Response();
            var enderecoResponse = new Response();

            var empresa = new Faker<DadosEmpresa>("pt_BR")
             .RuleFor(e => e.Municipio, f => f.Address.City())
             .RuleFor(e => e.Logradouro, f => f.Address.StreetName())
             .Generate();

            var dadosCnpjObj = new DadosCnpjBrasilApiResponse
            {
                Municipio = empresa.Municipio,
                Logradouro = empresa.Logradouro
            };

            empresaResponse.SetData(dadosCnpjObj);

            var endereco = new Faker<Endereco>("pt_BR")
             .RuleFor(e => e.Cidade, f => f.Address.City())
             .RuleFor(e => e.Logradouro, f => f.Address.StreetName())
             .RuleFor(e => e.Cep, f => f.Address.ZipCode("########"))
             .Generate();

            _brasilApiManagerMock
                .Setup(x => x.ObterDadosEmpresaBrasilApiAsync(cnpj, It.IsAny<CancellationToken>()))
                .ReturnsAsync(empresaResponse);

            _brasilApiManagerMock
                .Setup(x => x.ObterEnderecoPorCepBrasilApiAsync(cep, It.IsAny<CancellationToken>()))
                .ReturnsAsync(enderecoResponse);

            _mapperMock
                .Setup(x => x.Map<DadosEmpresa>(It.IsAny<DadosCnpjBrasilApiResponse>()))
                .Returns(empresa);

            _mapperMock
                .Setup(x => x.Map<Endereco>(It.IsAny<object>()))
                .Returns(endereco);

            enderecoResponse.SetData(new { street = "Rua B", cidade = "São Paulo" });

            var result = await _empresaProviderService.ProcessaValidacaoDadosEmpresaAsync(cnpj, cep, CancellationToken.None);

            Assert.NotEmpty(result.Notifications);
        }

        [Fact]
        public async Task ProcessarValidacao_DeveRetornarErro_QuandoEmpresaFalhar()
        {
            var cnpj = TestsExtensions.GerarCnpj();
            var cep = TestsExtensions.GerarCepSimples();

            var empresa = new Faker<DadosEmpresa>("pt_BR")
             .RuleFor(e => e.Municipio, f => f.Address.City())
             .RuleFor(e => e.Logradouro, f => f.Address.StreetName())
             .Generate();

            var endereco = new Faker<Endereco>("pt_BR")
             .RuleFor(e => e.Cidade, f => f.Address.City())
             .RuleFor(e => e.Logradouro, f => f.Address.StreetName())
             .RuleFor(e => e.Cep, f => f.Address.ZipCode("########"))
             .Generate();

            var empresaResponse = new Response();
            empresaResponse.AddNotification("erro");

            _brasilApiManagerMock
                .Setup(x => x.ObterDadosEmpresaBrasilApiAsync(cnpj, It.IsAny<CancellationToken>()))
                .ReturnsAsync(empresaResponse);

            var result = await _empresaProviderService.ProcessaValidacaoDadosEmpresaAsync(cnpj, cep, CancellationToken.None);

            Assert.False(result.Sucesso);
        }

        [Fact]
        public async Task ProcessarValidacao_DeveUsarFallback_QuandoBrasilApiFalhar()
        {
            var cnpj = TestsExtensions.GerarCnpj();
            var cep = TestsExtensions.GerarCepSimples();

            var empresaResponse = new Response();
            var falhaCep = new Response();
            falhaCep.AddNotification("erro");
            var sucessoViaCep = new Response();

            var empresa = new Faker<DadosEmpresa>("pt_BR")
                 .RuleFor(e => e.Municipio, f => f.Address.City())
                 .RuleFor(e => e.Logradouro, f => f.Address.StreetName())
                 .Generate();

            var dadosCnpjObj = new DadosCnpjBrasilApiResponse
            {
                Municipio = empresa.Municipio,
                Logradouro = empresa.Logradouro
            };
            empresaResponse.SetData(dadosCnpjObj);

            var endereco = new Faker<Endereco>("pt_BR")
                .RuleFor(e => e.Cep, _ => cep)
                .RuleFor(e => e.Cidade, _ => empresa.Municipio)
                .RuleFor(e => e.Logradouro, _ => empresa.Logradouro)
                .Generate();

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
                .Setup(x => x.Map<DadosEmpresa>(It.IsAny<DadosCnpjBrasilApiResponse>()))
                .Returns(empresa);

            _mapperMock
                .Setup(x => x.Map<Endereco>(It.IsAny<object>()))
                .Returns(endereco);

            sucessoViaCep.SetData(new { logradouro = empresa.Logradouro, localidade = empresa.Municipio });

            var result = await _empresaProviderService.ProcessaValidacaoDadosEmpresaAsync(cnpj, cep, CancellationToken.None);

            Assert.Empty(result.Notifications);

            _viaCepApiManagerMock.Verify(x =>
                x.ObterEnderecoViaCepAsync(cep, It.IsAny<CancellationToken>()
            ), Times.Once);
        }
    }
}
