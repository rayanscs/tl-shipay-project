using TL.Shipay.Project.Domain.Models.Http;

namespace TL.Shipay.Project.Application.Interfaces
{
    public interface IBrasilApiService
    {
        Task<Response> ConsultaDadosCnpjAsync(string cnpj);
        Task<Response> ConsultaDadosCepAsync(string cnpj);
    }
}
