using TL.Shipay.Project.Api.AppService.v1.Interfaces;
using TL.Shipay.Project.Domain.Interfaces.Services;
using TL.Shipay.Project.Domain.Models.Http;

namespace TL.Shipay.Project.Api.AppService.v1
{
    public sealed class ClienteAppService(IEmpresaProviderService _provider) : IClienteAppService
    {
        public async Task<Response> ProcessaValidacaoDadosEmpresa(string cnpj, string cep, CancellationToken cancellationToken)
        {
            return await _provider.ProcessaValidacaoDadosEmpresa(cnpj, cep, cancellationToken);
        }
    }
}
