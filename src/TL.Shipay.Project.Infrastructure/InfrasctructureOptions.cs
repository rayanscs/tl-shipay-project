namespace TL.Shipay.Project.Infrastructure
{

    public class InfrasctructureOptions
    {
        public ResilienciaConfig ResilienciaConfig { get; set; }
        public SerilogConfig SerilogConfig { get; set; }
    }

    public class ResilienciaConfig
    {
        public ServicoResilienciaConfig? Servicos { get; set; }

        public int RetryCount { get; set; } = 1;
    }

    public class ServicoResilienciaConfig
    {
        public string ServicoPrincipal { get; set; } = string.Empty;

        public string ServicoFallback { get; set; } = string.Empty;
    }

    public class SerilogConfig
    {
        public string? Path { get; set; }
    }
}
