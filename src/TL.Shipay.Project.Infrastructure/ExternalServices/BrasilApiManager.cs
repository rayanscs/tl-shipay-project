using TL.Shipay.Project.Domain.Interfaces;
using TL.Shipay.Project.Domain.Models.Http;

namespace TL.Shipay.Project.Infrastructure.ExternalServices
{
    public class BrasilApiManager : IBrasilApiManager
    {
        public Task<Response> ObterDadosEmpresaBrasilApiAsync(string cnpj)
        {
            throw new NotImplementedException();
        }

        public Task<Response> ObterEnderecoBrasilApiAsync(string cep)
        {
            throw new NotImplementedException();
        }
    }
}
