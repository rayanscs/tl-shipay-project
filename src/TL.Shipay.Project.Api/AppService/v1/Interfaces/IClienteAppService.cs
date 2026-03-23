using TL.Shipay.Project.Domain.Models.Http;

namespace TL.Shipay.Project.Api.AppService.v1.Interfaces
{
    public interface IClienteAppService
    {
        Task<Response> ProcessaValidacaoDadosEmpresaAsync(string cnpj, string cep, CancellationToken cancellationToken);
    }
}
