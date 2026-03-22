using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.Net.Http.Json;
using TL.Shipay.Project.Domain.Enums;
using TL.Shipay.Project.Domain.ExtensionsMethods;
using TL.Shipay.Project.Domain.Interfaces.ApiManager;
using TL.Shipay.Project.Domain.Models.Http;
using TL.Shipay.Project.Domain.Models.Responses.BrasilApi.DadosCep;
using TL.Shipay.Project.Domain.Models.Responses.BrasilApi.DadosCnpj;

namespace TL.Shipay.Project.Infrastructure.ExternalServices
{
    public class BrasilApiManager : IBrasilApiManager
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<BrasilApiManager> _logger;
        private readonly ApiManagerUrlOptions _options;
        private readonly string _baseUrl;
        private readonly string _dadosCnpjUrl;
        private readonly string _dadosCepUrl;

        public BrasilApiManager(HttpClient httpClient, ILogger<BrasilApiManager> logger, ApiManagerUrlOptions options)
        {
            _httpClient = httpClient;
            _logger = logger;
            _options = options;
            _baseUrl = $"{_options.ApiManagerUrl.BrasilApi.BaseUrl}";
            _dadosCnpjUrl = $"{_options.ApiManagerUrl.BrasilApi.DadosCnpj}";
            _dadosCepUrl = $"{_options.ApiManagerUrl.BrasilApi.DadosCep}";
        } 

        public async Task<Response> ObterDadosEmpresaBrasilApiAsync(string cnpj, CancellationToken cancellationToken)
        {
            var response = new Response();

            try
            {
                var httpResponse = await _httpClient.GetAsync(string.Format($"{_baseUrl}{_dadosCnpjUrl}", cnpj), cancellationToken);
                if (!httpResponse.IsSuccessStatusCode)
                {
                    _logger.LogWarning($"Consulta BrasilApi Cnpj falhou. Cnpj: {cnpj} StatusCode: {httpResponse.StatusCode}");
                    response.AddNotification(ECodeTypeLog.BrasilApiCnpjError.Codigo(), 
                                             ETitleLog.BrasilApiErroConsultaCnpj.Texto(), 
                                             $"Não foi possível obter os dados para o Cnpj {cnpj}, {LogMessagesExtensions.TenteNovamenteMaisTarde()}");

                    return response;
                }

                var BrasilApiCnpj = await httpResponse.Content.ReadFromJsonAsync<DadosCnpjBrasilApiResponse>(cancellationToken: cancellationToken);
                if (BrasilApiCnpj is null)
                {
                    _logger.LogWarning($"Consulta BrasilApi: Consulta do Cnpj retornou nula. Cnpj: {cnpj} StatusCode: {StatusCodes.Status404NotFound}");
                    response.AddNotification(ECodeTypeLog.BrasilApiNotFound.Codigo(), 
                                             ETitleLog.BrasilApiErroConsultaCnpj.Texto(), 
                                             $"Não foi encontrado os dados para o Cnpj {cnpj}, a consulta retornou vazia.");

                    return response;
                }

                response.SetData(BrasilApiCnpj);
                return response;
            }
            catch (HttpRequestException ex)
            {
                var mensagem = ex.Message;
                var detalheInterno = ex.InnerException?.Message ?? "sem detalhe";
                var status = ex.StatusCode?.ToString() ?? "sem status";
                var httpError = ex.HttpRequestError;

                _logger.LogError(ex, $"Erro ao consultar BrasilApi Cnpj. Status: {status} Mensagem: Erro ao consultar o Cnpj {cnpj}. {LogMessagesExtensions.ComDetalheOpcional(mensagem, detalheInterno)}");
                response.AddNotification(ECodeTypeLog.BrasilApiExceptionError.Codigo(), 
                                         ETitleLog.BrasilApiErroConsultaCnpj.Texto(),
                                         $"Erro ao consultar o Cnpj {cnpj}, {LogMessagesExtensions.TenteNovamenteMaisTardeComSuporte()}");

                return response;
            }
        }

        public async Task<Response> ObterEnderecoPorCepBrasilApiAsync(string cep, CancellationToken cancellationToken)
        {
            var response = new Response();

            try
            {
                var httpResponse = await _httpClient.GetAsync(string.Format($"{_baseUrl}{_dadosCepUrl}", cep), cancellationToken);
                if (!httpResponse.IsSuccessStatusCode)
                {
                    _logger.LogWarning($"Consulta BrasilApi falhou. Cep: {cep} StatusCode: {httpResponse.StatusCode}");

                    response.AddNotification(ECodeTypeLog.BrasilApiCepError.Codigo(),
                                             ETitleLog.BrasilApiErroConsultaCep.Texto(),
                                             $"Não foi possível obter os dados para o Cep {cep}. StatusCode: {httpResponse.StatusCode}");
                    
                    return response;
                }

                var BrasilApiCnpj = await httpResponse.Content.ReadFromJsonAsync<BrasilApiCepResponse>(cancellationToken: cancellationToken);
                if (BrasilApiCnpj is null)
                {
                    _logger.LogWarning($"Consulta BrasilApi: Consulta do Cep retornou nula. Cep: {cep} StatusCode: {StatusCodes.Status404NotFound}");
                    response.AddNotification(ECodeTypeLog.BrasilApiNotFound.Codigo(), 
                                             ETitleLog.BrasilApiErroConsultaCep.Texto(), 
                                             $"Não foi encontrado os dados para o Cep {cep}, consulta retornou vazia. StatusCode: {StatusCodes.Status404NotFound}");
                    
                    return response;
                }

                response.SetData(BrasilApiCnpj);
                return response;
            }
            catch (HttpRequestException ex)
            {
                var mensagem = ex.Message;
                var detalheInterno = ex.InnerException?.Message ?? "sem detalhe";
                var status = ex.StatusCode?.ToString() ?? "sem status";
                var httpError = ex.HttpRequestError;

                _logger.LogError(ex, $"Erro ao consultar BrasilApi. Status: {status} {LogMessagesExtensions.ComDetalheOpcional(mensagem, detalheInterno)}");
                response.AddNotification(ECodeTypeLog.BrasilApiExceptionError.Codigo(),
                                        ETitleLog.BrasilApiErroConsultaCep.Texto(), 
                                        $"Erro ao consultar o Cep {cep}. Mensagem: {LogMessagesExtensions.ComDetalheOpcional(mensagem, detalheInterno)})");
                
                return response;
            }
        }
    }
}
