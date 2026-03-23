using TL.Shipay.Project.Domain.Models.Http;

namespace TL.Shipay.Project.Domain.Interfaces.Services
{
    public interface IEmpresaProviderService
    {
        Task<Response> ProcessaValidacaoDadosEmpresaAsync(string cnpj, string cep, CancellationToken cancellationToken);
    }
}
