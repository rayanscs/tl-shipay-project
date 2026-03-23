using Microsoft.Extensions.Options;

namespace TL.Shipay.Project.Infrastructure.Utils
{
    public static class InfrastructureExtensions
    {
        public static string ObterServicoPrincipal(IOptions<InfrasctructureOptions> _resConfig)
        {
            var resConfig = _resConfig.Value;
            return resConfig.Servicos?.ServicoPrincipal ?? "";
        }
    }
}
