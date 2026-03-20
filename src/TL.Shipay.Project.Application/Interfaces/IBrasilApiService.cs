using TL.Shipay.Project.Domain.Responses;

namespace TL.Shipay.Project.Application.Interfaces
{
    public interface IBrasilApiService
    {
        Task<Response> ConsultaDadosCnpjAsync(string cnpj);
        Task<Response> ConsultaDadosCepAsync(string cnpj);
    }
}
