using Microsoft.Extensions.Options;

namespace TL.Shipay.Project.Infrastructure.Utils
{
    public static class InfrastructureUtils
    {
        public static string ObterServicoPrincipal(IOptions<InfrastructureOptions> _resConfig)
        {
            var resConfig = _resConfig.Value;
            return resConfig.ResilienciaConfig.Services?.ServicePrincipal ?? "";
        }
    }
}
