using Microsoft.Extensions.Options;
using TL.Shipay.Project.Domain.Interfaces.ApiManager;
using TL.Shipay.Project.Domain.Interfaces.Services;
using TL.Shipay.Project.Domain.Models.Http;
using TL.Shipay.Project.Domain.Models.Responses.BrasilApi.DadosCnpj;
using TL.Shipay.Project.Infrastructure;

namespace TL.Shipay.Project.Application.Services
{
    public class DadosEmpresaProvider(IBrasilApiManager brasilApiManager, IViaCepManager viaCepManager, IOptions<ResilienciaConfig> resConfig) : IDadosEmpresaProvider
    {
        public async Task<Response> ObterDadosEmpresaBrasilApiAsync(string cnpj, CancellationToken cancellationToken)
            => await brasilApiManager.ObterDadosEmpresaBrasilApiAsync(cnpj, cancellationToken);

        public async Task<Response> ObterEnderecoPorCepBrasilApiAsync(string cep, CancellationToken cancellationToken)
            => await brasilApiManager.ObterEnderecoPorCepBrasilApiAsync(cep, cancellationToken);

        public async Task<Response> ObterEnderecoViaCepAsync(string cep, CancellationToken cancellationToken)
           => await viaCepManager.ObterEnderecoViaCepAsync(cep, cancellationToken);

        public Task<Response> DefinirEnderecoPorCepAsync(string cep, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<bool> ValidaMatchEnderecos(Response enderecoBrasilApi, Response enderecoViaCep)
        {
            /*
             Verificar se enderecoBrasilApi.City.Upper() == enderecoViaCep.Localidade.Upper()        OU
             Verificar se enderecoBrasilApi.Municipio.Upper() == enderecoViaCep.Localidade.Upper()
             Verificar se enderecoBrasilApi.Street.Upper() == enderecoViaCep.Logradouro.Upper()
             Verificar se enderecoBrasilApi.Logradouro.Upper() == enderecoViaCep.Logradouro.Upper()
             
             */

            throw new NotImplementedException();
        }

        public async Task<Response> ProcessaValidacaoDadosEmpresa(string cnpj, CancellationToken cancellationToken)
        {
            var dadosEmpresaResponse = await ObterDadosEmpresaBrasilApiAsync(cnpj, cancellationToken);
            if (!dadosEmpresaResponse.Sucesso)
            { }

            var dadosEmpresa = dadosEmpresaResponse.GetDataJson<DadosCnpjBrasilApiResponse>();


        }

    }
}
