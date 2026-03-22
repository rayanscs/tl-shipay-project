using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.Net.Http.Json;
using TL.Shipay.Project.Domain.Interfaces;
using TL.Shipay.Project.Domain.Models.Http;
using TL.Shipay.Project.Domain.Models.Responses.ViaCep;

namespace TL.Shipay.Project.Infrastructure.ExternalServices
{
    public class ViaCepManager : IViaCepManager
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<ViaCepManager> _logger;
        private readonly ApiManagerUrlOptions _options;

        public ViaCepManager(HttpClient httpClient, ILogger<ViaCepManager> logger, ApiManagerUrlOptions options)
        {
            _httpClient = httpClient;
            _logger = logger;
            _options = options;
        }

        public async Task<Response> ObterEnderecoViaCepAsync(string cep, CancellationToken cancellationToken = default)
        {
            var urlBase = $"{_options.ApiManagerUrl.ViaCep}";
            var urlCep = $"{_options.ApiManagerUrl.ViaCep.DadosCep}";
            var response = new Response();

            try
            {
                var httpResponse = await _httpClient.GetAsync(string.Format($"{urlBase}{urlCep}", cep), cancellationToken);

                if (!httpResponse.IsSuccessStatusCode)
                {
                    _logger.LogWarning("Consulta ViaCep falhou. Cep: {Cep} StatusCode: {StatusCode}", 
                                        cep, 
                                        httpResponse.StatusCode);

                    response.AddNotification("ViaCepError", 
                                             "Erro na consulta ViaCep", 
                                             $"Não foi possível obter o endereço para o CEP {cep}. StatusCode: {httpResponse.StatusCode}");
                    return response; 
                }

                var viaCep = await httpResponse.Content.ReadFromJsonAsync<ViaCepResponse>(cancellationToken: cancellationToken);
                if (viaCep == null)
                {

                    _logger.LogWarning($"Consulta ViaCep: CEP não encontrado. Cep: {cep} StatusCode: {StatusCodes.Status404NotFound}",
                                        cep,
                                        StatusCodes.Status404NotFound);

                    response.AddNotification("ViaCepNotFound",
                                             "Erro na consulta ViaCep",
                                             $"Não foi encontrado o endereço para o CEP {cep}. StatusCode: {httpResponse.StatusCode}");

                    return response;   
                }

                response.SetData(viaCep);
                return response;
            }
            catch (HttpRequestException ex)
            {
                var mensagem = ex.Message;
                var detalheInterno = ex.InnerException?.Message;
                var status = ex.StatusCode?.ToString() ?? "sem status";
                var httpError = ex.HttpRequestError;

                _logger.LogError(ex, 
                                "Erro ao consultar ViaCep. Status: {Status} Mensagem: {Mensagem}", 
                                status, 
                                mensagem);

                response.AddNotification("ViaCepExceptionError",
                                         "Erro na consulta ViaCep",
                                         $"Erro ao consultar o CEP {cep}. {mensagem} - {detalheInterno ?? ""} (Status: {status}) ");

                return response;
            }
        }
    }
}
