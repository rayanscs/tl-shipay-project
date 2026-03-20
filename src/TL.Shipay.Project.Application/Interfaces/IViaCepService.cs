using TL.Shipay.Project.Domain.Responses;

namespace TL.Shipay.Project.Application.Interfaces
{
    public interface IViaCepService
    {
        Task<Response> ConsultaDadosCepAsync(string cnpj);
    }
}
