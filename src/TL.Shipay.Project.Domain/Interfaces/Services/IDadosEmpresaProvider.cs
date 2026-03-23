using TL.Shipay.Project.Domain.Models.Http;

namespace TL.Shipay.Project.Domain.Interfaces.Services
{
    public interface IDadosEmpresaProvider
    {
        Task<Response> ProcessaValidacaoDadosEmpresa(string cnpj, string cep, CancellationToken cancellationToken);
    }
}
