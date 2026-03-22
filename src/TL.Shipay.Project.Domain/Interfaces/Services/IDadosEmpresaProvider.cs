using TL.Shipay.Project.Domain.Models.Http;

namespace TL.Shipay.Project.Domain.Interfaces.Services
{
    public interface IDadosEmpresaProvider
    {
        Task<Response> ObterEnderecoPorCepAsync(string cep, CancellationToken cancellationToken);
    }
}
