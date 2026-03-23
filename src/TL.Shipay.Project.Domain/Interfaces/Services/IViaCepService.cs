using TL.Shipay.Project.Domain.Models.Http;

namespace TL.Shipay.Project.Application.Interfaces
{
    public interface IViaCepService
    {
        Task<Response> ConsultaDadosCepAsync(string cnpj);
    }
}
