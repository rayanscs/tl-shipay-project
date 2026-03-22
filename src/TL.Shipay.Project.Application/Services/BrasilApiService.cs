using TL.Shipay.Project.Application.Interfaces;
using TL.Shipay.Project.Domain.Models.Http;

namespace TL.Shipay.Project.Application.Services
{
    public class BrasilApiService : IBrasilApiService
    {
        public Task<Response> ConsultaDadosCepAsync(string cnpj)
        {
            throw new NotImplementedException();
        }

        public Task<Response> ConsultaDadosCnpjAsync(string cnpj)
        {
            throw new NotImplementedException();
        }
    }
}
