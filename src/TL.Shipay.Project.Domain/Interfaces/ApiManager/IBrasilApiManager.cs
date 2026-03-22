using TL.Shipay.Project.Domain.Models.Http;

namespace TL.Shipay.Project.Domain.Interfaces.ApiManager
{
    public interface IBrasilApiManager
    {
        Task<Response> ObterDadosEmpresaBrasilApiAsync(string cnpj, CancellationToken cancellationToken);

        Task<Response> ObterEnderecoPorCepBrasilApiAsync(string cep, CancellationToken cancellationToken);
    }
}
