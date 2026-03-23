using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Net.Http.Json;
using TL.Shipay.Project.Domain.Enums;
using TL.Shipay.Project.Domain.Interfaces.ApiManager;
using TL.Shipay.Project.Domain.Models.Http;
using TL.Shipay.Project.Domain.Models.Responses.ViaCep;
using TL.Shipay.Project.Domain.Utils;

namespace TL.Shipay.Project.Infrastructure.ExternalServices
{
    public class ViaCepManager : IViaCepManager
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<ViaCepManager> _logger;
        private readonly IOptions<ApiManagerUrlOptions> _options;
        private readonly string _baseUrl;
        private readonly string _cepUrl;

        public ViaCepManager(HttpClient httpClient, ILogger<ViaCepManager> logger, IOptions<ApiManagerUrlOptions> options)
        {
            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _options = options ?? throw new ArgumentNullException(nameof(options));
            _baseUrl = $"{_options.Value.ViaCep.BaseUrl}";
            _cepUrl = $"{_options.Value.ViaCep.DadosCep}";
        }

        public HttpClient HttpClient { get; }
        public ILogger<ViaCepManager> Logger { get; }
        public IOptions<ApiManagerUrlOptions> Options { get; }

        public async Task<Response> ObterEnderecoViaCepAsync(string cep, CancellationToken cancellationToken)
        {
            var response = new Response();
            var cepLimpo = cep.CepSomenteNumeros();

            try
            {
                var httpResponse = await _httpClient.GetAsync(string.Format($"{_baseUrl}{_cepUrl}", cepLimpo), cancellationToken);
                if (!httpResponse.IsSuccessStatusCode)
                {
                    _logger.LogWarning($"Consulta ViaCep falhou. Cep: {cepLimpo} StatusCode: {httpResponse.StatusCode}");
                    response.AddNotification(ECodeTypeLog.ViaCepError.Codigo(),
                                             ETitleLog.ViaCepErroConsulta.Texto(),
                                             $"Não foi possível obter o endereço para o CEP {cep}, {LogMessagesExtensions.TenteNovamenteMaisTarde()}");

                    return response;
                }

                var viaCep = await httpResponse.Content.ReadFromJsonAsync<ViaCepResponse>(cancellationToken: cancellationToken);
                if (viaCep is null)
                {
                    _logger.LogWarning($"Consulta ViaCep: Consulta do Cep retornou nula. Cep: {cepLimpo} StatusCode: {StatusCodes.Status404NotFound}");
                    response.AddNotification(ECodeTypeLog.ViaCepNotFound.Codigo(),
                                             ETitleLog.ViaCepErroConsulta.Texto(),
                                             $"Não foram encontrados os dados de endereço para o Cep {cep}, a consulta retornou vazia.");

                    return response;
                }

                response.SetData(viaCep);
                return response;
            }
            catch (HttpRequestException ex)
            {
                var mensagem = ex.Message;
                var detalheInterno = ex.InnerException?.Message ?? "sem detalhe";
                var status = ex.StatusCode?.ToString() ?? "sem status";
                var httpError = ex.HttpRequestError;

                _logger.LogError(ex, $"Erro ao consultar ViaCep. Status: {status} Mensagem: Erro ao consultar o Cep {cepLimpo}. {LogMessagesExtensions.ComDetalheOpcional(mensagem, detalheInterno)}");
                response.AddNotification(ECodeTypeLog.ViaCepExceptionError.Codigo(),
                                         ETitleLog.ViaCepErroConsulta.Texto(),
                                         $"Erro ao consultar o Cep {cep}, {LogMessagesExtensions.TenteNovamenteMaisTardeComSuporte()}");

                return response;
            }
        }
    }
}
