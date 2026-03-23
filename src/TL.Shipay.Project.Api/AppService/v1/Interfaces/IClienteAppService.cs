using TL.Shipay.Project.Domain.Models.Http;

namespace TL.Shipay.Project.Api.AppService.v1.Interfaces
{
    public interface IClienteAppService
    {
        Task<Response> ProcessaValidacaoDadosEmpresa(string cnpj, string cep, CancellationToken cancellationToken);
    }
}
