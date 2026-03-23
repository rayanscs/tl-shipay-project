using AutoMapper;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System.Text;
using TL.Shipay.Project.Domain.Enums;
using TL.Shipay.Project.Domain.ExtensionsMelthods;
using TL.Shipay.Project.Domain.ExtensionsMethods;
using TL.Shipay.Project.Domain.Interfaces.ApiManager;
using TL.Shipay.Project.Domain.Interfaces.Services;
using TL.Shipay.Project.Domain.Models;
using TL.Shipay.Project.Domain.Models.Http;
using TL.Shipay.Project.Domain.Models.Responses.BrasilApi.DadosCep;
using TL.Shipay.Project.Domain.Models.Responses.BrasilApi.DadosCnpj;
using TL.Shipay.Project.Domain.Models.Responses.ViaCep;
using TL.Shipay.Project.Infrastructure;
using TL.Shipay.Project.Infrastructure.ExternalServices;
using TL.Shipay.Project.Infrastructure.Utils;

namespace TL.Shipay.Project.Application.Services
{
    public class DadosEmpresaProvider(IBrasilApiManager _brasilApiManager, 
                                      IViaCepManager _viaCepManager, 
                                      IOptions<ResilienciaConfig> _resConfig, 
                                      ILogger<DadosEmpresaProvider> _logger, 
                                      IMapper _mapper) : IDadosEmpresaProvider
    {
        private async Task<Response> ObterDadosEmpresaBrasilApiAsync(string cnpj, CancellationToken cancellationToken)
            => await _brasilApiManager.ObterDadosEmpresaBrasilApiAsync(cnpj, cancellationToken);

        private async Task<Response> ObterEnderecoPorCepBrasilApiAsync(string cep, CancellationToken cancellationToken)
            => await _brasilApiManager.ObterEnderecoPorCepBrasilApiAsync(cep, cancellationToken);

        private async Task<Response> ObterEnderecoViaCepAsync(string cep, CancellationToken cancellationToken)
           => await _viaCepManager.ObterEnderecoViaCepAsync(cep, cancellationToken);

        private async Task<Response> ObterEnderecoPorCepAsync(string cep, string? resPrincipal, CancellationToken cancellationToken)
        {
            Func<Task<Response>> execute = resPrincipal switch
            {
                "BrasilApi" => async () =>
                {
                    var resp = await ObterEnderecoPorCepBrasilApiAsync(cep, cancellationToken);
                    if (!resp.Sucesso)
                    {
                        _logger.LogInformation("BrasilApi falhou, chamando ViaCep para fallback");
                        var fallback = await ObterEnderecoViaCepAsync(cep, cancellationToken);
                        if (!fallback.Sucesso)
                        {
                            _logger.LogInformation("Chamada do Fallback falhou, não foi possivel obter o endereço.");
                            fallback.AddNotification(ECodeTypeLog.ViaCepFallbackFail.Codigo(), ETitleLog.ViaCepErroFallback.Texto(), LogMessagesExtensions.TenteNovamenteMaisTarde());
                            return fallback;
                        }
                        return fallback;
                    }
                    return resp;
                }
                ,
                "ViaCep" => async () =>
                {
                    var resp = await ObterEnderecoViaCepAsync(cep, cancellationToken);
                    if (!resp.Sucesso)
                    {
                        _logger.LogInformation("ViaCep falhou, chamando BrasilApi para fallback");
                        var fallback = await ObterEnderecoPorCepBrasilApiAsync(cep, cancellationToken);
                        if (!fallback.Sucesso)
                        {
                            _logger.LogInformation("Chamada do Fallback falhou, não foi possivel obter o endereço.");
                            fallback.AddNotification(ECodeTypeLog.BrasilApiFallbackFail.Codigo(), ETitleLog.BrasilApiErroFallback.Texto(), LogMessagesExtensions.TenteNovamenteMaisTarde());
                            return fallback;
                        }
                        return fallback;
                    }
                    return resp;
                }
                ,
                _ => async () =>
                {
                    _logger.LogInformation($"{LogMessagesExtensions.SemConfiguracaoResiliencia()}");
                    var respNotConfig = new Response();
                    respNotConfig.AddNotification(ECodeTypeLog.None.Codigo(), ETitleLog.None.Texto(), LogMessagesExtensions.SemConfiguracaoResiliencia());
                    return respNotConfig;
                }
            };

            return await execute();
        }

        private static bool ValidaMatchEnderecos(DadosEmpresa empresa, Endereco endereco)
        {
            var municipioMatch = string.Equals(StringExtensions.NormalizaString(empresa.Municipio), StringExtensions.NormalizaString(endereco.Cidade), StringComparison.Ordinal);
            var logradouroMatch = string.Equals(StringExtensions.NormalizaString(empresa.Logradouro), StringExtensions.NormalizaString(endereco.Logradouro), StringComparison.Ordinal);
            return municipioMatch && logradouroMatch;    
        }

        public async Task<Response> ProcessaValidacaoDadosEmpresa(string cnpj, string cep, CancellationToken cancellationToken)
        {
            var dadosEmpresaResponse = await ObterDadosEmpresaBrasilApiAsync(cnpj, cancellationToken);
            if (!dadosEmpresaResponse.Sucesso)
                 return dadosEmpresaResponse;

            DadosEmpresa dadosEmpresa = _mapper.Map<DadosEmpresa>(dadosEmpresaResponse);

            var resPrincipal = InfrastructureExtensions.ObterServicoPrincipal(_resConfig);
            var enderecoResponse = await ObterEnderecoPorCepAsync(cep, resPrincipal, cancellationToken);
            if (!enderecoResponse.Sucesso) 
                return enderecoResponse;

            var objeto = enderecoResponse.GetData<object>();
            var servicoUtilizando = objeto.IdentificarTypeofEndereco("street", "logradouro");
            var json = JsonConvert.SerializeObject(objeto);

            var endereco = new Endereco();

            if (servicoUtilizando == EResilienciaServico.BrasilApi)
            {
                var obj = JsonConvert.DeserializeObject<BrasilApiCepResponse>(json);
                endereco = _mapper.Map<Endereco>(obj);

            }
            else if (servicoUtilizando == EResilienciaServico.ViaCep)
            {
                var obj = JsonConvert.DeserializeObject<ViaCepResponse>(json);
                endereco = _mapper.Map<Endereco>(obj);
            }

            var response = new Response();

            if (ValidaMatchEnderecos(dadosEmpresa, endereco))
            {
                _logger.LogInformation($"{ETitleLog.ValidacaoDadosSucesso.Texto()}");
                response.MensagemPrincipal = $"{LogMessagesExtensions.InformacoesCoincidem}";
            }
            else
            {
                _logger.LogInformation($"{LogMessagesExtensions.InformacoesNaoCoincidem()}");
                response.AddNotification(ECodeTypeLog.DataValidateFail.Codigo(), ETitleLog.ValidacaoDadosFalha.Texto(), LogMessagesExtensions.InformacoesNaoCoincidem());
            }

            return response;
        }
    }
}
