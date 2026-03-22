using TL.Shipay.Project.Application.Interfaces;
using TL.Shipay.Project.Domain.Models.Http;

namespace TL.Shipay.Project.Application.Services
{
    public class ClienteService : IClienteService
    {
        public ClienteService()
        {
            
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
