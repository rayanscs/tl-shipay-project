using TL.Shipay.Project.Domain.Models.Http;
using TL.Shipay.Project.Domain.Models.Responses;

namespace TL.Shipay.Project.Application.Interfaces
{
    public interface IViaCepService
    {
        Task<Response> ConsultaDadosCepAsync(string cnpj);
    }
}
