namespace TL.Shipay.Project.Infrastructure
{
    public class InfrastructureOptions
    {
        public ResilienciaConfig ResilienciaConfig { get; set; }
        public SerilogConfig SerilogConfig { get; set; }
    }

    public class ResilienciaConfig
    {
        public ServicoResilienciaConfig? Services { get; set; }
        public int RetryCount { get; set; } = 1;
    }

    public class SerilogConfig
    {
        public string? Path { get; set; }
    }

    public class ServicoResilienciaConfig
    {
        public string ServicePrincipal { get; set; } = string.Empty;
        public string ServiceFallback { get; set; } = string.Empty;
    }
}
