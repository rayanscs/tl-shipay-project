using Microsoft.Extensions.Options;

namespace TL.Shipay.Project.Infrastructure.Utils
{
    public static class InfrastructureUtils
    {
        public static string ObterServicoPrincipal(InfrastructureOptions resConfig)
        {
            return resConfig.ResilienciaConfig.Services?.ServicePrincipal ?? "";
        }
    }
}
