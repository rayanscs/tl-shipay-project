using TL.Shipay.Project.Domain.Models.Http;

namespace TL.Shipay.Project.Domain.Interfaces
{
    public interface IBrasilApiManager
    {
        Task<Response> ObterDadosEmpresaBrasilApiAsync(string cnpj);

        Task<Response> ObterEnderecoBrasilApiAsync(string cep);
    }
}
