using TL.Shipay.Project.Application.Interfaces;
using TL.Shipay.Project.Domain.Interfaces.Services;
using TL.Shipay.Project.Domain.Models.Http;
using TL.Shipay.Project.Domain.Models.Responses.BrasilApi.DadosCnpj;

namespace TL.Shipay.Project.Application.Services
{
    public class ClienteService(IDadosEmpresaProvider dadosEmpresaProvider) : IClienteService
    {
        
        public static void ObterEnderecoPorCepAsync(string cep, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }



        public async Task<Response> ProcessaVerificação(string cnpj, string cep, CancellationToken cancellationToken)
        {
            var response = new Response();



            var enderecoBrasilApiResponse = await _brasilApiManager.ObterDadosEmpresaBrasilApiAsync(cnpj, cancellationToken);
            if (!enderecoBrasilApiResponse.Sucesso)
            {

            }

            var dadosEmpresa = enderecoBrasilApiResponse.GetDataJson<DadosCnpjBrasilApiResponse>();


            return response;
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
    }
}
